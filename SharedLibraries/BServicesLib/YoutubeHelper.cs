using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Sobees.Library.BGenericLib;
using Sobees.Library.BServicesLib.Properties;
using Sobees.Tools.Logging;

namespace Sobees.Library.BServicesLib
{
  public enum EnumYoutubeTime
  {
    today,
    this_week,
    this_month,
    all_time,
  }

  public enum EnumYoutubeRacy
  {
    include,
    exclude,
  }

  public enum EnumYoutubeOrderBy
  {
    relevance,
    published,
    viewCount,
    rating
  }

  public class YoutubeHelper
  {
    public static List<Entry> SearchYoutube(string author)
    {
      return SearchYoutube(string.Empty, author, EnumLanguages.all, EnumYoutubeOrderBy.relevance, 30, 1);
    }

    public static List<Entry> SearchYoutube(string query,
                                            EnumLanguages lang)
    {
      return SearchYoutube(query,
                           lang,
                           EnumYoutubeOrderBy.relevance,
                           30,
                           1);
    }

    public static List<Entry> SearchYoutube(string query,
                                            EnumLanguages lang,
                                            EnumYoutubeOrderBy orderBy,
                                            int rpp,
                                            int startIndex)
    {
      return SearchYoutube(query, string.Empty, lang, orderBy, rpp, startIndex);
    }

    public enum EnumYoutubeOrderBy
    {
      relevance,
      published,
      viewCount,
      rating
    }

    public static List<Entry> SearchYoutube(string query,
                                            string author,
                                            EnumLanguages lang,
                                            EnumYoutubeOrderBy orderBy,
                                            int rpp,
                                            int startIndex)
    {
      try
      {
        query = "%22" + query.Replace(' ',
                                      '+') + "%22";

        string path = Resources.searchYoutube + "orderby=" + orderBy + "&max-results=" + rpp +
                      "&start-index=" + startIndex + "&racy=exclude";

        if (string.IsNullOrEmpty(author))
        {
          path += "&vq=" + query;
          //path += "&lr=" + lang;
        }
        else
        {
          path += "&author=" + author;
        }

        if (!string.IsNullOrEmpty(lang.ToString()) && !lang.ToString().ToLower().Equals("all"))
        {
          path += "&lr=" + lang;
        }

        //var reader = new XmlTextReader(path);
        XDocument feedXML = XDocument.Load(path);
        List<Entry> entries;
        XNamespace atom = "http://www.w3.org/2005/Atom";
        XNamespace media = "http://search.yahoo.com/mrss/";
        XNamespace yt = "http://gdata.youtube.com/schemas/2007";
        XNamespace gd = "http://schemas.google.com/g/2005";
        entries = (from entry in feedXML.Descendants(atom + "entry")
                   let rating = entry.Element(gd + "rating")
                   select new Entry
                            {
                              Id = entry.Element(atom + "id").Value,
                              Title = entry.Element(atom + "title").Value,
                              Link = entry.Element(atom + "link").Attribute("href").Value,
                              UpdateDate = DateTime.Parse(entry.Element(atom + "updated").Value),
                              PubDate = DateTime.Parse(entry.Element(atom + "published").Value),
                              User = new User
                                       {
                                         Id = Guid.NewGuid().ToString(),
                                         Name = entry.Element(atom + "author").Element(atom + "name").Value,
                                         ProfileUrl =
                                           Resources.youtubeUserUrl +
                                           entry.Element(atom + "author").Element(atom + "name").Value,
                                         NickName =
                                           entry.Element(atom + "author").Element(atom + "uri").Value.Substring(
                                           entry.Element(atom + "author").Element(atom + "uri").Value.LastIndexOf("/"))
                                       },
                              Medias = (from m in entry.Descendants(media + "group") ?? null
                                        select new Media
                                                 {
                                                   Title = m.Element(media + "title").Value,
                                                   Player = m.Element(media + "player").Attribute("url").Value,
                                                   Description = m.Element(media + "description").Value,
                                                   Keywords =
                                                     m.Element(media + "keywords").Value.Trim().Split(',').ToList(),
                                                   DurationInSeconds =
                                                     int.Parse(m.Element(yt + "duration").Attribute("seconds").Value),
                                                   Contents = (from content in m.Descendants(media + "content") ?? null
                                                               where content != null && content.Attribute("url") != null
                                                               select new Content
                                                                        {
                                                                          Url = content.Attribute("url").Value,
                                                                          Type =
                                                                            content.Attribute("type") != null
                                                                              ? content.Attribute("type").Value
                                                                              : string.Empty,
                                                                        }).ToList(),
                                                   Thumbnails =
                                                     (from thumbnail in m.Descendants(media + "thumbnail") ?? null
                                                      where thumbnail != null && thumbnail.Attribute("url") != null
                                                      select new Thumbnail
                                                               {
                                                                 Url = thumbnail.Attribute("url").Value,
                                                                 Width =
                                                                   double.Parse(thumbnail.Attribute("width").Value),
                                                                 Height =
                                                                   double.Parse(thumbnail.Attribute("height").Value),
                                                               }).ToList(),
                                                 }).ToList(),
                              Rating = rating != null
                                         ? new Rating
                                             {
                                               Min = int.Parse(entry.Element(gd + "rating").Attribute("min").Value),
                                               Max = int.Parse(entry.Element(gd + "rating").Attribute("max").Value),
                                               NbRaters =
                                                 int.Parse(entry.Element(gd + "rating").Attribute("numRaters").Value),
                                               Average =
                                                 double.Parse(entry.Element(gd + "rating").Attribute("average").Value),
                                             }
                                         : null,
                              Type = EnumType.Youtube,
                            }).ToList();
        return entries;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("GenericLib -> SearchYoutube() -> ", ex);
        return new List<Entry>();
      }
    }
  }
}