#region

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using LinqToTwitter;
using Sobees.Library.BGenericLib;
using Sobees.Library.BTwitterLib.Helpers;
using Sobees.Tools.Crypto;
using Sobees.Tools.Logging;

#endregion

namespace Sobees.Library.BTwitterLib
{
  public class TwitterLib
  {
    /// <summary>
    ///   SearchSummize2
    /// </summary>
    /// <param name="query"></param>
    /// <param name="lang"></param>
    /// <param name="rpp"></param>
    /// <param name="geoCode"></param>
    /// <param name="token"></param>
    /// <param name="secret"></param>
    /// <param name="errorMsg"></param>   
    /// <returns></returns>
    public static List<TwitterEntry> SearchSummize2(string token, string secret,
                                                    string query,
                                                    EnumLanguages lang,
                                                    int rpp,
                                                    string geoCode,
                                                    out string errorMsg)
    {
      errorMsg = string.Empty;
      var entries = new List<TwitterEntry>();

      string CONSUMER_KEY = "TFuWcVHyUGtAFUV2oTVQdQ";
      string CONSUMER_SECRET = "gLa1LNpbfiyg1z0eRrWzzoFyX042kwTxfhYwA9JPJ0";
      string MY_ACCESS_TOKEN = "6995022-w7mmi15YI4jXpJAKZ6DXAmwPFqO74Fg7qUbgttBJRd";
      string MY_ACCESS_TOKEN_SECRET = "Ra1R6P5JdHoKNVPgwNt1Z75XhxGg3kR7by7MuCDWZk";

      var tentative = 0;
    StartSearch:
      try
      {
        using (var twitterCtx = Context.CreateTwitterContext(token, secret))
        {
          var res = (from search in twitterCtx.Search
                     where search.Type == SearchType.Search &&
                           search.ResultType == ResultType.Mixed &&
                       //search.PageSize == rpp && 
                           search.SearchLanguage == lang.ToString() &&
                           search.Query == query
                     //&& search.GeoCode == geoCode
                     select search).SingleOrDefault();

          if (res != null && res.Statuses != null && res.Statuses.Any())
            entries.AddRange(
              res.Statuses.Select(LinqToTwitterHelper.ConvertStatusToTwitterEntry).OrderByDescending(t => t.PubDate));
        }
        return entries;
      }
      catch (Exception ex)
      {
        if (ex.Message.Contains("JsonException Invalid token '34'") || ex.Message.Contains("evaluate"))
        {
          if (tentative < 3)
          {
            tentative++;
            Thread.Sleep(TimeSpan.FromSeconds(1));
            goto StartSearch;
          }
        }

        errorMsg = ex.Message;
        TraceHelper.Trace("TwitterLib -> SearchSummize() -> ",
                          ex);
        return entries;
      }
    }

    public static List<TwitterEntry> SearchSummize(string query,
                                                   EnumLanguages lang,
                                                   int rpp,
                                                   string geoCode,
                                                   out string errorMsg)
    {
      errorMsg = string.Empty;

      try
      {
        var path = string.Format("{0}q={1}&rpp={2}&lang={3}", TwitterResources.searchSummize,
                                 HttpUtility.UrlEncode(query),
                                 rpp, lang); // + "&geocode=" + geoCode
        var doc = new XmlDocument();
        doc.LoadXml(XDocument.Load(path).ToString().Replace(@"twitter:",
                                                            @"twitter_").Replace("georss:point", "georss_point"));
        var xnr = new XmlNodeReader(doc);
        var feedXML = XDocument.Load(xnr);
        List<TwitterEntry> entries;
        XNamespace atom = "http://www.w3.org/2005/Atom";
        entries = (from entry in feedXML.Descendants(atom + "entry")
                   let xElement = entry.Element(atom + "title")
                   where xElement != null
                   select new TwitterEntry
                            {
                              Id = entry.Element(atom + "id").Value,
                              Title = HttpUtility.HtmlDecode(xElement.Value),
                              Link = entry.Element(atom + "link").Attribute("href").Value,
                              UpdateDate = DateTime.Parse(entry.Element(atom + "updated").Value),
                              PubDate = DateTime.Parse(entry.Element(atom + "published").Value),
                              SourceName = entry.Element(atom + "twitter_source").Value,
                              Type = EnumType.TwitterSearch,
                              User = new TwitterUser
                                       {
                                         Id = FindCorrectId(entry, atom),
                                         Name = entry.Element(atom + "author").Element(atom + "name").Value,
                                         ProfileUrl = entry.Element(atom + "author").Element(atom + "uri").Value,
                                         NickName =
                                           entry.Element(atom + "author").Element(atom + "uri").Value.Substring(
                                             entry.Element(atom + "author").Element(atom + "uri").Value.LastIndexOf("/") +
                                             1),
                                         ProfileImgUrl = (from e in entry.Descendants(atom + "link")
                                                          where
                                                            e.Attribute("rel") != null &&
                                                            e.Attribute("rel").Value.Equals("image")
                                                          select e).First().Attribute("href").Value,
                                         Geolocation = GetLocation(entry),
                                       },
                              InReplyToUserName = GetInReplyTo(HttpUtility.HtmlDecode(xElement.Value))
                            }).ToList();
        feedXML = null;
        return entries;
      }
      catch (Exception ex)
      {
        errorMsg = ex.Message;
        TraceHelper.Trace("TwitterLib -> SearchSummize() -> ",
                          ex);
        return new List<TwitterEntry>();
      }
    }


