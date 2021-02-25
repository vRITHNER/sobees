namespace Sobees.Library.BFacebookLibV2.Objects.Attachments
{
  #region

  using Sobees.Library.BFacebookLibV2.Json;
  using Sobees.Library.BFacebookLibV2.Objects.Medias;

  #endregion

  /// <summary>
  /// </summary>
  public class FacebookAttachement : SocialJsonObject
  {
    public FacebookAttachement(JsonObjectEx obj) : base(obj)
    {
    }

    /// <summary>
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///   Gets or sets the type.
    /// </summary>
    /// <value>The type.</value>
    public string Type { get; set; }

    /// <summary>
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    ///   Gets or sets the description_ tags.
    /// </summary>
    /// <value>The description_ tags.</value>
    // ReSharper disable once InconsistentNaming
    public string Description_Tags { get; set; }

    /// <summary>
    /// </summary>
    public FacebookMedias Medias { get; set; }

    #region Static methods

    public static FacebookAttachement Parse(JsonObjectEx obj)
    {
      if (obj == null) return null;
      return new FacebookAttachement(obj)
      {
        Name = obj.GetString("name"),
        Description = obj.GetString("description"),
        Type = obj.GetString("type"),
        Description_Tags = obj.GetArray("description_tags", FacebookObject.Parse).ToString(),
        Medias = obj.GetObject("medias", FacebookMedias.Parse),
        Title = obj.GetString("title"),
        Url = obj.GetString("url")
      };
    }

    #endregion
  }
}