#region

using Sobees.Library.BFacebookLibV2.Json;

#endregion

namespace Sobees.Library.BFacebookLibV2.Comments
{
  #region

  

  #endregion

  public class FacebookComments : SocialJsonObject
  {
    #region Constructors

    private FacebookComments(JsonObjectEx obj) : base(obj)
    {
    }

    #endregion

    /// <summary>
    ///   Gets the total amounbt of comments. This value might not always be
    ///   present in the API response - in such cases the count will be zero.
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    ///   An array of comments. For a post, photo or similar with many
    ///   comments, this array will only be a subset of all comments.
    /// </summary>
    public FacebookCommentSummary[] Data { get; private set; }

    //public ObservableCollection<FacebookCommentSummary> DataAsObservableCollection { get { return Data.ToObservableCollection(); } }

    public static FacebookComments Parse(JsonObjectEx obj)
    {
      if (obj == null) return new FacebookComments(null) {Data = new FacebookCommentSummary[0]};
      return new FacebookComments(obj)
      {
        Count = obj.GetInt32("count"),
        Data = obj.GetArray("data", FacebookCommentSummary.Parse) ?? new FacebookCommentSummary[0]
      };
    }
  }
}