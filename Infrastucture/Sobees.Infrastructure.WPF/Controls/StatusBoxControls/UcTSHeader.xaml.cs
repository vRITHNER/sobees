using System;
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

namespace Sobees.Infrastructure.Controls.StatusBoxControls
{
  /// <summary>
  /// Interaction logic for UcTSHeader.xaml
  /// </summary>
  public partial class UcTSHeader : UserControl
  {
    public UcTSHeader()
    {
      InitializeComponent();
      Loaded += new RoutedEventHandler(UcTSHeader_Loaded);
    }

    void UcTSHeader_Loaded(object sender, RoutedEventArgs e)
    {
      
    }
  }
}
