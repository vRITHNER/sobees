#region

using System;
using Sobees.Tools.Helpers;

#endregion

namespace Sobees.Tools.Extensions
{
  public static class DateExtensions
  {
    public static DateTime ToConvertFromUniversalTimeToDate(this ulong timestamp)
    {
      return DateTimeHelper.ConvertUniversalTimeToDate(timestamp);
    }
  }
}