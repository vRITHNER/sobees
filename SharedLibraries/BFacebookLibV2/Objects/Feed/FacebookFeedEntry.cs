#region

using System;
using Sobees.Library.BFacebookLibV2.Comments;
using Sobees.Library.BFacebookLibV2.Interfaces;
using Sobees.Library.BFacebookLibV2.Json;
using Sobees.Library.BFacebookLibV2.Objects.Attachments;
using Sobees.Library.BFacebookLibV2.Objects.Likes;

#endregion

namespace Sobees.Library.BFacebookLibV2.Objects.Feed
{
  #region

  

  #endregion

  public class FacebookFeedEntry : SocialJsonObject, ISocialTimelineEntry
  {
    #region Constructors

    private FacebookFeedEntry(JsonObjectEx obj) : base(obj)
    {
    }

    #endregion

    #region Static methods

    public static FacebookFeedEntry Parse(JsonObjectEx obj)
    {
      if (obj == null) return null;
      return new FacebookFeedEntry(obj)
      {
        Id = obj.GetString("id"),
        From = obj.GetObject("from", FacebookObject.Parse),
        Message = obj.GetString("message"),
        Description = obj.GetString("description"),
        Story = obj.GetString("story"),
        Picture = obj.GetString("picture"),
        Link = obj.GetString("link"),
        Name = obj.GetString("name"),
        Caption = obj.GetString("caption"),
        Icon = obj.GetString("icon"),
        Type = obj.GetString("type"),
        StatusType = obj.GetString("status_type"),
        Application = obj.GetObject("application", FacebookObject.Parse),
        CreatedTime = obj.GetDateTime("created_time"),
        UpdatedTime = obj.GetDateTime("updated_time"),
        Comments = obj.GetObject("comments", FacebookComments.Parse),
        Shares = obj.GetObject("shares", FacebookShares.Parse),
        Likes = obj.GetObject("likes", FacebookLikes.Parse),
        Attachements = obj.GetObject("attachments", FacebookAttachements.Parse)
      };
    }

    #endregion

    #region Properties

    public string Id { get; private set; }
    public FacebookObject From { get; private set; }
    public string Message { get; private set; }
    public string Description { get; private set; }
    public string Story { get; private set; }
    public string Picture { get; private set; }
    public string Link { get; private set; }
    public string Name { get; private set; }
    public string Caption { get; private set; }
    public string Icon { get; private set; }
    public string Type { get; private set; }
    public string StatusType { get; private set; }
    public FacebookObject Application { get; private set; }
    public DateTime CreatedTime { get; private set; }
    public DateTime UpdatedTime { get; set; }

    public FacebookAttachements Attachements { get; private set; }

    /// <summary>
    ///   Gets information about how many times the feed entry has been shared. If the feed entry
    ///   hasn't yet been shared, this property will return <code>NULL</code>.
    /// </summary>
    public FacebookShares Shares { get; private set; }

    public FacebookLikes Likes { get; set; }
    public FacebookComments Comments { get; private set; }
    public long? ObjectId { get; private set; }

    public DateTime SortDate => CreatedTime;

    #endregion
  }
}