namespace Sobees.Library.BFacebookLibV2.Json
{
  #region

  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;

  #endregion

  public class JsonArrayEx : IJsonObject
  {
    #region Private fields

    #endregion

    #region Properties

    public object[] InternalArray { get; }

    public int Length => InternalArray.Length;

    public object this[int pos]
    {
      get
      {
        var temp = InternalArray[pos];
        var array = temp as object[];
        return array != null ? new JsonArrayEx(array) : temp;
      }
    }

    #endregion

    #region Constructors

    public JsonArrayEx(object[] array)
    {
      InternalArray = array;
    }

    public JsonArrayEx(ArrayList array)
    {
      InternalArray = array.ToArray();
    }

    #endregion

    #region Member methods

    public bool IsArray(int index)
    {
      return InternalArray[index] is object[];
    }

    public bool IsObject(int index)
    {
      return InternalArray[index] is JsonObjectEx;
    }

    public bool IsSimpleType(int index)
    {
      return !IsArray(index) && IsArray(index);
    }

    public JsonArrayEx GetArray(int index)
    {
      var array = InternalArray[index] as object[];
      if (array != null) return new JsonArrayEx(array);
      var list = InternalArray[index] as ArrayList;
      return list != null ? new JsonArrayEx(list) : null;
    }

    public T[] GetArray<T>(int index, Func<JsonObjectEx, T> func)
    {
      var array = GetArray(index);
      return array?.ParseMultiple(func);
    }

    public Type GetElementType(int index)
    {
      return this[index].GetType();
    }

    public JsonObjectEx GetObject(int index)
    {
      var v1 = this[index] as JsonObjectEx;
      var v2 = this[index] as Dictionary<string, object>;
      if (v1 != null) return v1;
      return v2 == null ? null : new JsonObjectEx(v2);
    }

    public int GetInt32(int index)
    {
      return (int) Convert.ChangeType(this[index], typeof (int));
    }

    public long GetInt64(int index)
    {
      return (long) Convert.ChangeType(this[index], typeof (long));
    }

    public float GetFloat(int index)
    {
      return (float) Convert.ChangeType(this[index], typeof (float));
    }

    public float GetFloat(int index, IFormatProvider provider)
    {
      return (float) Convert.ChangeType(this[index], typeof (float), provider);
    }

    public double GetDouble(int index)
    {
      return (double) Convert.ChangeType(this[index], typeof (double));
    }

    public double GetDouble(int index, IFormatProvider provider)
    {
      return (double) Convert.ChangeType(this[index], typeof (double), provider);
    }

    public string GetString(int index)
    {
      return (string) Convert.ChangeType(this[index], typeof (string));
    }

    public T GetSimpleType<T>(int index)
    {
      return (T) Convert.ChangeType(this[index], typeof (T));
    }

    public T[] ParseMultiple<T>(Func<JsonObjectEx, T> func)
    {
      var temp = new T[Length];
      for (var i = 0; i < Length; i++) temp[i] = func(GetObject(i));
      return temp;
    }

    /// <summary>
    ///   Iterates over each item in the array using a for loop to let you parse each item individually.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="func">The function used to parse each item.</param>
    public T[] For<T>(Func<JsonArrayEx, int, T> func)
    {
      var array = new T[Length];
      for (var index = 0; index < Length; index++)
      {
        array[index] = func(this, index);
      }
      return array;
    }

    public T[] Cast<T>()
    {
      return InternalArray.Cast<T>().ToArray();
    }

    /// <summary>
    ///   Gets a JSON string representing the array.
    /// </summary>
    public string ToJson()
    {
      return JsonConverter.ToJson(this);
    }

    /// <summary>
    ///   Save the array to a JSON file at the specified <code>path</code>.
    /// </summary>
    /// <param name="path">The path to save the file.</param>
    public void SaveJson(string path)
    {
      File.WriteAllText(path, ToJson());
    }

    #endregion

    #region Static methods

    /// <summary>
    ///   Loads an instance of <code>JsonArray</code> from the JSON file at the specified <code>path</code>.
    /// </summary>
    /// <param name="path">The path to the file.</param>
    public static JsonArrayEx LoadJson(string path)
    {
      return JsonConverter.ParseArray(File.ReadAllText(path));
    }

    /// <summary>
    ///   Loads an array of <code>T</code> from the JSON file at the specified <code>path</code>.
    /// </summary>
    /// <param name="path">The path to the file.</param>
    /// <param name="func">The function used to parse the array.</param>
    public static T[] LoadJson<T>(string path, Func<JsonArrayEx, T[]> func)
    {
      return JsonConverter.ParseArray(File.ReadAllText(path), func);
    }

    /// <summary>
    ///   Gets an instance of <code>JsonArray</code> from the specified JSON string.
    /// </summary>
    /// <param name="json">The JSON string representation of the array.</param>
    public static JsonArrayEx ParseJson(string json)
    {
      return JsonConverter.ParseArray(json);
    }

    /// <summary>
    ///   Gets an array of <code>T</code> from the specified JSON string.
    /// </summary>
    /// <param name="json">The JSON string representation of the array.</param>
    /// <param name="func">The function used to parse the array.</param>
    public static T[] ParseJson<T>(string json, Func<JsonArrayEx, T[]> func)
    {
      return JsonConverter.ParseArray(json, func);
    }

    #endregion
  }
}