
using Sobees.Library.BGenericLib;

namespace Sobees.Library.BTwitterLib
{
  public class TwitterUser : User
  {
    public Geoloc Geolocation { get; set; }
    public int StatusUseCount { get; set; }
    public new TwitterEntry LastStatus { get; set; }
    public int NbFavorites { get; set; }
    public bool IsProtected { get; set; }
    public bool IsVerified { get; set; }
    public string UserTimeZone { get; set; }
    public int UtcOffset { get; set; }
    public bool IsFollowing { get; set; }
    public string Lang { get; set; }
  }
}