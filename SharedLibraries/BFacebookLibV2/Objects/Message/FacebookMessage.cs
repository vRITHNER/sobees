#region

using System;
using Sobees.Library.BFacebookLibV2.Json;
using Sobees.Library.BFacebookLibV2.Objects.Users;

#endregion

namespace Sobees.Library.BFacebookLibV2.Objects.Message
{
  public class FacebookMessage : SocialJsonObject
  {
    #region Constructors

    private FacebookMessage(JsonObjectEx obj) : base(obj)
    {
    }

    #endregion

    #region Static methods

    public static FacebookMessage Parse(JsonObjectEx obj)
    {
      return new FacebookMessage(obj)
      {
        Id = obj.GetInt64("id"),
        From = obj.GetObject("from", FacebookFrom.Parse),
        Message = obj.GetString("message"),
        CreatedTime = obj.GetDateTime("created_time"),
        Subject = obj.GetString("subject"),
        To = obj.GetArray("to", FacebookUser.Parse),
        UnRead = obj.GetInt32("unread"),
        UnSeen = obj.GetInt32("unseen"),
        Tags = obj.GetArray("tags", FacebookMessageTag.Parse)
      };
    }

    #endregion

    #region Properties

    public long Id { get; set; }

    public int UnRead { get; set; }

    public int UnSeen { get; set; }

    public FacebookFrom From { get; set; }

    public FacebookUser[] To { get; set; }

    public string Message { get; set; }

    public string Subject { get; set; }

    public FacebookMessageTag[] Tags { get; set; }

    public DateTime CreatedTime { get; set; }

    #endregion
  }
}