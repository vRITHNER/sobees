namespace Sobees.Library.BFacebookLibV2.Objects
{
  #region

  using Sobees.Library.BFacebookLibV2.Json;

  #endregion

  public class FacebookFrom : SocialJsonObject
  {
    #region Constructors

    private FacebookFrom(JsonObjectEx obj) : base(obj)
    {
    }

    #endregion

    #region Static methods

    public static FacebookFrom Parse(JsonObjectEx obj)
    {
      if (obj == null) return null;
      return new FacebookFrom(obj)
      {
        Id = obj.GetString("id"),
        Name = obj.GetString("name"),
        Category = obj.GetString("category"),
        CategoryList = obj.GetArray("category_list", FacebookObject.Parse)
      };
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets the ID of the user or page.
    /// </summary>
    public string Id { get; private set; }

    /// <summary>
    ///   Gets the name of the user or page.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    ///   Gets the primary category of a page. Is <code>NULL</code> for users.
    /// </summary>
    public string Category { get; private set; }

    /// <summary>
    ///   Gets list of sub categories of a page. Is <code>NULL</code> for users.
    /// </summary>
    public FacebookObject[] CategoryList { get; private set; }

    #endregion
  }
}