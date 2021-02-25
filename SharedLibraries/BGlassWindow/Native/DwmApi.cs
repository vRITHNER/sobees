using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using WPF_Aero_Window.Native;

namespace BGlassWindow.Native
{
  internal static class DwmApi
  {

    [DllImport("dwmapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern bool DwmDefWindowProc(IntPtr Handle, int Message, IntPtr wParam, IntPtr lParam, ref IntPtr Result);
    [DllImport("dwmapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern void DwmExtendFrameIntoClientArea(IntPtr WindowHandle, ref MARGINS Margins);
    [DllImport("dwmapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern void DwmGetColorizationColor(ref int Color, ref bool Opaque);
    [DllImport("dwmapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern void DwmIsCompositionEnabled(ref bool Enabled);

    internal static bool DwmEnabled
    {
      get
      {
        if (System.Environment.GetCommandLineArgs().Contains("-xp")) return false;
        if (Environment.OSVersion.Version.Major < 6) return false;
        bool enabled = false;
        DwmIsCompositionEnabled(ref enabled);
        return enabled;
      }
    }

    public static void SetGlassMargin(IntPtr Window, Thickness? Margin)
    {
      if (DwmEnabled)
      {
        var x = (MARGINS)Margin;
        DwmExtendFrameIntoClientArea(Window, ref x);
      }
    }

    public static void SetGlassMargin(Window Window, Thickness? Margin)
    {
      var wndHandle = Helpers.GetWindowHandle(Window);
      System.Windows.Interop.HwndSource.FromHwnd(wndHandle.Handle).CompositionTarget.BackgroundColor
        = System.Windows.Media.Colors.Transparent;
      SetGlassMargin(wndHandle.Handle, Margin);
    }
  }
}


