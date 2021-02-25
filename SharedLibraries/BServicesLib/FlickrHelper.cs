using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Sobees.Library.BGenericLib;
using Sobees.Tools.Helpers;
using Sobees.Tools.Logging;
using Sobees.Library.BServicesLib.Properties;

namespace Sobees.Library.BServicesLib
{
  public class FlickrHelper
  {
    #region Flickr
    public static string GetUserId(string userName)
    {
      if (!string.IsNullOrEmpty(userName))
      {
        string path = Resources.flickrFindByUsername +
                      "&api_key=" + Resources.flickrApiKey +
                      "&username=" + userName;

        try
        {
          XDocument feedXML = XDocument.Load(path);

          //var query = from entry in feedXML.Descendants("user").
          XElement xe = feedXML.Element("rsp").Element("user");
          if (xe != null && xe.Attribute("id") != null)
          {
            return xe.Attribute("id").Value;
          }
        }
        catch (Exception ex)
        {
          return string.Empty;
        }
      }
      return string.Empty;
    }

    public static List<Entry> SearchFlickr(string query,
                                       EnumFlickrSort sort,
                                       int rpp,
                                       int startPage)
    {
      try
      {
        string path = Resources.searchFlickr +
            "&api_key=" + Resources.flickrApiKey +
            "&text=" + query +
            "&sort=" + FlickrEnum.GetStateDescription(sort) +
            "&per_page=" + rpp +
            "&page=" + startPage +
            "&safe_search=1";

        XDocument feedXML = XDocument.Load(@path);
        return BuildFlickrEntries(feedXML);
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("GenericLib::SearchFlickr:", ex);
        return new List<Entry>();
      }
    }

    public static List<Entry> SearchFlickrForUser(string userId, EnumFlickrSort sort, int rpp, int startPage)
    {
      return SearchFlickr(userId,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    new DateTime(),
                    new DateTime(),
                    new DateTime(),
                    new DateTime(),
                    sort,
                    rpp,
                    startPage,
                    16,
                    7,
                    string.Empty);
    }

    public static List<Entry> GetUserFlickrPhotos(string token, string secret, string userId, EnumFlickrSort sort, int rpp, int startPage)
    {
      try
      {
        SortedDictionary<string, string> args = new SortedDictionary<string, string>();
        args.Add("api_key", Resources.flickrApiKey);
        args.Add("auth_token", token);
        args.Add("method", "flickr.photos.search");
        args.Add("page", startPage.ToString());
        args.Add("per_page", rpp.ToString());
        args.Add("safe_search", "1");
        args.Add("sort", FlickrEnum.GetStateDescription(sort));
        args.Add("user_id", userId);

        // Build signature & Path
        StringBuilder sbSig = new StringBuilder();
        StringBuilder sbPath = new StringBuilder();
        sbSig.Append(secret);
        sbPath.Append(Resources.flickrRestApiUrl);

        int i = 0;
        foreach (var arg in args)
        {
          sbPath.Append(arg.Key);
          sbPath.Append("=");
          sbPath.Append(UrlEncode(arg.Value));

          if (i < args.Count - 1)
            sbPath.Append("&");

          sbSig.Append(arg.Key);
          sbSig.Append(arg.Value);

          ++i;
        }

        string s = sbSig.ToString();
        var apiSig = Sobees.Tools.Crypto.MD5Helper.CalculateMD5(s).ToLower();

        sbPath.Append("&api_sig=" + apiSig.ToLower());

        string path = sbPath.ToString();

        //string path = Resources.searchFlickr +
        //              "&api_key=" + Resources.flickrApiKey +
        //              "&user_id=" + userId +
        //              "&sort=" + FlickrEnum.GetStateDescription(sort) +
        //              "&per_page=" + rpp +
        //              "&page=" + startPage +
        //              "&safe_search=1";

        XDocument feedXML = XDocument.Load(@path);
        return BuildFlickrEntries(feedXML);
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("GenericLib::GetUserFlickrPhotos:", ex);
        return new List<Entry>();
      }
    }

    internal static List<Entry> BuildFlickrEntries(XDocument feedXML)
    {
      List<Entry> entries =
        (from entry in feedXML.Descendants("photo")
         let id = entry.Attribute("id").Value
         let title = entry.Attribute("title").Value
         let link = string.Format("http://flickr.com/photos/{0}/{1}/",
                                entry.Attribute("owner").Value,
                                id)
         let imgUrl = string.Format("http://farm{0}.static.flickr.com/{1}/{2}_{3}.jpg",
                                          entry.Attribute("farm").Value,
                                          entry.Attribute("server").Value,
                                          id,
                                          entry.Attribute("secret").Value)
         select new Entry
         {
           Id = id,
           Title = title,
           Link = link,
           Medias = new List<Media>(){new Media
            				{
            					Title = title,
											Link = link,
											Contents = new List<Content>(){new Content
                   					{
                   						Url = imgUrl,
                   					}},
											Thumbnails = new List<Thumbnail>(){new Thumbnail
                     				{
															Url = imgUrl.Replace(".jpg", "_m.jpg"),
														}},
            				}},
           Type = EnumType.Flickr,
         }).ToList();

      return entries;
    }

