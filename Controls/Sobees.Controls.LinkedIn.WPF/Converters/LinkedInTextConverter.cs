#region

using System;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Windows.Data;
using Sobees.Library.BLinkedInLib;
using Sobees.Library.BLocalizeLib;

#endregion

namespace Sobees.Controls.LinkedIn.Converters
{
  public class LinkedInTextConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var entry = value as LinkedInEntry;
      if (entry == null) return "ERROR";

      switch (entry.UpdateType)
      {
        case "NCON":
          return new LocText("Sobees.Configuration.BGlobals:Resources:txtLinkedInNCON").ResolveLocalizedValue();
        case "CONN":
          return new LocText("Sobees.Configuration.BGlobals:Resources:txtLinkedInCONN").ResolveLocalizedValue();
        case "PICU":
          return new LocText("Sobees.Configuration.BGlobals:Resources:txtLinkedInPICU").ResolveLocalizedValue();
        case "SHAR":
          if (entry.Likes != null && entry.Likes.Any())
          {
            var txt = string.Format(" likes {0} {1}", entry.Likes.Select(l => l.User.NickName), entry.Likes.Select(l => l.Href));
            return txt;
            //new LocText("Sobees.Configuration.BGlobals:Resources:txtLinkedInSHAR").ResolveLocalizedValue();
          }
          return string.Format(" {0}", entry.Title);
        case "PROF":
          if (entry.Educations != null && entry.Educations.Any())
            {
              var txt =
                new LocText("Sobees.Configuration.BGlobals:Resources:txtLinkedInPROFstuding").ResolveLocalizedValue();
              foreach (var education in entry.Educations)
              {
                txt += string.Format("{0}{1}{2}", education.FieldOfStudy, new LocText("Sobees.Configuration.BGlobals:Resources:txtLinkedInPROFstudingAt")
                    .ResolveLocalizedValue(), education.SchoolName);
              }
              return txt;
            }
          return new LocText("Sobees.Configuration.BGlobals:Resources:txtLinkedInPROFUpdated").ResolveLocalizedValue();
        case "JGRP":
          if (entry.Groups != null)
            if (entry.Groups.Any())
            {
              var txt = new LocText("Sobees.Configuration.BGlobals:Resources:txtLinkedInJGRP").ResolveLocalizedValue();
              foreach (var group in entry.Groups)
              {
                txt += @group.Name;
              }
              return txt;
            }
          return "TODO";
        case "PREC":

          if (entry.Recommendations != null && entry.Recommendations.Any())
          {
            var txt = new LocText("Sobees.Configuration.BGlobals:Resources:txtLinkedInPREC").ResolveLocalizedValue();
            foreach (var recommendation in entry.Recommendations)
            {
              txt += string.Format("{0} {1}", recommendation.Recommendee.Name, recommendation.Snippet);
            }
            return txt;
          }
          return "TODO";
        case "APPM":
          if (entry.Activities != null) return HttpUtility.HtmlDecode(entry.Activities[0].Body);
          return "TODO";
        case "JOBP":
          return new LocText("Sobees.Configuration.BGlobals:Resources:txtLinkedInJOBP").ResolveLocalizedValue() +
                 entry.Job.Title;
        default:
          return "";
      }

      return "ERROR";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    public string AddLink(string body)
    {
      return body.Replace("<a href=", "[link=").Replace("\">", "\" target=\"_blank\"]").Replace("</a>", "[/link]");
    }

    #endregion
  }
}