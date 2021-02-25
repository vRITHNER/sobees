using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Sobees.Infrastructure.Converters
{
  public class TwitpicVisibilityConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null) return Visibility.Collapsed;
      var source = value as String;
      if (source != null) return source.Contains("twitpic.com/") || source.Contains("tweetphoto.com/") ? Visibility.Visible : Visibility.Collapsed;
      return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
    #endregion
  }
}