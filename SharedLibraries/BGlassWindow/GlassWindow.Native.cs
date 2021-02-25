using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using BGlassWindow.Native;
using WPF_Aero_Window;
using WPF_Aero_Window.Native;
using Point = WPF_Aero_Window.Native.Point;

namespace BGlassWindow
{
  public partial class GlassWindow
  {
    private bool IsSourceInitialized = false;

    protected override void OnSourceInitialized(EventArgs e)
    {
      base.OnSourceInitialized(e);
      System.IntPtr handle = new WindowInteropHelper(this).Handle;
      HwndSource.FromHwnd(handle).AddHook(WindowProc);
      DwmApi.SetGlassMargin(this, this.FrameThickness);
      IsSourceInitialized = true;
    }

    private System.IntPtr WindowProc(System.IntPtr hwnd, int msg, System.IntPtr wParam, System.IntPtr lParam, ref bool handled)
    {
      switch (DwmApi.DwmEnabled)
      {
        case true:
          handled = true;
          switch ((WM)msg)
          {
            case WM.NCCALCSIZE:
              return new IntPtr(-1);
            case WM.NCACTIVATE:
              return User32.DefWindowProc(hwnd, msg, wParam, lParam);
            case WM.NCHITTEST:
              var result = IntPtr.Zero;
              handled = DwmApi.DwmDefWindowProc(hwnd, msg, wParam, lParam, ref result);
              return result;
            case WM.DWMCOMPOSITIONCHANGED:
              this.IsCompositionEnabled = DwmApi.DwmEnabled;
              if (IsCompositionEnabled)
                DwmApi.SetGlassMargin(this, this.FrameThickness);
              break;
            case WM.GETMINMAXINFO:
              WM_GETMINMAXINFO(hwnd, lParam, this);
              handled = true;
              break;
            default:
              handled = false;
              break;
          }
          break;
        case false:
          switch ((WM)msg)
          {
            case WM.DWMCOMPOSITIONCHANGED:
              this.IsCompositionEnabled = DwmApi.DwmEnabled;
              if (IsCompositionEnabled)
                DwmApi.SetGlassMargin(this, this.FrameThickness);
              break;
            case WM.GETMINMAXINFO:
              WM_GETMINMAXINFO(hwnd, lParam, this);
              handled = true;
              break;
          }
          break;
      }
      return IntPtr.Zero;
    }

    private static void WM_GETMINMAXINFO(IntPtr Handle, IntPtr lParam, GlassWindow Window)
    {
      MinMaxInfo info = lParam.ToStruct<MinMaxInfo>();
      IntPtr hMonitor = User32.MonitorFromWindow(Handle, 2);
      if (hMonitor != IntPtr.Zero)
      {
        MonitorInfo lpmi = new MonitorInfo();
        lpmi.cbSize = (uint)Marshal.SizeOf(typeof(MonitorInfo));
        bool monitorInfo = User32.GetMonitorInfo(hMonitor, ref lpmi);
        var rcWork = lpmi.rcWork;
        var rcMonitor = lpmi.rcMonitor;
        info.ptMaxPosition.X = (int)(rcWork.Left - Window.ResizeMargin.Left);
        info.ptMaxPosition.Y = (int)(rcWork.Top - Window.ResizeMargin.Top);
        info.ptMaxSize.X = (int)((rcWork.Right - rcWork.Left) + (Window.ResizeMargin.Left + Window.ResizeMargin.Right));
        info.ptMaxSize.Y = (int)((rcWork.Bottom - rcWork.Top) + (Window.ResizeMargin.Top + Window.ResizeMargin.Bottom));
        info.ptMinTrackSize = new Point((int)Window.MinWidth, (int)Window.MinHeight);
      }
      lParam.WriteStruct(info);
    }

    protected void SetFrameThickness(FrameworkElement Element)
    {
      if (Element == null) return;
      DoEvents();
      UpdateLayout();
      if (!this.IsAncestorOf(Element)) return;
      var trans = Element.TransformToAncestor(this).TransformBounds(new System.Windows.Rect(0, 0, Element.ActualWidth, Element.ActualHeight));
      var Margin = new Thickness(trans.Left, trans.Top, ActualWidth - trans.Right, ActualHeight - trans.Bottom);
      this.FrameThickness = Margin;
    }

    public static void DoEvents()
    {
      try
      {
        Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate { }));
      }
      catch
      {
      }
    }
  }
}


