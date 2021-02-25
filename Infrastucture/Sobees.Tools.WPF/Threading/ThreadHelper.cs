#region

using System;
using System.Security.Permissions;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Sobees.Tools.Logging;

#endregion

namespace Sobees.Tools.Threading
{
  public class ThreadHelper
  {
    #region DoEvents

    /// <summary>
    ///   Forces the WPF message pump to process all enqueued messages
    ///   that are above the input parameter DispatcherPriority.
    /// </summary>
    /// <param name="priority">
    ///   The DispatcherPriority to use
    ///   as the lowest level of messages to get processed
    /// </param>
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public static void DoEvents(DispatcherPriority priority)
    {
      if (Mouse.PrimaryDevice.LeftButton.Equals(MouseButtonState.Pressed))
      {
        TraceHelper.Trace("mouse pressed", "Do events Aborded");
        return;
      }
      var timer = new DispatcherTimer();
      var frame = new DispatcherFrame();
      var dispatcherOperation = Dispatcher.CurrentDispatcher.BeginInvoke(priority,
                                                                         new DispatcherOperationCallback(
                                                                           ExitFrameOperation), frame);
      timer.Interval = new TimeSpan(0, 0, 0, 1, 0);
      timer.Tick += delegate
                      {
                        ExitFrameOperation(frame);
                        timer.Stop();
                      };
      timer.Start();
      Dispatcher.PushFrame(frame);
      if (dispatcherOperation.Status != DispatcherOperationStatus.Completed)
        dispatcherOperation.Abort();
    }


    /// <summary>
    ///   Forces the WPF message pump to process all enqueued messages
    ///   that are DispatcherPriority.Background or above
    /// </summary>
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public static void DoEvents()
    {
      DoEvents(DispatcherPriority.Background);
    }

    /// <summary>
    ///   Stops the dispatcher from continuing
    /// </summary>
    private static object ExitFrameOperation(object obj)
    {
      ((DispatcherFrame) obj).Continue = false;
      return null;
    }

    #endregion

    public static void Refresh(DependencyObject obj)
    {
      try
      {
        obj.Dispatcher.Invoke(
          DispatcherPriority.ApplicationIdle,
          (NoArgDelegate) delegate { });
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("", ex);
      }
    }

    public static Thread RunThread(ThreadStart ts,
                                   bool bWaitForCompletion)
    {
      try
      {
        var thread = new Thread(ts);
        thread.SetApartmentState(ApartmentState.STA);
        thread.IsBackground = true;
        thread.Start();

        if (bWaitForCompletion)
          thread.Join();

        return thread;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("ThreadHelper::RunThread:", ex);
      }
      return null;
    }

    #region Nested type: NoArgDelegate

    private delegate void NoArgDelegate();

    #endregion
  }
}