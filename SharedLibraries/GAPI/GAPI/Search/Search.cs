using System;
using System.Web;
using Sobees.Library.BGoogleLib.Core;
using Sobees.Library.BGoogleLib.Json;

namespace Sobees.Library.BGoogleLib.Search
{
  public enum SearchType
  {
    Web,
    Video,
    Local,
    Blog,
    News,
    Book,
    Image,
    Patent
  }

  public class Searcher
  {
    private const string SearchApiVersion = "1.0";
    private const string SearchBlogUrl = "http://ajax.googleapis.com/ajax/services/search/blogs?v={0}&q={1}";
    private const string SearchBookUrl = "http://ajax.googleapis.com/ajax/services/search/books?v={0}&q={1}";
    private const string SearchImageUrl = "http://ajax.googleapis.com/ajax/services/search/images?v={0}&q={1}";
    private const string SearchLocalUrl = "http://ajax.googleapis.com/ajax/services/search/local?v={0}&q={1}";
    private const string SearchNewsUrl = "http://ajax.googleapis.com/ajax/services/search/news?v={0}&q={1}";
    private const string SearchPatentUrl = "http://ajax.googleapis.com/ajax/services/search/patent?v={0}&q={1}";
    private const string SearchVideoUrl = "http://ajax.googleapis.com/ajax/services/search/video?v={0}&q={1}";
    private const string SearchWebUrl = "http://ajax.googleapis.com/ajax/services/search/web?v={0}&q={1}";

    public static SearchResults Search(string apiKey, SearchType searchType, string phrase)
    {
      return Search(apiKey, searchType, phrase, 0);
    }

    public static SearchResults Search(string apiKey, SearchType searchType, string phrase, int pageIndex)
    {
      string searchUrl;

      switch (searchType)
      {
        case SearchType.Web:
          searchUrl = SearchWebUrl;
          break;
        case SearchType.Video:
          searchUrl = SearchVideoUrl;
          break;
        case SearchType.Patent:
          searchUrl = SearchPatentUrl;
          break;
        case SearchType.News:
          searchUrl = SearchNewsUrl;
          break;
        case SearchType.Local:
          searchUrl = SearchLocalUrl;
          break;
        case SearchType.Image:
          searchUrl = SearchImageUrl;
          break;
        case SearchType.Book:
          searchUrl = SearchBookUrl;
          break;
        case SearchType.Blog:
          searchUrl = SearchBlogUrl;
          break;
        default:
          throw new NotSupportedException("SearchType: " + searchType);
      }

      return DoSearch(apiKey, phrase, searchUrl, pageIndex);
    }



