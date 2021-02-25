#region

using System.Windows;
using System.Windows.Controls;

#endregion

namespace Sobees.Controls.Facebook.Templates
{
  /// <summary>
  ///   Interaction logic for DtGroup.xaml
  /// </summary>
  public partial class DtGroup : UserControl
  {
    public DtGroup()
    {
      InitializeComponent();
    }

    private void ucDtGroup_Unloaded(object sender, RoutedEventArgs e)
    {
      //((FrameworkElement)sender).Tag = null;
    }
  }
}