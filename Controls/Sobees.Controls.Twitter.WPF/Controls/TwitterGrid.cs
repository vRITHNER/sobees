#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Sobees.Controls.Twitter.Cls;
using Sobees.Controls.Twitter.ViewModel;
using Sobees.Tools.Logging;

#endregion

namespace Sobees.Controls.Twitter.Controls
{
  public class TwitterGrid : Grid, INotifyPropertyChanged
  {
    public static DependencyProperty TwitterWorkspacesProperty = DependencyProperty.Register("TwitterWorkspaces",
                                                                                             typeof (
                                                                                               ObservableCollection
                                                                                               <
                                                                                               TwitterWorkspaceViewModel
                                                                                               >), typeof (TwitterGrid),
                                                                                             null);

    public static DependencyProperty TwitterWorkspaceSettingsProperty =
      DependencyProperty.Register("TwitterWorkspaceSettings", typeof (ObservableCollection<TwitterWorkspaceSettings>),
                                  typeof (TwitterGrid), null);

    public static DependencyProperty DataTemplateProperty = DependencyProperty.Register("DataTemplate",
                                                                                        typeof (DataTemplate),
                                                                                        typeof (TwitterGrid), null);

    public TwitterGrid()
    {
      if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
        Loaded += TwitterGridLoaded;
      Unloaded += TwitterGridUnloaded;
    }

    public ObservableCollection<TwitterWorkspaceViewModel> TwitterWorkspaces
    {
      get
      {
        if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
          return new ObservableCollection<TwitterWorkspaceViewModel>();

        return GetValue(TwitterWorkspacesProperty) as ObservableCollection<TwitterWorkspaceViewModel>;
      }
      set { SetValue(TwitterWorkspacesProperty, value); }
    }

    public ObservableCollection<TwitterWorkspaceSettings> TwitterWorkspaceSettings
    {
      get
      {
        if (GetValue(TwitterWorkspaceSettingsProperty) == null)
          return new ObservableCollection<TwitterWorkspaceSettings>();

        return GetValue(TwitterWorkspaceSettingsProperty) as ObservableCollection<TwitterWorkspaceSettings>;
      }
      set { SetValue(TwitterWorkspaceSettingsProperty, value); }
    }

    public DataTemplate DataTemplate
    {
      get
      {
        if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
          return new DataTemplate();

        return GetValue(DataTemplateProperty) as DataTemplate;
      }
      set { SetValue(DataTemplateProperty, value); }
    }

    private void TwitterGridUnloaded(object sender, RoutedEventArgs e)
    {
#if !SILVERLIGHT
      Unloaded -= TwitterGridUnloaded;
#endif
      if (TwitterWorkspaces != null) TwitterWorkspaces.CollectionChanged -= TwitterWorkspacesCollectionChanged;
    }

    //    private void TwitterGridUnloaded(object sender, RoutedEventArgs e)
    //    {
    //#if !SILVERLIGHT
    //      Unloaded -= TwitterGridUnloaded;
    //#endif
    //      TwitterWorkspaces.CollectionChanged -= TwitterWorkspacesCollectionChanged;
    //      if (TwitterWorkspaces != null)
    //      {
    //        TwitterWorkspaces.Clear();
    //        TwitterWorkspaces = null;
    //      }
    //      if (TwitterWorkspaceSettings != null)
    //      {
    //        TwitterWorkspaceSettings.Clear();
    //        TwitterWorkspaceSettings = null;
    //      }
    //      foreach (var child in Children)
    //      {
    //        if (typeof(GridSplitter).Equals(child.GetType()))
    //        {
    //#if !SILVERLIGHT
    //          ((GridSplitter)child).DragCompleted -= GsDragCompleted;
    //#else
    //           ((GridSplitter)child).MouseLeftButtonUp -= new MouseButtonEventHandler(gs_MouseLeftButtonUp);
    //#endif
    //        }
    //        else
    //        {
    //          ((ContentControl) child).Content = null;
    //        }
    //      }
    //      Children.Clear();
    //      SetValue(TwitterWorkspacesProperty, null);
    //      SetValue(TwitterWorkspaceSettingsProperty, null);
    //    }

