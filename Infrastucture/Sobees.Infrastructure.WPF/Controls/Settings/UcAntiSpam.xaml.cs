using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Sobees.Tools.KeysHelper;

namespace Sobees.Infrastructure.Controls.Settings
{
  /// <summary>
  /// Interaction logic for UcAntiSpam.xaml
  /// </summary>
  public partial class UcAntiSpam
  {
    public UcAntiSpam()
    {
      InitializeComponent();
    }

    //private void txtSpam_KeyUp(object sender, KeyEventArgs e)
    //{
    //  if (!KeysHelper.CheckEnterKey(e)) return;
    //  if (string.IsNullOrEmpty(txtSpam.Text)) return;
    //  btnAddSpam.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, btnAddSpam));
    //}

    private void txtKeywords1_KeyDown(object sender, KeyEventArgs e)
    {
      if (KeysHelper.CheckEnterKey(e))
      {
        if (string.IsNullOrEmpty(txtSpam.Text)) return;
        btnAddSpam.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, btnAddSpam));
        btnAddSpam.Command.Execute(null);
      }
    }
  }
}