    public static string GetInReplyTo(string title)
    {
      try
      {
        if (title[0] == '@')
        {
          var splited = title.Split(' ');
          if (splited.Any())
          {
            return splited[0].Replace("@", "");
          }
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }
      return string.Empty;
    }

    public static string FindCorrectId(XElement entry, XNamespace atom)
    {
      try
      {
        var url = (from e in entry.Descendants(atom + "link")
                   where
                     e.Attribute("rel") != null &&
                     e.Attribute("rel").Value.Equals("image")
                   select e).First().Attribute("href").Value;
        var splited = url.Split('/');

        return splited[splited.Count() - 2];
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }
      return Guid.NewGuid().ToString();
    }

    public static List<Entry> SearchTrends(string token, string secret, out string errorMsg, int number = 10)
    {
      return SearchTrends(token, secret, out errorMsg, number, null);
    }

    /// <summary>
    /// SearchTrends
    /// </summary>
    /// <param name="token"></param>
    /// <param name="secret"></param>
    /// <param name="errorMsg"></param>
    /// <param name="number"></param>
    /// <param name="proxy"></param>
    /// <returns></returns>
    public static List<Entry> SearchTrends(string token, string secret, out string errorMsg, int number = 10,
                                           WebProxy proxy = null)
    {
      errorMsg = string.Empty;
      var listWordSearch = new List<Entry>();

      try
      {

        using (var twitterCtx = Context.CreateTwitterContext(token, secret))
        {

          var trends =
                   (from trnd in twitterCtx.Trends
                    where trnd.Type == TrendType.Place &&
                          trnd.WoeID == 1
                    select trnd)
                   .ToList();

          listWordSearch.AddRange(trends.Select(LinqToTwitterHelper.ConvertTrendsToTwitterEntry));
        }
      }
      catch (Exception ex)
      {
        errorMsg = ex.Message;
        TraceHelper.Trace("TwitterLib -> SearchSummize() -> ",
                          ex);
      }
      return listWordSearch;
    }


    public static List<TwitterEntry> GetPublicTimeline(int count,
                                                       int pageNb,
                                                       string hashKey,
                                                       out string errorMsg)
    {
      return GetFeedFromUrl("",
                            "",
                            TwitterResources.publicTimeLine,
                            count,
                            pageNb,
                            false,
                            hashKey,
                            out errorMsg);
    }


    public static List<TwitterEntry> GetHomeTimeline(string login,
                                                     string pwdHash,
                                                     int count,
                                                     int pageNb,
                                                     long maxId, long SinceId,
                                                     string hashKey,
                                                     out string errorMsg)
    {
      return GetFeedFromUrl(login,
                            pwdHash,
                            TwitterResources.home_timeline,
                            count,
                            pageNb,
                            maxId,
                            SinceId,
                            false,
                            hashKey,
                            out errorMsg);
    }

    public static List<TwitterEntry> GetFriendsTimeline(string login,
                                                        string pwdHash,
                                                        int count,
                                                        int pageNb,
                                                        string hashKey,
                                                        out string errorMsg)
    {
      return GetFeedFromUrl(login,
                            pwdHash,
                            TwitterResources.friendsTimeline,
                            count,
                            pageNb,
                            false,
                            hashKey,
                            out errorMsg);
    }


    public static List<TwitterEntry> GetReplies(string login,
                                                string pwdHash,
                                                int count,
                                                int pageNb,
                                                string hashKey,
                                                out string errorMsg)
    {
      return GetFeedFromUrl(login,
                            pwdHash,
                            TwitterResources.replies,
                            count,
                            pageNb,
                            false,
                            hashKey,
                            out errorMsg);
    }

    public static List<TwitterEntry> GetTweetInfo(string login,
                                                  string pwdHash,
                                                  string id,
                                                  string hashKey,
                                                  out string errorMsg)
    {
      errorMsg = "";
      var entries = new List<TwitterEntry>();

      try
      {
        var path = string.Format("{0}{1}.xml",
                                 TwitterResources.tweetShow,
                                 id);
        var request =
          WebRequest.Create(path) as HttpWebRequest;
        var key = EncryptionHelper.GetHashKey(hashKey);
        var pwd = EncryptionHelper.Decrypt(key,
                                           pwdHash);
        request.ContentType = "application/xml";
        request.Headers.Add("Authorization",
                            "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(login + ":" + pwd)));
        request.Method = "GET";
        using (var reader = new XmlTextReader(request.GetResponse().GetResponseStream()))
        {
          var feedXML = LoadXDocument(reader,
                                      out errorMsg);

          entries = LoadFeed(feedXML);
          return entries;
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("TwitterLib::GetTwitterFriends()",
                          ex);
      }
      try
      {
        var path = string.Format("{0}{1}.xml",
                                 TwitterResources.tweetShow,
                                 id);
        using (var reader = new XmlTextReader(@path))
        {
          if (!string.IsNullOrEmpty(login))
          {
            var resolver = new XmlUrlResolver();

            // Add authentication to request  

            var key = EncryptionHelper.GetHashKey(hashKey);
            var pwd = EncryptionHelper.Decrypt(key,
                                               pwdHash);
            resolver.Credentials = new NetworkCredential(login,
                                                         pwd);

            reader.XmlResolver = resolver;
          }

          var feedXML = LoadXDocument(reader,
                                      out errorMsg);
          entries = LoadFeed(feedXML);
        }

        return entries;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("TwitterLib -> GetFeedFromUrl() -> ",
                          ex);
      }

      return entries;
    }

    public static List<TwitterEntry> GetDirectMessages(string login,
                                                       string pwdHash,
                                                       int count,
                                                       int pageNb,
                                                       string hashKey,
                                                       out string errorMsg)
    {
      var dmsReceived = GetFeedFromUrl(login,
                                       pwdHash,
                                       TwitterResources.directMessages,
                                       count,
                                       pageNb,
                                       true,
                                       hashKey,
                                       out errorMsg);
      var dmsSent = GetFeedFromUrl(login,
                                   pwdHash,
                                   TwitterResources.directMessagesSent,
                                   count,
                                   pageNb,
                                   true,
                                   hashKey,
                                   out errorMsg);
      if (dmsSent != null) if (dmsReceived != null) dmsReceived.AddRange(dmsSent);
      return dmsReceived;
    }

    public static List<TwitterEntry> GetUser(string login,
                                             string pwdHash,
                                             string userToGet,
                                             int count,
                                             int pageNb,
                                             string hashKey,
                                             out string errorMsg)
    {
      errorMsg = null;
      if (string.IsNullOrEmpty(userToGet))
        return new List<TwitterEntry>();

      return GetFeedFromUrl(login,
                            pwdHash,
                            string.Format("{0}{1}.xml",
                                          TwitterResources.userTimeline,
                                          userToGet),
                            count,
                            pageNb,
                            false,
                            hashKey,
                            out errorMsg);
    }

    public static List<TwitterEntry> GetFavorites(string login,
                                                  string pwdHash,
                                                  string userToGet,
                                                  int count,
                                                  int pageNb,
                                                  string hashKey,
                                                  out string errorMsg)
    {
      errorMsg = null;
      if (string.IsNullOrEmpty(userToGet))
        return new List<TwitterEntry>();

      return GetFeedFromUrl(login,
                            pwdHash,
                            string.Format("{0}{1}.xml",
                                          TwitterResources.favorites,
                                          userToGet),
                            count,
                            pageNb,
                            false,
                            hashKey,
                            out errorMsg);
    }

    public static List<string> GetFriendsIds(string userToGet,
                                             out string errorMsg)
    {
      errorMsg = null;
      var friendsIds = new List<string>();

      try
      {
        using (var reader = new XmlTextReader(string.Format("{0}{1}.xml",
                                                            TwitterResources.friendsIds,
                                                            userToGet)))
        {
          var feedXML = LoadXDocument(reader,
                                      out errorMsg);
          friendsIds = (from id in feedXML.Descendants("id")
                        select id.Value).ToList();
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("TwitterLib::GetTwitterFriends()",
                          ex);
      }
      return friendsIds;
    }

    public static List<string> GetFriendsIds(string login,
                                             string pwdHash,
                                             string hashKey, string userToGet,
                                             out string errorMsg)
    {
      List<string> friendsIds = null;
      try
      {
        // Create the web request  

        var request = WebRequest.Create(string.Format("{0}{1}.xml",
                                                      TwitterResources.friendsIds,
                                                      userToGet)) as HttpWebRequest;

        // Add authentication to request  
        var key = EncryptionHelper.GetHashKey(hashKey);
        var pwd = EncryptionHelper.Decrypt(key,
                                           pwdHash);

        // Add authentication to request  
        request.Headers.Add("Authorization",
                            "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(login + ":" + pwd)));
        request.Method = "GET";

        // Do the request to get the response
        using (var response = ExecuteWebRequest(request, out errorMsg))
        {
          // Get the response stream  
          if (response != null)
          {
            var xreader = XmlReader.Create(@response.GetResponseStream());
            var feedXML = XDocument.Load(@xreader);
            friendsIds = (from id in feedXML.Descendants("id")
                          select id.Value).ToList();
          }
        }
      }
      catch (Exception ex)
      {
        errorMsg = ex.Message;
        TraceHelper.Trace("TwitterLib::GetTwitterFriends()",
                          ex);
      }
      errorMsg = "";
      return friendsIds;
    }

    public static List<TwitterUser> GetFriends(string login,
                                               string pwdHash,
                                               string hashKey,
                                               out string errorMsg)
    {
      errorMsg = null;
      var friends = new List<TwitterUser>();

      try
      {
        var page = 1;

        using (var reader = new XmlTextReader(string.Format("{0}?page={1}",
                                                            TwitterResources.friends,
                                                            page)))
        {
          var resolver = new XmlUrlResolver();

          // Add authentication to request  
          var key = EncryptionHelper.GetHashKey(hashKey);
          var pwd = EncryptionHelper.Decrypt(key,
                                             pwdHash);

          resolver.Credentials = new NetworkCredential(login,
                                                       pwd);
          reader.XmlResolver = resolver;

          var feedXML = LoadXDocument(reader,
                                      out errorMsg);

          var f = LoadUser(feedXML);

          if (f.Count > 0)
          {
            friends.AddRange(f);
            page++;
          }
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("TwitterLib::GetTwitterFriends()",
                          ex);
      }
      return friends;
    }

    public static List<TwitterUser> GetFriendsCursor(string login,
                                                     string pwdHash,
                                                     string hashKey, string page,
                                                     out string errorMsg, out string nextCursor)
    {
      errorMsg = null;
      var friends = new List<TwitterUser>();

      try
      {
        using (var reader = new XmlTextReader(string.Format("{0}?cursor={1}",
                                                            TwitterResources.friends,
                                                            page)))
        {
          var resolver = new XmlUrlResolver();

          // Add authentication to request  
          var key = EncryptionHelper.GetHashKey(hashKey);
          var pwd = EncryptionHelper.Decrypt(key,
                                             pwdHash);

          resolver.Credentials = new NetworkCredential(login,
                                                       pwd);
          reader.XmlResolver = resolver;

          var feedXML = LoadXDocument(reader,
                                      out errorMsg).ToString();

          var f = LoadUser(XDocument.Parse(feedXML));

          if (f.Count > 0)
          {
            friends.AddRange(f);
          }

          var tempStartCursor = feedXML.ToLower().IndexOf("<next_cursor>");

          var tempNextCursor = feedXML.Substring(tempStartCursor + 13);

          var tempEndCursor = tempNextCursor.ToLower().IndexOf("</next_cursor>");
          nextCursor = tempNextCursor.Substring(0, tempEndCursor);
        }
      }
      catch (Exception ex)
      {
        errorMsg = ex.Message;
        TraceHelper.Trace("TwitterLib::GetTwitterFriends()",
                          ex);
        nextCursor = "-1";
      }

      return friends;
    }

    public static List<TwitterUser> GetFriends(string login,
                                               string pwdHash,
                                               string hashKey,
                                               int page,
                                               out string errorMsg)
    {
      errorMsg = null;
      var friends = new List<TwitterUser>();

      try
      {
        using (var reader = new XmlTextReader(string.Format("{0}?page={1}",
                                                            TwitterResources.friends,
                                                            page)))
        {
          var resolver = new XmlUrlResolver();

          // Add authentication to request  
          var key = EncryptionHelper.GetHashKey(hashKey);
          var pwd = EncryptionHelper.Decrypt(key,
                                             pwdHash);

          resolver.Credentials = new NetworkCredential(login,
                                                       pwd);
          reader.XmlResolver = resolver;

          var feedXML = LoadXDocument(reader,
                                      out errorMsg);

          var f = LoadUser(feedXML);

          if (f != null)
            if (f.Count > 0)
            {
              friends.AddRange(f);
            }
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("TwitterLib::GetTwitterFriends()",
                          ex);
      }
      return friends;
    }

    public static TwitterUser GetUserInfo(string userToGet,
                                          out string errorMsg)
    {
      return GetUserInfo(userToGet,
                         false,
                         out errorMsg);
    }

    public static TwitterUser GetUserInfo(string userToGet,
                                          bool bigImage,
                                          out string errorMsg)
    {
      errorMsg = null;

      try
      {
        using (var reader = new XmlTextReader(string.Format("{0}{1}.xml",
                                                            TwitterResources.extendedUserInfo,
                                                            userToGet)))
        {
          var resolver = new XmlUrlResolver();

          reader.XmlResolver = resolver;

          var feedXML = LoadXDocument(reader,
                                      out errorMsg);
          var users = LoadUser(feedXML);
          if (users == null ||
              users.Count < 1) return null;
          var user = users.First();
          if (user == null) return null;

          if (bigImage && !string.IsNullOrEmpty(user.ProfileImgUrl) && user.ProfileImgUrl.Contains("_normal"))
            user.ProfileImgUrl = user.ProfileImgUrl.Replace("_normal",
                                                            string.Empty);

          return user;
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("TwitterLib::GetTwitterFriends()",
                          ex);
      }
      return null;
    }

    public static bool FriendshipExists(string userA,
                                        string userB,
                                        out string errorMsg)
    {
      errorMsg = null;

      try
      {
        var path = string.Format("{0}user_a={1}&user_b={2}",
                                 TwitterResources.friendshipExists,
                                 userA,
                                 userB);
        using (var reader = new XmlTextReader(@path))
        {
          var resolver = new XmlUrlResolver();

          reader.XmlResolver = resolver;

          var feedXML = LoadXDocument(reader,
                                      out errorMsg);
          if (feedXML != null)
          {
            var result = feedXML.Element("friends").Value;
            return bool.Parse(result);
          }
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("TwitterLib::GetTwitterFriends()",
                          ex);
      }
      return false;
    }

    public static List<TwitterEntry> GetFeedFromUrl(string login,
                                                    string pwdHash,
                                                    string uri,
                                                    int count,
                                                    int pageNb,
                                                    bool isDirectMessages,
                                                    string hashKey,
                                                    out string errorMsg)
    {
      errorMsg = string.Empty;
      var entries = new List<TwitterEntry>();

      try
      {
        var path = string.Format("{0}?count={1}&pageNb={2}.xml",
                                 uri,
                                 count,
                                 pageNb);

        using (var reader = new XmlTextReader(@path))
        {
          if (!string.IsNullOrEmpty(login))
          {
            var resolver = new XmlUrlResolver();

            // Add authentication to request  
            var key = EncryptionHelper.GetHashKey(hashKey);
            var pwd = EncryptionHelper.Decrypt(key,
                                               pwdHash);
            resolver.Credentials = new NetworkCredential(login,
                                                         pwd);

            reader.XmlResolver = resolver;
          }

          var feedXML = LoadXDocument(reader,
                                      out errorMsg);

          if (feedXML == null)
          {
            return null;
          }
          if (!isDirectMessages)
          {
            entries = LoadFeed(feedXML);
          }
          else
          {
            entries = LoadDirectMessage(feedXML);
          }
        }

        return entries;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("TwitterLib -> GetFeedFromUrl() -> ",
                          ex);
      }
      return entries;
    }

    public static List<TwitterEntry> GetFeedFromUrl(string login,
                                                    string pwdHash,
                                                    string uri,
                                                    int count,
                                                    int pageNb,
                                                    long maxId, long SinceId,
                                                    bool isDirectMessages,
                                                    string hashKey,
                                                    out string errorMsg)
    {
      errorMsg = string.Empty;
      var entries = new List<TwitterEntry>();

      try
      {
        var txt = string.Empty;
        if (maxId != -1)
        {
          txt = "&max_id=" + maxId;
        }
        if (SinceId != -1)
        {
          txt += "&since_id=" + SinceId;
        }
        var path = string.Format("{0}?count={1}&pageNb={2}{3}.xml",
                                 uri,
                                 count,
                                 pageNb, txt);

        using (var reader = new XmlTextReader(@path))
        {
          if (!string.IsNullOrEmpty(login))
          {
            var resolver = new XmlUrlResolver();

            // Add authentication to request  
            var key = EncryptionHelper.GetHashKey(hashKey);
            var pwd = EncryptionHelper.Decrypt(key,
                                               pwdHash);
            resolver.Credentials = new NetworkCredential(login,
                                                         pwd);

            reader.XmlResolver = resolver;
          }
          var feedXML = LoadXDocument(reader,
                                      out errorMsg);
          if (feedXML == null)
          {
            return null;
          }

          if (!isDirectMessages)
          {
            entries = LoadFeed(feedXML);
          }
          else
          {
            entries = LoadDirectMessage(feedXML);
          }
        }

        return entries;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("TwitterLib -> GetFeedFromUrl() -> ",
                          ex);
      }
      return entries;
    }

    public static XDocument LoadXDocument(XmlReader reader,
                                          out string errorMsg)
    {
      errorMsg = null;
      try
      {
        var xdoc = XDocument.Parse(XDocument.Load(@reader).ToString().Replace("georss:point", "georss_point"));
        return xdoc;
      }
      catch (WebException webExcp)
      {
        errorMsg = ManageWebException(webExcp);
        return null;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("TwitterLib::LoadXDocument()",
                          ex);
        return null;
      }
    }

    internal static List<TwitterEntry> LoadFeed(XDocument feedXML)
    {
      if (feedXML == null)
      {
        return new List<TwitterEntry>();
      }
      var entries = from status in feedXML.Descendants("status")
                    let content =
                      GetTitle(HttpUtility.HtmlDecode(status.Element("text").Value), status)
                    let date = DateTime.ParseExact(status.Element("created_at").Value,
                                                   TwitterResources.createdAtDateFormat,
                                                   CultureInfo.InvariantCulture,
                                                   DateTimeStyles.AllowWhiteSpaces)
                    select new TwitterEntry
                             {
                               Id = status.Element("id").Value,
                               Title = content,
                               Content = content,
                               SourceName = status.Element("source").Value,
                               UpdateDate = date,
                               PubDate = date,
                               //                   Truncated = bool.Parse(status.Element("truncated").Value),
                               InReplyTo = status.Element("in_reply_to_status_id").Value,
                               InReplyToUserId = status.Element("in_reply_to_user_id").Value,
                               InReplyToUserName = status.Element("in_reply_to_screen_name").Value,
                               Type = EnumType.Twitter,
                               User = GetUser(status),
                               RetweeterUser = GetTwitterData(status)
                             };
      feedXML = null;
      return entries.ToList();
    }

    private static TwitterUser GetTwitterData(XElement status)
    {
      try
      {
        if (status.Element("retweeted_status") != null)
        {
          return new TwitterUser
                   {
                     Id = status.Element("user").Element("id").Value,
                     Name = status.Element("user").Element("name").Value,
                     NickName = status.Element("user").Element("screen_name").Value,
                     ProfileUrl =
                       TwitterResources.home +
                       status.Element("user").Element("screen_name").Value,
                     Location = status.Element("user").Element("location").Value,
                     Description =
                       status.Element("user").Element("description").Value,
                     ProfileImgUrl =
                       status.Element("user").Element("profile_image_url").Value,
                     Url = status.Element("user").Element("url").Value,
                     //Protected = bool.Parse(status.Element("user").Element("protected").Value),
                     FollowersCount =
                       int.Parse(
                         status.Element("user").Element("followers_count").Value),
                     StatusUseCount =
                       status.Element("statuses_count") != null
                         ? int.Parse(status.Element("statuses_count").Value)
                         : 0,
                     Geolocation = GetLocation(status)
                   };
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }
      return null;
    }

    private static TwitterUser GetUser(XElement status)
    {
      try
      {
        if (status.Element("retweeted_status") != null)
        {
          status = status.Element("retweeted_status");
          return new TwitterUser
                   {
                     Id = status.Element("user").Element("id").Value,
                     Name = status.Element("user").Element("name").Value,
                     NickName = status.Element("user").Element("screen_name").Value,
                     ProfileUrl =
                       TwitterResources.home +
                       status.Element("user").Element("screen_name").Value,
                     Location = status.Element("user").Element("location").Value,
                     Description =
                       status.Element("user").Element("description").Value,
                     ProfileImgUrl =
                       status.Element("user").Element("profile_image_url").Value,
                     Url = status.Element("user").Element("url").Value,
                     //Protected = bool.Parse(status.Element("user").Element("protected").Value),
                     FollowersCount =
                       int.Parse(
                         status.Element("user").Element("followers_count").Value),
                     FriendsCount = int.Parse(
                       status.Element("user").Element("friends_count").Value),
                     CreatedAt = DateTime.ParseExact(status.Element("user").Element("created_at").Value,
                                                     TwitterResources.createdAtDateFormat,
                                                     CultureInfo.InvariantCulture,
                                                     DateTimeStyles.AllowWhiteSpaces),
                     NbFavorites =
                       status.Element("user").Element("favourites_count") != null
                         ? int.Parse(status.Element("user").Element("favourites_count").Value)
                         : 0,
                     IsProtected =
                       status.Element("user").Element("protected") != null &&
                       !string.IsNullOrEmpty(status.Element("user").Element("protected").Value)
                         ? bool.Parse(status.Element("user").Element("protected").Value)
                         : false,
                     IsVerified =
                       status.Element("user").Element("verified") != null &&
                       !string.IsNullOrEmpty(status.Element("user").Element("verified").Value)
                         ? bool.Parse(status.Element("user").Element("verified").Value)
                         : false,
                     IsFollowing =
                       status.Element("user").Element("following") != null &&
                       !string.IsNullOrEmpty(status.Element("user").Element("following").Value)
                         ? bool.Parse(status.Element("user").Element("following").Value)
                         : false,
                     UserTimeZone =
                       status.Element("user").Element("time_zone") != null &&
                       !string.IsNullOrEmpty(status.Element("user").Element("time_zone").Value)
                         ? status.Element("user").Element("time_zone").Value
                         : string.Empty,
                     Lang =
                       status.Element("user").Element("lang") != null
                         ? status.Element("user").Element("lang").Value
                         : string.Empty,
                     UtcOffset =
                       status.Element("user").Element("utc_offset") != null &&
                       !string.IsNullOrEmpty(status.Element("user").Element("utc_offset").Value)
                         ? int.Parse(status.Element("user").Element("utc_offset").Value)
                         : 0,
                     StatusUseCount =
                       status.Element("user").Element("statuses_count") != null
                         ? int.Parse(status.Element("user").Element("statuses_count").Value)
                         : 0,
                     Geolocation = GetLocation(status)
                   };
        }
        return new TwitterUser
                 {
                   Id = status.Element("user").Element("id").Value,
                   Name = status.Element("user").Element("name").Value,
                   NickName = status.Element("user").Element("screen_name").Value,
                   ProfileUrl =
                     TwitterResources.home +
                     status.Element("user").Element("screen_name").Value,
                   Location = status.Element("user").Element("location").Value,
                   Description =
                     status.Element("user").Element("description").Value,
                   ProfileImgUrl =
                     status.Element("user").Element("profile_image_url").Value,
                   Url = status.Element("user").Element("url").Value,
                   //Protected = bool.Parse(status.Element("user").Element("protected").Value),
                   FollowersCount =
                     int.Parse(
                       status.Element("user").Element("followers_count").Value),
                   FriendsCount = int.Parse(
                     status.Element("user").Element("friends_count").Value),
                   CreatedAt = DateTime.ParseExact(status.Element("user").Element("created_at").Value,
                                                   TwitterResources.createdAtDateFormat,
                                                   CultureInfo.InvariantCulture,
                                                   DateTimeStyles.AllowWhiteSpaces),
                   NbFavorites =
                     status.Element("user").Element("favourites_count") != null
                       ? int.Parse(status.Element("user").Element("favourites_count").Value)
                       : 0,
                   IsProtected =
                     status.Element("user").Element("protected") != null &&
                     !string.IsNullOrEmpty(status.Element("user").Element("protected").Value)
                       ? bool.Parse(status.Element("user").Element("protected").Value)
                       : false,
                   IsVerified =
                     status.Element("user").Element("verified") != null &&
                     !string.IsNullOrEmpty(status.Element("user").Element("verified").Value)
                       ? bool.Parse(status.Element("user").Element("verified").Value)
                       : false,
                   IsFollowing =
                     status.Element("user").Element("following") != null &&
                     !string.IsNullOrEmpty(status.Element("user").Element("following").Value)
                       ? bool.Parse(status.Element("user").Element("following").Value)
                       : false,
                   UserTimeZone =
                     status.Element("user").Element("time_zone") != null &&
                     !string.IsNullOrEmpty(status.Element("user").Element("time_zone").Value)
                       ? status.Element("user").Element("time_zone").Value
                       : string.Empty,
                   Lang =
                     status.Element("user").Element("lang") != null
                       ? status.Element("user").Element("lang").Value
                       : string.Empty,
                   UtcOffset =
                     status.Element("user").Element("utc_offset") != null &&
                     !string.IsNullOrEmpty(status.Element("user").Element("utc_offset").Value)
                       ? int.Parse(status.Element("user").Element("utc_offset").Value)
                       : 0,
                   StatusUseCount =
                     status.Element("user").Element("statuses_count") != null
                       ? int.Parse(status.Element("user").Element("statuses_count").Value)
                       : 0,
                   Geolocation = GetLocation(status)
                 };
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }
      return null;
    }

    private static Geoloc GetLocation(XContainer status)
    {
      if (status != null)
      {
        if (status.Element("geo") != null && (status.Element("geo").HasAttributes || status.Element("geo").HasElements) &&
            status.Element("geo").Element("georss_point") != null)
        {
          var value = status.Element("geo").Element("georss_point").Value;
          var values = value.Split(' ');
          return new Geoloc { Latitude = double.Parse(values[0]), Longitude = double.Parse(values[1]) };
        }
        if (status.Element("geo") != null && status.Element("geo").HasElements)
        {
          XNamespace georss = "http://www.georss.org/georss";
          if (status.Element("geo").Element(georss + "point") != null)
          {
            var s = status.Element("geo").Element(georss + "point").Value;
            var values = s.Split(' ');
            return new Geoloc { Latitude = double.Parse(values[0]), Longitude = double.Parse(values[1]) };
          }
        }
        XNamespace atom = "http://www.w3.org/2005/Atom";
        if (status.Element(atom + "twitter_geo") != null &&
            (status.Element(atom + "twitter_geo").HasAttributes || status.Element(atom + "twitter_geo").HasElements) &&
            status.Element(atom + "twitter_geo").Element(atom + "georss_point") != null)
        {
          var value = status.Element(atom + "twitter_geo").Element(atom + "georss_point").Value;
          var values = value.Split(' ');
          return new Geoloc { Latitude = double.Parse(values[0]), Longitude = double.Parse(values[1]) };
        }
      }
      return null;
    }

    private static string GetTitle(string decode, XContainer element)
    {
      return element.Element("retweeted_status") != null
               ? HttpUtility.HtmlDecode(element.Element("retweeted_status").Element("text").Value)
               : decode;
    }

    internal static List<TwitterEntry> LoadDirectMessage(XDocument feedXML)
    {
      if (feedXML != null)
      {
        var statuses = from status in feedXML.Descendants("direct_message")
                       let content = HttpUtility.HtmlDecode(status.Element("text").Value)
                       let date = DateTime.ParseExact(status.Element("created_at").Value,
                                                      TwitterResources.createdAtDateFormat,
                                                      CultureInfo.InvariantCulture,
                                                      DateTimeStyles.AllowWhiteSpaces)
                       let sender = status.Element("sender")
                       let recipient = status.Element("recipient")
                       select new TwitterEntry
                                {
                                  Id = status.Element("id").Value,
                                  Title = content,
                                  Content = content,
                                  UpdateDate = date,
                                  PubDate = date,
                                  Type = EnumType.Twitter,
                                  //SenderScreenName = status.Element("sender_screen_name").Value,
                                  //RecipientScreenName = status.Element("recipient_screen_name").Value,
                                  User = new TwitterUser
                                           {
                                             Id = sender.Element("id").Value,
                                             Name = sender.Element("name").Value,
                                             NickName = sender.Element("screen_name").Value,
                                             ProfileUrl =
                                               TwitterResources.home +
                                               sender.Element("screen_name").Value,
                                             Location = sender.Element("location").Value,
                                             Description = sender.Element("description").Value,
                                             ProfileImgUrl =
                                               sender.Element("profile_image_url").Value,
                                             Url = sender.Element("url").Value,
                                             //Protected = bool.Parse(status.Element("sender").Element("protected").Value),
                                             FollowersCount =
                                               int.Parse(sender.Element("followers_count").Value),
                                           },
                                  ToUser = new TwitterUser
                                             {
                                               Id = recipient.Element("id").Value,
                                               Name = recipient.Element("name").Value,
                                               NickName = recipient.Element("screen_name").Value,
                                               ProfileUrl =
                                                 TwitterResources.home +
                                                 recipient.Element("screen_name").Value,
                                               Location = recipient.Element("location").Value,
                                               Description =
                                                 recipient.Element("description").Value,
                                               ProfileImgUrl =
                                                 recipient.Element("profile_image_url").Value,
                                               Url = recipient.Element("url").Value,
                                               //Protected = bool.Parse(status.Element("recipient").Element("protected").Value),
                                               FollowersCount =
                                                 int.Parse(
                                                   recipient.Element("followers_count").Value),
                                             },
                                };
        feedXML = null;
        return statuses.ToList();
      }
      return null;
    }

    public static List<TwitterUser> LoadUser(XDocument feedXML)
    {
      if (feedXML == null)
      {
        return null;
      }
      var friends = new List<TwitterUser>();
      var query = (from user in feedXML.Descendants("user")
                   let status = user.Element("status")
                   select new TwitterUser
                            {
                              Id = user.Element("id") != null ? user.Element("id").Value : string.Empty,
                              Name = user.Element("name") != null ? user.Element("name").Value : string.Empty,
                              NickName =
                                user.Element("screen_name") != null ? user.Element("screen_name").Value : string.Empty,
                              ProfileUrl =
                                user.Element("location") != null && user.Element("screen_name") != null
                                  ? TwitterResources.home + user.Element("screen_name").Value
                                  : string.Empty,
                              Location =
                                user.Element("location") != null ? user.Element("location").Value : string.Empty,
                              Description =
                                user.Element("description") != null ? user.Element("description").Value : string.Empty,
                              ProfileImgUrl =
                                user.Element("profile_image_url") != null
                                  ? user.Element("profile_image_url").Value
                                  : string.Empty,
                              Url = user.Element("url") != null ? user.Element("url").Value : string.Empty,
                              FollowersCount =
                                user.Element("followers_count") != null
                                  ? int.Parse(user.Element("followers_count").Value)
                                  : 0,
                              FriendsCount =
                                user.Element("friends_count") != null
                                  ? int.Parse(user.Element("friends_count").Value)
                                  : 0,
                              CreatedAt =
                                user.Element("screen_name") != null
                                  ? DateTime.ParseExact(user.Element("created_at").Value,
                                                        TwitterResources.createdAtDateFormat,
                                                        CultureInfo.InvariantCulture,
                                                        DateTimeStyles.AllowWhiteSpaces)
                                  : DateTime.Now,
                              NbFavorites =
                                user.Element("favourites_count") != null
                                  ? int.Parse(user.Element("favourites_count").Value)
                                  : 0,
                              IsProtected =
                                user.Element("protected") != null &&
                                !string.IsNullOrEmpty(user.Element("protected").Value)
                                  ? bool.Parse(user.Element("protected").Value)
                                  : false,
                              IsVerified =
                                user.Element("verified") != null &&
                                !string.IsNullOrEmpty(user.Element("verified").Value)
                                  ? bool.Parse(user.Element("verified").Value)
                                  : false,
                              IsFollowing =
                                user.Element("following") != null &&
                                !string.IsNullOrEmpty(user.Element("following").Value)
                                  ? bool.Parse(user.Element("following").Value)
                                  : false,
                              UserTimeZone =
                                user.Element("time_zone") != null &&
                                !string.IsNullOrEmpty(user.Element("time_zone").Value)
                                  ? user.Element("time_zone").Value
                                  : string.Empty,
                              Lang = user.Element("lang") != null ? user.Element("lang").Value : string.Empty,
                              UtcOffset =
                                user.Element("utc_offset") != null &&
                                !string.IsNullOrEmpty(user.Element("utc_offset").Value)
                                  ? int.Parse(user.Element("utc_offset").Value)
                                  : 0,
                              StatusUseCount =
                                user.Element("statuses_count") != null
                                  ? int.Parse(user.Element("statuses_count").Value)
                                  : 0,
                              Geolocation = GetLocation(status)
                            });
      //select new TwitterUser
      //         {
      //           Id = user.Element("id").Value,
      //           NickName = user.Element("screen_name").Value,
      //           Location = user.Element("location").Value,
      //           Description =
      //             HttpUtility.HtmlDecode(user.Element("description").Value),
      //           ProfileImgUrl = user.Element("profile_image_url").Value,
      //           Url = user.Element("url").Value,
      //           FollowersCount = int.Parse(user.Element("followers_count").Value),
      //           Name = user.Element("name").Value,
      //           FriendsCount = int.Parse(user.Element("friends_count").Value),
      //           CreatedAt = DateTime.ParseExact(user.Element("created_at").Value,
      //                                           TwitterResources.createdAtDateFormat,
      //                                           CultureInfo.InvariantCulture,
      //                                           DateTimeStyles.AllowWhiteSpaces),
      //           LastStatus = status != null
      //                          ? new TwitterEntry
      //                              {
      //                                PubDate =
      //                                  DateTime.ParseExact(
      //                                  status.Element("created_at").Value,
      //                                  TwitterResources.createdAtDateFormat,
      //                                  CultureInfo.InvariantCulture,
      //                                  DateTimeStyles.AllowWhiteSpaces),
      //                                Id = status.Element("id").Value,
      //                                Title =
      //                                  HttpUtility.HtmlDecode(
      //                                  status.Element("text").Value),
      //                                SourceName = status.Element("source").Value,
      //                                InReplyTo =
      //                                  status.Element("in_reply_to_screen_name").
      //                                  Value,
      //                                InReplyToUserId =
      //                                  status.Element("in_reply_to_user_id").
      //                                  Value,
      //                                Type = EnumType.Twitter,
      //                              }
      //                          : null,
      //         });
      friends = query.ToList();

      return friends;
    }

    public static TwitterEntry AddFavorite(string login,
                                           string pwdHash,
                                           string tweetId,
                                           string hashKey,
                                           bool destroy,
                                           out string errorMsg)
    {
      return AddFavorite(login,
                         pwdHash,
                         tweetId,
                         hashKey,
                         destroy,
                         out errorMsg,
                         null);
    }

    public static TwitterEntry AddFavorite(string login,
                                           string pwdHash,
                                           string tweetId,
                                           string hashKey,
                                           bool destroy,
                                           out string errorMsg,
                                           WebProxy proxy)
    {
      errorMsg = null;

      // Create the web request  
      var url = TwitterResources.favoritesCreate;
      if (destroy)
        url = TwitterResources.favoritesDestroy;

      url += string.Format("{0}.xml",
                           tweetId);

      var request = WebRequest.Create(url) as HttpWebRequest;
      if (proxy != null)
      {
        request.Proxy = proxy;
      }

      // Add authentication to request  
      var key = EncryptionHelper.GetHashKey(hashKey);
      var pwd = EncryptionHelper.Decrypt(key,
                                         pwdHash);

      // Add authentication to request  
      request.Credentials = new NetworkCredential(login,
                                                  pwd);

      request.Method = "POST";

      // Set values for the request back
      request.ContentType = "application/x-www-form-urlencoded";
      var sourceParam = "?source=" + TwitterResources.clientName;
      request.ContentLength = sourceParam.Length;
      request.ServicePoint.Expect100Continue = false;
      request.ProtocolVersion = HttpVersion.Version10;

      // Write the request paramater
      var stOut = new StreamWriter(request.GetRequestStream(),
                                   Encoding.ASCII);
      stOut.Write(sourceParam);
      stOut.Close();

      // Do the request to get the response
      using (var response = ExecuteWebRequest(request,
                                              out errorMsg))
      {
        // Get the response stream  
        if (response != null)
        {
          var xreader = XmlReader.Create(@response.GetResponseStream());
          var xdoc = XDocument.Load(@xreader);

          return LoadFeed(xdoc).First();
        }
        return null;
      }
    }


    public static TwitterEntry DeleteStatus(string login,
                                            string pwdHash,
                                            string tweetId,
                                            string hashKey,
                                            out string errorMsg)
    {
      return DeleteStatus(login,
                          pwdHash,
                          tweetId,
                          hashKey,
                          out errorMsg,
                          null);
    }

    public static TwitterEntry DeleteStatus(string login,
                                            string pwdHash,
                                            string tweetId,
                                            string hashKey,
                                            out string errorMsg,
                                            WebProxy proxy)
    {
      errorMsg = null;

      // Create the web request  

      var url = TwitterResources.statusDestroy;

      url += string.Format("{0}.xml",
                           tweetId);

      var request = WebRequest.Create(url) as HttpWebRequest;
      if (proxy != null)
      {
        request.Proxy = proxy;
      }

      // Add authentication to request  
      var key = EncryptionHelper.GetHashKey(hashKey);
      var pwd = EncryptionHelper.Decrypt(key,
                                         pwdHash);

      // Add authentication to request  
      request.Credentials = new NetworkCredential(login,
                                                  pwd);

      request.Method = "POST";

      // Set values for the request back
      request.ContentType = "application/x-www-form-urlencoded";
      var sourceParam = "?source=" + TwitterResources.clientName;
      request.ContentLength = sourceParam.Length;
      request.ServicePoint.Expect100Continue = false;
      request.ProtocolVersion = HttpVersion.Version10;

      // Write the request paramater
      var stOut = new StreamWriter(request.GetRequestStream(),
                                   Encoding.ASCII);
      stOut.Write(sourceParam);
      stOut.Close();

      // Do the request to get the response
      using (var response = ExecuteWebRequest(request, out errorMsg))
      {
        TraceHelper.Trace("TwitterLib::DeleteStatus:",
                          string.Format("errorMsg:{0}",
                                        errorMsg));

        // Get the response stream  
        if (response != null)
        {
          var xreader = XmlReader.Create(@response.GetResponseStream());
          var xdoc = XDocument.Load(@xreader);

          var _list = LoadFeed(xdoc);


          TraceHelper.Trace("TwitterLib::DeleteStatus:",
                            string.Format("_list Count:{0}",
                                          _list.Count));

          if (_list.Count > 0)
            return _list.First();
        }

        return null;
      }
    }


    public static TwitterUser Follow(string login,
                                     string pwdHash,
                                     string userIdToFollow,
                                     string hashKey,
                                     bool destroy,
                                     out string errorMsg)
    {
      return Follow(login,
                    pwdHash,
                    userIdToFollow,
                    hashKey,
                    destroy,
                    out errorMsg,
                    null);
    }

    public static TwitterUser Follow(string login,
                                     string pwdHash,
                                     string userIdToFollow,
                                     string hashKey,
                                     bool destroy,
                                     out string errorMsg,
                                     WebProxy proxy)
    {
      errorMsg = null;
      try
      {
        // Create the web request  
        var url = TwitterResources.follow;
        if (destroy)
          url = TwitterResources.unfollow;

        url += string.Format("{0}.xml",
                             userIdToFollow);

        var request = WebRequest.Create(url) as HttpWebRequest;

        var key = EncryptionHelper.GetHashKey(hashKey);
        var pwd = EncryptionHelper.Decrypt(key,
                                           pwdHash);

        // Add authentication to request  
        request.Credentials = new NetworkCredential(login,
                                                    pwd);

        request.Method = "POST";
        if (proxy != null)
        {
          request.Proxy = proxy;
        }

        // Set values for the request back
        request.ContentType = "application/x-www-form-urlencoded";
        var sourceParam = "?source=" + TwitterResources.clientName;
        request.ContentLength = sourceParam.Length;
        request.ServicePoint.Expect100Continue = false;
        request.ProtocolVersion = HttpVersion.Version10;

        // Write the request paramater
        var stOut = new StreamWriter(request.GetRequestStream(),
                                     Encoding.ASCII);
        stOut.Write(sourceParam);
        stOut.Close();

        // Do the request to get the response
        using (var response = ExecuteWebRequest(request,
                                                out errorMsg))
        {
          // Get the response stream  
          if (response != null)
          {
            var xreader = XmlReader.Create(@response.GetResponseStream());
            var xdoc = XDocument.Load(@xreader);

            return LoadUser(xdoc).First();
          }
          return null;
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("TwitterLib::Follow()",
                          ex);
        return null;
      }
    }

    public static TwitterEntry AddTweet(string login,
                                        string pwdHash,
                                        string message,
                                        bool convertUrlsToTinyUrls,
                                        string hashKey,
                                        out string errorMsg,
                                        string replyID)
    {
      return AddTweet(login,
                      pwdHash,
                      message,
                      convertUrlsToTinyUrls,
                      hashKey,
                      out errorMsg,
                      replyID,
                      null);
    }

    public static TwitterEntry AddTweet(string login,
                                        string pwdHash,
                                        string message,
                                        bool convertUrlsToTinyUrls,
                                        string hashKey,
                                        out string errorMsg,
                                        string replyID,
                                        WebProxy proxy)
    {
      try
      {
        errorMsg = null;
        var isDirectMessage = (message.StartsWith("d ",
                                                  StringComparison.CurrentCultureIgnoreCase));
        if (string.IsNullOrEmpty(message))
          return null;

        message = HttpUtility.UrlEncode(message);
        var key = EncryptionHelper.GetHashKey(hashKey);
        var pwd = EncryptionHelper.Decrypt(key,
                                           pwdHash);
        string param;
        string sourceParam;
        // Create the web request  
        HttpWebRequest request;
        if (isDirectMessage)
        {
          request = WebRequest.Create(TwitterResources.directMessageNew) as HttpWebRequest;
          request.Headers.Add("Authorization",
                              "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(login + ":" + pwd)));
          var user = message.Split('+');
          param = "user=" + user[1];
          message = "";
          var i = 0;
          foreach (var s in user)
          {
            if (i > 1)
            {
              message += "+";
              message += s;
            }
            i++;
          }
          sourceParam = "&text=" + message;
        }
        else
        {
          request = WebRequest.Create(TwitterResources.postNewStatus) as HttpWebRequest;
          param = "status=" + message;

          if (!string.IsNullOrEmpty(replyID))
          {
            sourceParam = "&source=" + TwitterResources.clientName + "&in_reply_to_status_id=" + replyID;
          }
          else
          {
            sourceParam = "&source=" + TwitterResources.clientName;
          }
        }
        request.ContentLength = param.Length + sourceParam.Length;
        if (proxy != null)
        {
          request.Proxy = proxy;
        }
        // Add authentication to request  
        request.Credentials = new NetworkCredential(login,
                                                    pwd);

        request.Method = "POST";

        // Set values for the request back
        request.ContentType = "application/x-www-form-urlencoded";
        request.ServicePoint.Expect100Continue = false;
        request.ProtocolVersion = HttpVersion.Version10;

        // Write the request paramater
        var stOut = new StreamWriter(request.GetRequestStream(),
                                     Encoding.ASCII);
        stOut.Write(param);
        stOut.Write(sourceParam);
        stOut.Close();

        // Do the request to get the response
        using (var response = ExecuteWebRequest(request,
                                                out errorMsg))
        {
          // Get the response stream  
          if (response != null)
          {
            var xreader = XmlReader.Create(@response.GetResponseStream());
            var xdoc = XDocument.Load(@xreader);
            if (isDirectMessage)
            {
              return LoadDirectMessage(xdoc).First();
            }
            return LoadFeed(xdoc).First();
          }
          return null;
        }
      }
      catch (Exception Ex)
      {
        TraceHelper.Trace("TwitterLib",
                          Ex);
        errorMsg = Ex.Message;
      }
      return null;
    }

    public static TwitterEntry Retweet(string login,
                                       string pwdHash,
                                       bool convertUrlsToTinyUrls,
                                       string hashKey,
                                       out string errorMsg,
                                       string retweetId,
                                       WebProxy proxy)
    {
      try
      {
        var key = EncryptionHelper.GetHashKey(hashKey);
        var pwd = EncryptionHelper.Decrypt(key,
                                           pwdHash);
        // Create the web request  
        HttpWebRequest request;

        request =
          WebRequest.Create(TwitterResources.retweet + retweetId + ".xml?source=" + TwitterResources.clientName) as
          HttpWebRequest;
        if (proxy != null)
        {
          request.Proxy = proxy;
        }
        // Add authentication to request  
        request.Credentials = new NetworkCredential(login,
                                                    pwd);

        request.Method = "POST";

        // Set values for the request back
        request.ContentType = "application/x-www-form-urlencoded";
        request.ServicePoint.Expect100Continue = false;
        request.ProtocolVersion = HttpVersion.Version10;

        // Do the request to get the response
        using (var response = ExecuteWebRequest(request,
                                                out errorMsg))
        {
          // Get the response stream  
          if (response != null)
          {
            var xreader = XmlReader.Create(@response.GetResponseStream());
            var xdoc = XDocument.Load(@xreader);
            return LoadFeed(xdoc).First();
          }
          return null;
        }
      }
      catch (Exception Ex)
      {
        TraceHelper.Trace("TwitterLib",
                          Ex);
        errorMsg = Ex.Message;
      }
      return null;
    }

    public static string CheckCredentials(string login,
                                          string pwdHash,
                                          string hashKey)
    {
      return CheckCredentials(login,
                              pwdHash,
                              hashKey,
                              null);
    }

    public static string CheckCredentials(string login,
                                          string pwdHash,
                                          string hashKey,
                                          WebProxy proxy)
    {
      var result = "Error! Please try again later.";

      try
      {
        //REMARK: We may want to refactor this to return the message returned by the API.
        // Create the web request  
        //lock (_syncRoot)
        //{
        var request = WebRequest.Create("http://api.twitter.com/1.1/account/verify_credentials.xml") as HttpWebRequest;

        // Add authentication to request  

        var key = EncryptionHelper.GetHashKey(hashKey);
        var pwd = EncryptionHelper.Decrypt(key,
                                           pwdHash);
        if (proxy != null)
        {
          request.Proxy = proxy;
        }
        request.Credentials = new NetworkCredential(login,
                                                    pwd);

        // Add configured web proxy
        //request.Proxy = HttpWebRequest.DefaultWebProxy;
        request.ContentType = "application/xml";
        //request.AllowWriteStreamBuffering = true;
        //request.UserAgent = "sobees";
        //request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(login + ":" + pwd)));
        //request.ServicePoint.Expect100Continue = false;
        request.Method = "GET";

        using (var response = ExecuteWebRequest(request,
                                                out result))
        {
          if (!string.IsNullOrEmpty(result))
            return result;

          if (response != null && response.StatusCode == HttpStatusCode.OK)
            return null;

          return result;
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("TwitterLib.CheckCredentials:",
                          ex);
      }
      return result;
    }

    public static TwitterRateLimit RemainingApi(string login,
                                                string pwdHash,
                                                string hashKey,
                                                out string errorMsg)
    {
      return RemainingApi(login,
                          pwdHash,
                          hashKey,
                          out errorMsg,
                          null);
    }

    public static TwitterRateLimit RemainingApi(string login,
                                                string pwdHash,
                                                string hashKey,
                                                out string errorMsg,
                                                WebProxy proxy)
    {
      errorMsg = null;
      TwitterRateLimit trl = null;
      try
      {
        var request = WebRequest.Create("http://api.twitter.com/1.1/application/rate_limit_status.xml") as HttpWebRequest;
        var key = EncryptionHelper.GetHashKey(hashKey);
        var pwd = EncryptionHelper.Decrypt(key,
                                           pwdHash);
        request.ContentType = "application/xml";
        request.Headers.Add("Authorization",
                            "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(login + ":" + pwd)));
        request.Method = "GET";
        if (proxy != null)
        {
          request.Proxy = proxy;
        }
        using (var reader = new XmlTextReader(request.GetResponse().GetResponseStream()))
        {
          var feedXML = LoadXDocument(reader,
                                      out errorMsg);
          if (feedXML != null)
          {
            var el = feedXML.Element("hash");
            if (el != null)
            {
              trl = new TwitterRateLimit();
              trl.RemainingHits = int.Parse(el.Element("X-Rate-Limit-Limit").Value);
              trl.HourlyLimit = int.Parse(el.Element("X-Rate-Limit-Limit").Value);
              trl.ResetTime = DateTime.Parse(el.Element("X-Rate-Limit-Reset").Value);
              return trl;
            }
          }
        }
        return null;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("TwitterLib.CheckCredentials:",
                          ex);
      }
      return trl;
    }



    protected static string GetPropertyFromXml(XmlNode twitterNode,
                                               string propertyName)
    {
      if (twitterNode != null)
      {
        var propertyNode = twitterNode.SelectSingleNode(propertyName);
        if (propertyNode != null)
        {
          return propertyNode.InnerText;
        }
      }
      return String.Empty;
    }

    private static HttpWebResponse ExecuteWebRequest(WebRequest request,
                                                     out string errorMsg)
    {
      errorMsg = null;
      try
      {
        // perform the destroy web request
        var response = request.GetResponse() as HttpWebResponse;
        return response;
      }
      catch (WebException webExcp)
      {
        errorMsg = ManageWebException(webExcp);
        return null;
      }
    }

    private static string ManageWebException(WebException webExcp)
    {
      string errorMsg = null;
      if (webExcp == null) return errorMsg;

      // Get the WebException status code.
      var status = webExcp.Status;

      // If status is WebExceptionStatus.ProtocolError, 
      //   there has been a protocol error and a WebResponse 
      //   should exist. Display the protocol error.
      if (status == WebExceptionStatus.ProtocolError)
      {
        // Get HttpWebResponse so that you can check the HTTP status code.
        var httpResponse = (HttpWebResponse)webExcp.Response;

        switch ((int)httpResponse.StatusCode)
        {
          case 400:
            errorMsg = "Bad request or Rate limit exceeded.";

            break;
          case 401: // unauthorized
            errorMsg = "Not Authorized.";
            break;
          case 403:
            errorMsg = "Forbidden. " + webExcp.Message;
            break;
          case 404:
            errorMsg = "Error 404:Not Found:  " + webExcp.Message;
            break;
          case 415:
            errorMsg = "Error 415: " + webExcp.Message;
            break;
          case 500:
            errorMsg = "Proxy authentication required.";
            break;
          case 502: //Bad Gateway, Twitter is freaking out.
            errorMsg = "Fail Whale!  There was a problem calling the Twitter API.  Please try again in a few minutes.";
            break;
          case 407:
            errorMsg = "Fail Whale!  There was a problem calling the Twitter API.  Please try again in a few minutes.";
            break;
          case 503:
            errorMsg = "Service Unavailable.";
            break;
        }


        if (webExcp.InnerException != null)
          TraceHelper.Trace("TwitterLib::ManageWebException::Code,Message,WebMessage,InnerExceptionMessage:",
                            string.Format("{0}|{1}|{2}|{3}",
                                          httpResponse.StatusCode, errorMsg,
                                          webExcp.Message, webExcp.InnerException.Message));
        else
          TraceHelper.Trace("TwitterLib::ManageWebException::Code,Message,WebMessage:",
                            string.Format("{0}|{1}|{2}",
                                          httpResponse.StatusCode, errorMsg,
                                          webExcp.Message));
      }
      else if (status == WebExceptionStatus.ProxyNameResolutionFailure)
      {
        errorMsg =
          "The proxy server could not be found.  Check that it was entered correctly in the Options dialog.  You may need to disable your web proxy in the Options, if for instance you use a proxy server at work and are now at home.";
        TraceHelper.Trace("TwitterLib::ManageWebException:",
                          "The proxy server could not be found.  Check that it was entered correctly in the Options dialog.  You may need to disable your web proxy in the Options, if for instance you use a proxy server at work and are now at home.");
      }
      else
      {
        errorMsg = webExcp.Message;
        TraceHelper.Trace("TwitterLib::ManageWebException:",
                          webExcp.Message);
      }
      return errorMsg;
    }

    public static List<TwitterList> LoadLists(XDocument feedXML)
    {
      var lists = new List<TwitterList>();
      if (feedXML != null)
      {
        var query = (from user in feedXML.Descendants("list")
                     let status = user.Element("status")
                     select new TwitterList
                              {
                                Id = user.Element("id").Value,
                                Name = user.Element("name").Value,
                                FullName = user.Element("full_name").Value,
                                Slug = user.Element("slug").Value,
                                SubscriberCount = int.Parse(user.Element("subscriber_count").Value),
                                MemberCount = int.Parse(user.Element("member_count").Value),
                                Url = "http://www.twitter.com" + user.Element("uri").Value,
                                Mode = user.Element("mode").Value,
                                Creator = new TwitterUser
                                            {
                                              Id = user.Element("user").Element("id").Value,
                                              Name = user.Element("user").Element("name").Value,
                                              Location =
                                                user.Element("user").Element("location").Value,
                                              NickName =
                                                user.Element("user").Element("screen_name").Value,
                                              Description =
                                                user.Element("user").Element("description").Value,
                                              ProfileImgUrl =
                                                user.Element("user").Element("profile_image_url").
                                                     Value,
                                            },
                              });
        lists = query.ToList();
      }

      return lists;
    }

    public static void Block(string login, string pwdHash, string hashKey, string userToBlock, out string errorMsg,
                             WebProxy proxy)
    {
      // Create the web request  
      var url = string.Format("http://api.twitter.com/1.1/blocks/create/{0}.xml", userToBlock);

      var request = WebRequest.Create(url) as HttpWebRequest;
      if (proxy != null)
      {
        request.Proxy = proxy;
      }
      else
      {
        request.Proxy = null;
      }

      // Add authentication to request  
      var key = EncryptionHelper.GetHashKey(hashKey);
      var pwd = EncryptionHelper.Decrypt(key,
                                         pwdHash);

      // Add authentication to request  
      request.Headers.Add("Authorization",
                          "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(login + ":" + pwd)));
      request.Method = "POST";

      // Set values for the request back
      //request.ContentType = "application/x-www-form-urlencoded";
      var sourceParam = string.Format("id={0}", HttpUtility.UrlEncode(userToBlock));
      request.ContentLength = sourceParam.Length;
      request.ServicePoint.Expect100Continue = false;
      request.ProtocolVersion = HttpVersion.Version10;

      // Write the request paramater
      var stOut = new StreamWriter(request.GetRequestStream(),
                                   Encoding.ASCII);
      stOut.Write(sourceParam);
      stOut.Close();

      // Do the request to get the response
      using (var response = ExecuteWebRequest(request, out errorMsg))
      {
        // Get the response stream  
        if (response != null)
        {
          var xreader = XmlReader.Create(@response.GetResponseStream());
          var xdoc = XDocument.Load(@xreader);

          //return LoadFeed(xdoc).First();
        }
      }
      return;
    }


    public static void ReportSpam(string login, string pwdHash, string hashKey, string userToBlock, out string errorMsg,
                                  WebProxy proxy)
    {
      // Create the web request  
      var url = "http://api.twitter.com/1.1/report_spam.xml";

      var request = WebRequest.Create(url) as HttpWebRequest;
      if (proxy != null)
      {
        request.Proxy = proxy;
      }
      else
      {
        request.Proxy = null;
      }

      // Add authentication to request  
      var key = EncryptionHelper.GetHashKey(hashKey);
      var pwd = EncryptionHelper.Decrypt(key,
                                         pwdHash);

      // Add authentication to request  
      request.Headers.Add("Authorization",
                          "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(login + ":" + pwd)));
      request.Method = "POST";

      // Set values for the request back
      //request.ContentType = "application/x-www-form-urlencoded";
      var sourceParam = string.Format("id={0}", HttpUtility.UrlEncode(userToBlock));
      request.ContentLength = sourceParam.Length;
      request.ServicePoint.Expect100Continue = false;
      request.ProtocolVersion = HttpVersion.Version10;

      // Write the request paramater
      var stOut = new StreamWriter(request.GetRequestStream(),
                                   Encoding.ASCII);
      stOut.Write(sourceParam);
      stOut.Close();

      // Do the request to get the response
      using (var response = ExecuteWebRequest(request, out errorMsg))
      {
        // Get the response stream  
        if (response != null)
        {
          var xreader = XmlReader.Create(@response.GetResponseStream());
          var xdoc = XDocument.Load(@xreader);

          //return LoadFeed(xdoc).First();
        }
      }
      return;
    }

    #region Lists

    public static string CreateNewList(string login, string pwdHash, string hashKey, string mode, string name,
                                       WebProxy proxy)
    {
      // Create the web request  
      var url = string.Format("http://api.twitter.com/1.1/{0}/lists.xml", login);

      var request = WebRequest.Create(url) as HttpWebRequest;
      if (proxy != null)
      {
        request.Proxy = proxy;
      }

      // Add authentication to request  
      var key = EncryptionHelper.GetHashKey(hashKey);
      var pwd = EncryptionHelper.Decrypt(key,
                                         pwdHash);

      // Add authentication to request  
      request.Headers.Add("Authorization",
                          "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(login + ":" + pwd)));
      request.Method = "POST";

      // Set values for the request back
      //request.ContentType = "application/x-www-form-urlencoded";
      var sourceParam = string.Format("name={0}&mode={1}", HttpUtility.UrlEncode(name), mode);
      request.ContentLength = sourceParam.Length;
      request.ServicePoint.Expect100Continue = false;
      request.ProtocolVersion = HttpVersion.Version10;

      // Write the request paramater
      var stOut = new StreamWriter(request.GetRequestStream(),
                                   Encoding.ASCII);
      stOut.Write(sourceParam);
      stOut.Close();

      // Do the request to get the response
      string errorMsg;
      using (var response = ExecuteWebRequest(request, out errorMsg))
      {
        // Get the response stream  
        if (response != null)
        {
          var xreader = XmlReader.Create(@response.GetResponseStream());
          var xdoc = XDocument.Load(@xreader);

          //return LoadFeed(xdoc).First();
        }
        return null;
      }
    }

    public static string ModifyList(string login, string pwdHash, string hashKey, string mode, string name,
                                    string oldName, WebProxy proxy)
    {
      // Create the web request  
      var url = string.Format("http://api.twitter.com/1.1/{0}/lists/{1}.xml", login, oldName);

      var request = WebRequest.Create(url) as HttpWebRequest;
      if (proxy != null)
      {
        request.Proxy = proxy;
      }

      // Add authentication to request  
      var key = EncryptionHelper.GetHashKey(hashKey);
      var pwd = EncryptionHelper.Decrypt(key,
                                         pwdHash);

      // Add authentication to request  
      request.Headers.Add("Authorization",
                          "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(login + ":" + pwd)));
      request.Method = "POST";

      // Set values for the request back
      //request.ContentType = "application/x-www-form-urlencoded";
      var sourceParam = string.Format("name={0}&mode={1}", HttpUtility.UrlEncode(name), mode);
      request.ContentLength = sourceParam.Length;
      request.ServicePoint.Expect100Continue = false;
      request.ProtocolVersion = HttpVersion.Version10;

      // Write the request paramater
      var stOut = new StreamWriter(request.GetRequestStream(),
                                   Encoding.ASCII);
      stOut.Write(sourceParam);
      stOut.Close();

      // Do the request to get the response
      string errorMsg;
      using (var response = ExecuteWebRequest(request, out errorMsg))
      {
        // Get the response stream  
        if (response != null)
        {
          var xreader = XmlReader.Create(@response.GetResponseStream());
          var xdoc = XDocument.Load(@xreader);

          //return LoadFeed(xdoc).First();
        }
        return null;
      }
    }

    public static List<TwitterList> GetListOwn(string login, string pwdHash, string hashKey, string username,
                                               out string errorMsg,
                                               WebProxy proxy)
    {
      var lists = new List<TwitterList>();

      try
      {
        using (var reader = new XmlTextReader(string.Format("http://api.twitter.com/1.1/{0}/lists.xml", username)))
        {
          var resolver = new XmlUrlResolver();

          // Add authentication to request  
          var key = EncryptionHelper.GetHashKey(hashKey);
          var pwd = EncryptionHelper.Decrypt(key,
                                             pwdHash);

          resolver.Credentials = new NetworkCredential(login,
                                                       pwd);
          reader.XmlResolver = resolver;

          var feedXML = LoadXDocument(reader,
                                      out errorMsg);

          var f = LoadLists(feedXML);

          if (f.Count > 0)
          {
            lists.AddRange(f);
          }
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("TwitterLib::GetTwitterFriends()",
                          ex);
      }
      errorMsg = "";
      return lists;
    }

    public static List<TwitterList> GetList(string login, string pwdHash, string hashKey, string username,
                                            out string errorMsg,
                                            WebProxy proxy)
    {
      var lists = new List<TwitterList>();

      try
      {
        using (
          var reader =
            new XmlTextReader(string.Format("http://api.twitter.com/1.1/{0}/lists/subscriptions.xml", username)))
        {
          var resolver = new XmlUrlResolver();

          // Add authentication to request  
          var key = EncryptionHelper.GetHashKey(hashKey);
          var pwd = EncryptionHelper.Decrypt(key,
                                             pwdHash);
          resolver.Credentials = new NetworkCredential(login,
                                                       pwd);
          reader.XmlResolver = resolver;

          var feedXML = LoadXDocument(reader,
                                      out errorMsg);

          var f = LoadLists(feedXML);

          if (f.Count > 0)
          {
            lists.AddRange(f);
          }
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("TwitterLib::GetTwitterFriends()",
                          ex);
      }
      errorMsg = "";
      return lists;
    }

    public static List<TwitterList> GetListMembership(string login, string pwdHash, string hashKey, string username,
                                                      out string errorMsg)
    {
      var lists = new List<TwitterList>();

      try
      {
        using (
          var reader = new XmlTextReader(string.Format("http://api.twitter.com/1.1/{0}/lists/memberships.xml", username))
          )
        {
          var resolver = new XmlUrlResolver();

          // Add authentication to request  
          var key = EncryptionHelper.GetHashKey(hashKey);
          var pwd = EncryptionHelper.Decrypt(key,
                                             pwdHash);
          resolver.Credentials = new NetworkCredential(login,
                                                       pwd);
          reader.XmlResolver = resolver;

          var feedXML = LoadXDocument(reader,
                                      out errorMsg);

          var f = LoadLists(feedXML);

          if (f.Count > 0)
          {
            lists.AddRange(f);
          }
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("TwitterLib::GetTwitterFriends()",
                          ex);
      }
      errorMsg = "";
      return lists;
    }

    public static List<TwitterEntry> GetListStatuse(string login, string pwdHash, string hashKey, string userList,
                                                    string listSlug, int nbGet, out string errorMsg)
    {
      var lists = new List<TwitterEntry>();

      try
      {
        var request =
          WebRequest.Create(string.Format("http://api.twitter.com/1.1/{0}/lists/{1}/statuses.xml?per_page={2}", userList,
                                          listSlug, nbGet)) as HttpWebRequest;
        var key = EncryptionHelper.GetHashKey(hashKey);
        var pwd = EncryptionHelper.Decrypt(key,
                                           pwdHash);
        request.ContentType = "application/xml";
        request.Headers.Add("Authorization",
                            "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(login + ":" + pwd)));
        request.Method = "GET";
        using (var reader = new XmlTextReader(request.GetResponse().GetResponseStream()))
        {
          var feedXML = LoadXDocument(reader,
                                      out errorMsg);

          var f = LoadFeed(feedXML);

          if (f.Count > 0)
          {
            lists.AddRange(f);
          }
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("TwitterLib::GetTwitterFriends()",
                          ex);
      }
      errorMsg = "";
      return lists;
    }

    public static bool AddToList(string login, string pwdHash, string hashKey, string userList, string listSlug,
                                 string userID, WebProxy proxy)
    {
      // Create the web request  
      var url = string.Format("http://api.twitter.com/1.1/{0}/{1}/members.xml", userList, listSlug);

      var request = WebRequest.Create(url) as HttpWebRequest;
      if (proxy != null)
      {
        request.Proxy = proxy;
      }
      else
      {
        request.Proxy = null;
      }

      // Add authentication to request  
      var key = EncryptionHelper.GetHashKey(hashKey);
      var pwd = EncryptionHelper.Decrypt(key,
                                         pwdHash);

      // Add authentication to request  
      request.Headers.Add("Authorization",
                          "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(login + ":" + pwd)));
      request.Method = "POST";

      // Set values for the request back
      //request.ContentType = "application/x-www-form-urlencoded";
      var sourceParam = string.Format("id={0}&list_id={1}", HttpUtility.UrlEncode(userID), listSlug);
      request.ContentLength = sourceParam.Length;
      request.ServicePoint.Expect100Continue = false;
      request.ProtocolVersion = HttpVersion.Version10;

      // Write the request paramater
      var stOut = new StreamWriter(request.GetRequestStream(),
                                   Encoding.ASCII);
      stOut.Write(sourceParam);
      stOut.Close();

      // Do the request to get the response
      string errorMsg;
      using (var response = ExecuteWebRequest(request, out errorMsg))
      {
        // Get the response stream  
        if (response != null)
        {
          var xreader = XmlReader.Create(@response.GetResponseStream());
          var xdoc = XDocument.Load(@xreader);

          return true;
        }
        return false;
      }
    }

    public static List<TwitterUser> GetListMembers(string login, string pwdHash, string hashKey, string userList,
                                                   string listSlug, out string errorMsg)
    {
      var lists = new List<TwitterUser>();

      try
      {
        var request =
          WebRequest.Create(string.Format("http://api.twitter.com/1.1/{0}/{1}/members.xml", userList, listSlug)) as
          HttpWebRequest;
        var key = EncryptionHelper.GetHashKey(hashKey);
        var pwd = EncryptionHelper.Decrypt(key,
                                           pwdHash);
        request.ContentType = "application/xml";
        request.Headers.Add("Authorization",
                            "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(login + ":" + pwd)));
        request.Method = "GET";
        using (var reader = new XmlTextReader(request.GetResponse().GetResponseStream()))
        {
          var feedXML = LoadXDocument(reader,
                                      out errorMsg);

          var f = LoadUser(feedXML);

          if (f.Count > 0)
          {
            lists.AddRange(f);
          }
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("TwitterLib::GetTwitterFriends()",
                          ex);
      }
      errorMsg = "";
      return lists;
    }

    public static bool DeleteListMembers(string login, string pwdHash, string hashKey, string userList, string listSlug,
                                         string userId, WebProxy proxy)
    {
      // Create the web request  
      var url = string.Format("http://api.twitter.com/1.1/{0}/{1}/members.xml", userList, listSlug);

      var request = WebRequest.Create(url) as HttpWebRequest;
      if (proxy != null)
      {
        request.Proxy = proxy;
      }
      else
      {
        request.Proxy = null;
      }

      // Add authentication to request  
      var key = EncryptionHelper.GetHashKey(hashKey);
      var pwd = EncryptionHelper.Decrypt(key,
                                         pwdHash);

      // Add authentication to request  
      request.Headers.Add("Authorization",
                          "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(login + ":" + pwd)));
      request.Method = "POST";

      // Set values for the request back
      //request.ContentType = "application/x-www-form-urlencoded";
      var sourceParam = string.Format("id={0}&list_id={1}&_method=DELETE", HttpUtility.UrlEncode(userId), listSlug);
      request.ContentLength = sourceParam.Length;
      request.ServicePoint.Expect100Continue = false;
      request.ProtocolVersion = HttpVersion.Version11;

      // Write the request paramater
      var stOut = new StreamWriter(request.GetRequestStream(),
                                   Encoding.ASCII);
      stOut.Write(sourceParam);
      stOut.Close();

      // Do the request to get the response
      string errorMsg;
      using (var response = ExecuteWebRequest(request, out errorMsg))
      {
        // Get the response stream  
        if (response != null)
        {
          var xreader = XmlReader.Create(@response.GetResponseStream());
          var xdoc = XDocument.Load(@xreader);

          return true;
        }
        return false;
      }
    }

    #endregion
  }
}