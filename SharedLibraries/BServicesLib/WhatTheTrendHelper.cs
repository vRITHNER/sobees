using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using Sobees.Library.BGenericLib;
using Sobees.Tools.Logging;

namespace Sobees.Library.BServicesLib
{
  public class WhatTheTrendHelper
  {
    public static List<Entry> GetTrends(WebProxy proxy)
    {
      const string requestUrl = "http://whatthetrend.com/api/trend/listAll/xml";
      var request = (HttpWebRequest) WebRequest.Create(requestUrl);
      if (proxy != null)
      {
        request.Proxy = proxy;
      }
      try
      {
        using (var reader = new StreamReader((request.GetResponse()).GetResponseStream()))
        {
          string data = reader.ReadToEnd();
          XNamespace xn = "";
          XDocument xdoc = XDocument.Parse(data);
          List<Entry> result = (from results in xdoc.Descendants(xn + "trend")
                                select new Entry
                                         {
                                           Title = results.Element(xn + "name").Value,
                                           UpdateDate =
                                             DateTime.Parse(
                                             results.Element(xn + "dates").Element(xn + "lastTrend").Value),
                                           PubDate =
                                             DateTime.Parse(
                                             results.Element(xn + "dates").Element(xn + "firstTrend").Value),
                                           OrigLink = results.Element(xn + "links").Element(xn + "url").Value,
                                           Type = EnumType.WhatTheTrend,
                                         }).ToList();
          return result;
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("GenericLib.Helpers.OneRiotHelper", ex);
      }

      return new List<Entry>();
    }
  }
}