#region

using System;
using System.Collections.Generic;
using System.Globalization;
using Sobees.Library.BFacebookLibV2.Objects.Feed;
using Sobees.Tools.Logging;

#endregion

namespace Sobees.Controls.Facebook.Cls
{
  public class FbHelper
  {
    public static List<FacebookFeedEntry> AddInfoUser(List<FacebookFeedEntry> entries, FacebookContactCollection friends,
      out List<long> idToComplete)
    {
      idToComplete = new List<long>();
      if (entries == null || friends == null)
        return entries;

      foreach (var entry in entries)
      {
        try
        {
          var entry1 = entry;
          var lst = friends.Where(u => u.UserId.ToString(CultureInfo.InvariantCulture).Equals(entry1.User.Id));
          if (lst.Any())
          {
            var user = lst.First();
            entry.User.Name = user.name;
            entry.User.NickName = user.name;
            entry.User.ProfileImgUrl = string.IsNullOrEmpty(user.pic_square)
              ? "http://static.ak.fbcdn.net/pics/q_silhouette.gif"
              : user.pic_square;
          }
          else
          {
            idToComplete.Add(Convert.ToInt64(entry.User.Id));
          }

          if (entry.ToUser != null)
          {
            if (entry.User.Id == entry.ToUser.Id)
              entry.ToUser = null;
            else
            {
              lst = friends.Where(u => u.UserId.ToString(CultureInfo.InvariantCulture).Equals(entry1.ToUser.Id));
              if (lst.Any())
              {
                var user = lst.First();
                entry.ToUser.Name = user.name;
                entry.ToUser.NickName = user.name;
                entry.ToUser.ProfileImgUrl = string.IsNullOrEmpty(user.pic_square)
                  ? "http://static.ak.fbcdn.net/pics/q_silhouette.gif"
                  : user.pic_square;
              }
              else
              {
                idToComplete.Add(Convert.ToInt64(entry.ToUser.Id));
              }

              //var temp = entry.ToUser;
              //entry.ToUser = entry.User;
              //entry.User = temp;
            }
          }
          foreach (var comment in entry.Comments)
          {
            var comment1 = comment;
            lst = friends.Where(u => u.UserId.ToString(CultureInfo.InvariantCulture).Equals(comment1.User.Id));
            if (lst.Any())
            {
              var user = lst.First();
              comment1.User.Name = user.name;
              comment1.User.NickName = user.name;
              comment1.User.ProfileImgUrl = string.IsNullOrEmpty(user.pic_square)
                ? "http://static.ak.fbcdn.net/pics/q_silhouette.gif"
                : user.pic_square;
            }
            else
            {
              idToComplete.Add(Convert.ToInt64(comment1.User.Id));
            }
          }
          if (entry.LikeFacebook.CanLike == 0) continue;
          foreach (var list in entry.LikeFacebook.SampleUsersLike)
          {
            var user1 = list;
            lst = friends.Where(u => u.UserId.ToString(CultureInfo.InvariantCulture).Equals(user1.Id));
            if (lst.Any())
            {
              var user = lst.First();
              list.Name = user.name;
              list.NickName = user.name;
              list.ProfileImgUrl = string.IsNullOrEmpty(user.pic_square)
                ? "http://static.ak.fbcdn.net/pics/q_silhouette.gif"
                : user.pic_square;
            }
            else
            {
              idToComplete.Add(Convert.ToInt64(list.Id));
            }
          }
          foreach (var list in entry.LikeFacebook.FriendsLike)
          {
            var user1 = list;
            lst = friends.Where(u => u.UserId.ToString(CultureInfo.InvariantCulture).Equals(user1.Id));
            if (lst.Any())
            {
              var user = lst.First();
              list.Name = user.name;
              list.NickName = user.name;
              list.ProfileImgUrl = string.IsNullOrEmpty(user.pic_square)
                ? "http://static.ak.fbcdn.net/pics/q_silhouette.gif"
                : user.pic_square;
            }
            else
            {
              idToComplete.Add(Convert.ToInt64(list.Id));
            }
          }
        }
        catch (Exception ex)
        {
          TraceHelper.Trace("FBHelper", ex);
        }
      }
      return entries;
    }

    public static List<FacebookFeedEntry> AddInfoUser(List<FacebookFeedEntry> entries, FacebookContactCollection users)
    {
      List<long> complete;
      return AddInfoUser(entries, users, out complete);
    }

    /// <summary>
    /// </summary>
    /// <param name="timestamp"></param>
    /// <returns></returns>
    public static DateTime ConvertUniversalTimeToDate(double timestamp)
    {
      // First make a System.DateTime equivalent to the UNIX Epoch.
      var dateTime = new DateTime(1970,
        1,
        1,
        0,
        0,
        0,
        0);

      // Add the number of seconds in UNIX timestamp to be converted.
      return dateTime.AddSeconds(timestamp);
    }
  }
}