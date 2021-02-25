#region

using System.Windows;
using System.Windows.Controls;

#endregion

namespace Sobees.Controls.Facebook.Templates
{
  /// <summary>
  ///   Interaction logic for DtVideo.xaml
  /// </summary>
  public partial class DtVideo : UserControl
  {
    public DtVideo()
    {
      InitializeComponent();
    }

    private void ucDtVideo_Unloaded(object sender, RoutedEventArgs e)
    {
      //((FrameworkElement)sender).Tag = null;
    }
  }
}