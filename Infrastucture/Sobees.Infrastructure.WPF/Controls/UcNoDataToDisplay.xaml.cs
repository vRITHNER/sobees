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
using System.Windows.Threading;
using Sobees.Tools.Threading.Extensions;

namespace Sobees.Infrastructure.Controls
{
  /// <summary>
  /// Interaction logic for NoDataToDisplay.xaml
  /// </summary>
  public partial class UcNoDataToDisplay : UserControl
  {
    DispatcherTimer _timer;
    public UcNoDataToDisplay()
    {
      InitializeComponent();
    }

    private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if ((bool)e.NewValue)
      {
        ccMain.Content = new UcNoDataToDisplayUpdate();
      }
      else
      {
        if (_timer == null)
          _timer = new DispatcherTimer();
        _timer.Tick += ((timer, arg) =>
        {
          if (_timer != null) _timer.Stop();
          _timer = null;
          Application.Current.Dispatcher.BeginInvokeIfRequired(() =>
          {
            ccMain.Content = new UcNoDataToDisplayUpdated();
          });

        });
        _timer.Interval = TimeSpan.FromSeconds(2);
        _timer.Start();
      }
    }
  }
}
