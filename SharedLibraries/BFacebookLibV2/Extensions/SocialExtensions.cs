namespace Sobees.Library.BFacebookLibV2.Extensions
{
  #region

  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;
  using System.IO;
  using System.Linq;
  using System.Net;
  using System.Web;
  using Sobees.Library.BFacebookLibV2.Cls;
  using Sobees.Library.BFacebookLibV2.Interfaces;
  using Sobees.Library.BFacebookLibV2.Json;
  using Sobees.Library.BFacebookLibV2.Objects.Feed;

  #endregion

  public static class SocialExtensions
  {
    /// <summary>
    ///   Serializes the specified collection of <code>SocialJsonObject</code> to a raw JSON
    ///   string.
    /// </summary>
    /// <param name="collection">The collection to be serialized.</param>
    public static string ToJson(this IEnumerable<SocialJsonObject> collection)
    {
      if (collection == null) return null;
      var array = (from item in collection select (object) item.JsonObjectEx).ToArray();
      return new JsonArrayEx(array).ToJson();
    }

    /// <summary>
    ///   Serializes and saves the specified collection of <code>SocialJsonObject</code> to a raw
    ///   JSON string.
    /// </summary>
    /// <param name="collection">The collection to be serialized.</param>
    /// <param name="path">The path to the file.</param>
    public static void SaveJson(this IEnumerable<SocialJsonObject> collection, string path)
    {
      if (collection == null) return;
      var array = (from item in collection select (object) item.JsonObjectEx).ToArray();

      new JsonArrayEx(array).SaveJson(path);
    }

    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> dataArray) where  T: class
    {
      var collection = new ObservableCollection<T>(dataArray);
      return collection;
    }
    /// <summary>
    /// To the json object ex.
    /// </summary>
    /// <param name="objectValue">The object value.</param>
    /// <returns>Sobees.Library.BFacebookLibV2.Json.JsonObjectEx.</returns>
    public static JsonObjectEx ToJsonObjectEx(this object objectValue)
    {
      if (string.IsNullOrEmpty(objectValue?.ToString())) return null;
      var obj = JsonObjectEx.ParseJson(objectValue.ToString());
      return obj;
    }

    /// <summary>
    ///   Calculates the distance in meters between two GPS locations.
    /// </summary>
    /// <param name="loc1">The first location.</param>
    /// <param name="loc2">The second location.</param>
    public static double GetDistance(this ILocation loc1, ILocation loc2)
    {
      return SocialUtils.GetDistance(loc1, loc2);
    }

    public static string GetAsString(this HttpWebResponse response)
    {
      if (response == null) return null;
      using (var stream = response.GetResponseStream())
      {
        if (stream == null) return null;
        using (var reader = new StreamReader(stream))
        {
          return reader.ReadToEnd();
        }
      }
    }

    public static IJsonObject GetAsJson(this HttpWebResponse response)
    {
      var stream = response.GetResponseStream();
      return stream == null ? null : JsonConverter.Parse(new StreamReader(stream).ReadToEnd());
    }

    public static JsonObjectEx GetAsJsonObject(this HttpWebResponse response)
    {
      return GetAsJson(response) as JsonObjectEx;
    }

    public static JsonArrayEx GetAsJsonArray(this HttpWebResponse response)
    {
      return GetAsJson(response) as JsonArrayEx;
    }

    public static void AppendQueryString(this UriBuilder builder, NameValueCollection values)
    {
      if (values == null || values.Count == 0) return;
      var nvc = HttpUtility.ParseQueryString(builder.Query);
      nvc.Add(values);
      builder.Query = SocialUtils.NameValueCollectionToQueryString(nvc);
    }

    public static UriBuilder MergeQueryString(this UriBuilder builder, NameValueCollection values)
    {
      if (values == null || values.Count == 0) return builder;
      builder.Query = SocialUtils.NameValueCollectionToQueryString(HttpUtility.ParseQueryString(builder.Query).Set(values));
      return builder;
    }

    public static NameValueCollection Set(this NameValueCollection subject, NameValueCollection values)
    {
      if (values == null || values.Count == 0) return subject;
      foreach (var key in values.AllKeys)
      {
        subject.Set(key, values[key]);
      }
      return subject;
    }
  }
}