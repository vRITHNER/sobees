using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Infrastructure.Model;
using Sobees.Infrastructure.ViewModelBase;
using Sobees.Tools.Logging;
using System.Windows.Controls.Primitives;

namespace Sobees.Infrastructure.Controls
{
  public class BServiceGrid : Grid, INotifyPropertyChanged
  {
    public static DependencyProperty BTemplateProperty = DependencyProperty.Register("BTemplate", typeof(BTemplate), typeof(BServiceGrid), null);
    public bool IsMaximize { get; set; }
    public BTemplate BTemplate
    {
      get
      {

        if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
          return new BTemplate();

        if (GetValue(BTemplateProperty) == null)
        {
          // If BTemplateProperty is not set, return a one-cell-grid!
          var bt = new BTemplate
          {
            BPositions = new List<BPosition> { new BPosition(0, 0, 1, 1) },
            //Columns = 1,
            Columns = 3,
            Rows = 1,
            GridSplitterPositions = new List<BPosition>()
          };

          return bt;
        }

        return GetValue(BTemplateProperty) as BTemplate;
      }
      set
      {
        SetValue(BTemplateProperty, value);
        OnPropertyChanged("BTemplate");
      }
    }

    public static DependencyProperty WorkspacesProperty = DependencyProperty.Register("Workspaces", typeof(ObservableCollection<BServiceWorkspaceViewModel>), typeof(BServiceGrid), null);
    private ObservableCollection<UIElement> _oldElement;

    private ObservableCollection<UIElement> OldElement
    {
      get { return _oldElement ?? (_oldElement = new ObservableCollection<UIElement>()); }
      set
      {
        _oldElement = value;
      }
    }

    public ObservableCollection<BServiceWorkspaceViewModel> Workspaces
    {
      get { return GetValue(WorkspacesProperty) as ObservableCollection<BServiceWorkspaceViewModel>; }
      set { SetValue(WorkspacesProperty, value); }
    }

    public BServiceGrid()
    {

      if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
        Loaded += BGridLoaded;
      Unloaded += BGridUnLoaded;
      //Register to get all the PropertyChangedMessages broadcasted.
      Messenger.Default.Register<string>(this, DoAction);
    }

    private void DoAction(string param)
    {
      switch (param)
      {
        case "Maximize":
          IsMaximize = true;
          Maximize();
          break;
        case "Minimize":
          IsMaximize = false;
          Minimize();
          break;
        case "OpenChangeTemplate1":
          IsChangeTemplateOpen = true;
          break;
        case "CloseChangeTemplate1":
          IsChangeTemplateOpen = false;

          break;

        default:
          break;
      }

    }

    void BGridLoaded(object sender, RoutedEventArgs e)
    {
      Loaded -= BGridLoaded;
      HorizontalAlignment = HorizontalAlignment.Stretch;
      VerticalAlignment = VerticalAlignment.Stretch;

      BTemplate.PropertyChanged += BTemplatePropertyChanged;
      if (Workspaces != null)
      {
        Workspaces.CollectionChanged += WorkspacesCollectionChanged;
      }
      else
      {

      }

      BuildGrid();
    }

    void BGridUnLoaded(object sender, RoutedEventArgs e)
    {
      Unloaded += BGridUnLoaded;
      if (BTemplate != null) BTemplate.PropertyChanged -= BTemplatePropertyChanged;
      if (Workspaces != null) Workspaces.CollectionChanged -= WorkspacesCollectionChanged;

      //BuildGrid();
    }

    void BTemplatePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      BTemplate = sender as BTemplate;
      BuildGrid();
    }

