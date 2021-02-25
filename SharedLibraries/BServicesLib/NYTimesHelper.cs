using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Sobees.Library.BGenericLib;
using Sobees.Tools.Logging;

namespace Sobees.Library.BServicesLib
{
  public class NYTimesHelper
  {
    public static List<Entry> GetNewsNYold(out string errorMsg)
    {
      errorMsg = string.Empty;

      try
      {
        var entries = new List<Entry>();

#if !SILVERLIGHT
        XDocument doc =
          XDocument.Load(
            "http://api.nytimes.com/svc/news/v2/all/recent.xml?api-key=9b45ea28eef0025e1024c19e0ba4dff7:15:57491279");

        var document = new XmlDocument();
        document.LoadXml(doc.Root.ToString());

        //list node news_item
        XmlNodeList listeNodesNews = document.SelectNodes("/result_set/results/news_item");
        XmlNodeList listeCurrentNews;

        XmlNodeList listeNodeMedia;
        XmlNodeList listeNodeMediaItem;

        string caseSwitch;
        string valueCurrentItem;

        string caseSwitchMedia;
        string valueCurrentItemMedia;

        //Console.WriteLine(listeNodesNews.Item(0).OuterXml);
        //Console.WriteLine(listeNodesNews.Count);

        //course of all news
        for (int j = 0; j < listeNodesNews.Count; j++)
        {
          //child node list of a news
          listeCurrentNews = listeNodesNews.Item(j).ChildNodes;

          //NewsNY object to be added to the list
          var currentNews = new Entry();
          currentNews.Medias = new List<Media>();
          currentNews.Comments = new ObservableCollection<Comment>();
          //assigns value to the url of article
          currentNews.OrigLink = listeCurrentNews.Item(j).ParentNode.Attributes["url"].Value;

          //course of all attributes of a node news
          for (int i = 0; i < listeCurrentNews.Count; i++)
          {
            caseSwitch = listeCurrentNews.Item(i).Name;
            valueCurrentItem = listeCurrentNews.Item(i).InnerText;
            switch (caseSwitch)
            {
              case "section":
                currentNews.SourceName = valueCurrentItem;
                //currentNews.section = valueCurrentItem;
                //Console.WriteLine("Section " + currentNews.section);
                break;
              case "subsection":
                //currentNews.subsection = valueCurrentItem;
                //Console.WriteLine("subsection " + currentNews.subsection);
                break;
              case "headline":
                currentNews.Title = valueCurrentItem;
                //currentNews.headline = valueCurrentItem;
                //Console.WriteLine("headline " + currentNews.headline);
                break;
              case "summary":

                currentNews.Content = valueCurrentItem;


                //byte[] utf8Bytes = Encoding.UTF8.GetBytes(valueCurrentItem);
                //currentNews.Content = Encoding.UTF8.GetString(valueCurrentItem);

                //currentNews.summary = valueCurrentItem;
                //Console.WriteLine("summary " + currentNews.summary);
                break;
              case "byline":
                currentNews.DisplayLink = valueCurrentItem;
                //Console.WriteLine("byline " + currentNews.byline);
                break;
              case "platform":
                //currentNews.platform = valueCurrentItem;
                //Console.WriteLine("platform " + currentNews.platform);
                break;
              case "id":
                currentNews.Id = valueCurrentItem;
                //currentNews.id = valueCurrentItem;
                //Console.WriteLine("id " + currentNews.id);
                break;
              case "type":
                //currentNews.type = valueCurrentItem;
                //Console.WriteLine("type " + currentNews.type);
                break;
              case "source":
                //currentNews.source = valueCurrentItem;
                //Console.WriteLine("source " + currentNews.source);
                break;
              case "updated":
                //currentNews.updated = DateTime.Parse(valueCurrentItem);
                //Console.WriteLine("updated " + currentNews.updated);
                break;
              case "created":
                currentNews.PubDate = DateTime.Parse(valueCurrentItem);
                //currentNews.created = DateTime.Parse(valueCurrentItem);
                //Console.WriteLine("created " + currentNews.created);
                break;
              case "pubdate":
                //currentNews.pubdate = DateTime.Parse(valueCurrentItem);
                //Console.WriteLine("pubdate " + currentNews.pubdate);
                break;
              case "subtype":
                //currentNews.subtype = valueCurrentItem;
                //Console.WriteLine("subtype " + currentNews.subtype);
                break;
              case "kicker":
                //currentNews.kicker = valueCurrentItem;
                //Console.WriteLine("kicker " + currentNews.kicker);
                break;
              case "subheadline":
                //currentNews.subheadline = valueCurrentItem;
                //Console.WriteLine("subheadline " + currentNews.subheadline);
                break;
              case "terms":
                //currentNews.terms = valueCurrentItem;
                //Console.WriteLine("terms " + currentNews.terms);
                break;
              case "organizations":
                //currentNews.subheadline = valueCurrentItem;
                //Console.WriteLine("subheadline " + currentNews.subheadline);
                break;
              case "people":
                //currentNews.people = valueCurrentItem;
                //Console.WriteLine("people " + currentNews.people);
                break;
              case "locations":
                //currentNews.locations = valueCurrentItem;
                //Console.WriteLine("locations " + currentNews.locations);
                break;
              case "indexing_terms":
                //currentNews.indexing_terms = valueCurrentItem;
                //Console.WriteLine("indexing_terms " + currentNews.indexing_terms);
                break;
              case "related_urls":
                //currentNews.related_urls = valueCurrentItem;
                //Console.WriteLine("related_urls " + currentNews.related_urls);
                break;
              case "categories_tags":
                //currentNews.categories_tags = valueCurrentItem;
                //Console.WriteLine("categories_tags " + currentNews.categories_tags);
                break;
              case "blog_name":
                //currentNews.blog_name = valueCurrentItem;
                //Console.WriteLine("blog_name " + currentNews.blog_name);
                break;

              case "media":

                //child node list of a media
                listeNodeMedia = listeCurrentNews.Item(i).ChildNodes;

                //course of all attributes of a node media
                if (listeNodeMedia.Count > 0)
                {
                  //NewsPicture object to be added to attribute media

                  //NewsPicture currentPicture = new NewsPicture();
                  var currentPicture = new Media();

                  //I takes only the first media
                  listeNodeMediaItem = listeNodeMedia.Item(0).ChildNodes;

                  //course of all attributes of a node media
                  for (int k = 0; k < listeNodeMediaItem.Count; k++)
                  {
                    caseSwitchMedia = listeNodeMediaItem.Item(k).Name;
                    valueCurrentItemMedia = listeNodeMediaItem.Item(k).InnerText;
                    //Console.WriteLine("caseSwitchMedia -----> " + caseSwitchMedia);
                    //Console.WriteLine("valueCurrentItemMedia *******> " + valueCurrentItemMedia);

                    switch (caseSwitchMedia)
                    {
                      case "subtype":
                        currentPicture.Description = valueCurrentItemMedia;
                        //currentPicture.subtype = valueCurrentItemMedia;
                        break;
                      case "caption":
                        currentPicture.Title = valueCurrentItemMedia;
                        //currentPicture.caption = valueCurrentItemMedia;
                        break;
                      case "copyright":
                        //currentPicture.copyright = valueCurrentItemMedia;
                        break;
                      case "media-metadata":
                        currentPicture.Link = valueCurrentItemMedia;
                        //currentPicture.url = valueCurrentItemMedia;
                        break;

                      default:
                        break;
                    }
                  }

                  //adding the picture to the media attribute
                  currentNews.Medias.Add(currentPicture);
                  //Console.WriteLine("URLE url " + currentNews.Medias[0].Link);
                }

                break;

              default:
                break;
            }
          }

          //Adding news to the list    
          currentNews.Type = EnumType.NYtimes;
          entries.Add(currentNews);
        }

        return entries;
#else
        return null;

#endif
      }
      catch (Exception ex)
      {
        errorMsg = ex.Message;
        TraceHelper.Trace("TwitterLib -> SearchSummize() -> ",
                          ex);
        return new List<Entry>();
      }
    }


