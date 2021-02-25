using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Configuration.BGlobals;
using Sobees.Infrastructure.Model;
using Sobees.Infrastructure.ViewModelBase;
using Sobees.Tools.Logging;
#if SILVERLIGHT
using System.Windows.Input;

#else
using System.Windows.Controls.Primitives;

#endif

namespace Sobees.Infrastructure.Controls
{
  public class BMultiViewsGrid : Grid, INotifyPropertyChanged
  {
    public static DependencyProperty CurrentDataTemplateProperty = DependencyProperty.Register("CurrentDataTemplate", typeof(DataTemplate), typeof(BMultiViewsGrid), new PropertyMetadata(OnCurrentDataTemplatePropertyChanged));
    private static void OnCurrentDataTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((BMultiViewsGrid)d).UpdateGrid();
    }
    public DataTemplate CurrentDataTemplate
    {
      get
      {
#if SILVERLIGHT
        if (DesignerProperties.IsInDesignTool)
          return new DataTemplate();
#else
        if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
          return new DataTemplate();
#endif

        return GetValue(CurrentDataTemplateProperty) as DataTemplate;
      }
      set
      {
        SetValue(CurrentDataTemplateProperty, value);
        OnPropertyChanged("CurrentDataTemplate");
      }
    }

    public static DependencyProperty WorkspacesProperty = DependencyProperty.Register("Workspaces", typeof(ObservableCollection<DataTemplate>), typeof(BMultiViewsGrid), null);

    public ObservableCollection<DataTemplate> Workspaces
    {
      get { return GetValue(WorkspacesProperty) as ObservableCollection<DataTemplate>; }
      set { SetValue(WorkspacesProperty, value); }
    }

    public BMultiViewsGrid()
    {
#if SILVERLIGHT
      if (!DesignerProperties.IsInDesignTool)
        Loaded += BGridLoaded;
#else
      if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
        Loaded += BGridLoaded;
      Unloaded += BGridUnLoaded;
#endif
      //Register to get all the PropertyChangedMessages broadcasted.
      Messenger.Default.Register<string>(this, DoAction);
    }

    private void DoAction(string param)
    {
      switch (param)
      {
        case "CreatePicture":
          CreatePicture();
          break;
      }
    }

    void BGridLoaded(object sender, RoutedEventArgs e)
    {
      Loaded -= BGridLoaded;
      HorizontalAlignment = HorizontalAlignment.Stretch;
      VerticalAlignment = VerticalAlignment.Stretch;
      Workspaces.CollectionChanged += WorkspacesCollectionChanged;
      BuildGrid();
#if !SILVERLIGHT
      Unloaded -= BGridUnLoaded;
#endif
    }

    void BGridUnLoaded(object sender, RoutedEventArgs e)
    {
#if !SILVERLIGHT
      Unloaded += BGridUnLoaded;
#endif
      if (Workspaces != null) Workspaces.CollectionChanged -= WorkspacesCollectionChanged;

      //BuildGrid();
    }

