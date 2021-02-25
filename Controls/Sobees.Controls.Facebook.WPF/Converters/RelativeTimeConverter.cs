#region

using System;
using System.Globalization;
using System.Windows.Data;
using Sobees.Tools.Logging;
#if !SILVERLIGHT
using Sobees.Library.BLocalizeLib;
#else
using Telerik.Windows.Controls;
#endif

#endregion

namespace Sobees.Controls.Facebook.Converters
{
  public class RelativeTimeConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null) return null;
      if (!value.GetType().Equals(typeof (DateTime))) return null;

      return GetRelativeTime((DateTime) value);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion

    public string GetRelativeTime(DateTime dt)
    {
      var relativeTime = string.Empty;

      try
      {
        //var ts = new TimeSpan(DateTime.Now.Ticks - dt.Ticks);
        var ts = new TimeSpan(DateTimeOffset.UtcNow.Ticks - dt.Ticks);
        var delta = ts.TotalSeconds;
#if !SILVERLIGHT
        if (delta <= 1)
        {
          relativeTime = new LocText("Sobees.Configuration.BGlobals:Resources:timAgoASecond").ResolveLocalizedValue();
        }
        else if (delta < 60)
        {
          relativeTime = ts.Seconds +
                         new LocText("Sobees.Configuration.BGlobals:Resources:timAgoXSecond").ResolveLocalizedValue();
        }
        else if (delta < 120)
        {
          relativeTime = new LocText("Sobees.Configuration.BGlobals:Resources:timAgoAMinute").ResolveLocalizedValue();
        }
        else if (delta < (60*60))
        {
          //BDuleSettings.Language.ToLower().Equals("fr")
          relativeTime = ts.Minutes +
                         new LocText("Sobees.Configuration.BGlobals:Resources:timeAgoXMinute").ResolveLocalizedValue();
        }
        else if (delta < (90*60))
        {
          relativeTime = new LocText("Sobees.Configuration.BGlobals:Resources:timAgoAHour").ResolveLocalizedValue();
        }
        else if (delta < (24*60*60))
        {
          relativeTime = new LocText("Sobees.Configuration.BGlobals:Resources:timAgoAbout").ResolveLocalizedValue() +
                         ts.Hours +
                         new LocText("Sobees.Configuration.BGlobals:Resources:timeAgoXHour").ResolveLocalizedValue();
        }
        else if (delta < (48*60*60))
        {
          relativeTime = new LocText("Sobees.Configuration.BGlobals:Resources:timeAgoADay").ResolveLocalizedValue();
        }
        else
        {
          relativeTime = ts.Days +
                         new LocText("Sobees.Configuration.BGlobals:Resources:timeAgoXDay").ResolveLocalizedValue();
        }
      }
#else
        if (delta <= 1)
        {
            relativeTime = LocalizationManager.GetString("timAgoASecond");

        }
        else if (delta < 60)
        {
          relativeTime = ts.Seconds + LocalizationManager.GetString("timAgoXSecond");
        }
        else if (delta < 120)
        {
          relativeTime = LocalizationManager.GetString("timAgoAMinute");
        }
        else if (delta < (60 * 60))
        {
          relativeTime = ts.Minutes + LocalizationManager.GetString("timeAgoXMinute");
        }
        else if (delta < (90 * 60))
        {
          relativeTime = LocalizationManager.GetString("timAgoAHour");
        }
        else if (delta < (24 * 60 * 60))
        {
          relativeTime = LocalizationManager.GetString("timAgoAbout") + ts.Hours + LocalizationManager.GetString("timeAgoXHour");
        }
        else if (delta < (48 * 60 * 60))
        {
          relativeTime = LocalizationManager.GetString("timeAgoADay");
        }
        else
        {
          relativeTime = ts.Days + LocalizationManager.GetString("timeAgoXDay");
        }
      }
#endif
      catch (Exception ex)
      {
        TraceHelper.Trace(this,
          ex);
      }
      return relativeTime;
    }
  }
}