using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Sobees.Library.BTwitterLib;
using Sobees.Tools.Logging;

namespace Sobees.Controls.Twitter.Converters
{
  public class DeleteButtonVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      try
      {
        if (value == null) return Visibility.Collapsed;
        var entry = value as TwitterEntry;
        if (entry != null)
        {
          if (entry.CanPost == 1)
          {
            return Visibility.Visible;
          }
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
      return null;
    }
  }
}