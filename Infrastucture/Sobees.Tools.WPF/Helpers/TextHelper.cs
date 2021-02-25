#region

using System;
using System.Linq;

#endregion

namespace Sobees.Tools.Helpers
{
  /// <summary>
  ///   Many functions to apply to string.
  /// </summary>
  public class TextHelper
  {
    /// <summary>
    ///   Remove the first carriage return
    /// </summary>
    /// <param name = "value">the string to proceed</param>
    /// <returns></returns>
    public static string RemoveFirstCarriageReturn(string value)
    {
      try
      {
        if (value.Length == 0)
          return value;

        var cr = value.IndexOf("\n") == 1;
        return !cr ? value : value.Substring(2).Trim();
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }
      return value;
    }


    /// <summary>
    ///   Capitalize first letter of each word
    /// </summary>
    /// <param name = "str"></param>
    /// <returns></returns>
    public static string ToTitleCase(string str)
    {
      var result = str;
      if (!string.IsNullOrEmpty(str))
      {
        var words = str.Split(' ');
        for (var index = 0; index < words.Length; index++)
        {
          var s = words[index];
          if (s.Length > 0)
          {
            words[index] = s[0].ToString().ToUpper() + s.Substring(1);
          }
        }
        result = string.Join(" ", words);
      }
      return result;
    }

    public static string AddSToName(string name)
    {
      if (name != null)
      {
        return name.LastOrDefault() == 's' ? name + "'" : name + "'s";
      }
      return "";
    }

    public static string TruncateAtWord(string value,int length)
    {
      value = HtmlHelper.CleanContent(value);
      if (value == null || value.Length < length || value.IndexOf(" ", length) == -1)
        return value;

      return string.Format("{0} ...", value.Substring(0, value.IndexOf(" ", length)));
    }
  }
}