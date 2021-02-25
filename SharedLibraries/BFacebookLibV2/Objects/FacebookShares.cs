namespace Sobees.Library.BFacebookLibV2.Objects
{
  #region

  using Sobees.Library.BFacebookLibV2.Json;

  #endregion

  public class FacebookShares : SocialJsonObject
  {
    #region Constructors

    private FacebookShares(JsonObjectEx obj) : base(obj)
    {
    }

    #endregion

    #region Properties

    public int Count { get; private set; }

    #endregion

    #region Static methods

    public static FacebookShares Parse(JsonObjectEx obj) => new FacebookShares(obj)
    {
      Count = obj.GetInt32("id")
    };

    #endregion
  }
}