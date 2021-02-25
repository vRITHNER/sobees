#region

using Sobees.Library.BFacebookLibV2.Json;

#endregion

namespace Sobees.Library.BFacebookLibV2.Objects.Likes
{
  #region

  

  #endregion

  public class FacebookLikes : SocialJsonObject
  {
    #region Constructors

    private FacebookLikes(JsonObjectEx obj) : base(obj)
    {
    }

    #endregion

    #region Static methods

    public static FacebookLikes Parse(JsonObjectEx obj)
    {
      // TODO: Should we just return NULL if "obj" is NULL?
      if (obj == null) return new FacebookLikes(null) {Data = new FacebookObject[0]};
      return new FacebookLikes(obj)
      {
        Count = obj.GetInt32("count"),
        Data = obj.GetArray("data", FacebookObject.Parse) ?? new FacebookObject[0]
      };
    }

    #endregion

    #region Properties

    public int Count { get; set; }
    public FacebookObject[] Data { get; private set; }

    #endregion
  }
}