    private void TwitterGridLoaded(object sender, RoutedEventArgs e)
    {
      Loaded -= TwitterGridLoaded;
      HorizontalAlignment = HorizontalAlignment.Stretch;
      VerticalAlignment = VerticalAlignment.Stretch;
      TwitterWorkspaces.CollectionChanged += TwitterWorkspacesCollectionChanged;

      BuildGrid();
    }

    private void TwitterWorkspacesCollectionChanged(object sender,
                                                    NotifyCollectionChangedEventArgs e)
    {
      if (e == null) return;

#if !SILVERLIGHT
      if (e.Action.Equals(NotifyCollectionChangedAction.Move))
#else
      if (e.Action.ToString().ToLower().Equals("move"))
#endif
      {
        if (e.NewItems != null)

          Debug.WriteLine(string.Format("Action||OldItems|Index||NewItems|Index:{0}||{1}|{2}||{3}|{4}",
                                        e.Action, e.OldItems.Count, e.OldStartingIndex, e.NewItems.Count,
                                        e.NewStartingIndex));

        foreach (var child in Children)
        {
          if (child.GetType() == typeof (ContentControl))
          {
            var cc = child as ContentControl;
            if (cc == null) continue;

            // Re-attribute correct ColumnInGrid property to each Twitter workspace!
            var twvm = cc.Content as TwitterWorkspaceViewModel;
            if (twvm != null)
            {
              if (twvm.WorkspaceSettings.ColumnInGrid == e.OldStartingIndex)
              {
                twvm.WorkspaceSettings.ColumnInGrid = e.NewStartingIndex;
                SetColumn(cc,
                          twvm.WorkspaceSettings.ColumnInGrid);
                continue;
              }

              if (twvm.WorkspaceSettings.ColumnInGrid == e.NewStartingIndex)
              {
                twvm.WorkspaceSettings.ColumnInGrid = e.OldStartingIndex;
                SetColumn(cc,
                          twvm.WorkspaceSettings.ColumnInGrid);
                continue;
              }
            }
          }
        }
      }

      if (e.Action.Equals(NotifyCollectionChangedAction.Remove))
      {
        // Remove item from GRID
        foreach (var item in e.OldItems)
        {
          var twvmToRemove = item as TwitterWorkspaceViewModel;
          if (twvmToRemove == null) continue;

          var columnRemoved = twvmToRemove.WorkspaceSettings.ColumnInGrid;
          if (ColumnDefinitions.Count >= columnRemoved)
          {
            ColumnDefinitions.RemoveAt(columnRemoved);
          }

          var toRemove = new List<UIElement>();
          foreach (var child in Children)
          {
            if (child.GetType() == typeof (ContentControl))
            {
              var cc = child as ContentControl;
              if (cc == null) continue;

              // Re-attribute correct ColumnInGrid property to each Twitter workspace!
              var twvm = cc.Content as TwitterWorkspaceViewModel;
              if (twvm != null)
              {
                if (twvm.WorkspaceSettings.ColumnInGrid > columnRemoved)
                {
                  twvm.WorkspaceSettings.ColumnInGrid = twvm.WorkspaceSettings.ColumnInGrid - 1;
                  SetColumn(cc,
                            twvm.WorkspaceSettings.ColumnInGrid);
                }
              }

              if (cc.Content.Equals(item))
                toRemove.Add(child as UIElement);
            }

            if (child.GetType() == typeof (GridSplitter))
            {
              var gs = child as GridSplitter;
              if (gs != null)
                if (GetColumn(gs) ==
                    ColumnDefinitions.Count - 1)
                  toRemove.Add(gs);
            }
          }

          foreach (var uiElement in toRemove)
          {
            Children.Remove(uiElement);
          }
        }
        ResetColumnsWidth();
      }
      else if (e.Action.Equals(NotifyCollectionChangedAction.Add))
      {
        // Add item to the Grid

        if (e.NewItems != null)
          foreach (var item in e.NewItems)
          {
            if (item.GetType() == typeof (TwitterWorkspaceViewModel))
            {
              var twvm = item as TwitterWorkspaceViewModel;
              if (twvm == null) return;

              AddColumn(twvm.WorkspaceSettings.ColumnInGrid);

              if (twvm.WorkspaceSettings.ColumnInGridWidth != -1)
                ColumnDefinitions[twvm.WorkspaceSettings.ColumnInGrid].Width =
                  new GridLength(twvm.WorkspaceSettings.ColumnInGridWidth,
                                 GridUnitType.Star);
              else
                ResetColumnsWidth();

              AddContentControl(twvm);
            }
          }
      }
    }

