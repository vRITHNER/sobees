#region

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;

#endregion

namespace Sobees.Library.BUtilities
{
  public static class MouseUtilities
  {
    public static Point CorrectGetPosition(Visual relativeTo)
    {
      var w32Mouse = new Win32Point();
      GetCursorPos(ref w32Mouse);
      return new Point(w32Mouse.X, w32Mouse.Y);
    }

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetCursorPos(ref Win32Point pt);

    #region Nested type: Win32Point

    [StructLayout(LayoutKind.Sequential)]
    internal struct Win32Point
    {
      public Int32 X;
      public Int32 Y;
    } ;

    #endregion
  }
}