    void WorkspacesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      try
      {
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

                if (cc.ContentTemplate.GetType().Equals(item.GetType()))
                {
                  if (cc.ContentTemplate.Equals(item))
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
            if (item.GetType().BaseType.Equals(typeof(DataTemplate)))
            {
              var wvm = item as DataTemplate;
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
        else if (e.Action.Equals(NotifyCollectionChangedAction.Replace))
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

                if (cc.ContentTemplate.GetType().Equals(item.GetType()))
                {
                  if (cc.ContentTemplate.Equals(item))
                    cc.ContentTemplate = e.NewItems[0] as DataTemplate;
                }
              }
            }

            //foreach (var uiElement in toRemove)
            //{
            //  Children.Remove(uiElement);
            //}
          }
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    void BuildGrid()
    {
      try
      {
        RowDefinitions.Clear();
        ColumnDefinitions.Clear();
        Children.Clear();


        RowDefinitions.Add(new RowDefinition());

        for (var i = 0; i < Workspaces.Count; i++)
        {
          var cd = new ColumnDefinition();
          ColumnDefinitions.Add(cd);
        }
        foreach (var workspace in Workspaces)
        {
          AddContentControl(workspace);
        }
#if !SILVERLIGHT
        UpdateLayout();
#endif
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    private void UpdateGrid()
    {
      foreach (ContentControl child in Children)
      {
        child.Visibility = child.ContentTemplate == CurrentDataTemplate ? Visibility.Visible : Visibility.Collapsed;
        ColumnDefinitions[Children.IndexOf(child)].Width = child.ContentTemplate == CurrentDataTemplate ? new GridLength(1, GridUnitType.Star) : new GridLength(0);
        RowDefinitions[0].Height = new GridLength(1, GridUnitType.Star);
      }
      CreatePicture();
    }

    public void CreatePicture()
    {
#if !SILVERLIGHT
      foreach (var child in Children)
      {
        SaveToPng((FrameworkElement)child);
      }
#endif
    }
#if !SILVERLIGHT
    void SaveToPng(FrameworkElement visual)
    {
      var encoder = new PngBitmapEncoder();
      SaveUsingEncoder(visual, encoder);
    }

    public void SaveUsingEncoder(FrameworkElement visual, BitmapEncoder encoder)
    {
      try
      {

        if (visual.ActualHeight == 0.0 || visual.ActualWidth == 0.0) return;
        var height = visual.ActualHeight > visual.ActualWidth ? 500 : (visual.ActualHeight/visual.ActualWidth)*500;
        var width = visual.ActualWidth > visual.ActualHeight ? 500 : (visual.ActualWidth / visual.ActualHeight) * 500;
        //        (int)visual.ActualWidth,
        //(int)visual.ActualHeight,
        var dpiX = (width / visual.ActualWidth) * 96;
        var dpiY = (height / visual.ActualHeight) * 96;
        var bitmap = new RenderTargetBitmap(
                  (int)width, (int)height,
          dpiX,
          dpiY,
          PixelFormats.Pbgra32);
        bitmap.Render(visual);
        var frame = BitmapFrame.Create(bitmap);
        encoder.Frames.Add(frame);
        var fileName = $"{BGlobals.FOLDER}\\{Children.IndexOf(visual)}.png";
        //TODO Better :)
        using (var stream = File.Create(fileName))
        {
          encoder.Save(stream);
          stream.Close();
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }
    }
    //private static BitmapSource captureVisualBitmap(Visual targetVisual, double dpiX, double dpiY)
    //{

    //  var bounds = VisualTreeHelper.GetDescendantBounds(targetVisual);
    //  var renderTargetBitmap = new RenderTargetBitmap(
    //    (int)(bounds.Width * dpiX / 96.0),
    //    (int)(bounds.Height * dpiY / 96.0),
    //    dpiX,
    //    dpiY,

    //    //PixelFormats.Default
    //    PixelFormats.Pbgra32
    //    );

    //  var drawingVisual = new DrawingVisual();
    //  using (DrawingContext drawingContext = drawingVisual.RenderOpen())
    //  {
    //    var visualBrush = new VisualBrush(targetVisual);
    //    drawingContext.DrawRectangle(visualBrush, null, new Rect(new Point(), bounds.Size));
    //  }
    //  renderTargetBitmap.Render(drawingVisual);

    //  return renderTargetBitmap;
    //}
#endif
    private void AddContentControl(DataTemplate workspace)
    {
      try
      {
        var cc = new ContentControl
        {
          ContentTemplate = workspace,
          Visibility = workspace == CurrentDataTemplate ? Visibility.Visible : Visibility.Collapsed,
          Content = DataContext,
          HorizontalAlignment = HorizontalAlignment.Stretch,
          VerticalAlignment = VerticalAlignment.Stretch,
          HorizontalContentAlignment = HorizontalAlignment.Stretch,
          VerticalContentAlignment = VerticalAlignment.Stretch,
        };
        ////cc.Margin = new Thickness(5, 5, 5, 5);
        //cc.SetBinding(ContentControl.ContentTemplateProperty,
        //              new Binding("DataTemplateView") { Source = workspace });
        ColumnDefinitions[Workspaces.IndexOf(workspace)].Width = workspace == CurrentDataTemplate ? new GridLength(1, GridUnitType.Star) : new GridLength(0);
        SetColumn(cc, Workspaces.IndexOf(workspace));
        SetRow(cc, 0);
        SetColumnSpan(cc, 1);
        SetRowSpan(cc, 1);

        Children.Add(cc);
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
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



  }

}