    void WorkspacesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      try
      {
        if (IsChangeTemplateOpen) return;
        if (Workspaces != null) TraceHelper.Trace(this, "WorkspacesCollectionChanged --> " + Workspaces.Count);

        if (e.Action.Equals(NotifyCollectionChangedAction.Remove))
        {
          // Remove item from GRID
          foreach (var item in e.OldItems)
          {
            var toRemove = new List<UIElement>();
            foreach (var child in Children)
            {
              if (child.GetType().Equals(typeof(ContentControl)))
              {
                var cc = child as ContentControl;
                if (cc == null) continue;

                if (cc.Content.GetType().Equals(item.GetType()))
                {
                  if (cc.Content.Equals(item))
                    toRemove.Add(child as UIElement);
                }
              }
            }

            foreach (var uiElement in toRemove)
            {
              Children.Remove(uiElement);
            }
          }
        }
        else if (e.Action.Equals(NotifyCollectionChangedAction.Add))
        {
          // Add item to the Grid

          //Check if there is a space in the grid where we can add the content
          //if (BTemplate.BPositions.Count <= GetNbOfElementInGrid())
          //{
          //  TraceHelper.Trace(this, "No more place in bGrid!");
          //  return;
          //}

          foreach (var item in e.NewItems)
          {
            if (item.GetType().BaseType.Equals(typeof(BServiceWorkspaceViewModel)))
            {
              var wvm = item as BServiceWorkspaceViewModel;
              if (wvm == null) return;

              AddContentControl(wvm);
            }
          }
        }
        else if (e.Action.Equals(NotifyCollectionChangedAction.Reset))
        {
          var childToRemove = new List<ContentControl>();
          foreach (var child in Children)
          {
            if (child.GetType().Equals(typeof(ContentControl)))
              childToRemove.Add(child as ContentControl);
          }

          foreach (var toRemove in childToRemove)
          {
            Children.Remove(toRemove);
          }
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    protected bool IsChangeTemplateOpen
    {
      get;
      set;
    }

    void BuildGrid()
    {
      this.Visibility = System.Windows.Visibility.Collapsed;
      try
      {
        if (!IsMaximize)
        {
          RowDefinitions.Clear();
          ColumnDefinitions.Clear();
          OldElement.Clear();
          foreach (var child in Children)
          {
            if (child.GetType().Equals(typeof(GridSplitter))) continue;
            OldElement.Add((UIElement)child);
          }
          Children.Clear();

          for (var i = 0; i < BTemplate.Rows; i++)
          {
            var rd = new RowDefinition();
            //rd.MinHeight = 150;

            RowDefinitions.Add(rd);
          }
          for (var i = 0; i < BTemplate.Columns; i++)
          {
            var cd = new ColumnDefinition();
            //cd.MinWidth = 150;

            ColumnDefinitions.Add(cd);
          }

          foreach (var gridSplitterPosition in BTemplate.GridSplitterPositions)
          {
            const double gridSplitterWidth = 10;
            var gs = new GridSplitter();
            Canvas.SetZIndex(gs, 10);
            if (gridSplitterPosition.VerticalAlignment.Equals(VerticalAlignment.Stretch))
            {
              gs.Width = gridSplitterWidth;

              gs.Style = TryFindResource("GridSplitterStyleDeckVertical") as Style;
            }
            else
            {
              gs.Height = gridSplitterWidth;

              gs.Style = TryFindResource("GridSplitterStyleDeckHorizontal") as Style;
            }

            Children.Add(gs);
            BPosition.SetPosition(gs, gridSplitterPosition);
            gs.VerticalAlignment = gridSplitterPosition.VerticalAlignment;
            gs.HorizontalAlignment = gridSplitterPosition.HorizontalAlignment;
            gs.ShowsPreview = false;
            gs.DragCompleted += gs_DragCompleted;
          }

          foreach (var BPosition in BTemplate.BPositions)
          {
            try
            {
              if (BPosition.Width != -1 && ActualWidth > 0.0)
                ColumnDefinitions[BPosition.Col].Width = new GridLength(BPosition.Width / ActualWidth, BPosition.UnitType);
              else
                ColumnDefinitions[BPosition.Col].Width = new GridLength(1.0, BPosition.UnitType);

              if (BPosition.Height != -1 && ActualHeight > 0.0)
                RowDefinitions[BPosition.Row].Height = new GridLength(BPosition.Height / ActualHeight, BPosition.UnitType);
              else
                RowDefinitions[BPosition.Row].Height = new GridLength(1.0, BPosition.UnitType);
            }
            catch (Exception ex)
            {
              TraceHelper.Trace(this, ex);
              TraceHelper.Trace(this, "BPosition: Col " + BPosition.Col);
              TraceHelper.Trace(this, "BPosition: Row " + BPosition.Row);
              TraceHelper.Trace(this, "ColumnDefinitions " + ColumnDefinitions.Count);
              TraceHelper.Trace(this, "RowDefinitions" + RowDefinitions.Count);
            }
          }

          if (Workspaces != null)
            foreach (var workspace in Workspaces)
            {
              if (!workspace.IsClosed)
                AddContentControl(workspace);
            }
          UpdateLayout();
        }
        OldElement.Clear();
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
      this.Visibility = System.Windows.Visibility.Visible;
    }
    public void Minimize()
    {
      if (Workspaces != null)
        foreach (var model in Workspaces)
        {
          if (model.IsMaxiMize) return;
        }
      foreach (var BPosition in BTemplate.BPositions)
      {
        try
        {
          if (BPosition.Width != -1 && ActualWidth > 0.0)
            ColumnDefinitions[BPosition.Col].Width = new GridLength(BPosition.Width / ActualWidth, BPosition.UnitType);
          else
            ColumnDefinitions[BPosition.Col].Width = new GridLength(1.0, BPosition.UnitType);

          if (BPosition.Height != -1 && ActualHeight > 0.0)
            RowDefinitions[BPosition.Row].Height = new GridLength(BPosition.Height / ActualHeight, BPosition.UnitType);
          else
            RowDefinitions[BPosition.Row].Height = new GridLength(1.0, BPosition.UnitType);
        }
        catch (Exception ex)
        {
          TraceHelper.Trace(this, ex);
          TraceHelper.Trace(this, "BPosition: Col " + BPosition.Col);
          TraceHelper.Trace(this, "BPosition: Row " + BPosition.Row);
          TraceHelper.Trace(this, "ColumnDefinitions " + ColumnDefinitions.Count);
          TraceHelper.Trace(this, "RowDefinitions" + RowDefinitions.Count);
        }
      }
      foreach (var child in Children)
      {
        ((UIElement)child).Visibility = Visibility.Visible;

      }
    }

    public void Maximize()
    {
      try
      {
        var ismaxi = false;
        if (Workspaces != null)
          foreach (var model in Workspaces)
          {
            if (model.IsMaxiMize) ismaxi = true;
          }
        if (!ismaxi)
        {
          return;
        }
        foreach (var child in Children)
        {
          if (child.GetType().Equals(typeof(GridSplitter)))
          {
            ((UIElement)child).Visibility = Visibility.Collapsed;
            continue;
          }
          var cc = ((ContentControl)child);
          var service = ((BServiceWorkspaceViewModel)cc.Content);
          if (!service.IsMaxiMize)
          {
            cc.Visibility = Visibility.Collapsed;
            ColumnDefinitions[service.PositionInGrid.Col].Width = new GridLength(0);
            RowDefinitions[service.PositionInGrid.Row].Height = new GridLength(0);
          }
        }
        foreach (var child in Children)
        {
          if (child.GetType().Equals(typeof(GridSplitter))) continue;
          var cc = ((ContentControl)child);
          var service = ((BServiceWorkspaceViewModel)cc.Content);
          if (service.IsMaxiMize)
          {
            ColumnDefinitions[service.PositionInGrid.Col].Width = new GridLength(1, GridUnitType.Star);
            RowDefinitions[service.PositionInGrid.Row].Height = new GridLength(1, GridUnitType.Star);
          }
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }
    }
    void gs_DragCompleted(object sender, DragCompletedEventArgs e)
    {
      ChangeColumnsWidthRowsHeight();
    }
    private void ChangeColumnsWidthRowsHeight()
    {
      var changeColumnsWidthRowsHeight = false;
      foreach (var childa in Children)
      {
        if (childa.GetType().Equals(typeof(ContentControl)))
        {
          var cc = childa as ContentControl;
          if (cc == null) return;

          if (cc.Content == null) return;
          if (cc.Content.GetType() == null) return;
          if (cc.Content.GetType().BaseType == null) return;
          if (cc.Content.GetType().BaseType.Equals(typeof(BServiceWorkspaceViewModel)))
          {
            if (((BServiceWorkspaceViewModel)cc.Content).IsServiceWorkspace)
            {
              changeColumnsWidthRowsHeight = true;
              break;
            }
          }
        }
      }

      if (!changeColumnsWidthRowsHeight) return;

      foreach (var child in Children)
      {
        if (child.GetType().Equals(typeof(ContentControl)))
        {
          var cc = child as ContentControl;
          if (cc == null) return;

          var fe = child as FrameworkElement;
          if (fe == null) continue;

          var content = (from pos in BTemplate.BPositions
                         where
                           pos.Row.Equals(GetRow(fe)) && pos.Col.Equals(GetColumn(fe)) &&
                           pos.RowSpan.Equals(GetRowSpan(fe)) && pos.ColSpan.Equals(GetColumnSpan(fe))
                         select pos);

          if (content.Count() > 0)
          {
            var bp = content.First();
            if (bp != null)
            {
              //Change width and height in BPositions of BTemplate
              bp.Width = ColumnDefinitions[GetColumn(fe)].ActualWidth;
              bp.Height = RowDefinitions[GetRow(fe)].ActualHeight;

              //Change width and height in BPosition of ServiceWorkspaceViewModel contained in current ContentControl
              var swvm = cc.Content as BServiceWorkspaceViewModel;
              if (swvm != null)
              {
                swvm.PositionInGrid.Width = bp.Width;
                swvm.PositionInGrid.Height = bp.Height;
              }
            }
          }
        }
      }
    }

    private void AddContentControl(BServiceWorkspaceViewModel workspace)
    {
      try
      {
        ContentControl oldCc = null;
        foreach (var element in OldElement)
        {
          if (((ContentControl) element).Content == workspace)
          {
            oldCc = (ContentControl) element;
          }
        }
        if (oldCc != null)
        {
          SetColumn(oldCc, workspace.PositionInGrid.Col);
          SetRow(oldCc, workspace.PositionInGrid.Row);
          SetColumnSpan(oldCc, workspace.PositionInGrid.ColSpan);
          SetRowSpan(oldCc, workspace.PositionInGrid.RowSpan);
          Children.Add(oldCc);
          OldElement.Remove(oldCc);
          return;
        }
        var cc = new ContentControl
                   {
                     //Name = "ccService",
                     Content = workspace,
                     HorizontalAlignment = HorizontalAlignment.Stretch,
                     VerticalAlignment = VerticalAlignment.Stretch,
                     HorizontalContentAlignment = HorizontalAlignment.Stretch,
                     VerticalContentAlignment = VerticalAlignment.Stretch,
                   };
        cc.SetBinding(ContentControl.ContentTemplateProperty,
                      new Binding("DataTemplateView") {Source = workspace});

        SetColumn(cc, workspace.PositionInGrid.Col);
        SetRow(cc, workspace.PositionInGrid.Row);
        SetColumnSpan(cc, workspace.PositionInGrid.ColSpan);
        SetRowSpan(cc, workspace.PositionInGrid.RowSpan);

        Children.Add(cc);
        cc.DragOver += cc_DragOver;
        cc.DragLeave += cc_DragLeave;
        cc.Drop += cc_Drop;

      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    private void cc_Drop(object sender,
                         DragEventArgs e)
    {
      try
      {
        if (!e.Data.GetDataPresent(typeof(BPosition)) || e.Source == null)
        {
          e.Effects = DragDropEffects.None;
          return;
        }

        e.Effects = e.AllowedEffects & DragDropEffects.Move;
        DropTargetHelper.Drop(e.Data,
                              e.GetPosition(this),
                              e.Effects);

        var fromBPosition = e.Data.GetData(typeof(BPosition)) as BPosition;
        if (fromBPosition == null) return;

        var destContentControl = e.Source as ContentControl;
        if (destContentControl == null) return;

        var destServiceModelView = destContentControl.Content as BServiceWorkspaceViewModel;
        if (destServiceModelView == null) return;

        var destBPosition = destServiceModelView.PositionInGrid;
        if (destBPosition == null) return;
        var originalUiElement =
            Children.Cast<UIElement>().Where(
                child => child.GetType() == typeof(ContentControl)).FirstOrDefault(
                    child =>
                    GetColumn(child) == fromBPosition.Col &&
                    GetRow(child) == fromBPosition.Row);

        var originalContentControl = originalUiElement as ContentControl;
        if (originalContentControl == null) return;
        var originalServiceWorkspaceViewModel = originalContentControl.Content as BServiceWorkspaceViewModel;
        if (originalServiceWorkspaceViewModel == null) return;



        MoveService(fromBPosition, destContentControl, destServiceModelView, destBPosition, originalContentControl, originalServiceWorkspaceViewModel);
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this,
                          ex);
      }
      e.Handled = true;
    }


    private void cc_DragOver(object sender,
                             DragEventArgs e)
    {
      try
      {
        if (!e.Data.GetDataPresent(typeof(BPosition)))
        {
          e.Effects = DragDropEffects.None;
          return;
        }
        if (e.Source == null) return;

        e.Effects = e.AllowedEffects & DragDropEffects.Move;
        DropTargetHelper.DragOver(e.GetPosition(this),
                              e.Effects);

        var fromBPosition = e.Data.GetData(typeof(BPosition)) as BPosition;
        if (fromBPosition == null) return;

        var currentContentControl = e.Source as ContentControl;
        if (currentContentControl == null) return;

        var currentServiceModelView = currentContentControl.Content as BServiceWorkspaceViewModel;
        if (currentServiceModelView == null) return;

        var destBPosition = currentServiceModelView.PositionInGrid;

        if (currentServiceModelView.PositionInGrid.Col == fromBPosition.Col &&
                      currentServiceModelView.PositionInGrid.Row == fromBPosition.Row)
        {
          e.Effects = DragDropEffects.None;
          TraceHelper.Trace("cc_DragOver:",
                            $"cc_DragOver:currentServiceModelView.PositionInGrid.Col|fromBPosition.Col|destBPosition.Col::{currentServiceModelView.PositionInGrid.Col}|{fromBPosition.Col}|{destBPosition.Col}:");
        }
        else
        {
          e.Effects = e.AllowedEffects & DragDropEffects.Move;
        }

        DropTargetHelper.DragOver(e.GetPosition(this),
                                  e.Effects);

        e.Handled = true;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this,
                          ex);
      }
    }

    void cc_DragLeave(object sender, DragEventArgs e)
    {
      DropTargetHelper.DragLeave(e.Data);
      e.Handled = true;
    }


    private void MoveService(BPosition fromBPosition, ContentControl destContentControl, BServiceWorkspaceViewModel destServiceModelView, BPosition destBPosition, ContentControl originalUiElement, BServiceWorkspaceViewModel originalServiceWorkspaceViewModel)
    {
      if (BTemplate.Rows == 1)
      {
        if (destBPosition.Col > fromBPosition.Col)
        {
          //ex:  3 > 1
          for (var offset = fromBPosition.Col; offset < destBPosition.Col; offset++)
          {
            SwapServices(offset, offset + 1);
          }
        }
        else
        {
          //ex:  0 < 2
          for (var offset = fromBPosition.Col; offset > destBPosition.Col; offset--)
          {
            SwapServices(offset - 1, offset);
          }
        }
      }
      else
      {

        SetColumn(originalUiElement, destBPosition.Col);
        SetColumnSpan(originalUiElement, destBPosition.ColSpan);
        SetColumn(destContentControl, fromBPosition.Col);
        SetColumnSpan(destContentControl, fromBPosition.ColSpan);
        SetRow(originalUiElement, destBPosition.Row);
        SetRowSpan(originalUiElement, destBPosition.RowSpan);
        SetRow(destContentControl, fromBPosition.Row);
        SetRowSpan(destContentControl, fromBPosition.RowSpan);
        originalServiceWorkspaceViewModel.PositionInGrid = destBPosition;
        destServiceModelView.PositionInGrid = fromBPosition;

      }
    }


    private void SwapServices(int fromCol, int destCol)
    {
      //TODO: Manage Rows
      try
      {
        var fromControl =
          Children.Cast<FrameworkElement>().Where(
                                            child => child.GetType() == typeof(ContentControl)).
            FirstOrDefault(child => GetColumn(child) == fromCol && GetRow(child) == 0)
          as ContentControl;

        var destControl =
          Children.Cast<FrameworkElement>().Where(
                                            child => child.GetType() == typeof(ContentControl)).
            FirstOrDefault(child => GetColumn(child) == destCol && GetRow(child) == 0)
          as ContentControl;

        var fromServiceViewModel = fromControl.Content as BServiceWorkspaceViewModel;
        var destServiceViewModel = destControl.Content as BServiceWorkspaceViewModel;
        var fromBPosition = fromServiceViewModel.PositionInGrid;
        var destBPosition = destServiceViewModel.PositionInGrid;

        // Update the swapping panel row and column
        SetColumn(fromControl,
                  destCol);
        SetRow(fromControl,
               0);

        // Update the dragging panel row and column
        SetColumn(destControl,
                  fromCol);
        SetRow(destControl,
               0);

        // Animate the layout to the new positions
        //this.AnimatePanelLayout();
        fromServiceViewModel.PositionInGrid = destBPosition;
        destServiceViewModel.PositionInGrid = fromBPosition;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this,
                          (ex));
      }

    }

    private int GetNbOfElementInGrid()
    {
      var nbGs = 0;
      foreach (var child in Children)
      {
        if (child.GetType().Equals(typeof(GridSplitter)))
          ++nbGs;
      }

      return Children.Count - nbGs;
    }

    #region INotifyPropertyChanged Members

    /// <summary>
    /// Raised when a property on this object has a new value.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raises this object's PropertyChanged event.
    /// </summary>
    /// <param name="propertyName">The property that has a new value.</param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
      var handler = PropertyChanged;
      if (handler == null) return;
      var e = new PropertyChangedEventArgs(propertyName);
      handler(this, e);
    }

    #endregion // INotifyPropertyChanged Members


    #region Window drag handlers

    //
    // These provide a drag image while dragging over the Window
    //
    protected override void OnDragEnter(DragEventArgs e)
    {
      try
      {
        e.Effects = DragDropEffects.None;
        DropTargetHelper.DragEnter(null, e.Data, e.GetPosition(this), DragDropEffects.None);
        e.Handled = true;
        base.OnDragEnter(e);
      }
      catch (Exception exception)
      {
        Console.WriteLine(exception);
      }
    }

    protected override void OnDragOver(DragEventArgs e)
    {
      e.Effects = DragDropEffects.None;
      DropTargetHelper.DragOver(e.GetPosition(this), DragDropEffects.None);
      e.Handled = true;
      base.OnDragOver(e);
    }

    protected override void OnDragLeave(DragEventArgs e)
    {
      DropTargetHelper.DragLeave(e.Data);
      e.Handled = true;
      base.OnDragLeave(e);
    }

    protected override void OnDrop(DragEventArgs e)
    {
      e.Effects = DragDropEffects.None;
      DropTargetHelper.Drop(e.Data, e.GetPosition(this), DragDropEffects.None);
      e.Handled = true;
      base.OnDrop(e);
    }

    #endregion // Window drag handlers



  }
}