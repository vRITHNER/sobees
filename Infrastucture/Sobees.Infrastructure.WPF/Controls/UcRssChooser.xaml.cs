using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Linq;
using Sobees.Infrastructure.Cls;
using Sobees.Library.BGenericLib;
using Sobees.Tools.Logging;
using Sobees.Tools.Threading.Extensions;

namespace Sobees.Infrastructure.Controls
{
  /// <summary>
  /// Interaction logic for UcRssChooser.xaml
  /// </summary>
  public partial class UcRssChooser
  {
    #region FeedTypeEnum enum

    public enum FeedTypeEnum
    {
      Unknown,
      RSS,
      Atom
    }

    #endregion

    public UcRssChooser()
    {
      RssSources = new DispatcherNotifiedObservableCollection<UrlCategory>();
      InitializeComponent();
      Loaded += UcRssChooserLoaded;
    }

    public DispatcherNotifiedObservableCollection<UrlCategory> RssSources { get; set; }
    private DispatcherNotifiedObservableCollection<string> _rssLanguages;
    public DispatcherNotifiedObservableCollection<string> RssLanguages
    {
      get { return _rssLanguages ?? (_rssLanguages = new DispatcherNotifiedObservableCollection<string>()); }
      set { _rssLanguages = value; }
    }
    private DispatcherNotifiedObservableCollection<FeedMetaData> _rssChoose;
    public DispatcherNotifiedObservableCollection<FeedMetaData> RssChoose
    {
        get { return _rssChoose ?? (_rssChoose = new DispatcherNotifiedObservableCollection<FeedMetaData>()); }
      set { _rssChoose = value; }
    }

    private void UcRssChooserLoaded(object sender, RoutedEventArgs e)
    {
      Loaded -= UcRssChooserLoaded;
      UpdateRss(Languages.GetStateFromDescription(SobeesSettingsLocator.SobeesSettingsStatic.Language));
    }

    private void UpdateRss(EnumLanguages lang)
    {
#if !SILVERLIGHT

      using (var worker = new BackgroundWorker())
      {
        worker.DoWork += delegate(object s,
                                  DoWorkEventArgs args)
                           {
                             if (worker.CancellationPending)
                             {
                               args.Cancel = true;
                               return;
                             }
                             try
                             {
                               var rssLanguages = new DispatcherNotifiedObservableCollection<EnumLanguages>();
                               
                               var results =
                                 GetSobeesRssUrls(new List<EnumLanguages> { lang },
                                                  out rssLanguages);
                               if (results == null || results.Count == 0) return;
                               Dispatcher.BeginInvokeIfRequired(() =>
                                                                  {
                                                                    try
                                                                    {
                                                                      foreach (UrlCategory urlCategory in results)
                                                                      {
                                                                        RssSources.Insert(0, urlCategory);
                                                                      }
                                                                      lstCategories.ItemsSource = RssSources;
                                                                      foreach (EnumLanguages urllang in
                                                                        rssLanguages.Where(urllang => !RssLanguages.Contains(Languages.GetStateDescription(urllang))))
                                                                      {
                                                                        RssLanguages.Insert(0, Languages.GetStateDescription(urllang));
                                                                      }
                                                                      cbxLanguage.ItemsSource = RssLanguages;
                                                                    }
                                                                    catch (Exception exception)
                                                                    {
                                                                      TraceHelper.Trace("UcRssChooserLoaded", exception);
                                                                    }
                                                                  });
                             }
                             catch (Exception exception)
                             {
                               TraceHelper.Trace("UcRssChooserLoaded", exception);
                             }
                           };
        worker.RunWorkerAsync();
      }
#endif

    }

    public static List<UrlCategory> GetSobeesRssUrls(List<EnumLanguages> langs,
                                                     out DispatcherNotifiedObservableCollection<EnumLanguages>
                                                       rssLanguages)
    {
      rssLanguages = new DispatcherNotifiedObservableCollection<EnumLanguages>();
      try
      {
        XDocument feedXML =
          XDocument.Load(@"http://sobeessql002.sobeesdata.com/bDules/MybDule/Rss/RssUrls.xml");
        List<EnumLanguages> EnumLang = (from url in feedXML.Descendants("Url")
                             let cat = url.Attribute("categoryId").Value
                             let urlLang =
                               ((EnumLanguages)
                                Enum.Parse(typeof(EnumLanguages), url.Attribute("lang").Value, true))
                                        select urlLang).Distinct().ToList();

        List<RssUrl> urls = (from url in feedXML.Descendants("Url")
                             let cat = url.Attribute("categoryId").Value
                             let urlLang =
                               ((EnumLanguages)
                                Enum.Parse(typeof (EnumLanguages), url.Attribute("lang").Value, true))
                             where langs.Contains(urlLang)
                             select new RssUrl
                                      {
                                        CategoryId = url.Attribute("categoryId").Value,
                                        CategoryName = url.Attribute("categoryName").Value,
                                        Name = url.Attribute("name").Value,
                                        Logo = url.Attribute("logo") != null
                                                 ? url.Attribute("logo").Value
                                                 : string.Empty,
                                        Lang = urlLang,
                                        Url = url.Attribute("url").Value,
                                      }).ToList();

        urls.OrderBy(url => url.Name);
        IEnumerable<IGrouping<string, RssUrl>> catUrl = urls.GroupBy(url => url.CategoryId);
        IEnumerable<IGrouping<EnumLanguages, RssUrl>> catlang = urls.GroupBy(url => url.Lang);

        foreach (var lang in EnumLang)
        {
          rssLanguages.Add(lang);
        }
        var cats = new List<UrlCategory>();

        foreach (var cat in catUrl)
        {
          var urlCategory = new UrlCategory
                              {
                                Name = cat.First().CategoryName,
                                Id = cat.First().CategoryId,
                                Urls = new List<RssUrl>()
                              };
          foreach (RssUrl url in cat)
          {
            urlCategory.Urls.Add(url);
          }
          urlCategory.Urls.OrderBy(url => url.Name);
          cats.Add(urlCategory);
        }
        return cats;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("GetSobeesRssUrls", ex);
      }
      return null;
    }

