namespace Sobees.Library.BFacebookLibV2.Objects
{
  #region

  using System.Web;
  using Sobees.Library.BFacebookLibV2.Json;

  #endregion

  public class FacebookPaging : SocialJsonObject
  {
    #region Constructors

    private FacebookPaging(JsonObjectEx obj) : base(obj)
    {
    }

    #endregion

    #region Static methods

    public static FacebookPaging Parse(JsonObjectEx obj)
    {
      // TODO: Should we just return NULL if "obj" is NULL?
      if (obj == null) return new FacebookPaging(null);
      return new FacebookPaging(obj)
      {
        Previous = obj.GetString("previous"),
        Next = obj.GetString("next")
      };
    }

    #endregion

    #region Properties

    /// <summary>
    ///   A link to the previous page.
    /// </summary>
    public string Previous { get; private set; }

    /// <summary>
    ///   A link to the next page.
    /// </summary>
    public string Next { get; private set; }

    /// <summary>
    ///   The timestamp used for the <var>Previous</var> link.
    /// </summary>
    public int? Since
    {
      get
      {
        if (Previous == null) return null;
        var response = HttpUtility.ParseQueryString(Previous);
        if (response["since"] != null) return int.Parse(response["since"]);
        return null;
      }
    }

    /// <summary>
    ///   The timestamp used for the <var>Next</var> link.
    /// </summary>
    public int? Until
    {
      get
      {
        if (Next == null) return null;
        var response = HttpUtility.ParseQueryString(Next);
        if (response["until"] != null) return int.Parse(response["until"]);
        return null;
      }
    }

    #endregion
  }
}