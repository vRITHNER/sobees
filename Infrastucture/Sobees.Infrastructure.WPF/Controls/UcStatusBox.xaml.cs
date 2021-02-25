using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Sobees.Infrastructure.Cls;
using Sobees.Infrastructure.Controls.StatusBoxControls;
using Sobees.Infrastructure.ViewModelBase;

namespace Sobees.Infrastructure.Controls
{
  /// <summary>
  /// Interaction logic for UcStatusBox.xaml
  /// </summary>
  public partial class UcStatusBox
  {
    public UcStatusBox()
    {
      InitializeComponent();
      Loaded += UcStatusBoxLoaded;
    }

    private void UcStatusBoxLoaded(object sender, RoutedEventArgs e)
    {
      Loaded -= UcStatusBoxLoaded;
      var service = DataContext as BServiceWorkspaceViewModel;
      if (service != null)
      {
        switch (service.AccountType)
        {
          case EnumAccountType.Twitter:
            ccStatusBox.Content = new UcTwStatusBox {Tag = btnSendTweet};
            ccStatusBoxControls.Content = new UcTwStatusBoxButtons();
            btntUseSpellCheck.Tag = ((UserControl) ccStatusBox.Content).FindName("txtTweet");
            break;
          case EnumAccountType.Facebook:
            //ccStatusBox.Content = new UcDefaultStatusBox {Tag = btnSendTweet};
            //ccStatusBoxControls.Content = new UcFbStatusBoxButtons();
            //btntUseSpellCheck.Tag = ((UserControl) ccStatusBox.Content).FindName("txtTweet");
            break;
          case EnumAccountType.TwitterSearch:
            ccHeaderControls.Content = new UcTSHeader();
            ccStatusBox.Content = new UcTwStatusBox {Tag = btnSendTweet};
            ccStatusBoxControls.Content = new UcTwStatusBoxButtons();
            btntUseSpellCheck.Tag = ((UserControl) ccStatusBox.Content).FindName("txtTweet");
            break;
          //case EnumAccountType.MySpace:
          //  ccStatusBox.Content = new UcDefaultStatusBox {Tag = btnSendTweet};
          //  ccStatusBoxControls.Content = new UcMsStatusBoxButtons();
          //  btntUseSpellCheck.Tag = ((UserControl) ccStatusBox.Content).FindName("txtTweet");
          //  break;
          //case EnumAccountType.LinkedIn:
          //  ccStatusBox.Content = new UcDefaultStatusBox {Tag = btnSendTweet};
          //  btntUseSpellCheck.Tag = ((UserControl) ccStatusBox.Content).FindName("txtTweet");
          //  break;
          case EnumAccountType.Rss:
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }
    }

    private void grTweets_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      var grid = sender as Grid;
      if (grid != null) grid.Visibility = (bool) e.NewValue ? Visibility.Visible : Visibility.Collapsed;
    }

    private void btntUseSpellCheck_Checked(object sender, RoutedEventArgs e)
    {
      if (((ToggleButton)sender).Tag != null)
      {
        ((TextBox) ((ToggleButton) sender).Tag).SpellCheck.IsEnabled = (bool) ((ToggleButton) sender).IsChecked;
      }
    }
  }
}