using Sobees.Controls.Twitter.Controls;

namespace Sobees.Controls.Twitter.Views
{
  /// <summary>
  /// Interaction logic for Profile.xaml
  /// </summary>
  public partial class Profile
  {
    public Profile()
    {
      InitializeComponent();
    }

    private void ShowTweets(object sender, System.Windows.RoutedEventArgs e)
    {
      ccContent.Content = new TwitterWorkspace();
    }

    private void ShowLists(object sender, System.Windows.RoutedEventArgs e)
    {
      ccContent.Content = new UcProfileLists();
    }
  }
}