using System;
using System.Globalization;
using System.Windows.Data;
using Sobees.Library.BGenericLib;
using Sobees.Tools.Logging;

namespace Sobees.Infrastructure.Converters
{
  public class ImageServiceConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null) return "";

      EnumType source=EnumType.Other;
#if !SILVERLIGHT
      source = value is EnumType ? (EnumType)value : EnumType.Other;
#else
      try
      {
        source = (EnumType)value;
      }
      catch (Exception ex)
      {
        source = EnumType.Other;
        TraceHelper.Trace(this, ex);
      }
#endif

      switch (source)
      {
        case EnumType.Facebook:
          return "/Sobees.Templates;Component/Images/Services/facebook_small.png";
          break;
        case EnumType.Twitter:
          break;
        case EnumType.TwitterSearch:
          return "/Sobees.Templates;Component/Images/Services/twitter_search_small.png";
        case EnumType.Flickr:
          break;
        case EnumType.NYtimes:
          break;
        case EnumType.TwitterBitlynow:
          break;
        case EnumType.TweetMeme:
          break;
        case EnumType.WhatTheTrend:
          break;
        case EnumType.LinkedIn:
          break;
        case EnumType.Rss:
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
      return "";
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