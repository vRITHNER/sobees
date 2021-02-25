using System;
using System.IO;
using System.Net;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Xml;

namespace UniveRss.HtmlConverter
{
  public class ConversionUtility
  {
    public static string ConvertUrlToHtmlText(string url)
    {
      try
      {
        var request = WebRequest.Create(new Uri(url)) as HttpWebRequest;
        var response = request.GetResponse() as HttpWebResponse;
        var reader = new StreamReader(response.GetResponseStream());
        string convertedText = reader.ReadToEnd();
        return convertedText;
      }
      catch (WebException)
      {
        return null;
      }
      catch (Exception)
      {
        return null;
      }
    }

    public static FlowDocument ConvertTextToFlowDocument(string text)
    {
      try
      {
        string xaml = HtmlToXamlConverter.ConvertHtmlToXaml(text,
                                                            true);
        return XamlReader.Load(new XmlTextReader(new StringReader(xaml))) as FlowDocument;
      }
      catch
      {
        return null;
      }
    }

    public static string ConvertHtmlToClearText(string html)
    {
      // Create well-formed Xml from Html string
      XmlElement htmlElement = HtmlParser.ParseHtml(html);

      //return innertext of the htmlElement;
      return htmlElement.InnerText;
    }
  }
}