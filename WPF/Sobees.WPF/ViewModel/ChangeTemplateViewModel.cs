#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Cls;
using Sobees.Infrastructure.Controls;
using Sobees.Infrastructure.Helpers;
using Sobees.Infrastructure.Model;
using Sobees.Infrastructure.ViewModelBase;
using Sobees.Tools.Logging;

#endregion

namespace Sobees.ViewModel
{
  public class ChangeTemplateViewModel : BWorkspaceViewModel
  {
    #region Fields

    private ObservableCollection<BServiceWorkspaceViewModel> _bServiceWorkspaces = new ObservableCollection<BServiceWorkspaceViewModel>();
    private BTemplate _bDragGridTemplate;
    private EnumView CurrentView;

    #endregion Fields

    #region Properties

    public ObservableCollection<BServiceWorkspaceViewModel> BServiceWorkspaces
    {
      get => _bServiceWorkspaces;
        set => _bServiceWorkspaces = value;
    }

    public BTemplate BDragGridTemplate
    {
      get => _bDragGridTemplate ?? (_bDragGridTemplate = new BTemplate
                                                         {
                                                             BPositions = new List<BPosition>
                                                                          {
                                                                              new BPosition(0, 0, 1, 1)
                                                                          },
                                                             Columns = 1,
                                                             Rows = 1,
                                                             GridSplitterPositions = new List<BPosition>()
                                                         });
        set
      {
        if (_bDragGridTemplate == null)
          _bDragGridTemplate = new BTemplate();

        _bDragGridTemplate.BPositions = value.BPositions;
        _bDragGridTemplate.Columns = value.Columns;
        _bDragGridTemplate.GridSplitterPositions = value.GridSplitterPositions;
        _bDragGridTemplate.ImgUrl = value.ImgUrl;
        _bDragGridTemplate.Rows = value.Rows;
        _bDragGridTemplate.DependencyPropertyDescriptorHack = value.DependencyPropertyDescriptorHack;

        RaisePropertyChanged();
      }
    }

    public RelayCommand<Button> RemoveServiceCommand { get; set; }

    public RelayCommand<BDragGrid> ChangeTemplateCommand { get; set; }

    public override DataTemplate DataTemplateView
    {
      get
      {
        const string dt = "<DataTemplate x:Name='dtMainView' " +
                          "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                          "xmlns:Views='clr-namespace:Sobees.Views;assembly=Sobees' " +
                          "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >" +
                          "<Views:ChangeTemplate/> " +
                          "</DataTemplate>";

        var stringReader = new StringReader(dt);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
      set => base.DataTemplateView = value;
    }

    #endregion Properties

    public ChangeTemplateViewModel(EnumView view)
    {
      CurrentView = view;
      CloseCommand = new RelayCommand(() =>
                                        {
                                          switch (view)
                                          {
                                            case EnumView.First:
                                              Messenger.Default.Send("CloseChangeTemplate1");
                                              break;

                                            case EnumView.Second:
                                              Messenger.Default.Send("CloseChangeTemplate2");
                                              break;
                                          }
                                        });
      switch (view)
      {
        case EnumView.First:
          foreach (var serviceWorkspace in BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaces1)
          {
            if (!serviceWorkspace.IsServiceWorkspace) continue;
            BServiceWorkspaces.Add(serviceWorkspace);
            TraceHelper.Trace(this, serviceWorkspace + " added to list of currently loaded services.");
          }
          break;

        case EnumView.Second:
          break;

        default:
          throw new ArgumentOutOfRangeException("view");
      }
    }

    public void SetSelectedTemplate(BTemplate template)
    {
      BDragGridTemplate = template;
    }

    protected override void InitCommands()
    {
      ChangeTemplateCommand = new RelayCommand<BDragGrid>(ChangeTemplate);
      RemoveServiceCommand = new RelayCommand<Button>(RemoveService);
      base.InitCommands();
    }

    private void RemoveService(Button btn)
    {
      if (btn == null) return;

      var cc = WPFHelper.FindUIParentOfType<ContentControl>(btn);
      if (cc == null) return;
      BServiceWorkspaces.Add(btn.DataContext as BServiceWorkspaceViewModel);
      cc.Content = new UcDragTemplate();
    }

    private void ChangeTemplate(BDragGrid grid)
    {
      try
      {
        // We close all loaded SourceLoaders
        var commandToExecute = new List<ICommand>();
        foreach (var serviceWorkspace in BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaces1)
        {
          if (serviceWorkspace.IsMaxiMize)
          {
            serviceWorkspace.IsMaxiMize = false;
            Messenger.Default.Send("Minimize");
          }
          if (!serviceWorkspace.IsServiceWorkspace)
          {
            commandToExecute.Add(serviceWorkspace.CloseCommand);
            serviceWorkspace.IsClosed = true;
          }
        }

        TraceHelper.Trace(this, "Change template and re-position all ServiceWorkspaces (Twitter, Facebook,...)");
        foreach (var child in grid.Children)
        {
          if (child.GetType().Equals(typeof (ContentControl)))
          {
            var cc = child as ContentControl;
            if (cc == null) continue;

            var drag = cc.Content as UcDragTemplateComplete;
            if (drag != null)

              if (drag.DataContext != null && drag.DataContext.GetType().BaseType.Equals(typeof (BServiceWorkspaceViewModel)))
              {
                var col = Grid.GetColumn(cc);
                var row = Grid.GetRow(cc);
                var colSpan = Grid.GetColumnSpan(cc);
                var rowSpan = Grid.GetRowSpan(cc);

                var positionInDragGrid = new BPosition(col, row, rowSpan, colSpan);

                foreach (var model in BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaces1)
                {
                  if (drag.DataContext.Equals(model))
                  {
                    TraceHelper.Trace(this,
                                      "Old position: " + model.PositionInGrid.Col + " " + model.PositionInGrid.Row + " -> New position: " +
                                      +positionInDragGrid.Col + " " + positionInDragGrid.Row);
                    model.PositionInGrid = positionInDragGrid;

                    BServiceWorkspaces.Remove(drag.DataContext as BServiceWorkspaceViewModel);

                    break;
                  }
                }
              }
          }
        }

        foreach (var serviceWorkspace in BServiceWorkspaces)
        {
          TraceHelper.Trace(this, "Mark " + serviceWorkspace + " to be closed!");

          // HACK - we say it's not a real ServiceWorkspace (like sourceloaderviewmodel for example)
          // -> so that no sourceLoader is loaded on TOP of the closed serviceWorkspace
          serviceWorkspace.IsServiceWorkspace = false;

          commandToExecute.Add(serviceWorkspace.CloseCommand);
        }
        foreach (var command in commandToExecute)
        {
          TraceHelper.Trace(this, "Closing " + command);
          if (command != null) command.Execute(null);
        }

        // Load SourceLoaders in empty spaces
        TraceHelper.Trace(this, "Foreach empty positions, we add SourceLoaders");
        foreach (var emptyPosition in BViewModelLocator.SobeesViewModelStatic.GetEmptyPositions(grid.BTemplate))
        {
          TraceHelper.Trace(this, emptyPosition.ToString());
          BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaces1.Add(new SourceLoaderViewModel(emptyPosition, null));
        }
        BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1 = grid.BTemplate;

        Messenger.Default.Send("CloseChangeTemplate1");
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }
  }
}