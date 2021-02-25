#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Sobees.Infrastructure.Controls;
using Sobees.Infrastructure.Model;
using Sobees.Infrastructure.ViewModelBase;
using Sobees.Tools.Logging;
using Sobees.ViewModel;

#endregion

namespace Sobees.Cls
{
  public class BDragGrid : Grid, INotifyPropertyChanged
  {
    public static DependencyProperty BTemplateProperty = DependencyProperty.Register("BTemplate", typeof (BTemplate),
      typeof (BDragGrid), new PropertyMetadata(BTemplateChanged));

    private static void BTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((BDragGrid) d).BuildGrid();
    }

    public BTemplate BTemplate
    {
      get
      {
        if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
          return new BTemplate();

        if (GetValue(BTemplateProperty) != null) return GetValue(BTemplateProperty) as BTemplate;
        // If BTemplateProperty is not set, return a one-cell-grid!
        var bt = new BTemplate
        {
          BPositions = new List<BPosition> {new BPosition(0, 0, 1, 1)},
          Columns = 1,
          Rows = 1,
          GridSplitterPositions = new List<BPosition>()
        };

        return bt;
      }
      set
      {
        SetValue(BTemplateProperty, value);
        OnPropertyChanged("bTemplate");
      }
    }

    public BDragGrid()
    {
      if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
        Loaded += bGrid_Loaded;
    }

    private void bGrid_Loaded(object sender, RoutedEventArgs e)
    {
      HorizontalAlignment = HorizontalAlignment.Stretch;
      VerticalAlignment = VerticalAlignment.Stretch;

      //BuildGrid();
    }

