using System;
using System.Windows;
using System.Windows.Interop;

namespace BGlassWindow.Native
{
  internal static class Helpers
  {
    internal static WindowInteropHelper GetWindowHandle(Window I)
    {
      WindowInteropHelper helper = new WindowInteropHelper(I);
      if (helper.Handle == IntPtr.Zero)
        throw new InvalidOperationException("The Window must be shown before retriving the handle");
      return helper;
    }
  }
}


