using Sobees.Library.BGenericLib;

namespace Sobees.Library.BFacebookLibV1.Cls
{
  ///<summary>
  ///</summary>
  public class FacebookUser : User
  {
    public FacebookUser(User user)
    {
      Birthday = user.Birthday;
      BirthdayDateTimeActu = user.BirthdayDateTimeActu;
      CreatedAt = user.CreatedAt;
      FirstName = user.FirstName;
      FollowersCount = user.FollowersCount;
      FriendsCount = user.FriendsCount;
      Id = user.Id;
      IsSelected = user.IsSelected;
      LastStatus = user.LastStatus;
      Location = user.Location;
      Name = user.Name;
      NickName = user.NickName;
      Online = user.Online;
      ProfileImgUrl = user.ProfileImgUrl;
      ProfileUrl = user.ProfileUrl;
      Url = user.Url;
    }

    public FacebookUser()
    {

    }

    ///<summary>
    ///</summary>
    public string FacebookActivities { get; set; }

    ///<summary>
    ///</summary>
    public string FacebookInterest { get; set; }

    ///<summary>
    ///</summary>
    public string FacebookMovies { get; set; }

    ///<summary>
    ///</summary>
    public string FacebookMusic { get; set; }

    ///<summary>
    ///</summary>
    public string FacebookCountry { get; set; }

    ///<summary>
    ///</summary>
    public string FacebookCompany { get; set; }

    ///<summary>
    ///</summary>
    public string FacebookFeatures { get; set; }

    ///<summary>
    ///</summary>
    public string FacebookMission { get; set; }

    ///<summary>
    ///</summary>
    public string FacebookProducts { get; set; }

    public string FacebookBook { get; set; }

    public string FacebookPolitical { get; set; }

    public string FacebookTv { get; set; }

    public int WallCount { get; set; }
  }
}