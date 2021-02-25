using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Sobees.Infrastructure.Converters
{
  public class NullVisibilityConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if(value == null)
      {
        return Visibility.Collapsed;
      }
      if(value is string)
      {
        var intValue = -1L;
        long.TryParse(value.ToString(), out intValue);

        bool bisVisible;

        if (intValue == -1)
          bisVisible = !string.IsNullOrEmpty(value.ToString());
        else
          bisVisible = intValue > 0;
        
        return bisVisible ? Visibility.Visible : Visibility.Collapsed;
      }
      if (value is int)
      {
        return (int) value == 0 ? Visibility.Collapsed : Visibility.Visible;
      }
      if (value is long)
      {
        return (long)value == 0 ? Visibility.Collapsed : Visibility.Visible;
      }
      if (value is double)
      {
        return (double)value == 0 ? Visibility.Collapsed : Visibility.Visible;
      }
      if (value is DateTime)
      {
        return (DateTime)value == new DateTime(1970,1,1,0,0,0,0) ? Visibility.Collapsed : Visibility.Visible;
      }
      return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion

  }
}