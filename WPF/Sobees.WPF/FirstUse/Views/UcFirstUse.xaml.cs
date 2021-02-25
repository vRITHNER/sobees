#region


using System.Windows.Controls;
using Sobees.Infrastructure.Cls;

#endregion

namespace Sobees.FirstUse.Views
{
  /// <summary>
  ///   Interaction logic for ucFirstUse.xaml
  /// </summary>
  public partial class UcFirstUse
  {
    public UcFirstUse()
    {
      InitializeComponent();
    }

    private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var type = lstServices.SelectedItem is EnumAccountType
                               ? (EnumAccountType) lstServices.SelectedItem
                               : EnumAccountType.Twitter;
      switch (type)
      {
        case EnumAccountType.Twitter:
          ccConnectService.Content = new UcConnectTwitter();
          break;
        case EnumAccountType.Facebook:
          ccConnectService.Content = new UcConnectFacebook();
          break;
        case EnumAccountType.TwitterSearch:
          ccConnectService.Content = new UcSearch();
          break;
        //case EnumAccountType.MySpace:
        //  ccConnectService.Content = new UcConnectMySpace();
        //  break;
        case EnumAccountType.LinkedIn:
          ccConnectService.Content = new UcConnectLinkedIn();
          break; 
        //case EnumAccountType.Rss:
        //  ccConnectService.Content = new UcRss();
        //  break;     
        //case EnumAccountType.NyTimes:
        //  ccConnectService.Content = new UcNYTimes();
          break;
      }
    }
  }
}