namespace Sobees.Library.BFacebookLibV2.Objects
{
  #region

  using Sobees.Library.BFacebookLibV2.Json;

  #endregion

  public class FacebookObject : SocialJsonObject
  {
    #region Constructors

    private FacebookObject(JsonObjectEx obj) : base(obj)
    {
    }

    #endregion

    #region Static methods

    public static FacebookObject Parse(JsonObjectEx obj)
    {
      if (obj == null) return null;
      return new FacebookObject(obj)
      {
        Id = obj.GetString("id"),
        Name = obj.GetString("name")
      };
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets the ID of the object.
    /// </summary>
    public string Id { get; internal set; }

    /// <summary>
    ///   Gets the name of the object.
    /// </summary>
    public string Name { get; internal set; }

    #endregion
  }
}