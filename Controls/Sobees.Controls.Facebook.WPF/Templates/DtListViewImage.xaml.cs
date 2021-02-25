#region

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

#endregion

namespace Sobees.Controls.Facebook.Templates
{
  /// <summary>
  ///   Interaction logic for DtListViewImage.xaml
  /// </summary>
  public partial class DtListViewImage
  {
    public DtListViewImage()
    {
      InitializeComponent();
    }

    private void listboxApp_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
      var listView = sender as ListView;
      if (e == null) return;
      var eventArg = new MouseWheelEventArgs(e.MouseDevice,
        e.Timestamp,
        e.Delta) {RoutedEvent = MouseWheelEvent, Source = listView};
      if (listView != null)
      {
        var parent = listView.Parent as UIElement;
        if (parent != null)
        {
          parent.RaiseEvent(eventArg);
        }
      }
      e.Handled = true;
    }

    private void dtListViewImage_Unloaded(object sender, RoutedEventArgs e)
    {
      //((FrameworkElement)sender).Tag = null;
    }
  }
}