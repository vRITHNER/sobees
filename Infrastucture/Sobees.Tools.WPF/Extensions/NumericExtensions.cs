#region

using System.Text.RegularExpressions;

#endregion

namespace Sobees.Tools.Extensions
{
  public static class NumericExtensions
  {
    public static bool IsNumeric(this string valueasstring)
    {
      return Regex.IsMatch(valueasstring, @"^-?\d*[0-9]?(|.\d*[0-9]|,\d*[0-9])?$");
    }
  }
}