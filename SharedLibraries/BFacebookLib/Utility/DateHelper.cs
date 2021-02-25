using System;

namespace Sobees.Library.BFacebookLibV1.Utility
{
    ///<summary>
    /// Contains helper for converting to and from the date formats provided by facebook
    ///</summary>
    public static class DateHelper
    {
        ///<summary>
        /// Returns a datetime corresponding to 1/1/1970
        ///</summary>
        public static DateTime BaseUTCDateTime
        {
            get { return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc); }
        }

        ///<summary>
        /// Returns Local time of the server from UnixTime UTC
        ///</summary>
        public static DateTime ConvertUnixTimeToLocalTime(double secondsSinceEpoch)
        {
            return BaseUTCDateTime.AddSeconds(secondsSinceEpoch).ToLocalTime();
        }


        /// <summary>
        /// Returns UnixTime UTC from Local time of the server
        /// </summary>
        public static double ConvertLocalTimeToUnixTime(DateTime dateToConvert)
        {
            return (double)((dateToConvert.ToUniversalTime() - BaseUTCDateTime).TotalSeconds);
        }
        //////////////////////////////////////////////////////////////////////////////////////

        ///<summary>
        /// Returns Local time of the server from UnixTime UTC
        ///</summary>
        public static DateTime ConvertUnixTimeToDateTime(long secondsSinceEpoch)
        {
            return ConvertUnixTimeToLocalTime((double)secondsSinceEpoch);
        }

        /// <summary>
        /// Convert datetime to UTC time, as understood by Facebook.
        /// </summary>
        /// <param name="dateToConvert">The date that we need to pass to the api.</param>
        /// <returns>The number of seconds since Jan 1, 1970.</returns>
        public static long ConvertDateToFacebookDate(DateTime dateToConvert)
        {
            return (long)ConvertLocalTimeToUnixTime(dateToConvert);
        }

        /// <summary>
        /// Convert UTC time, as returned by Facebook, to localtime.
        /// </summary>
        /// <param name="secondsSinceEpoch">The number of seconds since Jan 1, 1970.</param>
        /// <returns>Local time.</returns>
        internal static DateTime ConvertDoubleToDate(double secondsSinceEpoch)
        {
#if !SILVERLIGHT
            return ConvertUnixTimeToLocalTime(secondsSinceEpoch);
#else
            return TimeZoneInfo.ConvertTime(BaseUTCDateTime.AddSeconds(secondsSinceEpoch), TimeZoneInfo.Local);
#endif
        }

        /// Returns Local time of the server from UnixTime UTC
        internal static DateTime ConvertDoubleToEventDate(double secondsSinceEpoch)
        {
            return ConvertUnixTimeToLocalTime(secondsSinceEpoch);
        }

        /// <summary>
        /// Convert datetime to UTC time, as understood by Facebook.
        /// </summary>
        /// <param name="dateToConvert">The date that we need to pass to the api.</param>
        /// <returns>The number of seconds since Jan 1, 1970.</returns>
        internal static double? ConvertDateToDouble(DateTime? dateToConvert)
        {
            return dateToConvert != null ? new double?(ConvertLocalTimeToUnixTime(dateToConvert.Value)) : null;
        }
    }
}