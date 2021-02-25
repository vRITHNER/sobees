namespace Sobees.Library.BFacebookLibV2.Objects
{
  using Sobees.Library.BFacebookLibV2.Json;

  public class FacebookImage : SocialJsonObject
  {
    #region Properties

    /// <summary>
    /// Gets the width of the image.
    /// </summary>
    public int Width { get; private set; }

    /// <summary>
    /// Gets the height of the image.
    /// </summary>
    public int Height { get; private set; }

    /// <summary>
    /// Gets the URL of the image.
    /// </summary>
    public string Source { get; private set; }

    #endregion Properties

    #region Constructors

    private FacebookImage(JsonObjectEx obj) : base(obj)
    {
    }

    #endregion Constructors

    #region Static methods

    public static FacebookImage Parse(JsonObjectEx obj)
    {
      if (obj == null) return null;
      return new FacebookImage(obj)
      {
        Width = obj.GetInt32("width"),
        Height = obj.GetInt32("height"),
        Source = obj.GetString("source")
      };
    }

    #endregion Static methods
  }
}