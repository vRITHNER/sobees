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

namespace Sobees.Controls.Twitter.Views
{
  /// <summary>
  /// Interaction logic for Credentials.xaml
  /// </summary>
  public partial class Credentials : UserControl
  {
    public Credentials()
    {
      InitializeComponent();
    }

    private void txtTwitterLogin_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (string.IsNullOrEmpty(((TextBox)sender).Text))
      {
        txtTwitterPinCode.Clear();
      }
    }

    private void txtTwitterLogin_KeyDown(object sender, KeyEventArgs e)
    {
      if (!KeysHelper.CheckEnterKey(e)) return;
      if (string.IsNullOrEmpty(txtTwitterPinCode.Text)) return;
      btnTwitterPinCodeSignIn.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, btnTwitterPinCodeSignIn));
      btnTwitterPinCodeSignIn.Command.Execute(txtTwitterPinCode.Text);
    }
  }
}