    internal static string UrlEncode(string oldString)
    {
      if (oldString == null) return String.Empty;
      StringBuilder sb = new StringBuilder(oldString.Length * 2);
      Regex reg = new Regex("[a-zA-Z0-9$-_.+!*'(),]");

      foreach (char c in oldString)
      {
        if (reg.IsMatch(c.ToString()))
        {
          sb.Append(c);
        }
        else
        {
          sb.Append(ToHex(c));
        }
      }
      return sb.ToString();
    }

    private static string ToHex(char c)
    {
      return ((int)c).ToString("X");
    }

    public static List<Entry> SearchFlickr(string tags,
                                           string query,
                                           EnumFlickrSort sort,
                                           int rpp,
                                           int startPage)
    {
      return SearchFlickr(string.Empty,
                          tags,
                          string.Empty,
                          query,
                          new DateTime(),
                          new DateTime(),
                          new DateTime(),
                          new DateTime(),
                          sort,
                          rpp,
                          startPage,
                          16,
                          7,
                          string.Empty);
    }

    public static List<Entry> SearchFlickr(string userId,
                                           string tags,
                                           string tagMode,
                                           string query,
                                           DateTime minUploadDate,
                                           DateTime maxUploadDate,
                                           DateTime minTakenDate,
                                           DateTime maxTakenDate,
                                           EnumFlickrSort sort,
                                           int rpp,
                                           int startPage,
                                           int accuracy,
                                           int contentType,
                                           string media)
    {
      try
      {
        string minUploadDateS = string.Empty;
        if (!new DateTime().Equals(minUploadDate))
          minUploadDateS = DateTimeHelper.ConvertToUnixTimestamp(minUploadDate.ToUniversalTime()).ToString();

        string maxUploadDateS = string.Empty;
        if (!new DateTime().Equals(maxUploadDate))
          maxUploadDateS = DateTimeHelper.ConvertToUnixTimestamp(maxUploadDate.ToUniversalTime()).ToString();

        string minTakenDateS = string.Empty;
        if (!new DateTime().Equals(minTakenDate))
          minTakenDateS = DateTimeHelper.ConvertToUnixTimestamp(minTakenDate.ToUniversalTime()).ToString();

        string maxTakenDateS = string.Empty;
        if (!new DateTime().Equals(maxTakenDate))
          maxTakenDateS = DateTimeHelper.ConvertToUnixTimestamp(maxTakenDate.ToUniversalTime()).ToString();

        string path = Resources.searchFlickr +
                      "&api_key=" + Resources.flickrApiKey +
                      "&user_id=" + userId +
          //"&tags=" + tags + 
          //"&tagMode=" + tagMode + 
                      "&text=" + query +
          //"&min_upload_date=" + minUploadDateS +
          //"&max_upload_date=" + maxUploadDateS +
          //"&min_taken_date=" + minTakenDateS +
          //"&max_taken_date=" + maxTakenDateS + 
                      "&sort=" + FlickrEnum.GetStateDescription(sort) +
                      "&per_page=" + rpp +
                      "&page=" + startPage +
          //"&accuracy=" + accuracy +
          //"&content_type=" + contentType + 
          //"&media=" + media +
                      "&safe_search=1"
          ;

        //var reader = new XmlTextReader(path);
        XDocument feedXML = XDocument.Load(path);
        List<Entry> entries;
        entries = (from entry in feedXML.Descendants("photo")
                   select new Entry
                   {
                     Id = entry.Attribute("id").Value,
                     Title = entry.Attribute("title").Value,
                     Link = string.Format("http://flickr.com/photos/{0}/{1}/",
                                          entry.Attribute("owner").Value,
                                          entry.Attribute("id").Value),
                     //Link = string.Format("http://farm{0}.static.flickr.com/{1}/{2}_{3}.jpg",
                     //                     entry.Attribute("farm").Value,
                     //                     entry.Attribute("server").Value,
                     //                     entry.Attribute("id").Value,
                     //                     entry.Attribute("secret").Value),
                     Type = EnumType.Flickr,

                   }).ToList();

        XDocument commentsXML;
        foreach (Entry entry in entries)
        {
          path = Resources.flickrPhotosGetInfo +
                 "&api_key=" + Resources.flickrApiKey +
                 "&photo_id=" + entry.Id;

          feedXML = XDocument.Load(path);
          XElement xe = feedXML.Element("rsp").Element("photo");
          if (xe != null)
          {
            var imgUrl = string.Format("http://farm{0}.static.flickr.com/{1}/{2}_{3}.jpg",
                                       xe.Attribute("farm").Value,
                                       xe.Attribute("server").Value,
                                       xe.Attribute("id").Value,
                                       xe.Attribute("secret").Value);

            entry.User = new User(xe.Element("owner").Attribute("nsid").Value,
                                  xe.Element("owner").Attribute("realname").Value,
                                  xe.Element("owner").Attribute("username").Value,
                                  Resources.flickrGalleryPath + xe.Element("owner").Attribute("nsid").Value);
            entry.PubDate =
              DateTimeHelper.ConvertFromUnixTimestamp(double.Parse(xe.Element("dates").Attribute("posted").Value));

            entry.UpdateDate =
              DateTimeHelper.ConvertFromUnixTimestamp(double.Parse(xe.Element("dates").Attribute("lastupdate").Value));
            entry.Content = xe.Element("description").Value;
            
            entry.Medias = new List<Media>
                             {
                               new Media
                                 {
                                   Title = entry.Title,
                                   Description = entry.Content,
                                   Contents = new List<Content>
                                                {
                                                  new Content
                                                    {
                                                      Url = imgUrl,
                                                    }
                                                },
                                   Thumbnails = new List<Thumbnail>
                                                  {
                                                    new Thumbnail
                                                      {
                                                        Url = imgUrl.Replace(".jpg",
                                                                                 "_m.jpg"),
                                                      }
                                                  }
                                 }
                             };
            //string commentsPath = Properties.Resources.flickrPhotosCommentsGetList +
            //                      "&api_key=" + Properties.Resources.flickrApiKey +
            //                      "&photo_id=" + entry.Id;
            //commentsXML = XDocument.Load(commentsPath);
            //entry.Comments = (from comment in commentsXML.Descendants("comment")
            //                  select new Comment
            //                          {
            //                            Id = comment.Attribute("id").Value,
            //                            Date = Helpers.Date.TimeHelper.ConvertFromUnixTimestamp(double.Parse(comment.Attribute("datecreate").Value)),
            //                            Body = comment.Value,
            //                            User = new User(comment.Attribute("author").Value, comment.Attribute("authorname").Value, comment.Attribute("authorname").Value, Properties.Resources.flickrGalleryPath + comment.Attribute("authorname").Value),
            //                          }).ToList();
          }
        }
        commentsXML = null;
        feedXML = null;
        return entries;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("GenericLib -> SearchFlickr() -> ", ex);
        return new List<Entry>();
      }
    }
    #endregion
  }
  public enum EnumFlickrTagMode
  {
    OR,
    AND,
  }

