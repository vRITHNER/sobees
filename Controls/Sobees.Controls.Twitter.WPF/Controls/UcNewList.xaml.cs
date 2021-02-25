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

namespace Sobees.Controls.Twitter.Controls
{
  /// <summary>
  /// Interaction logic for UcNewList.xaml
  /// </summary>
  public partial class UcNewList : UserControl
  {
    public UcNewList()
    {
      InitializeComponent();
    }

    private void txtKeywords1_KeyDown(object sender, KeyEventArgs e)
    {
      if (KeysHelper.CheckEnterKey(e))
      {
        var btn = SaveList as Button;
        if (btn != null)
        {
          btn.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, btn));
          btn.Command.Execute(null);
        }

      }
    }
  }
}
