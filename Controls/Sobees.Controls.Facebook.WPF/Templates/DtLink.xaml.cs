#region

using System.Windows;
using System.Windows.Controls;

#endregion

namespace Sobees.Controls.Facebook.Templates
{
  /// <summary>
  ///   Interaction logic for DtLink.xaml
  /// </summary>
  public partial class DtLink : UserControl
  {
    public DtLink()
    {
      InitializeComponent();
    }

    private void ucDtLink_Unloaded(object sender, RoutedEventArgs e)
    {
      //((FrameworkElement)sender).Tag = null;
    }
  }
}