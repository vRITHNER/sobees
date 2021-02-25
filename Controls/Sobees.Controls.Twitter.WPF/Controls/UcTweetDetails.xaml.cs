#region

using System.Windows;
using System.Windows.Controls;

#endregion

namespace Sobees.Controls.Twitter.Controls
{
  /// <summary>
  ///   Interaction logic for UcTweetDetails.xaml
  /// </summary>
  public partial class UcTweetDetails : UserControl
  {
    public UcTweetDetails()
    {
      InitializeComponent();
    }

    private void ucTweetDetails_Unloaded(object sender, RoutedEventArgs e)
    {
      //((FrameworkElement)sender).Tag = null;
    }
  }
}