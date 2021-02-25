using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Sobees.Infrastructure.Converters
{
  public class InvertBoolConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return !(value is bool ? (bool) value : false);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}