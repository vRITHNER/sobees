using System;
using System.Globalization;
using System.Windows.Data;
using Sobees.Library.BTwitterLib;
using Sobees.Tools.Logging;

namespace Sobees.Controls.TwitterSearch.Converters
{
  public class GeolocLongitudeConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      try
      {
        if (value == null) return 0;
        if (!value.GetType().Equals(typeof(TwitterEntry))) return 0;
        var entry = value as TwitterEntry;
        if (entry != null)
        {
          return ((TwitterUser)entry.User).Geolocation == null ? 0 : ((TwitterUser)entry.User).Geolocation.Longitude;
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }

      return 0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}