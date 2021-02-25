using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Sobees.Library.BTwitterLib;
using Sobees.Tools.Logging;

namespace Sobees.Controls.TwitterSearch.Converters
{
  public class InReplyToVisibilityConverter : IValueConverter
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
          return string.IsNullOrEmpty(entry.InReplyToUserName) ? Visibility.Collapsed : Visibility.Visible;
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