#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using BUtility;
using HtmlAgilityPack;
using Sobees.Tools.Extensions;
using Sobees.Tools.Logging;

#endregion

namespace Sobees.Tools.Helpers
{
  /**
  * The HtmlToText class is a simple helper for converting HTML to plain text.
  */

  /// <summary>
  ///   Utility helper for html.
  /// </summary>
  public class HtmlHelper
  {
    private const string APPNAME = "HtmlHelper";

    /// <summary>
    ///   Compiled regular expression for performance.
    /// </summary>
    private static readonly Regex HtmlRegex = new Regex("<.*?>", RegexOptions.Compiled);

    /// <summary>
    ///   remove all balize in htmlstring
    /// </summary>
    /// <param name = "html"></param>
    /// <returns></returns>
    public static string CleanContent(string html)
    {
      return Regex.Replace(
        html.Replace("<br ?/?>", "\n").Replace("<BR ?/?>", "\n").Replace("</p ?>", "\n\n").Replace("</P ?>", "\n\n"), "</?[^ap].*?>", String.Empty, RegexOptions.Compiled);
    }

    /// <summary>
    ///   Remove all html
    /// </summary>
    /// <param name = "html"></param>
    /// <returns></returns>
    public static string CleanHtml(string html)
    {
      var doc = new HtmlDocument();
      doc.LoadHtml(html);

      var res = "";
      try
      {
        if (doc.DocumentNode.SelectNodes("//*[name()='p' or name()='h1' or name()='h2' or name()='h3']") != null)
        {
          res = doc.DocumentNode.SelectNodes("//*[name()='p' or name()='h1' or name()='h2' or name()='h3']").Aggregate(
            res,
            (current,
             node) => string.Format("{0}{1}", current, ("<" + node.Name + ">" + CleanContent(node.InnerHtml.Trim()) + "</" + node.Name + ">\r\n")));
        }
        else
        {
          return html;
        }
      }
      catch (NullReferenceException)
      {
        return "";
      }
      catch (Exception ex)
      {
      }

      return res;
    }


    /// <summary>
    ///   Helping method to convert Html to plain text.@param .
    /// </summary>
    /// <param name = "html">html is the Html which will be convert</param>
    /// <returns>the html as plain text</returns>
    public static string ConvertHtml(string html)
    {
      var doc = new HtmlDocument();
      if (html != null)
        doc.LoadHtml(html);

      var sw = new StringWriter();
      ConvertTo(doc.DocumentNode, sw);
      sw.Flush();
      return sw.ToString();
    }

    private static void ConvertContentTo(HtmlNode node,
                                         TextWriter outText)
    {
      foreach (var subnode in node.ChildNodes)
      {
        ConvertTo(subnode, outText);
      }
    }

    private static void ConvertTo(HtmlNode node,
                                  TextWriter outText)
    {
      string html;
      switch (node.NodeType)
      {
        case HtmlNodeType.Comment:
          // don't output comments
          break;

        case HtmlNodeType.Document:
          ConvertContentTo(node, outText);
          break;

        case HtmlNodeType.Text:
          // script and style must not be output
          var parentName = node.ParentNode.Name;
          if ((parentName == "script") || (parentName == "style"))
            break;

          // get text
          html = ((HtmlTextNode) node).Text;

          // is it in fact a special closing node output as text?
          if (HtmlNode.IsOverlappedClosingElement(html))
            break;

          // check the text is meaningful and not a bunch of whitespaces
          if (html.Trim().Length > 0)
          {
            outText.Write(HtmlEntity.DeEntitize(html));
          }
          break;

        case HtmlNodeType.Element:
          switch (node.Name)
          {
            case "p":
              // treat paragraphs as crlf
              outText.Write("\r\n\r\n");
              break;
            case "br":
              // treat paragraphs as crlf
              outText.Write("\r\n");
              break;
          }

          if (node.HasChildNodes)
          {
            ConvertContentTo(node, outText);
          }
          break;
      }
    }

