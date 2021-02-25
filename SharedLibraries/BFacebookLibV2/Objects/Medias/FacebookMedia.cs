namespace Sobees.Library.BFacebookLibV2.Objects.Medias
{
  #region

  using Sobees.Library.BFacebookLibV2.Json;

  #endregion

  /// <summary>
  /// </summary>
  public class FacebookMedia : SocialJsonObject
  {
    public FacebookMedia(JsonObjectEx obj) : base(obj)
    {
    }

    /// <summary>
    /// </summary>
    public FacebookImage Image { get; set; }

    #region Static methods

    public static FacebookMedia Parse(JsonObjectEx obj)
    {
      if (obj == null) return null;
      return new FacebookMedia(obj)
      {
        Image = obj.GetObject("image", FacebookImage.Parse)
      };
    }

    #endregion
  }
}