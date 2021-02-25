#region

using System;
using Sobees.Library.BFacebookLibV2.Comments;
using Sobees.Library.BFacebookLibV2.Json;
using Sobees.Library.BFacebookLibV2.Objects.Users;

#endregion

namespace Sobees.Library.BFacebookLibV2.Objects.Thread
{
  public class FacebookThread : SocialJsonObject
  {
    #region Constructors

    private FacebookThread(JsonObjectEx obj) : base(obj)
    {
    }

    #endregion

    #region Static methods

    public static FacebookThread Parse(JsonObjectEx obj)
    {
      return new FacebookThread(obj)
      {
        Id = obj.GetInt32("id"),
        To = obj.GetArray("to", FacebookUser.Parse),
        Comments = obj.GetArray("to", FacebookComment.Parse),
        Count = obj.GetArray("to", FacebookComment.Parse).Length,
        UpdatedTime = obj.GetDateTime("updatetime"),
        UnSeen = obj.GetInt32("unseen"),
        UnRead = obj.GetInt32("unread")
      };
    }

    #endregion

    #region Properties

    public long Id { get; set; }

    public int UnRead { get; set; }

    public int UnSeen { get; set; }

    public FacebookUser[] To { get; set; }

    public FacebookComment[] Comments { get; set; }

    public DateTime UpdatedTime { get; set; }

    public int Count { get; private set; }

    #endregion
  }
}