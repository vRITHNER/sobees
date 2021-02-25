#region

using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Data;
using Sobees.Library.BFacebookLibV2.Objects.Feed;
using System.Web;

#endregion

namespace Sobees.Controls.Facebook.Converters
{
  public class FacebookAppDescriptionConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null) return "";
      var entry = value as FacebookFeedEntry;
      if (entry?.Attachements != null && entry.Attachements.Data.Any(a=> a.Description != null))
      {
        var text = entry.Attachements.Data.Select(a=> a.Description).FirstOrDefault();
        while (text != null && text.Contains("  "))
        {
          text = text.Replace("  ", " ");
        }
        text = text.Replace("\n", "");
        text = text.Replace("<br />", "\n");
        text = text.Replace("<br/>", "\n");
        var regexXml = new Regex("<[\\s\\S]*?>");
        return HttpUtility.HtmlDecode(regexXml.Replace(text,
          ""));
      }

      return string.Empty;
    }


    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}