#region

using System.Collections.ObjectModel;
using Sobees.Library.BFacebookLibV2.Json;

#endregion

namespace Sobees.Library.BFacebookLibV2.Objects.Feed
{
  public class FacebookFeedCollection : SocialJsonObject
  {
    #region Properties

    public FacebookFeedEntry[] Data { get; private set; }

    public ObservableCollection<FacebookFeedEntry> FacebookFeedEntries { get; set; }

    public FacebookPaging Paging { get; private set; }

    #endregion

    #region Constructors

    private FacebookFeedCollection(JsonObjectEx obj) : base(obj)
    {
    }

    #endregion

    #region Public Methods

    #region Static methods

    public static FacebookFeedCollection Parse(JsonObjectEx obj)
    {
      if (obj == null) return null;
      var collection = new FacebookFeedCollection(obj)
      {
        Data = obj.GetArray("data", FacebookFeedEntry.Parse),
        Paging = obj.GetObject("paging", FacebookPaging.Parse)
      };
      return collection;
    }

    #endregion

    #endregion
  }
}