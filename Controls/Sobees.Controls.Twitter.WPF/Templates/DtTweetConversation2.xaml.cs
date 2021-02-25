#region

using System.Windows;
using System.Windows.Controls;
using Sobees.Library.BGenericLib;

#endregion

namespace Sobees.Controls.Twitter.Templates
{
  /// <summary>
  ///   Interaction logic for DtTweetConversation.xaml
  /// </summary>
  public partial class DtTweetConversation2 : UserControl
  {
    public DtTweetConversation2()
    {
      InitializeComponent();
    }

    private void MenuItem_Click(object sender, RoutedEventArgs e)
    {
      Clipboard.SetText(((Entry) DataContext).Title);
    }
  }
}