#region

using System.Windows;
using System.Windows.Input;
using Sobees.Controls.Twitter.Controls;
using Sobees.Library.BGenericLib;

#endregion

namespace Sobees.Controls.Twitter.Templates
{
  /// <summary>
  ///   Interaction logic for DtTweet.xaml
  /// </summary>
  public partial class DtTweet
  {
    public DtTweet()
    {
      InitializeComponent();
    }

    private void grAvatarMouseEnter(object sender, MouseEventArgs e)
    {
      ccAvatar.Visibility = Visibility.Visible;
      ccAvatar.Content = new UcAvatar();
    }

    private void grAvatarMouseLeave(object sender, MouseEventArgs e)
    {
      ccAvatar.Visibility = Visibility.Collapsed;
      ccAvatar.Content = null;
    }

    private void MenuItem_Click(object sender, RoutedEventArgs e)
    {
      Clipboard.SetText(((Entry) DataContext).Title);
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      ucTweet.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) {RoutedEvent = Mouse.MouseDownEvent});
    }

    private void txtBlContent_Unloaded(object sender, RoutedEventArgs e)
    {
      //((FrameworkElement)sender).Tag = null;
    }
  }
}