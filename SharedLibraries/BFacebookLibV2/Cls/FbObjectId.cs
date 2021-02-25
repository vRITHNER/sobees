// BFacebookLib

#region

using Facebook;

#endregion

namespace Sobees.Library.BFacebookLibV2.GraphApi
{
  public static class FbObjectId
  {
    public static dynamic ObjectIdInfos(this FacebookClient fb, string objectid)
    {
      var url = $"{objectid}";
      return fb.Get(url);
    }
  }
}