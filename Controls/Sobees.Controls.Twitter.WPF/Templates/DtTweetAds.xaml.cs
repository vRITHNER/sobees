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

namespace Sobees.Controls.Twitter.Templates
{
  /// <summary>
  /// Interaction logic for DtTweetAds.xaml
  /// </summary>
  public partial class DtTweetAds : UserControl
  {
    public DtTweetAds()
    {
      InitializeComponent();
    }

    private void txtBlContent_Unloaded(object sender, RoutedEventArgs e)
    {
        //((FrameworkElement)sender).Tag = null;
    }
  }
}
