using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Sobees.Controls.LinkedIn.Converters
{
  public class LinkedInVisibilityTypeConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var typeToDisplay = parameter as string;
      var type = value as string;
      if (type == typeToDisplay)
      {
        return Visibility.Visible;
      }
      return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}