using System.Windows;
using Sobees.Glass;
using Sobees.Windows.Extensions;

namespace Sobees.Settings.Views
{
  /// <summary>
  /// Interaction logic for SettingWindow.xaml
  /// </summary>
  public partial class SettingWindow : GlassWindow
  {
    public SettingWindow(MainWindow mainWindow) :
      base("SettingWindow", true, mainWindow.GetWindowLocation())
    {
      InitializeComponent();
      Loaded += SettingWindow_Loaded;
    }

    void SettingWindow_Loaded(object sender, RoutedEventArgs e)
    {
      UpdateFrame();
    }

    protected override void OnStateChanged(System.EventArgs e)
    {
      UpdateFrame();
      base.OnStateChanged(e);
    }

    private const int _marginMouseHandlerWidth = 3;
    private void UpdateFrame()
    {
      //switch (WindowState)
      //{
      //  //TODO:Fine tune ...
      //  case WindowState.Maximized:
      //    grMain.Margin = new Thickness(-ResizeMargin.Left / 2 + 6, -18, -ResizeMargin.Right / 2 + 6, -ResizeMargin.Bottom / 2 + 6);
      //    Bg.Margin = new Thickness(ResizeMargin.Left, ResizeMargin.Top, ResizeMargin.Right, ResizeMargin.Bottom);
      //    break;

      //  case WindowState.Normal:
      //    grMain.Margin = new Thickness(-ResizeMargin.Left / 2 + 2, -18, -ResizeMargin.Right / 2 + 2, -ResizeMargin.Bottom / 2 + 2);
      //    Bg.Margin = new Thickness(-ResizeMargin.Left / 2 + _marginMouseHandlerWidth, 25, -ResizeMargin.Right / 2 + _marginMouseHandlerWidth, -ResizeMargin.Bottom / 2 + _marginMouseHandlerWidth);
      //    break;
      //}
    }
  }
}