    private void BuildGrid()
    {
      try
      {
        RowDefinitions.Clear();
        ColumnDefinitions.Clear();
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

        foreach (var pos in BTemplate.BPositions)
        {
          AddContentControl(null, pos);
        }
        UpdateLayout();
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    private void AddContentControl(object content, BPosition positionInGrid)
    {
      try
      {
        var cc = new ContentControl();

#if !SILVERLIGHT
        cc.AllowDrop = true;
        cc.Drop += CcDrop;
        cc.DragOver += CcDragOver;

        cc.Style = TryFindResource("ccStyle") as Style;
#else

				RadDragAndDropManager.SetAllowDrop(cc, true);
				RadDragAndDropManager.SetAllowDrag(cc, true);

				// Allow dropping into the ListBox and GridView only if the dragged item is a BServiceWorkspace
				RadDragAndDropManager.AddDropQueryHandler(cc, OnDropQuery);
				// Change the drag cue and choose an action for the sucessful drop
				RadDragAndDropManager.AddDropInfoHandler(cc, OnDropInfo);

				// Allow dragging of the Wishlist and Order items:
				RadDragAndDropManager.AddDragQueryHandler(cc, OnDragQuery);

				// Handle the case when items are dragged away from  the ListBox
				// and the Order:
				RadDragAndDropManager.AddDragInfoHandler(cc, OnDragInfo);

        cc.Style = SobeesApplication.Current.Resources["ccStyle"] as Style;
#endif
        cc.Content = new UcDragTemplate();

        cc.HorizontalAlignment = HorizontalAlignment.Stretch;
        cc.VerticalAlignment = VerticalAlignment.Stretch;
        cc.HorizontalContentAlignment = HorizontalAlignment.Stretch;
        cc.VerticalContentAlignment = VerticalAlignment.Stretch;

        SetColumn(cc, positionInGrid.Col);
        SetRow(cc, positionInGrid.Row);
        SetColumnSpan(cc, positionInGrid.ColSpan);
        SetRowSpan(cc, positionInGrid.RowSpan);

        Children.Add(cc);
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

#if SILVERLIGHT
  	private void OnDragInfo(object sender, DragDropEventArgs e)
  	{
			var cc = sender as ContentControl;
			var draggedItem = e.Options.Payload as BServiceWorkspaceViewModel;

			if (e.Options.Status == DragStatus.DragInProgress)
			{
				//Set up a drag cue:
				ContentControl cue = new ContentControl();
				cue.ContentTemplate = cc.ContentTemplate;
				cue.Content = draggedItem;
				e.Options.DragCue = cue;
				e.Options.ArrowCue = RadDragAndDropManager.GenerateArrowCue();
				e.Options.ArrowCue.Background = new SolidColorBrush(Colors.Red);
				e.Options.ArrowCue.BorderBrush = new SolidColorBrush(Colors.Red);
			}
  	}

  	private void OnDragQuery(object sender, DragDropQueryEventArgs e)
  	{
			var cc = sender as ContentControl;
			if (cc != null)
			{
				var serviceViewModel = cc.Content as BServiceWorkspaceViewModel;
				e.Options.Payload = serviceViewModel;
			}

			e.QueryResult = true;
			e.Handled = true;
  	}

  	private void OnDropInfo(object sender, DragDropEventArgs e)
  	{
			ContentControl ccDestination = e.Options.Destination as ContentControl;
			//if (ccDestination == null) return;
			var draggedItem = e.Options.Payload as BServiceWorkspaceViewModel;

			// Get the drag cu that the TreeView or we have created
			var cue = e.Options.ArrowCue;

			if (e.Options.Status == DragStatus.DropPossible)
			{
				cue.Background = new SolidColorBrush(Colors.Green);
				cue.BorderBrush = new SolidColorBrush(Colors.Green);
			}
			else if (e.Options.Status == DragStatus.DropImpossible)
			{
				cue.Background = new SolidColorBrush(Colors.Red);
				cue.BorderBrush = new SolidColorBrush(Colors.Red);
			}
			else if (e.Options.Status == DragStatus.DropComplete)
			{
				// Check if there's already another BServiceWorkspaceViewModel!
				if(ccDestination.Content.GetType() != null && ccDestination.Content.GetType().BaseType.Equals(typeof(BServiceWorkspaceViewModel)))
				{
					if (e.Options.Source.GetType().Equals(typeof(ContentControl)))
					{
						(e.Options.Source as ContentControl).Content = ccDestination.Content;
					}
					ccDestination.Content = draggedItem;
				}
				else
				{
					ccDestination.ContentTemplate = (e.Options.DragCue as ContentControl).ContentTemplate;
					ccDestination.Content = draggedItem;

					if(e.Options.Source != null && e.Options.Source.GetType().Equals(typeof(ContentControl)))
					{
						(e.Options.Source as ContentControl).ContentTemplate = null;
						(e.Options.Source as ContentControl).Content = new UcDragTemplate();
					}
				}
			}
  	}

  	private void OnDropQuery(object sender, DragDropQueryEventArgs e)
  	{
  		// We allow drop only if the dragged items are BServiceWorkspaces
			var draggedItem = e.Options.Payload as BServiceWorkspaceViewModel;
			if (draggedItem == null) 
				e.QueryResult = false;
			else
				e.QueryResult = true;

			e.Handled = true;

			// Note that here we agree to accept a drop. We will be notified
			// in the DropInfo event whether a drop is actually possible.
  	}
#else
    private void CcDragOver(object sender, DragEventArgs e)
    {
      try
      {
        var cc = sender as ContentControl;
        if (!cc.Content.GetType().Equals(typeof (UcDragTemplate)))
        {
          e.Effects = DragDropEffects.None;

          return;
        }
        e.Effects = e.AllowedEffects & DragDropEffects.Move;
        DropTargetHelper.DragOver(e.GetPosition(this), e.Effects);
        DropTargetHelper.Show(true);
        e.Handled = true;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this,
          ex);
      }
    }

    private void CcDrop(object sender, DragEventArgs e)
    {
      try
      {
        var cc = sender as ContentControl;

        var hashcode = (int) e.Data.GetData("service");
        BServiceWorkspaceViewModel ccService = null;
        if (cc != null)
        {
          var lstService =
            ((ChangeTemplateViewModel) cc.DataContext).BServiceWorkspaces;
          foreach (var service in lstService)
          {
            if (hashcode == service.GetHashCode())
            {
              ccService = service;
              break;
            }
          }
          if (ccService == null) return;
          lstService.Remove(ccService);
          cc.Content = new UcDragTemplateComplete {DataContext = ccService};
        }

        e.Effects = DragDropEffects.None;
        DropTargetHelper.Drop(e.Data, e.GetPosition(this), DragDropEffects.None);
        base.OnDrop(e);
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this,
          ex);
      }
      e.Handled = true;
    }
#endif

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

    #endregion // INotifyPropertyChanged Members

#if SILVERLIGHT

    public object FindResource(string name)
    {
      if (SobeesApplication.Current.Resources.Contains(name))
      {
        return SobeesApplication.Current.Resources[name];
      }
      var root = SobeesApplication.Current.RootVisual as FrameworkElement;
      return FindResource(root, name);
    }

    internal object FindResource(FrameworkElement root, string name)
    {
      if (root != null && root.Resources.Contains(name))
      {
        return root.Resources[name];
      }
      try
      {
        if (root != null) return root.FindName(name);
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("FindResource", ex);
      }

      return null;

    }
#endif
  }
}