namespace Sobees.Library.BFacebookLibV2.Tools
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  /// <summary>
  /// JSON (JavaScript Object Notation) Utility Methods.
  /// </summary>
  public static class JsonHelper
  {
    ///<summary>
    /// Converts a Dictionary to a JSON-formatted Associative Array.
    ///</summary>
    ///<param name="dict">Source Dictionary collection [string|string].</param>
    ///<returns>JSON Associative Array string.</returns>
    public static string ConvertToJsonAssociativeArray(Dictionary<string, string> dict)
    {
      return "{" + string.Join(",", (from pair in dict where !string.IsNullOrEmpty(pair.Value) select string.Format("\"{0}\":{2}{1}{2}", EscapeJsonString(pair.Key), EscapeJsonString(pair.Value), IsJsonArray(pair.Value) || IsBoolean(pair.Value) ? string.Empty : "\"")).ToArray()) + "}";
    }

    /// <summary>
    /// Determines if input string is a formatted JSON Array.
    /// </summary>
    /// <param name="test">string</param>
    /// <returns>bool</returns>
    public static bool IsJsonArray(string test)
    {
      return test.StartsWith("{") && !test.StartsWith("{*") || test.StartsWith("[");
    }

    /// <summary>
    /// Determines if input string is a boolean value.
    /// </summary>
    /// <param name="test">string</param>
    /// <returns>bool</returns>
    public static bool IsBoolean(string test)
    {
      return test.Equals("false") || test.Equals("true");
    }

    /// <summary>
    /// Converts a List collection of type string to a JSON Array.
    /// </summary>
    /// <param name="list">List of strings</param>
    /// <returns>string</returns>
    public static string ConvertToJSonArray(List<string> list)
    {
      if (list == null || list.Count == 0)
      {
        return "[]";
      }

      StringBuilder builder = new StringBuilder();
      builder.Append("[");
      foreach (var item in list)
      {
        builder.Append(string.Format("{0}{1}{0},", IsJsonArray(item) || IsBoolean(item) ? string.Empty : "\"", EscapeJsonString(item)));
      }
      builder.Replace(",", "]", builder.Length - 1, 1);
      return builder.ToString();
    }

    /// <summary>
    /// Converts a List collection of type long to a JSON Array.
    /// </summary>
    /// <param name="list">List of longs</param>
    /// <returns>string</returns>
    public static string ConvertToJSonArray(List<long> list)
    {
      if (list == null || list.Count == 0)
      {
        return "[]";
      }

      StringBuilder builder = new StringBuilder();
      builder.Append("[");
      foreach (var item in list)
      {
        builder.Append(string.Format("{0}{1}{0},", IsJsonArray(item.ToString()) || IsBoolean(item.ToString()) ? string.Empty : "\"", EscapeJsonString(item.ToString())));
      }
      builder.Replace(",", "]", builder.Length - 1, 1);
      return builder.ToString();
    }

    /// <summary>
    /// Converts a JSON Array string to a List collection of type string.
    /// </summary>
    /// <param name="array">JSON Array string</param>
    /// <returns>List of strings</returns>
    public static List<string> ConvertFromJsonArray(string array)
    {
      if (!string.IsNullOrEmpty(array))
      {
        array = array.Replace("[", "").Replace("]", "").Replace("\"", "");
        return new List<string>(array.Split(','));
      }

      return new List<string>();
    }

    /// <summary>
    /// Converts a JSON Array string to a Dictionary collection of type string, string.
    /// </summary>
    /// <param name="array">JSON Array string</param>
    /// <returns>Dictionary of string, string</returns>
    public static Dictionary<string, string> ConvertFromJsonAssoicativeArray(string array)
    {
      var dict = new Dictionary<string, string>();
      if (!string.IsNullOrEmpty(array))
      {
        array = array.Replace("{", "").Replace("}", "").Replace("\":", "|").Replace("\"", "").Replace("\\/", "/");
        var pairs = new List<string>(array.Split(','));
        foreach (var pair in pairs)
        {
          if (string.IsNullOrEmpty(pair)) continue;
          var pairArray = pair.Split('|');
          dict.Add(pairArray[0], pairArray[1]);
        }
        return dict;
      }

      return new Dictionary<string, string>();
    }

    /// <summary>
    /// Escape backslashes and double quotes of valid JSON content string.
    /// </summary>
    /// <param name="originalString">string</param>
    /// <returns>string</returns>
    public static string EscapeJsonString(string originalString)
    {
      return IsJsonArray(originalString) ? originalString : originalString.Replace("\\/", "/").Replace("/", "\\/").Replace("\\\"", "\"").Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\n", "\\n");
    }
  }
}
