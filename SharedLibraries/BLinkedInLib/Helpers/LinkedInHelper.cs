#region

using System;
using System.Linq;
using System.Xml.Linq;
using Sobees.Library.BGenericLib;

#endregion

namespace Sobees.Library.BLinkedInLib.Helpers
{
  public static class LinkedInHelper
  {
    #region Authentication URLs

    internal const string ACCESS_TOKEN_URL =
      "https://www.linkedin.com/uas/oauth2/accessToken?grant_type=authorization_code&code=";

    internal const string AUTHORIZATION_URL =
      "https://www.linkedin.com/uas/oauth2/authorization?response_type=code&client_id=";

    #endregion

    #region Updates URLs

    internal const string UPDATE_STATUS_URL = "https://api.linkedin.com/v1/people/~/shares?";
    internal const string GET_UPDATES_URL = "https://api.linkedin.com/v1/people/~/network/updates?";
    internal const string GET_UPDATES_BY_ID_URL = "https://api.linkedin.com/v1/people/id=$USER_ID$/network/updates?";

    internal const string GET_UPDATES_BY_URL_URL =
      "https://api.linkedin.com/v1/people/url=$USER_URL$/network/updates?";

    internal const string UPDATE_LIKES_URL =
      "https://api.linkedin.com/v1/people/~/network/updates/key={NETWORK UPDATE KEY}/likes";

    internal const string UPDATE_IS_LIKED_URL =
      "https://api.linkedin.com/v1/people/~/network/updates/key={NETWORK UPDATE KEY}/is-liked";

    internal const string UPDATE_COMMENTS_URL =
      "https://api.linkedin.com/v1/people/~/network/updates/key={NETWORK UPDATE KEY}/update-comments";

    #endregion

    #region Profile URLs

    internal const string PROFILE_URL = "https://api.linkedin.com/v1/people/";
    internal const string PROFILE_SELF_URL = "https://api.linkedin.com/v1/people/~";
    internal const string PROFILE_BY_ID_URL = "https://api.linkedin.com/v1/people/id=";
    internal const string PROFILE_BY_URL_URL = "https://api.linkedin.com/v1/people/url=";
    internal const string PROFILE_MULTIPLE_URL = "https://api.linkedin.com/v1/people::";

    #endregion

    #region Groups URLs

    internal const string GROUPS_SUGGESTIONS_URL = "https://api.linkedin.com/v1/people/~/suggestions/groups";
    internal const string GROUPS_MEMBERSHIP_URL = "https://api.linkedin.com/v1/people/~/group-memberships:(group";
    internal const string GROUP_POSTS_URL = "https://api.linkedin.com/v1/groups/{GROUP_ID}/posts";

    internal const string GROUP_MEMBER_POSTS_URL =
      "https://api.linkedin.com/v1/people/~/group-memberships/{GROUP_ID}/posts";

    internal const string POSTS_COMMENTS_URL = "https://api.linkedin.com/v1/posts/{POST_ID}/comments";
    internal const string GROUP_JOIN_URL = "https://api.linkedin.com/v1/people/~/group-memberships/{GROUP_ID}";
    internal const string POSTS_URL = "https://api.linkedin.com/v1/posts/{POST_ID}";
    internal const string POSTS_LIKE_URL = "https://api.linkedin.com/v1/posts/{POST_ID}/relation-to-viewer/is-liked";

    internal const string POSTS_FOLLOW_URL =
      "https://api.linkedin.com/v1/posts/{POST_ID}/relation-to-viewer/is-following";

    internal const string POSTS_FLAG_URL = "https://api.linkedin.com/v1/posts/{POST_ID}/category/code";
    internal const string COMMENTS_URL = "https://api.linkedin.com/v1/comments/{COMMENT_ID}";

    #endregion

    #region Messages URLs

    internal const string SEND_MESSAGE_URL = "https://api.linkedin.com/v1/people/~/mailbox";

    #endregion

    #region Search URLs

    internal const string PEOPLE_SEARCH_URL = "https://api.linkedin.com/v1/people-search";

    #endregion

    internal const string CALLBACK = "https://linkin/done";

    private static DateTime _unixStartDate = new DateTime(1970, 1, 1);

    internal static Comment BuildComment(XElement xp)
    {
      var liComment = new Comment();

      var xe = xp.Element("id");
      if (xe != null)
      {
        liComment.Id = xe.Value.Trim();
      }
      //xe = xp.Element("sequence-number");
      //if (xe != null)
      //{
      //  liComment.SequenceNumber = Convert.ToInt32(xe.Value.Trim());
      //}
      xe = xp.Element("comment");
      if (xe != null)
      {
        liComment.Body = xe.Value.Trim();
      }
      xe = xp.Element("timestamp");
      if (xe != null)
      {
        liComment.Date = GetRealDateTime(Convert.ToDouble(xe.Value.Trim()));
      }
      xe = xp.Element("person");
      if (xe != null)
      {
        liComment.User = BuildPerson(new LinkedInUser(), xe);
      }

      return liComment;
    }

    internal static DateTime GetRealDateTime(double milliseconds)
    {
      return _unixStartDate.AddMilliseconds(milliseconds).ToLocalTime();
    }


    internal static LinkedInUser BuildPerson(LinkedInUser liPerson, XElement xp)
    {
      //id
      var xe = xp.Element("id");
      if (xe != null)
        liPerson.Id = xe.Value.Trim();
      //first-name
      xe = xp.Element("first-name");
      if (xe != null)
        liPerson.FirstName = xe.Value.Trim();
      //last-name
      xe = xp.Element("last-name");
      if (xe != null)
        liPerson.Name = xe.Value.Trim();
      //headline
      xe = xp.Element("headline");
      if (xe != null)
        liPerson.Description = xe.Value.Trim();
      //picture-url
      xe = xp.Element("picture-url");
      if (xe != null)
        liPerson.ProfileImgUrl = xe.Value.Trim();
      //api-standard-profile-request
      ////xe = xp.Element("api-standard-profile-request");
      ////if (xe != null)
      ////  liPerson.ApiStandardProfileRquest = BuildApiStandardProfileRequest(xe);
      //site-standard-profile-request
      xe = xp.Element("site-standard-profile-request");
      if (xe != null)
      {
        var xn = xe.Element("url");
        if (xn != null)
          liPerson.Url = xn.Value.Trim();
      }
      return liPerson;
    }

    internal static bool IsAnyString(params string[] strings)
    {
      return strings.Any(s => !string.IsNullOrEmpty(s));
    }

    private enum CompanyUpdateType
    {
      Invalid,
      Profile,
      Status,
      Job,
      Person
    }
  }
}