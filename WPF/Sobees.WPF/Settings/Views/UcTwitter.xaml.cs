using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Sobees.Tools.KeysHelper;

namespace Sobees.Settings.Views
{
  /// <summary>
  /// Interaction logic for UcTwitter.xaml
  /// </summary>
  public partial class UcTwitter : UserControl
  {
    public UcTwitter()
    {
      InitializeComponent();
    }

    private void txbkPwdNew_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
    {

      if (!KeysHelper.CheckEnterKey(e)) return;
      //if (string.IsNullOrEmpty(txbkPwdNew.Text)) return;
      //btnUpdatePassword.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, btnUpdatePassword));
      //btnUpdatePassword.Command.Execute(txbkPwdNew);
    
    }
  }
}


