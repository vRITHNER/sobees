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
using Sobees.Controls.Twitter.Controls;
using Sobees.Library.BGenericLib;

namespace Sobees.Controls.Twitter.Templates
{
  /// <summary>
  /// Interaction logic for DtTweetConversation.xaml
  /// </summary>
  public partial class DtTweetConversation : UserControl
  {
    public DtTweetConversation()
    {
      InitializeComponent();
    }
    private void MenuItem_Click(object sender, RoutedEventArgs e)
    {
      Clipboard.SetText(((Entry)DataContext).Title);
    }
  }
}
