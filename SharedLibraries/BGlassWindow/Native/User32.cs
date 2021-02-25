using System;
using System.Runtime.InteropServices;
using WPF_Aero_Window.Native;

namespace BGlassWindow.Native
{
  internal static class User32
  {
    [DllImport("user32.dll", EntryPoint = "DefWindowProcW", CharSet = CharSet.Unicode)]
    internal static extern IntPtr DefWindowProc(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern IntPtr SendMessage(IntPtr Handle, WM Message, IntPtr wParam, IntPtr lParam);
    [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern bool GetMonitorInfo(IntPtr hMonitor, ref MonitorInfo lpmi);
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern bool GetWindowRect(IntPtr ControlHandle, ref Rect OutputRectangle);
    [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern bool InvalidateRect(IntPtr Handle, Rect Area, bool Erase);
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern IntPtr MonitorFromWindow(IntPtr Handle, uint Flags);

    internal static void BeginResizeWindow(System.Windows.Window Window, ResizeDirection Direction)
    {
      SendMessage(Helpers.GetWindowHandle(Window).Handle, WM.SYSCOMMAND, (IntPtr)(61440 + Direction), IntPtr.Zero);
    }
    internal static void BeginDragWindow(System.Windows.Window Window)
    {
      SendMessage(Helpers.GetWindowHandle(Window).Handle, WM.SYSCOMMAND, (IntPtr)(61458), IntPtr.Zero);
    }
  }
}


