#region

using System.Windows;
using System.Windows.Controls;

#endregion

namespace Sobees.Controls.Facebook.Templates
{
  /// <summary>
  ///   Interaction logic for DtApplication.xaml
  /// </summary>
  public partial class DtApplication : UserControl
  {
    public DtApplication()
    {
      InitializeComponent();
    }

    private void ucDtApplication_Unloaded(object sender, RoutedEventArgs e)
    {
      //((FrameworkElement)sender).Tag = null;
    }
  }
}