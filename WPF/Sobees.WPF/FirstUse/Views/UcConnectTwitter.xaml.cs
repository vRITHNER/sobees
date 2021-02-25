using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Sobees.Tools.KeysHelper;
using Sobees.Tools.Threading.Extensions;

namespace Sobees.FirstUse.Views
{
  /// <summary>
  /// Interaction logic for UcConnectTwitter.xaml
  /// </summary>
  public partial class UcConnectTwitter : UserControl
  {
    public UcConnectTwitter()
    {
      InitializeComponent();
      Loaded += UcConnectTwitter_Loaded;
    }

    void UcConnectTwitter_Loaded(object sender, RoutedEventArgs e)
    {
      //btnTwitterPinCodeLaunchBrowser.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, btnTwitterPinCodeLaunchBrowser));
      //MainWindow.GetInstance.Dispatcher.BeginInvokeIfRequired(
      //  () =>
      //  {

      //  });
    }

    private void txtTwitterLogin_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (string.IsNullOrEmpty(((TextBox) sender).Text))
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