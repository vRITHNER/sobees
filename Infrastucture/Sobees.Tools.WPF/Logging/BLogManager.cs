#region

using System;
using System.Diagnostics;
using System.Reflection;
using Sobees.Tools.Logging;
using log4net;

#endregion

namespace BUtility
{
  public class BLogManager
  {
    private static readonly string APPNAME = Assembly.GetExecutingAssembly().GetName().Name;
    private static readonly object _syncRoot = new object();
    private static readonly int _pid = Process.GetCurrentProcess().Id;

    private static readonly ILog _log = LogManager.GetLogger(APPNAME);

    public static void LogEntry(string procedureName,
                                string message)
    {
      try
      {
        _log.Info(string.Format("{0}::Message:{1}", procedureName, message));
      }
      catch (Exception ex)
      {

      }
    }

    public static void LogEntry(string procedureName,
                                string message,
                                bool traceInConsole)
    {
      try
      {
        var msg = string.Format("{0}::Message:{1}", procedureName, message);
        _log.Info(msg);

        if (traceInConsole)
          Debug.WriteLine(msg);
      }
      catch (Exception ex)
      {
      }
    }

    public static void LogEntry(string procedureName,
                                Exception exception,
                                out string result)
    {
      try
      {
        lock (_syncRoot)
        {
          var msg = exception.Message;
          result = msg;
          try
          {
            var exx = exception.InnerException;
            //
            while (true)
            {
              if (exx == null)
              {
                break;
              }
              msg += exx.Message;
              exx = exx.InnerException;
            }

            //var trace = new StackTrace(exception, true);
            //var frame = trace.GetFrame(0);
            //msg += string.Format("\n\rFileName:[{0}]|MethodName:[{1}]|Line#:[{2}]|Column#:[{3}]",frame.GetFileName(), frame.GetMethod().Name, frame.GetFileLineNumber(), frame.GetFileColumnNumber());
          }
          catch (Exception ex)
          {
            Debug.WriteLine("Error:LogManager::LogEntry:" + ex.Message);
          }

          _log.Error(string.Format("{0}::Error:{1}", procedureName, msg));
          TraceHelper.Trace(string.Format("{0}::{1}", APPNAME, procedureName), msg);
          result = msg;
          return;
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(APPNAME, (ex));

        result = string.Format("Error:LogEntry Error:{0}", ex.Message);
      }
    }

    public static void LogEntry(string message,
                                bool traceInConsole)
    {
      LogEntry(APPNAME, message, traceInConsole);
    }

    public static void LogEntry(string message)
    {
      LogEntry(APPNAME, message);
    }

    public static void LogEntry(object sender,
                                Exception exception)
    {
      LogEntry(sender.ToString(), exception);
    }

    public static void LogEntry(Exception exception)
    {
      LogEntry(APPNAME, exception);
    }

    public static void LogEntry(string procedureName,
                                Exception exception)
    {
      string result;
      LogEntry(procedureName, exception, out result);
    }

    public static void LogEntry(string moduleName, string procedureName, Exception exception)
    {
      var fullProcedureName = string.Format("{0}::{1}::", moduleName, procedureName.Replace("::", ""));
      LogEntry(fullProcedureName, exception);
    }

    public static void LogEntry(string moduleName, string procedureName, string message, bool traceInConsole)
    {
      var fullProcedureName = string.Format("{0}::{1}::", moduleName, procedureName.Replace("::", ""));
      LogEntry(fullProcedureName, message, traceInConsole);
    }

    public static void LogEntry(string applicationName, string moduleName, string procedureName, Exception exception)
    {
      var fullProcedureName = string.Format("{0}::{1}::{2}", applicationName.Replace("::", ""), moduleName.Replace("::", ""), procedureName.Replace("::", ""));
      LogEntry(fullProcedureName, exception);
    }

    public static void LogEntry(string applicationName, string moduleName, string procedureName, string message, bool traceInConsole)
    {
      var fullProcedureName = string.Format("{0}::{1}::{2}", applicationName.Replace("::", ""), moduleName.Replace("::", ""), procedureName.Replace("::", ""));
      LogEntry(fullProcedureName, message, traceInConsole);
    }
  }
}