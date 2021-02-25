#region

using System;
using System.IO;
using BUtility;
using Newtonsoft.Json;
using Sobees.Tools.Logging;

#endregion

namespace Sobees.Tools.Helpers
{
  /// <summary>
  ///   Serialization and deserialization of object into json.
  /// </summary>
  public class JsonHelper
  {
    #region Class Methods

    /// <summary>
    ///   convert JSon into an Object
    /// </summary>
    /// <param name = "value">The json to parse</param>
    /// <typeparam name = "T">The object created from json</typeparam>
    /// <returns></returns>
    public static T Deserialize<T>(string value)
    {
      try
      {
        var res = JsonConvert.DeserializeObject<T>(value);
        return res;
      }
      catch (Exception ex)
      {
        BLogManager.LogEntry("JsonHelper::Deserialize:", ex);
      }

      return default(T);
    }

    /// <summary>
    /// </summary>
    /// <param name = "s"></param>
    /// <param name = "maxSize"></param>
    /// <returns></returns>
    public static byte[] GetArray(Stream s,
                                  long maxSize)
    {
      try
      {
        var array = new byte[maxSize];
        s.Read(array, 0, array.Length);
        s.Close();
        return array;
      }
      catch (Exception ex)
      {
        BLogManager.LogEntry(ex);
      }
      return new byte[] {};
    }

    /// <summary>
    ///   Serialize an object into json
    /// </summary>
    /// <param name = "entry">the object to serialize</param>
    /// <returns></returns>
    public static string Serialize(Object entry)
    {
      try
      {
        var res = JsonConvert.SerializeObject(entry, Formatting.None);
        return res;
      }
      catch (Exception ex)
      {
        BLogManager.LogEntry("JsonHelper::Serialize:", ex);
      }
      return null;
    }

    #endregion
  }
}