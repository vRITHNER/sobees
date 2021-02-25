#region

using System.Windows;
using System.Windows.Controls;

#endregion

namespace Sobees.Controls.Facebook.Templates
{
  /// <summary>
  ///   Interaction logic for DtComment.xaml
  /// </summary>
  public partial class DtComment : UserControl
  {
    public DtComment()
    {
      InitializeComponent();
    }

    private void ucDtComment_Unloaded(object sender, RoutedEventArgs e)
    {
      //((FrameworkElement)sender).Tag = null;
    }
  }
}