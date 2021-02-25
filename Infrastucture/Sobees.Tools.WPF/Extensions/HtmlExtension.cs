#region

using System.Linq;
using System.Text.RegularExpressions;

#endregion

namespace Sobees.Tools.Extensions
{
  /// <summary>
  /// </summary>
  public static class HtmlExtension
  {
    /// <summary>
    ///   Compiled regular expression for performance.
    /// </summary>
    private static readonly Regex HtmlRegex = new Regex("<.*?>", RegexOptions.Compiled);

    /// <summary>
    /// </summary>
    /// <param name = "html"></param>
    /// <returns></returns>
    public static string RemoveHtmlTags(this string html)
    {
      //return StripTagsRegexCompiled(html);
      return StripTagsRegex(html);
    }

    /// <summary>
    /// </summary>
    /// <param name = "s"></param>
    /// <returns></returns>
    public static string ToCleanCarriageLine(this string s)
    {
      if (s == null)
        return null;

      var re = new Regex("\r\n\r\n");
      var res = re.Replace(s, " ");
      return res;
    }

    /// <summary>
    ///   Remove HTML from string with Regex.
    /// </summary>
    public static string StripTagsRegex(string source)
    {
      return Regex.Replace(source, "<.*?>", string.Empty);
    }

    /// <summary>
    ///   Remove HTML from string with compiled Regex.
    /// </summary>
    public static string StripTagsRegexCompiled(string source)
    {
      return HtmlRegex.Replace(source, string.Empty);
    }

    /// <summary>
    ///   Remove HTML tags from string using char array.
    /// </summary>
    public static string StripTagsCharArray(string source)
    {
      var array = new char[source.Length];
      var arrayIndex = 0;
      var inside = false;

      foreach (var @let in source)
      {
        if (let == '<')
        {
          inside = true;
          continue;
        }
        if (let == '>')
        {
          inside = false;
          continue;
        }
        if (!inside)
        {
          array[arrayIndex] = let;
          arrayIndex++;
        }
      }
      return new string(array, 0, arrayIndex);
    }
  }
}