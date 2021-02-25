#region

using System.Text.RegularExpressions;

#endregion

namespace Sobees.Tools.Helpers
{
  public class MailHelper
  {
    private static readonly Regex EmailRegex = new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");

    public static bool BisValidEmail(string email)
    {
      return !string.IsNullOrEmpty(email) && EmailRegex.IsMatch(email);
    }
  }
}