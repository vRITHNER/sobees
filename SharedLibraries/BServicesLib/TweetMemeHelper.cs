using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Linq;
using Sobees.Library.BGenericLib;
using Sobees.Tools.Logging;

namespace Sobees.Library.BServicesLib
{
  internal class TweetMemeHelper
  {
    public static List<Entry> GetTweetmemeRecent(out string errorMsg)
    {
      errorMsg = string.Empty;

      try
      {
        var entries = new List<Entry>();

#if !SILVERLIGHT
        XDocument doc = XDocument.Load("http://api.tweetmeme.com/recent.xml");

        var document = new XmlDocument();
        document.LoadXml(doc.Root.ToString());

        //list node news_item
        XmlNodeList listeNodesNews = document.SelectNodes("/result/stories/story");
        XmlNodeList listeCurrentNews;

        XmlNodeList listeNodeMedia;
        XmlNodeList listeNodeMediaItem;

        string caseSwitch;
        string valueCurrentItem;

        string caseSwitchMedia;
        string valueCurrentItemMedia;

        Console.WriteLine(listeNodesNews.Item(0).OuterXml);
        Console.WriteLine(listeNodesNews.Count);

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
          //currentNews.OrigLink = listeCurrentNews.Item(j).ParentNode.Attributes["url"].Value;

          //course of all attributes of a node news
          for (int i = 0; i < listeCurrentNews.Count; i++)
          {
            caseSwitch = listeCurrentNews.Item(i).Name;
            valueCurrentItem = listeCurrentNews.Item(i).InnerText;
            switch (caseSwitch)
            {
              case "url":
                currentNews.OrigLink = valueCurrentItem;
                break;
              case "media":
                currentNews.SourceName = valueCurrentItem;
                break;
              case "title":
                currentNews.Title = valueCurrentItem;
                break;
              case "created_at":
                currentNews.PubDate = DateTime.Parse(valueCurrentItem);
                break;
              case "tweetcount":
                currentNews.Content = valueCurrentItem;
                break;
              case "thumbnail":
                var currentPicture = new Media();
                currentPicture.Link = valueCurrentItem;
                currentNews.Medias.Add(currentPicture);
                break;
              case "alias":
                currentNews.DisplayLink = valueCurrentItem;
                break;

              case "retweet":
                //currentNews.Id = valueCurrentItem;
                //currentNews.id = valueCurrentItem;
                //Console.WriteLine("id " + currentNews.id);
                break;

              default:
                break;
            }
          }

          //Adding news to the list    
          currentNews.Type = EnumType.TweetMeme;
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
  }
}