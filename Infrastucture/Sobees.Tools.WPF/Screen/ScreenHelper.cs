#region

using System;
using System.Runtime.InteropServices;
using Sobees.Library.BUtilities;

#endregion

namespace Sobees.Tools.Screen
{
  public class ScreenHelper
  {
    [DllImport("user32")]
    private static extern bool GetMonitorInfo(IntPtr hMonitor,
                                              MONITORINFO lpmi);

    public static RECT GetCurrentMonitorSize(IntPtr handle)
    {
      // Adjust the maximized size and position to fit the work area of the correct monitor
      var MONITOR_DEFAULTTONEAREST = 0x00000002;
      var monitor = NativeMethods.MonitorFromWindow(handle, MONITOR_DEFAULTTONEAREST);

      if (monitor != IntPtr.Zero)
      {
        var monitorInfo = new MONITORINFO();
        GetMonitorInfo(monitor, monitorInfo);
        var rcWorkArea = monitorInfo.rcWork;
        var rcMonitorArea = monitorInfo.rcMonitor;
        var r = new RECT();
        r.Left = rcWorkArea.Left;
        r.Top = rcWorkArea.Top;
        r.Right = rcWorkArea.Right;
        r.Bottom = rcWorkArea.Bottom;
        return r;
      }

      return new RECT();
    }
  }
}