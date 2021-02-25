#region

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using BUtility;

#if !SILVERLIGHT
using System.Windows.Forms;
#endif

#endregion

namespace Sobees.Tools.Stats
{
  public class Stats
  {
    public EnumServiceType ServiceType { get; set; }

    public string UserId { get; set; }
    public string Id { get; set; }

    public string CreatorId { get; set; }
    public EnumEventType EventType { get; set; }

    public string PreviousTweet { get; set; }
    public string CurrentTweet { get; set; }
    public string NextTweet { get; set; }
    public DateTime Date { get; set; }
  }

  public enum EnumServiceType
  {
    TWITTER,
    FACEBOOK,
    LINKEDIN,
    TWITTERSEARCH,
  }

  public enum EnumEventType
  {
    CLICK,
    MORE,
    FULLDETAIL
  }

  public static class StatsHelper
  {
    private static bool isfirst = true;
    private static List<Stats> _listStats;

    public static List<Stats> ListStats
    {
      get { return _listStats ?? (_listStats = new List<Stats>()); }
      set { _listStats = value; }
    }

    /// <summary>
    /// </summary>
    /// <param name = "userId">Current User id</param>
    /// <param name = "servicesType">Typ of Services</param>
    /// <param name = "id">ID</param>
    /// <param name = "creatorId">Ceator of the Tweet</param>
    /// <param name = "eventType">Type of interactions</param>
    /// <param name = "previousTweet">previous tweet</param>
    /// <param name = "currentTweet">current tweet</param>
    /// <param name = "nextTweet">next tweet</param>
    public static void ReadContent(string userId,
                                   EnumServiceType servicesType,
                                   string id,
                                   string creatorId,
                                   EnumEventType eventType,
                                   string previousTweet,
                                   string currentTweet,
                                   string nextTweet)
    {
      //TODO uncomment his
      //ListStats.Add(new Stats
      //                  {
      //                      UserId = userId,
      //                      ServiceType = servicesType,
      //                      Id = id,
      //                      CreatorId = creatorId,
      //                      CurrentTweet = currentTweet,
      //                      EventType = eventType,
      //                      NextTweet = nextTweet,
      //                      PreviousTweet = previousTweet,
      //                      Date = DateTime.Now
      //                  });
      //SaveToFile();
    }

    private static void SaveToFile()
    {
      if (ListStats.Count == 0 && isfirst)
      {
        ReadOldStats();
      }
#if !SILVERLIGHT
      var folder = string.Format(@"{0}\{1}\{2}", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Application.CompanyName, Application.ProductName);

      //Check and create if apply the folder to store friends list
      if (!Directory.Exists(folder))
        Directory.CreateDirectory(folder);

      var writer = XmlWriter.Create(string.Format(@"{0}\UserInteractions.xml", folder), new XmlWriterSettings {Indent = true, NewLineOnAttributes = true});
#else
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                var isoStream = new IsolatedStorageFileStream("UserInteractions.xml",
                                                              FileMode.OpenOrCreate, store);
                var settings = new XmlWriterSettings {Indent = true};
                using (XmlWriter writer = XmlWriter.Create(isoStream, settings))
                {
#endif
      SaveIntoWriter(writer);
#if SILVERLIGHT
                }
                isoStream.Close();
            }
#endif
    }

    public static string GetXmlStats(bool mustClearData)
    {
      try
      {
        var strBuilder = new StringBuilder();
        var writer = XmlWriter.Create(strBuilder, new XmlWriterSettings {Indent = true, NewLineOnAttributes = true});
        SaveIntoWriter(writer);
        if (mustClearData)
        {
          ClearData();
        }
        return strBuilder.ToString();
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }
      return string.Empty;
    }

    public static void ClearData()
    {
      isfirst = false;
      ListStats.Clear();
    }

    private static void SaveIntoWriter(XmlWriter writer)
    {
      if (writer != null)
      {
        writer.WriteStartDocument();
        writer.WriteStartElement("Entries");
        foreach (var stat in ListStats)
        {
          writer.WriteStartElement("Entry");
          writer.WriteElementString("UserId", stat.UserId);
          writer.WriteElementString("ServiceType", stat.ServiceType.ToString());
          writer.WriteElementString("Id", stat.Id);
          writer.WriteElementString("CreatorId", stat.CreatorId);
          writer.WriteElementString("EventType", stat.EventType.ToString());
          writer.WriteElementString("Previous", stat.PreviousTweet);
          writer.WriteElementString("Current", stat.CurrentTweet);
          writer.WriteElementString("Next", stat.NextTweet);
          writer.WriteElementString("Date", stat.Date.ToString());
          writer.WriteEndElement();
        }

        writer.WriteEndElement();
        writer.WriteEndDocument();
        writer.Flush();
        writer.Close();
      }
    }

    private static void ReadOldStats()
    {
      try
      {
#if !SILVERLIGHT
        var xdoc =
          XDocument.Load(
            string.Format(
              "{0}/{1}/{2}/UserInteractions.xml", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Application.CompanyName, Application.ProductName));
#else
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    var isoStream = new IsolatedStorageFileStream("UserInteractions.xml",
                                                                  FileMode.Open, store);
                    XDocument xdoc = XDocument.Load(isoStream);
#endif
        //Parse the data received
        var result = (from user in xdoc.Descendants("Entry")
                      select
                        new Stats
                          {
                            Id = user.Element("Id").Value,
                            CreatorId = user.Element("CreatorId").Value,
                            UserId = user.Element("UserId").Value,
                            ServiceType = (EnumServiceType) Enum.Parse(typeof (EnumServiceType), user.Element("ServiceType").Value, true),
                            EventType = (EnumEventType) Enum.Parse(typeof (EnumEventType), user.Element("EventType").Value, true),
                            PreviousTweet = user.Element("Previous").Value,
                            CurrentTweet = user.Element("Current").Value,
                            NextTweet = user.Element("Next").Value,
                            Date = Convert.ToDateTime(user.Element("Date").Value)
                          }).ToList();
        foreach (var stat in result)
        {
          ListStats.Add(stat);
        }
#if SILVERLIGHT
                    isoStream.Close();
#endif
      }
#if SILVERLIGHT
            }
#endif
      catch (Exception e)
      {
        BLogManager.LogEntry("LoadFriendsList", e);
      }
    }

    public static void SendData(string url)
    {
#if !SILVERLIGHT
      var request = (HttpWebRequest) WebRequest.Create(url);
      request.Method = "POST";
      request.ContentType = "application/x-www-form-urlencoded";

      var xml = GetXmlStats(false);
      xml = HttpUtility.HtmlDecode(xml);
      HttpWebResponse response = null;
      try
      {
        var stream = request.GetRequestStream();
        var bytes = Encoding.UTF8.GetBytes(xml);
        stream.Write(bytes, 0, bytes.Length);
        stream.Flush();
        stream.Close();

        response = (HttpWebResponse) request.GetResponse();
      }
      catch (Exception ex)
      {
        BLogManager.LogEntry("FacteryHelper",ex);
      }
      if (response != null)
      {
        using (var reader = new StreamReader(response.GetResponseStream()))
        {
          try
          {
            var result = reader.ReadToEnd();
            ClearData();
          }
          catch (Exception ex)
          {
            BLogManager.LogEntry("StatsHelper", ex);
          }
        }
      }
#endif
    }
  }
}