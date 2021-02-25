// BFacebookLib

#region

using Facebook;

#endregion

namespace Sobees.Library.BFacebookLibV2.GraphApi
{
  public static class User
  {
    public static dynamic UserInfos(this FacebookClient fb, string idSource)
    {
      var url = string.Format("{0}", idSource);
      return fb.Get(url);
    }
  }
}