    /// <summary>
    ///   Extract all urls as List
    /// </summary>
    /// <param name = "str"></param>
    /// <returns></returns>
    public static List<string> ExtractURLsAsList(string str)
    {
      // match.Groups["name"].Value - URL Name
      // match.Groups["url"].Value - URI
      const string regexPattern = @"(https?|ftp|file)://[-A-Z0-9+&@#/%?=~_|!:,.;]*[-A-Z0-9+&@#/%=~_|]";

      // Find matches.
      var matches = Regex.Matches(str, regexPattern, RegexOptions.IgnoreCase);

      return (from Match match in matches select match.Groups[0].Value.Contains("utm_source") ? match.Groups[0].Value.Split('?').First() : match.Groups[0].Value).ToList();
    }

    /// <summary>
    ///   try if word is an url
    /// </summary>
    /// <param name = "word"></param>
    /// <returns></returns>
    public static bool IsHyperlink(string word)
    {
      try
      {
        const string strRegex = @"\b(((\S+)?)(@|mailto\:|(news|(ht|f)tp(s?))\://)\S+)\b";
        const RegexOptions myRegexOptions =
          RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant;
        var myRegex = new Regex(strRegex, myRegexOptions);

        return myRegex.Matches(word).Count > 0;

        //new Uri(word);
        //return true;
      }
      catch (Exception ex)
      {
        BLogManager.LogEntry(APPNAME + "::IsHyperlink", ex);
      }
      return false;
    }

    /// <summary>
    ///   Extract urls as List
    /// </summary>
    /// <param name = "str"></param>
    /// <returns></returns>
    public static string[] ExtractURLs(string str)
    {
      // match.Groups["name"].Value - URL Name
      // match.Groups["url"].Value - URI
      const string regexPattern = @"<a.*?href=[""'](?<url>.*?)[""'].*?>(?<name>.*?)</a>";

      // Find matches.
      var matches = Regex.Matches(str, regexPattern, RegexOptions.IgnoreCase);

      var matchList = new string[matches.Count];

      // Report on each match.
      var c = 0;
      foreach (Match match in matches)
      {
        matchList[c] = match.Groups["url"].Value;
        c++;
      }

      return matchList;
    }


    /// <summary>
    ///   Return the "real" url from a short one
    /// </summary>
    /// <param name = "shortUrl"></param>
    /// <returns></returns>
    public static string DecodeShortUrl(string shortUrl)
    {
      return shortUrl.ToRealShortUrl();
    }


    /// <summary>
    ///   Ensure url for image. For example an image with url ".../toto.jpg" will be change into "http://www...../toto.jpg
    /// </summary>
    /// <param name = "imageurl"></param>
    /// <param name = "pageUrl"></param>
    /// <returns></returns>
    public static string EnsureUrlForImage(string imageurl,
                                           string pageUrl)
    {
      var imageUrl = imageurl;
      try
      {
        if (!imageUrl.Contains("http") && imageUrl.Split('/').Count() > 1 && imageUrl.Split('/')[0].Contains('.'))
        {
          imageUrl = "http://" + imageUrl;
        }
        if (Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
        {
          return imageUrl;
        }
        imageUrl = imageurl;
        imageUrl = imageUrl.Replace('\\', '/');
        pageUrl = pageUrl.Replace('\\', '/');
        if (imageUrl.StartsWith("./"))
        {
          imageUrl = imageUrl.Remove(0, 2);
        }
        if (imageUrl.StartsWith("/"))
        {
          imageUrl = imageUrl.Remove(0, 1);
        }
        var baseurl = pageUrl.Substring(0, pageUrl.LastIndexOf('/'));
        while (imageUrl.StartsWith("../"))
        {
          baseurl = pageUrl.Substring(0, pageUrl.LastIndexOf('/'));
          imageUrl = imageUrl.Remove(0, 3);
        }
        if (Uri.IsWellFormedUriString(baseurl + '/' + imageUrl, UriKind.Absolute))
        {
          return baseurl + '/' + imageUrl;
        }
      }
      catch (Exception ex)
      {
        return imageurl;
      }
      return imageurl;
    }
  }
}