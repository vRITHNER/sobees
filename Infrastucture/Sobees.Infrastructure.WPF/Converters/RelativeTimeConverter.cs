using System;
using System.Globalization;
using System.Windows.Data;
#if !SILVERLIGHT
using Sobees.Library.BLocalizeLib;
#else
using Telerik.Windows.Controls;
#endif
using Sobees.Tools.Logging;

namespace Sobees.Infrastructure.Converters
{
  public class RelativeTimeConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null) return null;
      if (!value.GetType().Equals(typeof(DateTime))) return null;

      return GetRelativeTime((DateTime)value);
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
        var ts = new TimeSpan(DateTime.Now.Ticks - dt.Ticks);
        var delta = ts.TotalSeconds;
#if !SILVERLIGHT
        if (delta <= 1)
        {
          relativeTime = new LocText("Sobees.Configuration.BGlobals:Resources:timAgoASecond").ResolveLocalizedValue();

        }
        else if (delta < 60)
        {
          relativeTime = ts.Seconds + new LocText("Sobees.Configuration.BGlobals:Resources:timAgoXSecond").ResolveLocalizedValue();
        }
        else if (delta < 120)
        {
          relativeTime = new LocText("Sobees.Configuration.BGlobals:Resources:timAgoAMinute").ResolveLocalizedValue();
        }
        else if (delta < (60 * 60))
        {
          relativeTime = ts.Minutes + new LocText("Sobees.Configuration.BGlobals:Resources:timeAgoXMinute").ResolveLocalizedValue();
        }
        else if (delta < (90 * 60))
        {
          relativeTime = new LocText("Sobees.Configuration.BGlobals:Resources:timAgoAHour").ResolveLocalizedValue();
        }
        else if (delta < (24 * 60 * 60))
        {
          relativeTime = new LocText("Sobees.Configuration.BGlobals:Resources:timAgoAbout").ResolveLocalizedValue() + ts.Hours + new LocText("Sobees.Configuration.BGlobals:Resources:timeAgoXHour").ResolveLocalizedValue();
        }
        else if (delta < (48 * 60 * 60))
        {
          relativeTime = new LocText("Sobees.Configuration.BGlobals:Resources:timeAgoADay").ResolveLocalizedValue();
        }
        else
        {
          relativeTime = ts.Days + new LocText("Sobees.Configuration.BGlobals:Resources:timeAgoXDay").ResolveLocalizedValue();
        }

        //if (delta <= 1)
        //{
        //    relativeTime = "a second ago";

        //}
        //else if (delta < 60)
        //{
        //    relativeTime = ts.Seconds + " seconds ago";
        //}
        //else if (delta < 120)
        //{
        //    relativeTime = "about a minute ago";
        //}
        //else if (delta < (60*60))
        //{
        //    relativeTime = ts.Minutes + " minutes ago";
        //}
        //else if (delta < (90*60))
        //{
        //    relativeTime = "about an hour ago";
        //}
        //else if (delta < (24*60*60))
        //{
        //    relativeTime = "about " + ts.Hours + " hours ago";
        //}
        //else if (delta < (48*60*60))
        //{
        //    relativeTime = "1 day ago";
        //}
        //else
        //{
        //    relativeTime = ts.Days + " days ago";
        //}

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
          if (LocalizationManager.DefaultCulture.Name.ToUpper().Equals("ES"))
          {
            relativeTime = LocalizationManager.GetString("timeAgoXDay") + ts.Days + LocalizationManager.GetString("timeAgoXDayEs");
          }
          else
          {
            relativeTime = ts.Days + LocalizationManager.GetString("timeAgoXDay");
          }
        }
#endif
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this,
                          ex);
      }
      return relativeTime;
    }
  }
}