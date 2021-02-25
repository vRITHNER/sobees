#region

using System;
using System.Globalization;
using System.Linq;
using LinqToTwitter;
using Sobees.Library.BGenericLib;
using Sobees.Tools.Extensions;
using User = LinqToTwitter.User;

#endregion

namespace Sobees.Library.BTwitterLib.Helpers
{
  public class LinqToTwitterHelper
  {
    public const string PROFILE_IMAGE_ORIGINAL_SIZE = ".";
    public const string PROFILE_IMAGE_BIGGER_SIZE = "_bigger.";
    public const string PROFILE_IMAGE_NORMAL_SIZE = "_normal.";
    public const string PROFILE_IMAGE_IDEAL_SIZE = "_reasonably_small.";

    /// <summary>
    ///   Convert and return a TwitterUser from a LinqToTwitter User entry
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public static TwitterUser ConvertUserToTwitterUser(User user)
    {
      var f = new TwitterUser
      {
        CreatedAt = user.CreatedAt.ToLocalTime(),
        Description = user.Description,
        FollowersCount = user.FollowersCount,
        Id = user.UserID.ToString(),
        UserTimeZone = user.TimeZone,
        IsFollowing = user.Following,
        IsProtected = user.Protected,
        IsVerified = user.Verified,
        Lang = user.LangResponse,
        Location = user.Location,
        Name = user.Name,
        FriendsCount = user.FriendsCount,
        NbFavorites = user.FavoritesCount,
        NickName = user.ScreenNameResponse,
        ProfileImgUrl = user.ProfileImageUrl.Replace(PROFILE_IMAGE_NORMAL_SIZE, PROFILE_IMAGE_IDEAL_SIZE), //Ecapito
        ProfileUrl = user.Url,
        Url = user.Url,
        UtcOffset = !string.IsNullOrEmpty(user.UtcOffset.ToString(CultureInfo.InvariantCulture)) ? Convert.ToInt32(user.UtcOffset) : 0,
        StatusUseCount = user.StatusesCount,
        Geolocation =
          user.Status != null
            ? user.Status.Coordinates != null ? new Geoloc {Latitude = user.Status.Coordinates.Latitude, Longitude = user.Status.Coordinates.Longitude} : null
            : null
      };
      return f;
    }

    /// <summary>
    ///   Convert and return a TwitterUser from a LinqToTwitter User entry
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    public static TwitterEntry ConvertSearchToTwitterEntry(Status status)
    {
      var f = new TwitterEntry
      {
        Id = status.StatusID.ToString(),
        Content = status.Text,
        Title = status.Text,
        InReplyTo = status.InReplyToStatusID.ToString(),
        InReplyToUserId = status.InReplyToUserID.ToString(),
        InReplyToUserName = status.InReplyToScreenName,
        Type = EnumType.Twitter,
        SourceName = status.Source,
        PubDate = status.CreatedAt,
        UpdateDate = status.CreatedAt,
        User = ConvertUserToTwitterUser(status.User),
        RetweeterUser = status.RetweetedStatus != null ? status.RetweetedStatus.User != null ? ConvertUserToTwitterUser(status.RetweetedStatus.User) : null : null
      };
      return f;
    }

    /// <summary>
    ///   Convert and return a TwitterUser from a LinqToTwitter User entry
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    public static TwitterEntry ConvertStatusToTwitterEntry(Status status)
    {
      var f = new TwitterEntry
      {
        Id = status.StatusID.ToString(),
        Content = status.Text,
        Title = status.Text,
        InReplyTo = status.InReplyToStatusID.ToString(),
        InReplyToUserId = status.InReplyToUserID.ToString(),
        InReplyToUserName = status.InReplyToScreenName,
        Type = EnumType.Twitter,
        SourceName = status.Source,
        PubDate = status.CreatedAt.ToLocalTime(),
        UpdateDate = status.CreatedAt.ToLocalTime(),
        User = ConvertUserToTwitterUser(status.User),
        RetweeterUser = status.Retweeted ? status.RetweetedStatus.User != null ? ConvertUserToTwitterUser(status.RetweetedStatus.User) : null : null
      };
      return f;
    }

    public static TwitterList ConvertListToTwitterList(List twitterlist)
    {
      var f = new TwitterList
      {
        Id = twitterlist.ListIDResponse.ToString(),
        Name = twitterlist.Name,
        FullName = twitterlist.FullName,
        Mode = twitterlist.Mode,
        Slug = twitterlist.Slug ?? twitterlist.Name,
        MemberCount = twitterlist.MemberCount,
        SubscriberCount = twitterlist.SubscriberCount,
        Url = twitterlist.Uri,
        Creator = twitterlist.Users.Any() ? ConvertUserToTwitterUser(twitterlist.Users[0]) : null
      };
      return f;
    }

    public static Entry ConvertTrendsToTwitterEntry(Trend trend)
    {
      //ent = new Entry { Type = EnumType.TwitterSearch, Title = decodeWordSearch, Content = decodeWordSearchName };
      var f = new TwitterEntry
      {
        Type = EnumType.TwitterSearch,
        Title = trend.Name,
        Content = trend.Name
      };
      return f;
    }

    public static TwitterEntry ConvertSearchEntryToTwitterEntry(Status searchEntry)
    {
      var user = new TwitterUser
      {
        Id = searchEntry.UserID.ToString(),
        Name = searchEntry.User.Name,
        NickName = searchEntry.User.ScreenName,
        ProfileUrl = $"http://twitter.com/{searchEntry.User.ProfileImageUrl}",
        ProfileImgUrl = searchEntry.User.ProfileImageUrl,
        Geolocation = new Geoloc {Latitude = searchEntry.Coordinates?.Latitude ?? 0, Longitude = searchEntry.Coordinates?.Longitude ?? 0}
      };

      var f = new TwitterEntry
      {
        Id = searchEntry.ID.ToString(),
        Content = searchEntry.Text,
        Title = searchEntry.Text,
        Type = EnumType.TwitterSearch,
        Link = searchEntry.Source,
        SourceName = searchEntry.Source,
        PubDate = searchEntry.CreatedAt.ToUniversalTime(),
        UpdateDate = searchEntry.CreatedAt.ToUniversalTime(),
        User = user,
        InReplyToUserName = TwitterLib.GetInReplyTo(searchEntry.Text)
      };
      return f;
    }

    public static string ManageTwitterError(Exception exception)
    {
      return exception.ToCompleteExceptionMessage();
    }

    public static TwitterEntry ConvertDirectMessageToTwitterEntry(DirectMessage status)
    {
      var f = new TwitterEntry
      {
        Id = status.IDString,
        Content = status.Text,
        Title = status.Text,
        Type = EnumType.Twitter,
        PubDate = status.CreatedAt.ToLocalTime(),
        UpdateDate = status.CreatedAt.ToLocalTime(),
        User = ConvertUserToTwitterUser(status.Sender),
        ToUser = ConvertUserToTwitterUser(status.Recipient)
      };
      return f;
    }
  }
}