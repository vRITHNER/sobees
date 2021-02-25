#region

using System;
using System.Globalization;

#endregion

namespace Sobees.Tools.Helpers
{
  public class DateTimeHelper
  {
    public static DateTime ConvertFromUnixTimestamp(double timestamp)
    {
      var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
      return origin.AddSeconds(timestamp);
    }

    public static double ConvertToUnixTimestamp(DateTime date)
    {
      var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
      var diff = date - origin;
      return Math.Floor(diff.TotalSeconds);
    }

    public static DateTime ConvertUniversalTimeToDate(double timestamp)
    {
      // First make a System.DateTime equivalent to the UNIX Epoch.
      var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

      // Add the number of seconds in UNIX timestamp to be converted.
      return dateTime.AddSeconds(timestamp);
    }

    public static DateTime ConvertUniversalTimeToDate(ulong timestamp)
    {
      // First make a System.DateTime equivalent to the UNIX Epoch.
      var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

      // Add the number of seconds in UNIX timestamp to be converted.
      return dateTime.AddSeconds(timestamp);
    }

    /// <summary>
    ///   this method try to return a DateTime from a given string date value 
    ///   return the default date if parsing failed
    /// </summary>
    /// <param name = "date"></param>
    /// <returns></returns>
    public static DateTime TryParseDate(string date)
    {
      return TryParseDate(date, new DateTime());
    }

    /// <summary>
    ///   this method try to return a DateTime from a given string date value 
    ///   return the given default date if parsing failed
    /// </summary>
    /// <param name = "date"></param>
    /// <param name = "defaultDate"></param>
    /// <returns></returns>
    public static DateTime TryParseDate(string date,
                                        DateTime defaultDate)
    {
      DateTime newDate2;

      if (DateTime.TryParse(date, out newDate2) || DateTime.TryParse(date, new CultureInfo("Fr-fr"), DateTimeStyles.None, out newDate2) ||
          DateTime.TryParse(date, CultureInfo.CurrentUICulture, DateTimeStyles.None, out newDate2))
        return newDate2;

      return defaultDate;
    }

    public static DateTime TryParseDate(string date,
                                        string defaultCulture)
    {
      DateTime newDate2;

      if (DateTime.TryParse(date, new CultureInfo(defaultCulture), DateTimeStyles.None, out newDate2))
        return newDate2;

      if (DateTime.TryParse(date, out newDate2))
        return newDate2;

      if (DateTime.TryParse(date, CultureInfo.CurrentUICulture, DateTimeStyles.None, out newDate2))
        return newDate2;

      //TODO: TO finish it
      return DateTime.Parse("1-1-80");
    }
  }
}