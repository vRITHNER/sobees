#region

using System.Linq;
using LinqToTwitter;

#endregion

namespace Sobees.Library.BTwitterLib.Extensions
{
  public static class LinqToTwitterExtensions
  {
    public static User UserInfosByScreenName(this TwitterContext ctx, string screenName)
    {
      var users = ctx.User.Where(infos => infos.Type == UserType.Show && infos.ScreenName == screenName);
      return users.SingleOrDefault();
    }

    public static User UserInfosByUserId(this TwitterContext ctx, string userId)
    {
      var users = ctx.User.Where(infos => infos.Type == UserType.Show && infos.UserID == ulong.Parse(userId));
      return users.SingleOrDefault();
    }

    public static ulong ToUlongValue(this string stringValue)
    {
      ulong ulongValue = 0;
      ulong.TryParse(stringValue, out ulongValue);
      return ulongValue;
    }


  }
}