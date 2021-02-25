#region

using System.Windows;
using System.Windows.Controls;

#endregion

namespace Sobees.Controls.Facebook.Templates
{
  /// <summary>
  ///   Interaction logic for DtApplication.xaml
  /// </summary>
  public partial class DtAlbum : UserControl
  {
    public DtAlbum()
    {
      InitializeComponent();
    }

    private void ucDtlAlbum_Unloaded(object sender, RoutedEventArgs e)
    {
      //((FrameworkElement)sender).Tag = null;
    }
  }
}