    private void AddColumn(int columnInGrid)
    {
      var cd = new ColumnDefinition();
      ColumnDefinitions.Add(cd);

      if (columnInGrid > 0 &&
          columnInGrid < TwitterWorkspaceSettings.Count)
      {
        var gs = new GridSplitter {Width = 5};

        SetZIndex(gs,
                  10);

        SetColumn(gs,
                  columnInGrid);
        gs.VerticalAlignment = VerticalAlignment.Stretch;
        gs.HorizontalAlignment = HorizontalAlignment.Left;
        gs.ShowsPreview = false;

        gs.DragCompleted += GsDragCompleted;

        Children.Add(gs);
      }
    }

    private void GsDragCompleted(object sender, DragCompletedEventArgs e)
    {
      ChangeColumnsWidth();
    }

    private void ChangeColumnsWidth()
    {
      foreach (var child in Children)
      {
        var t = child.GetType();
        if (t == typeof (ContentControl))
        {
          var cc = child as ContentControl;
          if (cc == null) return;

          var fe = child as FrameworkElement;
 
          var content = (from ts in TwitterWorkspaceSettings
                         where ts.ColumnInGrid.Equals(GetColumn(fe))
                         select ts);

          if (content.Any())
          {
            var tw = content.First();
            if (tw != null)
            {
              var w = ColumnDefinitions[GetColumn(fe)].ActualWidth;

              if (w > 0.0 &&
                  ActualWidth > 0.0)
                w = w/ActualWidth;
              else
                w = -1.0;

              //Change width in TwitterSettings
              tw.ColumnInGridWidth = w;

              //Change width in Settings of TwitterWorkspaceViewModel contained in current ContentControl
              var twvm = cc.Content as TwitterWorkspaceViewModel;
              if (twvm != null)
                twvm.WorkspaceSettings.ColumnInGridWidth = w;
            }
          }
        }
      }
    }

    private void ResetColumnsWidth()
    {
      if (TwitterWorkspaces == null)
      {
        return;
      }
      foreach (var workspace in TwitterWorkspaces)
        ColumnDefinitions[workspace.WorkspaceSettings.ColumnInGrid].Width = new GridLength(1,
                                                                                           GridUnitType.Star);

      ChangeColumnsWidth();
    }

    private void BuildGrid()
    {
      try
      {
        RowDefinitions.Clear();
        ColumnDefinitions.Clear();
        Children.Clear();

        foreach (var workspace in TwitterWorkspaces)
        {
          AddColumn(workspace.WorkspaceSettings.ColumnInGrid);
          AddContentControl(workspace);
        }
        UpdateLayout();
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this,
                          ex);
      }
    }

    private void AddContentControl(BTwitterViewModel tworkspace)
    {
      try
      {
        var cc = new ContentControl
                   {
                     Content = tworkspace,
                     HorizontalAlignment = HorizontalAlignment.Stretch,
                     VerticalAlignment = VerticalAlignment.Stretch,
                     HorizontalContentAlignment = HorizontalAlignment.Stretch,
                     VerticalContentAlignment = VerticalAlignment.Stretch,
                     ContentTemplate = DataTemplate
                   };

        SetColumn(cc,
                  tworkspace.WorkspaceSettings.ColumnInGrid);
        SetRow(cc,
               0);
        SetColumnSpan(cc,
                      1);
        SetRowSpan(cc,
                   1);

        Children.Add(cc);
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this,
                          ex);
      }
    }

    #region INotifyPropertyChanged Members

    /// <summary>
    ///   Raised when a property on this object has a new value.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    ///   Raises this object's PropertyChanged event.
    /// </summary>
    /// <param name="propertyName">The property that has a new value.</param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
      var handler = PropertyChanged;
      if (handler != null)
      {
        var e = new PropertyChangedEventArgs(propertyName);
        handler(this, e);
      }
    }

    #endregion INotifyPropertyChanged Members
  }
}