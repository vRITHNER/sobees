#region

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Sobees.Infrastructure.ViewModelBase;

#endregion

namespace Sobees.Views
{
  /// <summary>
  ///   Interaction logic for ChangeTemplate.xaml
  /// </summary>
  public partial class ChangeTemplate : UserControl
  {
    public ChangeTemplate()
    {
      InitializeComponent();
      Loaded += ChangeTemplateLoaded;
    }

    private void ChangeTemplateLoaded(object sender, RoutedEventArgs e)
    {
      Loaded -= ChangeTemplateLoaded;
      //InitDragDrop();
    }

    private void InitDragDrop()
    {
      //DragEnter += (o,
      //              de) =>
      //               {
      //                 de.Effects = DragDropEffects.None;
      //                 DropTargetHelper.DragEnter(null,
      //                                            de.Data,
      //                                            de.GetPosition(this),
      //                                            DragDropEffects.None);
      //                 de.Handled = true;
      //                 base.OnDragEnter(de);
      //               };

      //DragOver += (o,
      //             de) =>
      //              {
      //                de.Effects = DragDropEffects.None;
      //                DropTargetHelper.DragOver(de.GetPosition(this),
      //                                          DragDropEffects.None);
      //                de.Handled = true;
      //                base.OnDragOver(de);
      //              };

      //DragLeave += (o,
      //              de) =>
      //               {
      //                 DropTargetHelper.DragLeave(de.Data);
      //                 de.Handled = true;
      //                 base.OnDragLeave(de);
      //               };
      //lstServices.PreviewMouseLeftButtonDown += lstServices_PreviewMouseLeftButtonDown;
    }

    //private static void lstServices_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    //{
    //  var DragSource = (ListBox)sender;
    //  var data = GetObjectDataFromPoint(DragSource,
    //                                    e.GetPosition(DragSource)) as BServiceWorkspaceViewModel;
    //  if (data != null)
    //  {
    //    //DragType = data.GetType();
    //    DragDrop.DoDragDrop(DragSource,
    //                        data,
    //                        DragDropEffects.Copy);
    //  }
    //}

    private void StackPanel_Loaded(object sender, RoutedEventArgs e)
    {
#if !SILVERLIGHT
      ((StackPanel) sender).PreviewMouseLeftButtonDown += ServicesPreviewMouseLeftButtonDown;
      ((StackPanel) sender).MouseLeftButtonDown += ServicesPreviewMouseLeftButtonDown;
#endif
    }

    private void StackPanel_Unloaded(object sender, RoutedEventArgs e)
    {
#if !SILVERLIGHT
      ((StackPanel) sender).PreviewMouseLeftButtonDown += ServicesPreviewMouseLeftButtonDown;
      ((StackPanel) sender).MouseLeftButtonDown -= ServicesPreviewMouseLeftButtonDown;
#endif
    }

    private void ServicesPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
#if !SILVERLIGHT
      var dragSource = (StackPanel) sender;
      var data = dragSource.DataContext as BServiceWorkspaceViewModel;
      if (data != null)
      {
        //DragType = data.GetType();
        //DragDrop.DoDragDrop(dragSource,
        //                    data,
        //                    DragDropEffects.Copy);
        DragSourceHelper.DoDragDrop(dragSource, e.GetPosition(dragSource), DragDropEffects.Move,
                                    new KeyValuePair<string, object>("service", data.GetHashCode()));
      }
#endif
    }

    #region Helper

    //private static object GetObjectDataFromPoint(ListBox source, Point point)
    //{
    //  var element = source.InputHitTest(point) as UIElement;
    //  if (element != null)
    //  {
    //    var data = DependencyProperty.UnsetValue;
    //    while (data == DependencyProperty.UnsetValue)
    //    {
    //      data = source.ItemContainerGenerator.ItemFromContainer(element);
    //      if (data == DependencyProperty.UnsetValue)
    //        element = VisualTreeHelper.GetParent(element) as UIElement;
    //      if (element == source)
    //        return null;
    //    }
    //    if (data != DependencyProperty.UnsetValue)
    //      return data;
    //  }
    //  return null;
    //}

    #endregion

    #region DragEnabled

    //public static readonly DependencyProperty DragEnabledProperty =
    //  DependencyProperty.RegisterAttached("DragEnabled",
    //                                      typeof(Boolean),
    //                                      typeof(ChangeTemplate),
    //                                      new FrameworkPropertyMetadata(OnDragEnabledChanged));

    //public static void SetDragEnabled(DependencyObject element, Boolean value)
    //{
    //  element.SetValue(DragEnabledProperty,
    //                   value);
    //}

    //public static Boolean GetDragEnabled(DependencyObject element)
    //{
    //  return (Boolean)element.GetValue(DragEnabledProperty);
    //}

    //public static void OnDragEnabledChanged
    //  (DependencyObject obj, DependencyPropertyChangedEventArgs args)
    //{
    //  if ((bool)args.NewValue)
    //  {
    //    var listbox = (ListBox)obj;
    //    listbox.PreviewMouseLeftButtonDown +=
    //      lstServices_PreviewMouseLeftButtonDown;
    //  }
    //}

    #endregion

    #region DropEnabled

    //public static readonly DependencyProperty DropEnabledProperty =
    //  DependencyProperty.RegisterAttached("DropEnabled",
    //                                      typeof(Boolean),
    //                                      typeof(ChangeTemplate),
    //                                      new FrameworkPropertyMetadata(OnDropEnabledChanged));

    //public static void SetDropEnabled(DependencyObject element, Boolean value)
    //{
    //  element.SetValue(DropEnabledProperty,
    //                   value);
    //}

    //public static Boolean GetDropEnabled(DependencyObject element)
    //{
    //  return (Boolean)element.GetValue(DropEnabledProperty);
    //}

    //public static void OnDropEnabledChanged
    //  (DependencyObject obj, DependencyPropertyChangedEventArgs args)
    //{
    //  if ((bool)args.NewValue)
    //  {
    //    var listbox = (ListBox)obj;
    //    listbox.AllowDrop = true;
    //    listbox.Drop += listbox_Drop;
    //  }
    //}

    //private static void listbox_Drop(object sender, DragEventArgs e)
    //{
    //  var data = e.Data.GetData(typeof(BServiceWorkspaceViewModel));
    //  //if (DragType.IsVisible == true)
    //  var lst = sender as ListBox;
    //  if (lst != null)
    //  {//TODO: Remove element from Grid
    //    lst.Items.Add(data);
    //  }
    //}

    #endregion
  }
}