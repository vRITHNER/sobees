using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Data;
using Sobees.Tools.Logging;

namespace Sobees.Infrastructure.Converters
{
  /// <summary>
  /// This class simply takes an enum and uses some reflection to obtain
  /// the friendly name for the enum. Where the friendlier name is
  /// obtained using the LocalizableDescriptionAttribute, which hold the localized
  /// value read from the resource file for the enum
  /// </summary>
  //[ValueConversion(typeof(object), typeof(String))]
  public class EnumToFriendlyNameConverter : IValueConverter
  {
    #region IValueConverter implementation

    /// <summary>
    /// Convert value for binding from source object
    /// </summary>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      try
      {
        if(value == null) return null;

        FieldInfo fi = value.GetType().GetField(value.ToString());
        DescriptionAttribute[] attributes =
              (DescriptionAttribute[])fi.GetCustomAttributes(
              typeof(DescriptionAttribute), false);
        return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
        return value;
      }
    }

    /// <summary>
    /// ConvertBack value from binding back to source object
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value;
    }
    #endregion
  }
}
