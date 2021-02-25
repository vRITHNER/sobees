using System;
using System.Collections.Generic;
using System.Text;
using Sobees.Library.BGoogleLib.Json;

namespace Sobees.Library.BGoogleLib.Search
{
  public enum SearchResultType
  {
    Unknown,
    Web,
    Video,
    Local,
    Blog,
    News,
    Book,
    Image,
    Patent
  }

  public class SearchResult
  {
    SearchResultType _resultType;
    JsonObject _jsonObject;

    public string Url => GetStringAttribute("unescapedUrl");

    public string Title => GetStringAttribute("title");

    public string Content => GetStringAttribute("content");

    public SearchResultType ResultType => _resultType;

    public JsonObject JsonObject => _jsonObject;

    public SearchResult(JsonObject jsonObject)
    {
      _jsonObject = jsonObject;

      if (jsonObject.ContainsKey("GsearchResultClass"))
        _resultType = ParseSearchResultType(((JsonString)jsonObject["GsearchResultClass"]).Value);
    }

    protected string GetStringAttribute(string key)
    {
      return JsonHelper.GetJsonStringAsString(this.JsonObject, key);
    }

    public static SearchResult Parse(JsonObject jsonObject)
    {
      SearchResultType resultType = SearchResultType.Unknown;

      if (jsonObject.ContainsKey("GsearchResultClass"))
        resultType = ParseSearchResultType(((JsonString)jsonObject["GsearchResultClass"]).Value);

      switch (resultType)
      {
        case SearchResultType.Web:
          return new WebSearchResult(jsonObject);
        default:
          return new SearchResult(jsonObject);
      }
    }

    private static SearchResultType ParseSearchResultType(string str)
    {
      switch (str)
      {
        case "GwebSearch":
          return SearchResultType.Web;
        case "GvideoSearch":
          return SearchResultType.Video;
        case "GlocalSearch":
          return SearchResultType.Local;
        case "GblogSearch":
          return SearchResultType.Blog;
        case "GnewsSearch":
          return SearchResultType.News;
        case "GbookSearch":
          return SearchResultType.Book;
        case "GimageSearch":
          return SearchResultType.Image;
        case "GpatentSearch":
          return SearchResultType.Patent;
      }

      return SearchResultType.Unknown;
    }
  }

  public class WebSearchResult : SearchResult 
  {
    public string CacheUrl => GetStringAttribute("cacheUrl");

    public WebSearchResult(JsonObject jsonObject) : base(jsonObject)
    {
    }
  }
}