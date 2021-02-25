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

namespace Sobees.Infrastructure.Controls
{
  /// <summary>
  /// Interaction logic for UcDragTemplateComplet.xaml
  /// </summary>
  public partial class UcDragTemplateComplete : UserControl
  {
    public UcDragTemplateComplete()
    {
      InitializeComponent();
    }

    private void ucDragTemplate_Unloaded(object sender, RoutedEventArgs e)
    {
        //((FrameworkElement)sender).Tag = null;
    }
  }
}
