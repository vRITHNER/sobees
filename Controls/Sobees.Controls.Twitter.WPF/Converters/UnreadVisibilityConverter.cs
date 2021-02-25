using System;
using System.Globalization;
using System.Windows.Data;

namespace Sobees.Controls.Twitter.Converters
{
  public class UnreadVisibilityConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null) return null;
      if (!value.GetType().Equals(typeof(bool))) return null;
      if ((bool)value)
      {
        return "Collapsed";
      }

      return "Visible";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}