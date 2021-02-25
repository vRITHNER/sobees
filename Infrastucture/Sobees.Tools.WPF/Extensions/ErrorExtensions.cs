#region

using System;

#endregion

namespace Sobees.Tools.Extensions
{
  public static class ErrorExtensions
  {
    public static string ToCompleteExceptionMessage(this Exception exception)
    {
      var ex = exception;
      var msg = exception.Message;
      while (ex.InnerException != null)
      {
        msg += "\n\r" + ex.InnerException.Message;
        ex = ex.InnerException;
      }
      return msg;
    }
  }
}