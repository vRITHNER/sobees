namespace Sobees.Library.BFacebookLibV2.Objects.Token
{
  using System;
  using Sobees.Library.BFacebookLibV2.Json;
  using Sobees.Library.BFacebookLibV2.Scope;

  public class DebugToken : SocialJsonObject
  {

    #region Properties

    public string App_id { get; internal set; }

    public string Application { get; internal set; }

    public DateTime Expire_At { get; internal set; }

    public bool Is_Valid { get; internal set; }

    public DateTime Issued_At { get; internal set; }

    public FacebookObject[] Metadata { get; internal set; }

    public JsonArrayEx Scopes { get; internal set; }

    public string User_Id { get; internal set; }
    
    #endregion 

    #region Constructors

    private DebugToken(JsonObjectEx obj) : base(obj) { }

    #endregion

    #region Static methods

    /// <summary>
    /// Parse the JSON object of an account.
    /// </summary>
    /// <param name="path">The JSON object.</param>
    /// <returns></returns>
    public static DebugToken Parse(string path)
    {
      if (path == null) return null;
      var obj = Json.JsonObjectEx.ParseJson(path);
      if (obj == null) return null;
      obj = obj.GetObject("data");
      return new DebugToken(obj)
      {
        App_id = obj.GetString("app_id"),
        Application = obj.GetString("application"),
        Expire_At = obj.GetDateTimeFromUnixTimestamp("expires_at"),
        Is_Valid = obj.GetBoolean("is_valid"),
        Issued_At = obj.GetDateTimeFromUnixTimestamp("issued_at"),
        Metadata = obj.GetArray("metadata", FacebookObject.Parse),
        Scopes = obj.GetArray("scopes"),
        User_Id = obj.GetString("user_id"),
      };
    }

    #endregion

  }

}