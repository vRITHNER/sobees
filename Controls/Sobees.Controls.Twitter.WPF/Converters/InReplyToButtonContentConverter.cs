using System;
using System.Globalization;
using System.Windows.Data;
using Sobees.Library.BLocalizeLib;
using Sobees.Library.BTwitterLib;
using Sobees.Tools.Logging;

namespace Sobees.Controls.Twitter.Converters
{
  public class InReplyToButtonContentConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      try
      {
        if (value == null) return "";
        var entry = value as TwitterEntry;
        if (entry != null)
        {
          var text = new LocText("Sobees.Configuration.BGlobals:Resources:txtInReplyTo").ResolveLocalizedValue() + entry.InReplyToUserName;
          return text;
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }

      return "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      //throw new NotImplementedException();
      return null;
    }
  }
}