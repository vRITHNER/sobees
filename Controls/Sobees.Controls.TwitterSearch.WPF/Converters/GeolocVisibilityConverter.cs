using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Sobees.Library.BTwitterLib;
using Sobees.Tools.Logging;
#if SILVERLIGHT
using TwitterServiceProxy;
#endif

namespace Sobees.Controls.TwitterSearch.Converters
{
  public class GeolocVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      try
      {
        if (value == null) return Visibility.Collapsed;
        if (!value.GetType().Equals(typeof(TwitterEntry))) return Visibility.Collapsed;
        var entry = value as TwitterEntry;
        if (entry != null)
        {
          return ((TwitterUser)entry.User).Geolocation == null ? Visibility.Collapsed : Visibility.Visible;
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }

      return Visibility.Collapsed;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}