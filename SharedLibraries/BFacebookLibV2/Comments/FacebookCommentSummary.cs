﻿namespace Sobees.Library.BFacebookLibV2.Comments
{
  #region

  using System;
  using Sobees.Library.BFacebookLibV2.Json;
  using Sobees.Library.BFacebookLibV2.Objects;

  #endregion

  public class FacebookCommentSummary : SocialJsonObject
  {
    #region Constructors

    private FacebookCommentSummary(JsonObjectEx obj) : base(obj)
    {
    }

    #endregion

    #region Static methods

    public static FacebookCommentSummary Parse(JsonObjectEx obj)
    {
      // TODO: Should we just return NULL if "obj" is NULL?
      if (obj == null) return null;
      return new FacebookCommentSummary(obj)
      {
        Id = obj.GetString("id"),
        From = obj.GetObject("from", FacebookObject.Parse),
        Message = obj.GetString("message"),
        MessageTags = obj.GetArray("message_tags", FacebookMessageTag.Parse),
        CreatedTime = obj.GetDateTime("created_time"),
        Likes = obj.HasValue("likes") ? obj.GetInt32("likes") : 0
      };
    }

    #endregion

    // TODO: Check whether this class is still used...

    #region Properties

    public string Id { get; private set; }
    public FacebookObject From { get; private set; }
    public string Message { get; private set; }
    public FacebookMessageTag[] MessageTags { get; private set; }
    public DateTime CreatedTime { get; private set; }
    public int Likes { get; private set; }

    #endregion
  }
}