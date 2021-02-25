namespace Sobees.Library.BFacebookLibV2.Json
{
  #region

  using Newtonsoft.Json;

  #endregion

  public abstract class SocialJsonObject
  {
    #region Constructor

    protected SocialJsonObject(JsonObjectEx obj)
    {
      JsonObjectEx = obj;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets the internal JsonObject the object was created from.
    /// </summary>
    [JsonIgnore]
    public JsonObjectEx JsonObjectEx { get; }

    #endregion

    #region Methods

    /// <summary>
    ///   Generates a JSON string representing the object.
    /// </summary>
    public virtual string ToJson()
    {
      return JsonObjectEx?.ToJson();
    }

    /// <summary>
    ///   Saves the object to a JSON file at the specified <var>path</var>.
    /// </summary>
    /// <param name="path">The path to save the file.</param>
    public virtual void SaveJson(string path)
    {
      JsonObjectEx?.SaveJson(path);
    }

    #endregion
  }
}