    public static List<Entry> GetNewsNY(out string errorMsg)
    {
      errorMsg = string.Empty;
      const string newsApiKey = "9b45ea28eef0025e1024c19e0ba4dff7:15:57491279";
      //List<Entry> entries = null;

      string url = string.Format("http://api.nytimes.com/svc/news/v2/all/recent.xml?api-key={0}", newsApiKey);
      var request = (HttpWebRequest)WebRequest.Create(url);

      try
      {
        using (var reader = new StreamReader((request.GetResponse()).GetResponseStream()))
        {
          string responseData = reader.ReadToEnd();
          Entry i = new Entry();
          
          XDocument xdoc = XDocument.Parse(responseData);
          List<Entry> entries = (from results in xdoc.Descendants("news_item")
                                 select new Entry
                                 {
                                   Type = EnumType.NYtimes,
                                   Id =
                                     results.Element("id") != null ? results.Element("id").Value : "",
                                   Title =
                                     results.Element("headline") != null ? results.Element("headline").Value : "",
                                   Content =
                                     results.Element("summary") != null ? results.Element("summary").Value : "",
                                   SourceName =
                                     results.Element("byline") != null ? results.Element("byline").Value : "",
                                   OrigLink =
                                     results.Attribute("url") != null ? results.Attribute("url").Value : "",
                                   Section =
                                     results.Element("section") != null ? results.Element("section").Value : "",
                                   UpdateDate =
                                     results.Element("updated") != null ? parseDate(results.Element("updated").Value) : new DateTime(),

                                   Medias = (from m in results.Descendants("media_item") ?? null
                                             select new Media
                                             {
                                               Title = results.Element("caption") != null ? results.Element("caption").Value : "",
                                               Description = results.Element("copyright") != null ? results.Element("copyright").Value : "",
                                               Thumbnails =
                                                 (from thumbnail in m.Descendants("media-metadata_item") ?? null
                                                  where thumbnail != null
                                                  select new Thumbnail
                                                  {
                                                    Url = thumbnail.Element("url") != null ? thumbnail.Element("url").Value : "",
                                                  }).ToList(),
                                             }).ToList(),

                                 }).ToList();

          return entries;
        }

      }
      catch (Exception ex)
      {

        TraceHelper.Trace("NYTimesHelper -> SearchArticleAsync() -> ",
                          ex);
      }
      return null;
    }

