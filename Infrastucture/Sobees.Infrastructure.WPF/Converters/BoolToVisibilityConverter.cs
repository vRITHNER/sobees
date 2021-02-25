using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Sobees.Infrastructure.Converters
{
  public class BoolToVisibilityConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {

      var param = (string) parameter != "false";
      if (value == null) return null;
      if (!value.GetType().Equals(typeof(bool))) return null;
      if ((bool)value)
      {
        return param ? Visibility.Visible:Visibility.Collapsed;
      }

      return param ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}