using System;
using System.Windows;
using System.Windows.Controls;
using Sobees.Controls.Twitter.Cls;
using Sobees.Controls.Twitter.ViewModel;
using Sobees.Tools.Logging;

namespace Sobees.Controls.Twitter.Controls
{
  /// <summary>
  /// Interaction logic for UcAvatar.xaml
  /// </summary>
  public partial class UcAvatar : UserControl
  {
    public UcAvatar()
    {
      InitializeComponent();
      Loaded += UcAvatar_Loaded;
    }

    private void UcAvatar_Loaded(object sender, RoutedEventArgs e)
    {
      try
      {
        TwitterWorkspaceViewModel ctx = null;
        if (ucAvatar != null)
        {
          ctx = ucAvatar.Tag as TwitterWorkspaceViewModel;
        }

        if ((ctx == null) || (!ctx.WorkspaceSettings.Type.Equals(EnumTwitterType.Favorites)))

        {
          Favorit.Visibility = Visibility.Visible;
          UnFavorit.Visibility = Visibility.Collapsed;
          if (ctx != null)
            if (ctx.WorkspaceSettings.Type.Equals(EnumTwitterType.DirectMessages))
            {
              btnReplies.Visibility = Visibility.Collapsed;
              btnReTweet.Visibility = Visibility.Collapsed;
              return;
            }
          return;
        }

      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
      Favorit.Visibility = Visibility.Collapsed;
      UnFavorit.Visibility = Visibility.Visible;
    }

    private void ucAvatar_Unloaded(object sender, RoutedEventArgs e)
    {
        //((FrameworkElement)sender).Tag = null;
    }
  }
}