    public static List<Entry> SearchArticleAsync(string query, string offset)
    {
      string articleSearchKey = "992a2206354d07f79f079a6ff128c31d:17:57491279";
      List<Entry> entries = null;
      if (query == null)
        throw new ArgumentNullException("query");
      string url = string.Format("http://api.nytimes.com/svc/search/v1/article?query={0}&api-key={1}&offset={2}", query, articleSearchKey, offset);
      var request = (HttpWebRequest)WebRequest.Create(url);

      try
      {
        using (var reader = new StreamReader((request.GetResponse()).GetResponseStream()))
        {
          string responseData = reader.ReadToEnd();

          var json = JObject.Parse(responseData)["results"];


          //SourceName = result["byline"].ToString(),
          //PubDate = DateTime.ParseExact(result["date"].ToString(), "yyyymmdd", null)

          //Parse responseData (containing JSON or xml) to set entries with a List<Entry>
          entries = (from result in json
                     select new Entry()
                     {
                       Type = EnumType.NYtimes,
                       Title = result["title"] != null ? result["title"].ToString() : "",
                       Content = result["body"] != null ? result["body"].ToString() : "",
                       DisplayLink = result["url"] != null ? result["url"].ToString() : "http://www.nytimes.com",
                       Link = result["url"] != null ? result["url"].ToString() : "http://www.nytimes.com",
                       SourceName = result["byline"] != null ? result["byline"].ToString() : "",
                       PubDate = result["date"] != null ? convertDate(result["date"].ToString()) : new DateTime()
                     }).ToList();
        }

      }
      catch (Exception ex)
      {

        TraceHelper.Trace("NYTimesHelper -> SearchArticleAsync() -> ",
                          ex);
      }
      return entries;
    }

    private static DateTime convertDate(string d)
    {
      var dateReturn = new DateTime();
      try
      {
        var ex = new Exception();

        if (!string.IsNullOrEmpty(d))
        {
          if (d.Length > 8)
          {
            d = d.Substring(1, d.Length - 2);
          }
          var years = d.Substring(0, 4);
          var month = d.Substring(4, 2);
          var days = d.Substring(6, 2);

          var dateTemp = new DateTime(int.Parse(years), int.Parse(month), int.Parse(days));

          dateReturn = dateTemp;
        }
      }
      catch (Exception ex)
      {
        var exception = ex;
        TraceHelper.Trace("NYTimesHelper -> parseDate() -> ",
                          ex);
      }
      return dateReturn;
    }

    private static DateTime parseDate(string d)
    {
      var dateReturn = new DateTime();
      try
      {
        var ex = new Exception();

        if (!string.IsNullOrEmpty(d))
        {
          var dateTemp = DateTime.Parse(d);

          dateReturn = dateTemp;
        }
      }
      catch (Exception ex)
      {
        var exception = ex;
        TraceHelper.Trace("NYTimesHelper -> ConvertDate() -> ",
                          ex);
      }
      return dateReturn;
    }

    private static string parseUrl(string url)
    {
      var urlTemp = "";
      try
      {
        var ex = new Exception();

        if (!string.IsNullOrEmpty(url))
        {
          urlTemp = url.Replace("\"", "");
          urlTemp = url.Replace("\'", "");

        }
      }
      catch (Exception ex)
      {
        var exception = ex;
        TraceHelper.Trace("NYTimesHelper -> parseUrl() -> ",
                          ex);
      }
      return urlTemp;
    }
  }

    public class NyTimesFilter
    {
      public string Id { get; set; }
      public string Title { get; set; }

      public NyTimesFilter(string id, String title)
      {
        Id = id;
        Title = title;
      }
    }
  
}