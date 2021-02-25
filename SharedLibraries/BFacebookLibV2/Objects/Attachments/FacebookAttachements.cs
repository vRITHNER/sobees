namespace Sobees.Library.BFacebookLibV2.Objects.Attachments
{
  using Sobees.Library.BFacebookLibV2.Json;
  using System.Collections.ObjectModel;

  ///<summary>
  ///</summary>
  public class FacebookAttachements : SocialJsonObject
  {
    #region Constructors

    public FacebookAttachements(JsonObjectEx obj) : base(obj)
    {
    }

    #endregion Constructors

    public FacebookAttachement[] Data { get; set; }

    //public ObservableCollection<FacebookAttachement> DataAsObservableCollection { get; set; }

    public FacebookPaging Paging { get; private set; }

    #region Static methods

    public static FacebookAttachements Parse(JsonObjectEx obj)
    {
      if (obj == null) return null;
      var collection = new FacebookAttachements(obj)
      {
        Data = obj.GetArray("data", FacebookAttachement.Parse),
        Paging = obj.GetObject("paging", FacebookPaging.Parse)
      };
      return collection;
    }

    #endregion Static methods
  }
}