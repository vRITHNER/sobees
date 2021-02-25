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

namespace Sobees.Controls.TwitterSearch.Controls
{
  /// <summary>
  /// Interaction logic for UcTweetDetails.xaml
  /// </summary>
  public partial class UcTweetDetails : UserControl
  {
    public UcTweetDetails()
    {
      InitializeComponent();
    }

    private void ucTweetDetails_Unloaded(object sender, RoutedEventArgs e)
    {
        //((FrameworkElement)sender).Tag = null;
    }
  }
}
