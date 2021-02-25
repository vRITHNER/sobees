using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Infrastructure.Commands;
using Sobees.Infrastructure.Controls;
using Sobees.Infrastructure.Model;
using Sobees.Infrastructure.UI;
using Sobees.Infrastructure.ViewModelBase;
using Sobees.Tools.Logging;

namespace Sobees.ViewModel
{
  public class TemplateLoaderViewModel : BWorkspaceViewModel
  {
    #region Fields

    public readonly EnumView CurrentView;
    private List<BTemplate> _bTemplatesTemp;

    #region Commands Fields

    private ICommand _templateSelectedCommand;

    #endregion Commands Fields

    #endregion Fields

    #region Constructors

    public TemplateLoaderViewModel(EnumView first)
    {
      LoadTemplatesAsync();
      CurrentView = first;
      CloseCommand = new RelayCommand(() =>
                                        {
                                          switch (CurrentView)
                                          {
                                            case EnumView.First:
                                              Messenger.Default.Send("CloseFront1");
                                              break;

                                            case EnumView.Second:
                                              Messenger.Default.Send("CloseFront2");
                                              break;
                                          }
                                        });
    }

    #endregion Constructors

    #region Properties

    public ObservableCollection<BTemplate> BTemplates { get; set; }

    public override DataTemplate DataTemplateView
    {
      get
      {
        const string dt = "<DataTemplate x:Name='dtTemplateLoader' " +
                          "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                          "xmlns:Views='clr-namespace:Sobees.Views;assembly=Sobees' " +
                          "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >" +
                          "<Views:TemplateLoader/> " +
                          "</DataTemplate>";

        var stringReader = new StringReader(dt);
        XmlReader xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
    }

    #endregion Properties

    #region Commands

    /// <summary>
    /// Gets the template selected command.
    /// </summary>
    /// <value>The template selected command.</value>
    public ICommand TemplateSelectedCommand => _templateSelectedCommand ?? (_templateSelectedCommand = new BRelayCommand(TemplateSelected));

    #endregion Commands

    #region Methods

    #region Public Methods

    /// <summary>
    /// LoadTemplatesAsync
    /// </summary>
    public async void LoadTemplatesAsync()
    {
      try
      {
        if (BTemplates == null)
          BTemplates = new ObservableCollection<BTemplate>();

        var dispatcher = Dispatcher.CurrentDispatcher;
          Action mainAction = () =>
          {
              BTemplates.Clear();

              var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
              var xmlFilePath = string.Format("{0}\\Data\\BTemplates.xml", @location);

              var feedXml = XDocument.Load(xmlFilePath);
              LoadFeed(feedXml, location);

            foreach (var bTemplate in _bTemplatesTemp)
            {
              BTemplates.Add(bTemplate);
              Task.Yield();
            }
              
          };

        Action backgroundAction = () => dispatcher.BeginInvoke(DispatcherPriority.Render, mainAction);
        var task = Task.Factory.StartNew(backgroundAction).ContinueWith(_ => TraceHelper.Trace(this, "Template Loaded"), UiContext.Instance.Current);
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    #endregion Public Methods

    #region Private Methods

    private void TemplateSelected(object param)
    {
      var objs = BRelayCommand.CheckParams(param);

      var bt = objs[0] as BTemplate;
      if (bt == null)
        return;

      TraceHelper.Trace(this, "Following bTemplate was selected by user: " + bt);
      switch (CurrentView)
      {
        case EnumView.First:
          Messenger.Default.Send(new BMessage("TemplateChoosed1", bt));
          break;

        case EnumView.Second:
          Messenger.Default.Send(new BMessage("TemplateChoosed2", bt));
          break;

        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private void LoadFeed(XDocument xdoc, string location)
    {
      try
      {
        _bTemplatesTemp = (from template in xdoc.Descendants("Template")
                           select new BTemplate
                           {
                             Columns = int.Parse(template.Attribute("Columns").Value),
                             Rows = int.Parse(template.Attribute("Rows").Value),
                             ImgUrl = string.Format("/Sobees;Component/Resources/Templates/{0}", template.Attribute("ImgUrl").Value),
                             BPositions =
                               (from content in template.Descendants("Content")
                                select new BPosition
                                {
                                  Col =
                                    int.Parse(content.Attribute("Col").Value),
                                  Row =
                                    int.Parse(content.Attribute("Row").Value),
                                  ColSpan =
                                    int.Parse(content.Attribute("ColSpan").Value),
                                  RowSpan =
                                    int.Parse(content.Attribute("RowSpan").Value),
                                  Orientation =
                                    (Orientation)
                                    Enum.Parse(typeof(Orientation),
                                               content.Attribute("Orientation").
                                                 Value, true),
                                  Width =
                                    double.Parse(content.Attribute("Width").Value),
                                  Height =
                                    double.Parse(
                                    content.Attribute("Height").Value),
                                  UnitType =
                                    (GridUnitType)
                                    Enum.Parse(typeof(GridUnitType),
                                               content.Attribute("UnitType").
                                                 Value,
                                               true),
                                }).ToList(),
                             GridSplitterPositions =
                               (from gridSplitter in
                                  template.Descendants("GridSplitter")
                                select new BPosition
                                {
                                  Col =
                                    int.Parse(gridSplitter.Attribute("Col").Value),
                                  Row =
                                    int.Parse(gridSplitter.Attribute("Row").Value),
                                  ColSpan =
                                    int.Parse(
                                    gridSplitter.Attribute("ColSpan").Value),
                                  RowSpan =
                                    int.Parse(
                                    gridSplitter.Attribute("RowSpan").Value),
                                  VerticalAlignment =
                                    (VerticalAlignment)
                                    Enum.Parse(typeof(VerticalAlignment),
                                               gridSplitter.Attribute(
                                                 "VerticalAlignment").Value, true),
                                  HorizontalAlignment =
                                    (HorizontalAlignment)
                                    Enum.Parse(typeof(HorizontalAlignment),
                                               gridSplitter.Attribute(
                                                 "HorizontalAlignment").Value,
                                               true),
                                }).ToList(),
                           }).ToList();
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    #endregion Private Methods

    #endregion Methods
  }
}