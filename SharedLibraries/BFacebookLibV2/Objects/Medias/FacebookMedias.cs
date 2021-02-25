namespace Sobees.Library.BFacebookLibV2.Objects.Medias
{
  #region

  using Sobees.Library.BFacebookLibV2.Json;
  using System.Collections.ObjectModel;

  #endregion

  /// <summary>
  /// </summary>
  public class FacebookMedias : SocialJsonObject
  {
    #region Constructors

    public FacebookMedias(JsonObjectEx obj) : base(obj)
    {
    }

    #endregion Constructors

    public FacebookMedia[] Data { get; private set; }

    public ObservableCollection<FacebookMedia> Medias { get; set; }

    public FacebookPaging Paging { get; private set; }

    #region Static methods

    public static FacebookMedias Parse(JsonObjectEx obj)
    {
      if (obj == null) return null;
      var collection = new FacebookMedias(obj)
      {
        Data = obj.GetArray("data", FacebookMedia.Parse),
        Paging = obj.GetObject("paging", FacebookPaging.Parse)
      };
      return collection;
    }

    #endregion Static methods
  }
}