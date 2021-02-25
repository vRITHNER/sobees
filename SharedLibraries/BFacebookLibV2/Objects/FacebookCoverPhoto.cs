﻿namespace Sobees.Library.BFacebookLibV2.Objects
{
  using Sobees.Library.BFacebookLibV2.Json;

  public class FacebookCoverPhoto : SocialJsonObject
  {

    #region Properties

    /// <summary>
    /// Gets the ID of the photo that represents the cover photo.
    /// </summary>
    public string Id { get; internal set; }

    /// <summary>
    /// Gets the URL of the photo that represents the cover photo.
    /// </summary>
    public string Source { get; internal set; }

    /// <summary>
    /// Gets the vertical offset in pixels of the photo from the bottom.
    /// </summary>
    public int OffsetY { get; internal set; }

    /// <summary>
    /// Gets the horizontal offset in pixels of the photo from the left.
    /// </summary>
    public int OffsetX { get; internal set; }

    #endregion

    #region Constructors

    private FacebookCoverPhoto(JsonObjectEx obj) : base(obj) { }

    #endregion

    #region Static methods

    public static FacebookCoverPhoto Parse(JsonObjectEx obj)
    {
      if (obj == null) return null;
      return new FacebookCoverPhoto(obj)
      {
        Id = obj.GetString("id"),
        Source = obj.GetString("source"),
        OffsetY = obj.GetInt32("offset_y"),
        OffsetX = obj.GetInt32("offset_x")
      };
    }

    #endregion

  }

}