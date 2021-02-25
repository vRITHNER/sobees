using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Sobees.Controls.TwitterSearch.Controls
{
  /// <summary>
  /// Interaction logic for UcAdsTop.xaml
  /// </summary>
  public partial class UcAdsTop : UserControl
  {
    private bool _isHide;
    private DispatcherTimer _timer;

    public UcAdsTop()
    {
      InitializeComponent();
    }

    private void WrapPanel_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      UpdateVisibility();
    }

    private void UpdateVisibility()
    {
      ucAds.Visibility = _isHide 
                             ? Visibility.Collapsed
                             : wrpMain.DataContext == null ? Visibility.Collapsed : Visibility.Visible;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      _isHide = true;
      UpdateVisibility();
      _timer = new DispatcherTimer {Interval = TimeSpan.FromMinutes(30)};
      _timer.Tick += delegate
                       {
                         _isHide = false;
                         UpdateVisibility();
                         _timer.Stop();
                         _timer = null;
                       };
      _timer.Start();
    }

    private void Button_Unloaded(object sender, RoutedEventArgs e)
    {
      if (_timer == null) return;
      _timer.Stop();
      _timer = null;
    }

    private void wrpMain_Loaded(object sender, RoutedEventArgs e)
    {
      UpdateVisibility();
    }
  }
}