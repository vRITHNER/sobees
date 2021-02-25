namespace Sobees.Library.BFacebookLibV2.Json
{
  #region

  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Globalization;
  using System.IO;
  using System.Linq;
  using Sobees.Library.BFacebookLibV2.Cls;

  #endregion

  public class JsonObjectEx : IJsonObject
  {
    #region Properties

    public IDictionary<string, object> Dictionary { get; }

    public string[] Keys => Dictionary.Keys.ToArray();

    #endregion

    #region Constructors

    public JsonObjectEx(IDictionary<string, object> dictionary)
    {
      Dictionary = dictionary;
    }

    public JsonObjectEx()
    {
      Dictionary = new Dictionary<string, object>();
    }

    #endregion

    #region Member methods

    public bool HasValue(string name)
    {
      return Dictionary.ContainsKey(name) && Dictionary[name] != null;
    }

    public JsonObjectEx GetObject(string name)
    {
      object value;
      return Dictionary.TryGetValue(name, out value) && value is Dictionary<string, object> ? new JsonObjectEx((Dictionary<string, object>) value) : null;
    }

    public T GetObject<T>(string name, Func<JsonObjectEx, T> func)
    {
      var obj = GetObject(name);
      return obj == null ? default(T) : func(obj);
    }

    public JsonArrayEx GetArray(string name)
    {
      object value;
      return Dictionary.TryGetValue(name, out value) && value is ArrayList ? new JsonArrayEx((ArrayList) value) : null;
    }

    public T[] GetArray<T>(string name)
    {
      object value;
      var array = Dictionary.TryGetValue(name, out value) && value is ArrayList ? new JsonArrayEx((ArrayList) value) : null;
      return array?.Cast<T>();
    }

    public T[] GetArray<T>(string name, Func<JsonObjectEx, T> func)
    {
      var array = GetArray(name);
      return array?.ParseMultiple(func);
    }

    /// <summary>
    ///   Gets an array of strings.
    /// </summary>
    /// <param name="name">The name of the property holding the array.</param>
    public string[] GetStringArray(string name)
    {
      var array = GetArray(name);
      return array?.For((arr, index) => arr.GetString(index));
    }

    /// <summary>
    ///   Gets an array of 32-bit integers (<code>int</code>).
    /// </summary>
    /// <param name="name">The name of the property holding the array.</param>
    public int[] GetInt32Array(string name)
    {
      var array = GetArray(name);
      return array?.For((arr, index) => arr.GetInt32(index));
    }

    /// <summary>
    ///   Gets an array of 64-bit integers (<code>long</code>).
    /// </summary>
    /// <param name="name">The name of the property holding the array.</param>
    public long[] GetInt64Array(string name)
    {
      var array = GetArray(name);
      return array?.For((arr, index) => arr.GetInt64(index));
    }

    /// <summary>
    ///   Gets an array of floating point values (<code>float</code>).
    /// </summary>
    /// <param name="name">The name of the property holding the array.</param>
    public float[] GetFloatArray(string name)
    {
      var array = GetArray(name);
      return array?.For((arr, index) => arr.GetFloat(index));
    }

    /// <summary>
    ///   Gets an array of doubles.
    /// </summary>
    /// <param name="name">The name of the property holding the array.</param>
    public double[] GetDoubleArray(string name)
    {
      var array = GetArray(name);
      return array?.For((arr, index) => arr.GetDouble(index));
    }

    public T GetValue<T>(string name)
    {
      return GetValue<T>(name, CultureInfo.InvariantCulture);
    }

    public T GetValue<T>(string name, IFormatProvider provider)
    {
      if (!HasValue(name)) return default(T);
      if (typeof (T) != typeof (JsonObjectEx)) return (T) Convert.ChangeType(Dictionary[name], typeof (T), provider);
      var obj = Dictionary[name];
      if (obj is Dictionary<string, object>)
      {
        obj = new JsonObjectEx((Dictionary<string, object>) Dictionary[name]);
      }
      return (T) obj;
    }

    public int GetInt32(string name)
    {
      return GetValue<int>(name);
    }

    public long GetInt64(string name)
    {
      return GetValue<long>(name);
    }

    public float GetFloat(string name)
    {
      return GetValue<float>(name);
    }

    public float GetFloat(string name, IFormatProvider provider)
    {
      return GetValue<float>(name, provider);
    }

    public double GetDouble(string name)
    {
      return GetValue<double>(name);
    }

    public double GetDouble(string name, IFormatProvider provider)
    {
      return GetValue<double>(name, provider);
    }

    public bool GetBoolean(string name)
    {
      return GetValue<bool>(name);
    }

    public string GetString(string name)
    {
      return GetValue<string>(name);
    }

    public DateTime GetDateTime(string name)
    {
      return GetValue<DateTime>(name);
    }

    public DateTime GetDateTimeFromUnixTimestamp(string name)
    {
      return SocialUtils.GetDateTimeFromUnixTime(GetValue<int>(name));
    }

    public DateTimeOffset GetDateTimeOffset(string name)
    {
      return GetValue<DateTimeOffset>(name);
    }

    public bool IsObject(string name)
    {
      return HasValue(name) && Dictionary[name] is IDictionary<string, object>;
    }

    public bool IsArray(string name)
    {
      return HasValue(name) && Dictionary[name] is ArrayList;
    }

    public void GetLong(string name, long value)
    {
      Dictionary[name] = value;
    }

    public void GetDouble(string name, double value)
    {
      Dictionary[name] = value;
    }

    public void GetBoolean(string name, bool value)
    {
      Dictionary[name] = value;
    }

    public T GetEnum<T>(string name) where T : struct
    {
      return SocialUtils.ParseEnum<T>(GetString(name));
    }

    public T GetEnum<T>(string name, T fallback) where T : struct
    {
      return SocialUtils.ParseEnum(GetString(name), fallback);
    }

    public void SetInt(string name, int value)
    {
      Dictionary[name] = value;
    }

    public void SetString(string name, string value)
    {
      Dictionary[name] = value;
    }

    public void SetObject(string name, JsonObjectEx value)
    {
      Dictionary[name] = value?.Dictionary;
    }

    public void SetArray(string name, JsonArrayEx value)
    {
      Dictionary[name] = value?.InternalArray;
    }

    public void SetNull(string name)
    {
      Dictionary[name] = null;
    }

    /// <summary>
    ///   Gets a JSON string representing the object.
    /// </summary>
    public string ToJson()
    {
      return JsonConverter.ToJson(this);
    }

    /// <summary>
    ///   Save the object to a JSON file at the specified <code>path</code>.
    /// </summary>
    /// <param name="path">The path to save the file.</param>
    public void SaveJson(string path)
    {
      File.WriteAllText(path, ToJson());
    }

    #endregion

    #region Static methods

    /// <summary>
    ///   Loads an instance of <code>JsonObject</code> from the JSON file at the specified <code>path</code>.
    /// </summary>
    /// <param name="path">The path to the file.</param>
    public static JsonObjectEx LoadJson(string path)
    {
      return JsonConverter.ParseObject(File.ReadAllText(path));
    }

    /// <summary>
    ///   Loads an instance of <code>T</code> from the JSON file at the specified <code>path</code>.
    /// </summary>
    /// <param name="path">The path to the file.</param>
    /// <param name="func">The function used to parse the object.</param>
    public static T LoadJson<T>(string path, Func<JsonObjectEx, T> func)
    {
      return JsonConverter.ParseObject(File.ReadAllText(path), func);
    }

    /// <summary>
    ///   Gets an instance of <code>JsonObject</code> from the specified JSON string.
    /// </summary>
    /// <param name="json">The JSON string representation of the object.</param>
    public static JsonObjectEx ParseJson(string json)
    {
      return JsonConverter.ParseObject(json);
    }

    /// <summary>
    ///   Gets an instance of <code>T</code> from the specified JSON string.
    /// </summary>
    /// <param name="json">The JSON string representation of the object.</param>
    /// <param name="func">The function used to parse the object.</param>
    public static T ParseJson<T>(string json, Func<JsonObjectEx, T> func)
    {
      return JsonConverter.ParseObject(json, func);
    }

    #endregion
  }
}