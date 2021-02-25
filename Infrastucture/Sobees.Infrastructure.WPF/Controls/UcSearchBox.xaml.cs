using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Sobees.Tools.KeysHelper;

namespace Sobees.Infrastructure.Controls
{
  /// <summary>
  /// Interaction logic for UcSearchBox.xaml
  /// </summary>
  public partial class UcSearchBox
  {
    public UcSearchBox()
    {
      InitializeComponent();
    }

    private void txtKeywords1_KeyDown(object sender, KeyEventArgs e)
    {
      if (KeysHelper.CheckEnterKey(e))
      {
        btnSendKeywords.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, btnSendKeywords));
        if (btnSendKeywords.Command != null) btnSendKeywords.Command.Execute(null);
      }
      btnSendKeywords.IsEnabled = txtKeywords1.Text.Length > 0;
    }
  }
}