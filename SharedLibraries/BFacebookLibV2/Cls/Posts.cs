// BFacebookLib

#region

using Facebook;

#endregion

namespace Sobees.Library.BFacebookLibV2.GraphApi
{
  public static class FbPosts
  {
    /// <summary>
    /// Retourne poste d'une source
    /// </summary>
    /// <param name="fb">Contexte </param>
    /// <param name="idSource">id facebook </param>
    /// <param name="limit">le nombre de message</param>
    /// <param name="since">une date format timestamp</param>
    /// <returns>Message d'une source au format json </returns>
    public static dynamic SourcePosts(this FacebookClient fb, string idSource, int limit = 20, string since = BFacebookLibGlobal.FACEBOOK_NULL_LASTENTRY)
    {
      //format last entry
      var date = (since == BFacebookLibGlobal.FACEBOOK_NULL_LASTENTRY) ? string.Empty : "&since=" + since;
      //format graph api url
      var url = string.Format("{0}/posts?format=json&limit={1}{2}", idSource, limit, date);

      var posts = fb.Get(url);
      //Return post with graph api
      return posts;
    }
  }
}