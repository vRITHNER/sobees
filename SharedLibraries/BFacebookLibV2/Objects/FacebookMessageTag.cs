namespace Sobees.Library.BFacebookLibV2.Objects
{
  #region

  using System.Collections.Generic;
  using Sobees.Library.BFacebookLibV2.Json;

  #endregion

  public class FacebookMessageTag : SocialJsonObject
  {
    #region Constructors

    private FacebookMessageTag(JsonObjectEx obj) : base(obj)
    {
    }

    #endregion

    #region Properties

    public string Id { get; private set; }
    public string Name { get; private set; }
    public string Type { get; private set; }
    public int Offset { get; private set; }
    public int Length { get; private set; }

    #endregion

    #region Static methods

    public static FacebookMessageTag Parse(JsonObjectEx obj)
    {
      if (obj == null) return null;
      return new FacebookMessageTag(obj)
      {
        Id = obj.GetString("id"),
        Name = obj.GetString("name"),
        Type = obj.GetString("type"),
        Offset = obj.GetInt32("offset"),
        Length = obj.GetInt32("length")
      };
    }

    public static FacebookMessageTag[] ParseMultiple(JsonObjectEx obj)
    {
      if (obj == null) return null;
      var temp = new List<FacebookMessageTag>();
      foreach (var key in obj.Keys)
      {
        temp.AddRange(obj.GetArray(key, Parse));
      }
      return temp.ToArray();
    }

    #endregion
  }
}