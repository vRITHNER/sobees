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
using Sobees.Infrastructure.Cls;
using Sobees.Tools.KeysHelper;

namespace Sobees.Infrastructure.Controls.StatusBoxControls
{
  /// <summary>
  /// Interaction logic for UcDefaultStatusBox.xaml
  /// </summary>
  public partial class UcDefaultStatusBox : UserControl
  {
    public UcDefaultStatusBox()
    {
      InitializeComponent();
    }


    private void txtKeywords1_KeyDown(object sender, KeyEventArgs e)
    {
        if (KeysHelper.CheckEnterKey(e) && SobeesSettingsLocator.SobeesSettingsStatic.IsSendByEnter)
      {
        var btn = ucDefaultStatusBox.Tag as Button;
        if (btn != null)
        {
          btn.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, btn));
          btn.Command.Execute(null);
        }

      }
    }
  }
}