    private void Grid_Loaded(object sender, RoutedEventArgs e)
    {
      var lst = ucRssChooser.Tag as List<string>;
      if (lst != null)
        if (lst.Contains(((Button) ((Grid) sender).FindName("btnAddRss")).CommandParameter as string))
        {
          ((Button) ((Grid) sender).FindName("btnAddRss")).IsEnabled = false;
        }
    }

    private void BtnAddRssClick(object sender, RoutedEventArgs e)
    {
      ((Button) sender).IsEnabled = false;
    }

    private void CbxLanguageSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      RssSources.Clear();
      UpdateRss(Languages.GetStateFromDescription((string) cbxLanguage.SelectedItem));
    }

    #region Nested type: FeedSource

    public class FeedSource
    {
      public FeedSource(string name, string url)
      {
        Name = name;
        Url = url;
      }

      public string Name { get; set; }

      public string Url { get; set; }

      public string LongName => Name + " (" + Url + ")";
    }

    #endregion

    private void VerifiyUrl(object sender, RoutedEventArgs e)
    {
      RssChoose.Clear();
      isWaiting.IsAnimating = true;
      btnCheck.IsEnabled = false;
      var txt = txtUrl.Text;
#if !SILVERLIGHT

      using (var worker = new BackgroundWorker())
      {
        worker.DoWork += delegate(object s,
                                  DoWorkEventArgs args)
        {
          if (worker.CancellationPending)
          {
            args.Cancel = true;
            return;
          }
          try
          {
            Collection<FeedMetaData> allurl;
            var result = FeedUtil.DiscoverFeed(txt, out allurl);
            Dispatcher.BeginInvokeIfRequired(() =>
            {
              try
              {
                if(allurl!= null && allurl.Count > 1)
                {
                  foreach (var metaData in allurl)
                  {
                    RssChoose.Add(metaData);
                  }
                  lstItems2.ItemsSource = RssChoose;
                }
                else
                {
                  if (result.IsValid)
                  {

                    btnAddRss.Command.Execute(result.FeedUrl);
                    txtError.Text = string.Empty;
                  }
                  else
                  {
                    txtError.Text = "Error";

                  }
                }
                  
                isWaiting.IsAnimating = false;
                btnCheck.IsEnabled = true;


              }
              catch (Exception exception)
              {
                TraceHelper.Trace("UcRssChooserLoaded", exception);
              }
            });
          }
          catch (Exception exception)
          {
            TraceHelper.Trace("VerifiyUrl", exception);
          }
        };
        worker.RunWorkerAsync();
      }
#endif
    }
  }
  public enum EnumFeedType
  {
    RSS,
    ATOM,
    NONE,
  }
  public class LinkChecker
  {
    private const string PATTERN = "<link( [^>]*rel=\"{0}\"[^>]*)>";
    private static readonly Regex HREF = new Regex("href=\"(.+?)\"", RegexOptions.IgnoreCase | RegexOptions.Compiled);


    /// <summary>
    /// Finds the Feed links.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="html">The HTML.</param>
    /// <param name="s"></param>
    /// <returns></returns>
    public static Collection<FeedMetaData> FindLinks(string type, string html, string baseurl)
    {
      MatchCollection matches = Regex.Matches(html, string.Format(PATTERN, type),
                                              RegexOptions.IgnoreCase | RegexOptions.Singleline);

      var urls = new Collection<FeedMetaData>();
      EnumFeedType feedType = EnumFeedType.NONE;

      foreach (Match match in matches)
      {
        if (match.Groups.Count > 1)
        {
          string link = match.Groups[1].Value;
          Match hrefMatch = HREF.Match(link);
          if (link.ToLower().IndexOf("application/rss+xml") > -1)
            feedType = EnumFeedType.RSS;
          if (link.ToLower().IndexOf("application/atom+xml") > -1)
            feedType = EnumFeedType.ATOM;
          if (hrefMatch.Groups.Count > 1)
          {
            Uri url;
            string value = hrefMatch.Groups[1].Value;
              var title = GetTitle(match.ToString());

            if (Uri.TryCreate(value, UriKind.Absolute, out url))
            {
                              if(string.IsNullOrEmpty(title))
              {
                  title = url.AbsoluteUri;
              }
              var data = new FeedMetaData(feedType, null, true, url.AbsoluteUri,title);
              urls.Add(data);
            }
            else if (Uri.TryCreate(value, UriKind.Relative, out url))
            {
                if (string.IsNullOrEmpty(title))
                {
                    title = baseurl + url;
                }
                var data = new FeedMetaData(feedType, null, true, baseurl + url, title);
              urls.Add(data);
            }
          }
        }
      }
      return urls;
    }

      private static string GetTitle(string str)
      {
          if(str.Contains("title="))
          {
              try
              {
                  str = str.Substring(str.IndexOf("title=\"") + 7);
                  var strs = str.Split('"');
                  return strs[0];
              }
              catch (Exception ex)
              {
              }

          }
          return string.Empty;
      }
  }
  public class FeedUtil
  {
    public static FeedMetaData DiscoverFeed(string url, out Collection<FeedMetaData> allurl)
    {
      allurl = null;
      if(!url.Contains("http"))
      {
        url = $"http://{url}";
      }
      var feedMetaData = new FeedMetaData();
      var wc = new WebClient();
      WebClient wc2 = null;
      bool IsFeed = false;
      bool IsHTML = true;
      string sDoc = null;
      try
      {
        sDoc = wc.DownloadString(url);
        // is it Xml? then it's a feed, so load and get the feed type
        if (sDoc.IndexOf("<?xml") > -1)
        {
          //  BOM fix -chop off first 3 chars if present
          sDoc = sDoc.Substring(sDoc.IndexOf("<?xml"));
          if (sDoc.IndexOf("<rss") > -1)
          {
            IsFeed = true;
            IsHTML = false;
            feedMetaData.IsValid = true;
            feedMetaData.FeedType = EnumFeedType.RSS;
            feedMetaData.FeedContent = new XmlDocument();
            feedMetaData.FeedContent.LoadXml(sDoc);
            feedMetaData.FeedUrl = url;
          }
          else if (sDoc.IndexOf("<feed") > -1)
          {
            IsFeed = true;
            IsHTML = false;
            feedMetaData.IsValid = true;
            feedMetaData.FeedType = EnumFeedType.ATOM;
            feedMetaData.FeedContent = new XmlDocument();
            feedMetaData.FeedContent.LoadXml(sDoc);
            feedMetaData.FeedUrl = url;
          }
        }
        else
        {
          IsFeed = false;
          IsHTML = true;
          // find the rel link and get the actual feed if any
          allurl = LinkChecker.FindLinks("alternate", sDoc, url);
          if (allurl.Count > 0)
            feedMetaData.FeedUrl = allurl[0].FeedUrl;
          if (feedMetaData.FeedUrl == null)
            feedMetaData.IsValid = false;
          else
          {
            wc2 = new WebClient();
            sDoc = wc2.DownloadString(feedMetaData.FeedUrl);
            feedMetaData.FeedContent = new XmlDocument();
            //  BOM fix -chop off first 3 chars if present
            sDoc = sDoc.Substring(sDoc.IndexOf("<?xml"));
            feedMetaData.FeedContent.LoadXml(sDoc);
            feedMetaData.IsValid = true;
            feedMetaData.FeedType = allurl[0].FeedType;
          }
        }
      }

      catch (Exception ex)
      {
        feedMetaData.IsValid = false;
      }
      finally
      {
        wc.Dispose();
        if (wc2 != null)
          wc2.Dispose();
      }
      
      return feedMetaData;
    }
  }
