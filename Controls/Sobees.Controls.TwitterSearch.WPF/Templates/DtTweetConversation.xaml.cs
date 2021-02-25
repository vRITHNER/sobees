﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Sobees.Controls.TwitterSearch.Controls;
using Sobees.Library.BGenericLib;

namespace Sobees.Controls.TwitterSearch.Templates
{
  /// <summary>
  /// Interaction logic for DtTweetConversation.xaml
  /// </summary>
  public partial class DtTweetConversation : UserControl
  {
    public DtTweetConversation()
    {
      InitializeComponent();
    }
    private void grAvatarMouseEnter(object sender, MouseEventArgs e)
    {
      var entry = ccAvatar.DataContext as Entry;
      if (entry != null)
        if (entry.Type == EnumType.TwitterSearch)
        {
          ccAvatar.Visibility = Visibility.Visible;
          ccAvatar.Content = new UcAvatar();
        }
    }

    private void grAvatarMouseLeave(object sender, MouseEventArgs e)
    {
      ccAvatar.Visibility = Visibility.Collapsed;
      ccAvatar.Content = null;
    }

    private void CopyItem(object sender, RoutedEventArgs e)
    {
      Clipboard.SetText(((Entry)DataContext).Title);
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      ucTweet.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = Mouse.MouseDownEvent });
    }
  }
}
