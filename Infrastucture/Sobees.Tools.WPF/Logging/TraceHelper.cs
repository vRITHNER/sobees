#define TRACE

#region

using System;
using System.Diagnostics;
#if !SILVERLIGHT

#endif

#endregion

namespace Sobees.Tools.Logging
{
  public class TraceHelper
  {
    private const string SOBEES_CLIENT = "SOBEES_CLIENT";
#if !SILVERLIGHT
    private static readonly int _pid = Process.GetCurrentProcess().Id;
#else
    private static readonly int _pid = 0;
#endif

    public static void Trace(object sender,
                             string traceMessage)
    {
      Trace(sender, traceMessage, false);
    }

    public static void Trace(object sender,
                             string TraceMessage,
                             bool forceTrace)
    {
      if (forceTrace)
      {
        Debug.WriteLine(string.Format("Trace:{0}::{1}", sender, TraceMessage));
      }
      else
      {
        var _message = string.Empty;
        _message = string.Format("Trace:[{2}]-{0}::{1}:", sender, TraceMessage, _pid);
        Debug.WriteLine(_message);

//#if !SILVERLIGHT
//        LoggerService.FactoryResolver = new SimpleFactoryResolver(new Log4netLoggerFactory());
//        var logger = LoggerService.GetLogger(SOBEES_CLIENT);
//        logger.Info(_message);
//#endif
      }
    }

    public static DateTime TraceGetStartTime(object sender,
                                             string TraceMessage)
    {
      try
      {
        var StartTime = DateTime.Now;

        Debug.WriteLine(string.Format("Trace:{0}::{1}::StartTime:{2}", sender, TraceMessage, StartTime.ToShortTimeString()));
        return StartTime;
      }
      catch (Exception ex)
      {
        Trace("TraceHelper::TraceGetStartTime:", ex);
      }
      return DateTime.Now;
    }

    public static void TraceGetTimeDelay(object sender,
                                         string TraceMessage,
                                         DateTime StartTime)
    {
      try
      {
        var EndTime = DateTime.Now;
        var delay = EndTime.Subtract(StartTime);
        Debug.WriteLine(
          string.Format("Trace:{0}::{1}::StartTime:{2}|EndTime:{3}|Delay:{4}", sender, TraceMessage, StartTime.ToShortTimeString(), EndTime.ToShortTimeString(), delay));
      }
      catch (Exception ex)
      {
        Trace("TraceHelper::TraceGetTimeDelay:", ex);
      }
    }

    public static void Trace(object sender,
                             Exception ex)
    {
      Trace(sender, ex, false);
    }

    public static void Trace(object sender,
                             Exception ex,
                             bool forceTrace)
    {
      try
      {
        if (forceTrace)
        {
          var _message = string.Empty;

          if (ex.InnerException != null)
          {
            _message = string.Format("Trace Error:[{4}] - {0}::{1}||{2}||{3}:", ex.Data, ex.Message, ex.StackTrace, ex.InnerException.Message, _pid);
          }
          else
          {
            _message = string.Format("Trace Error:[{3}] - {0}::{1}||{2}", ex.Data, ex.Message, ex.StackTrace, _pid);
          }

          Debug.WriteLine(_message);

//#if !SILVERLIGHT

//          LogMessageASync(_message);

//#endif
        }
        else
        {
          var _message = string.Empty;

          if (ex.InnerException != null)
          {
            _message = string.Format("Trace Error:[{4}] - {0}::{1}||{2}||{3}", ex.Data, ex.Message, ex.StackTrace, ex.InnerException.Message, _pid);
          }
          else
          {
            _message = string.Format("Trace Error:[{3}] - {0}::{1}||{2}", ex.Data, ex.Message, ex.StackTrace, _pid);
          }

          Debug.WriteLine(_message);
//#if !SILVERLIGHT
//          LogMessageASync(_message);
//#endif
        }
      }

      catch (Exception ex1)
      {
        Debug.WriteLine("Error in TraceHelper:" + ex1.Message);
      }
    }

//#if !SILVERLIGHT
//    public static void LogMessageASync(string _message)
//    {
//      try
//      {

//                               LoggerService.FactoryResolver = new SimpleFactoryResolver(new Log4netLoggerFactory());
//                               var logger = LoggerService.GetLogger(SOBEES_CLIENT);
//                               logger.Info(_message);

//      }
//      catch (Exception ex)
//      {
//        BLogManager.LogEntry("TraceHelper::LogMessageASync:",
//                          (ex));
//      }
//    }
//#endif
  }
}