    public static SearchResults Search(string apiKey, SearchType searchType, string phrase, int pageIndex, string HostLanguage, string WebLanguage, string EditionLanguage)
    {
      string searchUrl;

      switch (searchType)
      {
        case SearchType.Web:
          searchUrl = SearchWebUrl;
          break;
        case SearchType.Video:
          searchUrl = SearchVideoUrl;
          break;
        case SearchType.Patent:
          searchUrl = SearchPatentUrl;
          break;
        case SearchType.News:
          searchUrl = SearchNewsUrl;
          break;
        case SearchType.Local:
          searchUrl = SearchLocalUrl;
          break;
        case SearchType.Image:
          searchUrl = SearchImageUrl;
          break;
        case SearchType.Book:
          searchUrl = SearchBookUrl;
          break;
        case SearchType.Blog:
          searchUrl = SearchBlogUrl;
          break;
        default:
          throw new NotSupportedException("SearchType: " + searchType);
      }

      return DoSearch(apiKey, phrase, searchUrl, pageIndex, HostLanguage, WebLanguage, EditionLanguage);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="apiKey"></param>
    /// <param name="searchOptions">Use specific search options class: WebSearchOptions, VideoSearchOptions,
    /// LocalSearchOptions, BlogSearchOptions, NewsSearchOptions, BookSearchOptions, ImageSearchOptions, 
    /// PatentSearchOptions, etc...
    /// </param>
    /// <param name="phrase"></param>
    /// <returns></returns>
    public static SearchResults Search(string apiKey, SearchOptions searchOptions, string phrase)
    {
      return Search(apiKey, searchOptions, phrase, 0);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="searchOptions">Use specific search options class: WebSearchOptions, VideoSearchOptions,
    /// LocalSearchOptions, BlogSearchOptions, NewsSearchOptions, BookSearchOptions, ImageSearchOptions, 
    /// PatentSearchOptions, etc...
    /// <param name="phrase"></param>
    /// <param name="pageIndex"></param>
    /// <returns></returns>
    public static SearchResults Search(string apiKey, SearchOptions searchOptions, string phrase, int pageIndex)
    {
      if (searchOptions == null)
        throw new NullReferenceException("searchOptions");

      string searchUrl;

      if (searchOptions is WebSearchOptions)
        searchUrl = SearchWebUrl;
      else if (searchOptions is VideoSearchOptions)
        searchUrl = SearchVideoUrl;
      else if (searchOptions is PatentSearchOptions)
        searchUrl = SearchPatentUrl;
      else if (searchOptions is NewsSearchOptions)
        searchUrl = SearchNewsUrl;
      else if (searchOptions is LocalSearchOptions)
        searchUrl = SearchLocalUrl;
      else if (searchOptions is ImageSearchOptions)
        searchUrl = SearchImageUrl;
      else if (searchOptions is BookSearchOptions)
        searchUrl = SearchBookUrl;
      else if (searchOptions is BlogSearchOptions)
        searchUrl = SearchBlogUrl;
      else
        throw new NotSupportedException("SearchType: " + searchOptions.GetType());

      searchUrl += searchOptions.ToString();

      return DoSearch(apiKey, phrase, searchUrl, pageIndex);
    }


    private static SearchResults DoSearch(string apiKey, string phrase, string searchUrl, int pageIndex)
    {
      if (string.IsNullOrEmpty(phrase))
        return new SearchResults();

      string url = string.Format(
        searchUrl,
        SearchApiVersion,
        HttpUtility.UrlEncode(phrase));

      // Append parameters
      url += "&rsz=large&start=" + pageIndex;

      //API Key
      if (!string.IsNullOrEmpty(apiKey))
        url += "&key=" + apiKey;

      string responseData = CoreHelper.PerformRequest(url);
      JsonObject jsonObject = CoreHelper.ParseGoogleAjaxAPIResponse(responseData);

      // Translation response validation
      // Get 'responseData'
      JsonHelper.ValidateJsonField(jsonObject, "responseData", typeof (JsonObject));

      // Get 'translatedText'
      var responseContent = (JsonObject) jsonObject["responseData"];

      SearchResults searchResults = SearchResults.Parse(responseContent);

      return searchResults;
    }

    private static SearchResults DoSearch(string apiKey, string phrase, string searchUrl, int pageIndex, string HostLanguage, string WebLanguage, string EditionLanguage)
    {
      if (string.IsNullOrEmpty(phrase))
        return new SearchResults();

      string url = string.Format(
        searchUrl,
        SearchApiVersion,
        HttpUtility.UrlEncode(phrase));

      // Append parameters
      url += "&rsz=large&start=" + pageIndex;

      //API Key
      if (!string.IsNullOrEmpty(apiKey))
        url += "&key=" + apiKey;


      //Language
      if (!string.IsNullOrEmpty(HostLanguage))
        url += "&hl=" + HostLanguage;

      if (!string.IsNullOrEmpty(WebLanguage))
        url += "&lr=" + WebLanguage;

      if (!string.IsNullOrEmpty(EditionLanguage))
        url += "&ned=" + EditionLanguage;




      string responseData = CoreHelper.PerformRequest(url);
      JsonObject jsonObject = CoreHelper.ParseGoogleAjaxAPIResponse(responseData);

      // Translation response validation
      // Get 'responseData'
      JsonHelper.ValidateJsonField(jsonObject, "responseData", typeof(JsonObject));

      // Get 'translatedText'
      var responseContent = (JsonObject)jsonObject["responseData"];

      SearchResults searchResults = SearchResults.Parse(responseContent);

      return searchResults;
    }
  }
}