using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Sobees.Tools.KeysHelper;

namespace Sobees.Infrastructure.Controls.Settings
{
  /// <summary>
  /// Interaction logic for UcNotifications.xaml
  /// </summary>
  public partial class UcNotifications : UserControl
  {
    public UcNotifications()
    {
      InitializeComponent();
    }

    private void txtAlertWords_KeyUp(object sender, KeyEventArgs e)
    {
      if (!KeysHelper.CheckEnterKey(e)) return;
      if (string.IsNullOrEmpty(txtAlertWords.Text)) return;
      btnAddAlertWords.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, btnAddAlertWords));
    }

    private void txtAlertUsers_KeyUp(object sender, KeyEventArgs e)
    {
      if (!KeysHelper.CheckEnterKey(e)) return;
      if (string.IsNullOrEmpty(txtAlertUsers.Text)) return;
      btnAddAlertUsers.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, btnAddAlertUsers));
    }

    private void txtAlertRemovedWords_KeyUp(object sender, KeyEventArgs e)
    {
      if (!KeysHelper.CheckEnterKey(e)) return;
      if (string.IsNullOrEmpty(txtAlertRemovedWords.Text)) return;
      btnAddAlertRemovedWords.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, btnAddAlertRemovedWords));
    }
  }
}
