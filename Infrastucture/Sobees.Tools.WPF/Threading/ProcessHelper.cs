#region

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Sobees.Library.BUtilities;
using Sobees.Tools.Logging;

#endregion

namespace Sobees.Tools.Threading
{

  public class ProcessHelper
  {
    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SetProcessWorkingSetSize(IntPtr process, UIntPtr minimumWorkingSetSize, UIntPtr maximumWorkingSetSize);

    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern IntPtr GetProcessHeap();

    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool HeapLock(IntPtr heap);

    [DllImport("kernel32.dll")]
    internal static extern uint HeapCompact(IntPtr heap, uint flags);

    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool HeapUnlock(IntPtr heap);

    public static bool CheckRunningProcess(string processName,
                                           int duration)
    {
      var nRetry = 00;

      //Check when the sobeesUpdate process is still alive
      while (true)
      {
        var sobeesProcesses = Process.GetProcessesByName(processName);

        //Process exist but we don't want to wait
        if (sobeesProcesses.Length != 0)
          return true;

        nRetry++;

        if (nRetry > duration)
          return false;

        ThreadHelper.DoEvents();
        Thread.Sleep(1000);
      }
    }

    public static bool CheckKilledProcess(string processName,
                                          int duration)
    {
      var nRetry = 00;

      //Check when the sobeesUpdate process is still alive
      while (true)
      {
        var sobeesProcesses = Process.GetProcessesByName(processName);

        //Process exist but we don't want to wait
        if (sobeesProcesses.Length == 0)
          return true;

        nRetry++;

        if (nRetry > duration)
          return false;

        ThreadHelper.DoEvents();
        Thread.Sleep(1000);
      }
    }

    public static void MinimizeMemory()
    {
      GC.Collect(GC.MaxGeneration);
      GC.WaitForPendingFinalizers();
      SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, (UIntPtr)0xFFFFFFFF, (UIntPtr)0xFFFFFFFF);

      var heap = GetProcessHeap();

      if (HeapLock(heap))
      {
        try
        {
          if (HeapCompact(heap, 0) == 0)
          {
          }
        }

        finally
        {
          HeapUnlock(heap);
        }
      }
    }

    /// <summary>
    /// GcCollect
    /// </summary>
    public static void GcCollect()
    {
      try
      {
        try
        {
          MinimizeMemory();
        }
        catch (Exception ex)
        {
          //Assert.Fail(ex.Message);
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("ProcessHelper::GCCollect:", ex);
      }
      finally
      {
        GC.Collect();
      }
    }

    public static long GetMemoryWorkingSet()
    {
      try
      {
        var p = Process.GetCurrentProcess();
        var m = p.WorkingSet64;
        return m;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("ProcessHelper::GetMemoryWorkingSet:", ex);
      }
      return 0;
    }

    public static long GetMemoryPrivateWorkingSet()
    {
      try
      {
        var p = Process.GetCurrentProcess();
        var m = p.PrivateMemorySize64;
        return m;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("ProcessHelper::GetMemoryPrivateWorkingSet:", ex);
      }
      return 0;
    }

    public static void PerformAggressiveCleanup()
    {
      GC.Collect(2);
      try
      {
        NativeMethods.SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("ProcessHelper::GCCollect:", ex);
      }
    }

    public static int GetCurrentProcessId()
    {
      try
      {
        return Process.GetCurrentProcess().Id;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("AssemblyHelper::GetExecutingAssemblyFullName:", (ex));
      }
      return -1;
    }
  }
}