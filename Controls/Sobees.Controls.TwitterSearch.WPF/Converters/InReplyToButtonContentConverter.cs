
using System;
using System.Globalization;
using System.Windows.Data;
using Sobees.Tools.Logging;
#if SILVERLIGHT
using TwitterServiceProxy;
#else
using Sobees.Library.BLocalizeLib;
using Sobees.Library.BTwitterLib;
#endif


namespace Sobees.Controls.TwitterSearch.Converters
{
  public class InReplyToButtonContentConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      try
      {
        if (value == null) return "";
        if (!value.GetType().Equals(typeof(TwitterEntry))) return "";
        var entry = value as TwitterEntry;
        if (entry != null)
        {
          //var text = " in reply to " + entry.InReplyToUserName;

#if!SILVERLIGHT
          var text = new LocText("Sobees.Configuration.BGlobals:Resources:txtInReplyTo").ResolveLocalizedValue() + entry.InReplyToUserName;
#else
          var text = " in reply to " + entry.InReplyToUserName;
#endif
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
      throw new NotImplementedException();
    }
  }
}
