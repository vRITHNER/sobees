#region

using System.Linq;
using LinqToTwitter;

#endregion

namespace Sobees.Library.BTwitterLib
{
  public static class Users
  {
    public static User UserInfosByScreenName(this TwitterContext ctx, string screenName)
    {
      var user = ctx.User.SingleOrDefault(infos => infos.Type == UserType.Show && infos.ScreenName == screenName);
      return user;
    }

    public static User UserInfosByUserId(this TwitterContext ctx, string userId)
    {
      var users = ctx.User.Where(infos => infos.Type == UserType.Show && infos.UserID == ulong.Parse(userId));
      return users.SingleOrDefault();
    }
  }
}