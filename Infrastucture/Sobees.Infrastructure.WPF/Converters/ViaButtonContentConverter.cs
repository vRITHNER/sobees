using System;
using System.Globalization;
using System.Windows.Data;

namespace Sobees.Infrastructure.Converters
{
  public class ViaButtonContentConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null) return "";
      var source = value as String;
      return GetNameLinkFromHTMLLink(source);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
    public static string GetNameLinkFromHTMLLink(string text)
    {
      if (text == null)
      {
        return null;
      }

      var split = text.Split("<>".ToCharArray());
      if (split.Length > 3)
      {
        return split[2];
      }
      return text;
    }
    #endregion
  }
}