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

namespace Sobees.Controls.Twitter.Templates
{
  /// <summary>
  /// Interaction logic for DtList.xaml
  /// </summary>
  public partial class DtList : UserControl
  {
    public DtList()
    {
      InitializeComponent();
    }

    private void ToggleButton_Click(object sender, RoutedEventArgs e)
    {
      cbxColor.Visibility = (bool)tgtbtnShow.IsChecked ? Visibility.Visible : Visibility.Collapsed;
    }
  }
}