public class FeedMetaData
  {
    public FeedMetaData()
    {
    }

    public FeedMetaData(EnumFeedType feedType, XmlDocument content, bool isValid, string feedUrl,string title)
    {
      FeedType = feedType;
      FeedContent = content;
      IsValid = isValid;
      FeedUrl = feedUrl;
        Title = title;
    }

    public EnumFeedType FeedType { get; set; }
    public XmlDocument FeedContent { get; set; }
    public string FeedUrl { get; set; }
    public string Title { get; set; }
    public bool IsValid { get; set; }
  }
  public class RssUrl
  {
    public RssUrl()
    {
    }

    public RssUrl(string url, EnumLanguages lang, bool isPersistentUrl)
    {
      Url = url;
      Lang = lang;
      IsPersistentUrl = isPersistentUrl;
    }

    public string Logo { get; set; }
    public string Name { get; set; }

    public string Url { get; set; }

    public EnumLanguages Lang { get; set; }


    public bool IsPersistentUrl { get; set; }

    public string CategoryId { get; set; }


    public string CategoryName { get; set; }


    public override bool Equals(object obj)
    {
      return ((RssUrl) obj).Url.Equals(Url);
    }
  }

  public class UrlCategory
  {
    public string Name { get; set; }

    public string Id { get; set; }

    public List<RssUrl> Urls { get; set; }
  }
}