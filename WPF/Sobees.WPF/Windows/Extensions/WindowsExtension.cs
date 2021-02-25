#region

using System;
using System.Windows;
using Sobees.Tools.Logging;
using Sobees.Windows;
using BUtility;

#endregion

namespace Sobees.Windows.Extensions
{
  public static class WindowExtension
  {
    private const string APPNAME = "WindowsExtension";

    /// <summary>
    ///   Return the current Window Position + Size
    /// </summary>
    /// <param name = "w"></param>
    /// <returns></returns>
    public static WindowLocation GetWindowLocation(this Window w)
    {
      var window = new WindowLocation {Name = w.Name, Top = w.Top, Left = w.Left, Width = w.ActualWidth, Height = w.ActualHeight};
      return window;
    }

    public static void CenterPositionInParentWindow(this Window window, WindowLocation parentWindow)
    {
      window.Top = parentWindow.Top + ((parentWindow.Height - window.Height)/2);
      window.Left = parentWindow.Left + ((parentWindow.Width - window.Width)/2);

      if (window.Top < 0)
        window.Top = 0;

      if (window.Left < 0)
        window.Left = 0;
    }

    public static void ShowWindow(this BWindowBase w, WindowLocation parentWindow)
    {
      if (parentWindow != null)
        if (w.CenterInParentWindow)
          CenterPositionInParentWindow(w, parentWindow);

      w.Visibility = Visibility.Visible;
      w.Activate();
    }

    public static void ShowDialogWindow(this BWindowBase w, WindowLocation parentWindow)
    {
      try
      {
        if (parentWindow != null)
          if (w.CenterInParentWindow)
            CenterPositionInParentWindow(w, parentWindow);

        w.ShowDialog();
      }
      catch (Exception ex)
      {
        BLogManager.LogEntry(APPNAME + "::ShowDialogWindow:", ex);
      }
    }
  }
}