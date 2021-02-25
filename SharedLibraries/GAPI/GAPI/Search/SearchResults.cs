using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sobees.Library.BGoogleLib.Json;

namespace Sobees.Library.BGoogleLib.Search
{
  public class SearchResults
  {
    List<SearchResult> _items;
    long _estimatedResultCount;
    string _moreResultsUrl;
    int _currentPageIndex;
    List<int> _optionalPages;

    public int[] Pages => (int[])_optionalPages.ToArray();

    public SearchResult[] Items => (SearchResult[])_items.ToArray();

    public int CurrentPageIndex => _currentPageIndex;

    public long EstimatedResultCount => _estimatedResultCount;

    internal SearchResults()
    {
      _items = new List<SearchResult>();
      _optionalPages = new List<int>();

      _estimatedResultCount = 0;
      _currentPageIndex = 0;
      _moreResultsUrl = null;
    }

    private void Add(SearchResult searchResult)
    {
      _items.Add(searchResult);
    }

    public static SearchResults Parse(JsonObject responseContent)
    {
      SearchResults searchResults = new SearchResults();

      // Parse cursor
      JsonHelper.ValidateJsonField(responseContent, "cursor", typeof(JsonObject));
      JsonObject cursor = (JsonObject)responseContent["cursor"];

      searchResults._estimatedResultCount = long.Parse(JsonHelper.GetJsonStringAsString(cursor, "estimatedResultCount", "0"));
      searchResults._currentPageIndex = int.Parse(JsonHelper.GetJsonStringAsString(cursor, "currentPageIndex", "0"));
      searchResults._moreResultsUrl = JsonHelper.GetJsonStringAsString(cursor, "moreResultsUrl");

      if (cursor.ContainsKey("pages") == true)
      {
        JsonHelper.ValidateJsonField(cursor, "pages", typeof(JsonArray));
        JsonArray pages = (JsonArray)cursor["pages"];
        foreach (JsonValue pageValue in pages.Items)
        {
          JsonObject pageValueObject = (JsonObject)pageValue;
          JsonHelper.ValidateJsonField(pageValueObject, "start", typeof(JsonString));

          searchResults._optionalPages.Add(
            int.Parse(JsonHelper.GetJsonStringAsString(pageValueObject, "start", "-1")));
        }
      }

      // Parse results
      JsonHelper.ValidateJsonField(responseContent, "results", typeof(JsonArray));
      JsonArray jsonResults = (JsonArray)(responseContent["results"]);

      foreach (JsonValue jsonResult in jsonResults.Items)
      {
        SearchResult searchResult = SearchResult.Parse((JsonObject)jsonResult);
        searchResults.Add(searchResult);
      }

      return searchResults;
    }
  }
}