namespace Sobees.Library.BFacebookLibV2.Json
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Web.Script.Serialization;

  public class JsonConverter : JavaScriptConverter
  {

    public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
    {

      if (dictionary == null)
        throw new ArgumentNullException(nameof(dictionary));

      if (type == typeof(object[]))
      {
        return new JsonObjectEx(dictionary);
      }

      if (type == typeof(object))
      {
        return new JsonObjectEx(dictionary);
      }

      return null;
    }

    public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
    {
      throw new NotImplementedException();
    }

    public override IEnumerable<Type> SupportedTypes => new ReadOnlyCollection<Type>(new List<Type>(new[] { typeof(object) }));

    /// <summary>
    /// Returns an instance of <code>IJsonObject</code>, which can either be either an instance of
    /// <code>JsonObject</code> or an instance of <code>JsonArray</code> depending on the specified JSON string.
    /// <code>NULL</code> may be returned if the parsed value cannot be converted to either of these two types.
    /// </summary>
    /// <param name="json">The JSON string to parsed.</param>
    public static IJsonObject Parse(string json)
    {

      // Setup the serializer
      JavaScriptSerializer jss = new JavaScriptSerializer();
      jss.RegisterConverters(new JavaScriptConverter[] { new JsonConverter() });

      // Deserialize the specified JSON string
      object deserialized = jss.Deserialize(json, typeof(object));

      // Handle arrays
      if (deserialized is object[])
      {
        return new JsonArrayEx((object[])deserialized);
      }

      // Return the JSON object
      return deserialized as JsonObjectEx;

    }

    /// <summary>
    /// Returns an instance of <code>JsonObject</code> based on the specified JSON string. <code>NULL</code> may be
    /// returned if the parsed value cannot be converted to an instance of <code>JsonObject</code>.
    /// </summary>
    /// <param name="json">The JSON string to parsed.</param>
    public static JsonObjectEx ParseObject(string json)
    {
      return Parse(json) as JsonObjectEx;
    }

    /// <summary>
    /// Returns an instance of <code>JsonArray</code> based on the specified JSON string. <code>NULL</code> may be
    /// returned if the parsed value cannot be converted to an instance of <code>JsonArray</code>.
    /// </summary>
    /// <param name="json">The JSON string to parsed.</param>
    public static JsonArrayEx ParseArray(string json)
    {
      return Parse(json) as JsonArrayEx;
    }

    public static T ParseObject<T>(string json, Func<JsonObjectEx, T> parse)
    {
      JsonObjectEx obj = ParseObject(json);
      return obj == null ? default(T) : parse(obj);
    }

    public static T[] ParseArray<T>(string json, Func<JsonArrayEx, T[]> parse)
    {
      var array = ParseArray(json);
      return array == null ? null : parse(array);
    }

    public static string ToJson(IJsonObject obj)
    {

      object value = null;

      if (obj is JsonArrayEx)
      {
        value = ToJsonInternal((JsonArrayEx)obj);
      }
      else if (obj is JsonObjectEx)
      {
        value = ToJsonInternal((JsonObjectEx)obj);
      }

      return new JavaScriptSerializer().Serialize(value);

    }

    private static object ToJsonInternal(JsonObjectEx obj)
    {

      Dictionary<string, object> temp = new Dictionary<string, object>();

      foreach (string key in obj.Dictionary.Keys)
      {
        if (obj.IsObject(key))
        {
          temp.Add(key, ToJsonInternal(obj.GetObject(key)));
        }
        else if (obj.IsArray(key))
        {
          temp.Add(key, ToJsonInternal(obj.GetArray(key)));
        }
        else
        {
          temp.Add(key, obj.Dictionary[key]);
        }
      }

      return temp;

    }

    public static object ToJsonInternal(JsonArrayEx array)
    {

      object[] temp = new object[array.Length];

      for (int i = 0; i < array.Length; i++)
      {
        if (array.IsObject(i))
        {
          temp[i] = ToJsonInternal(array.GetObject(i));
        }
        else if (array.IsArray(i))
        {
          temp[i] = ToJsonInternal(array.GetArray(i));
        }
        else
        {
          temp[i] = array.InternalArray[i];
        }
      }

      return temp;

    }

  }

}