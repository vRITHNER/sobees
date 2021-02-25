using System;
using System.Windows;
using System.Windows.Interop;

namespace Sobees.Glass.Native
{
  internal static class Helpers
  {
    internal static WindowInteropHelper GetWindowHandle(Window I)
    {
      var helper = new WindowInteropHelper(I);
      if (helper.Handle == IntPtr.Zero)
        throw new InvalidOperationException("The Window must be shown before retriving the handle");
      return helper;
    }
  }
}