  public enum EnumFlickrSort
  {
    [Description("date-posted-desc")]
    datePostedDesc,
    [Description("date-posted-asc")]
    datePostedAsc,
    [Description("date-taken-asc")]
    dateTakenAsc,
    [Description("date-taken-desc")]
    dateTakenDesc,
    [Description("interestingness-desc")]
    interestingnessDesc,
    [Description("interestingness-asc")]
    interestingnessAsc,
    [Description("relevance")]
    relevance,
  }
  public class FlickrEnum
  {
    public static string GetStateDescription(EnumFlickrSort state)
    {
      Type type = typeof(EnumFlickrSort);
      FieldInfo fieldInfo = type.GetField(state.ToString());
      var descAttributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute),
                                                         false) as DescriptionAttribute[];
      return (descAttributes.Length > 0) ? descAttributes[0].Description : null;
    }

    public static EnumFlickrSort GetStateFromDescription(string state)
    {
      Type type = typeof(EnumFlickrSort);
      var fields = new List<FieldInfo>(type.GetFields());
      var finder = new DescAttrFinder(state);
      FieldInfo fi = fields.Find(finder.FindPredicate);
      return (EnumFlickrSort)fi.GetRawConstantValue();
    }

    #region Nested type: DescAttrFinder

    private class DescAttrFinder
    {
      private readonly string descAttributeValue;

      public DescAttrFinder(string descAttributeValue)
      {
        this.descAttributeValue = descAttributeValue;
      }

      public bool FindPredicate(FieldInfo fi)
      {
        var descAttributes = fi.GetCustomAttributes(typeof(DescriptionAttribute),
                                                    false) as DescriptionAttribute[];
        string desc = (descAttributes.Length > 0) ? descAttributes[0].Description : null;
        return descAttributeValue.CompareTo(desc) == 0;
      }
    }

    #endregion
  }
}
