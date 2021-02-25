#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Xml;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Controls.Facebook.Cls;
using Sobees.Infrastructure.Cls;
using Sobees.Infrastructure.Controls;
using Sobees.Infrastructure.ViewModelBase;
using Sobees.Library.BFacebookLibV2.Objects.Users;
using Sobees.Library.BGenericLib;
using Sobees.Tools.Logging;

#endregion

namespace Sobees.Controls.Facebook.ViewModel
{
  public class ProfileViewModel : BFacebookViewModel, IBProfileViewModel
  {
    #region Fields

    private ProfileHomeViewModel _profileHomeViewModel;
    private FacebookUser _userDisplay;

    public override double GetRefreshTime()
    {
      return Settings.RefreshTime;
    }

    #endregion

    #region Properties

    private string _statusWall;

    public ProfileHomeViewModel ProfileHomeViewModel => _profileHomeViewModel ??
                                                        (_profileHomeViewModel = new ProfileHomeViewModel(FacebookViewModel, MessengerInstance as Messenger));

    public string StatusWall
    {
      get { return _statusWall; }
      set
      {
        _statusWall = value;
        RaisePropertyChanged();
      }
    }


    public FacebookUser CurrentUser
    {
      get { return _userDisplay; }
      set
      {
        _userDisplay = value;
        RaisePropertyChanged();
      }
    }

    public override DataTemplate DataTemplateView
    {
      get
      {
        const string DT = "<DataTemplate x:Name='dtGroupsView' " +
                          "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                          "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' " +
                          "xmlns:Views='clr-namespace:Sobees.Infrastructure.Controls;assembly=Sobees.Infrastructure'>" +
                          "<Grid HorizontalAlignment='Stretch' VerticalAlignment='Stretch'>" +
                          "<Views:UcProfile HorizontalAlignment='Stretch' VerticalAlignment='Stretch'/>" +
                          "</Grid>" +
                          "</DataTemplate>";

        var stringReader = new StringReader(DT);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
    }

    public BWorkspaceViewModel CurrentViewModel => this;

    public DataTemplate Profilcontrol
    {
      get
      {
        const string DT = "<DataTemplate x:Name='dtProfileView' " +
                          "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                          "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' " +
                          "xmlns:Views='clr-namespace:Sobees.Controls.Facebook.Views;assembly=Sobees.Controls.Facebook'>" +
                          "<Grid HorizontalAlignment='Stretch' VerticalAlignment='Stretch'>" +
                          "<Views:Profile HorizontalAlignment='Stretch' VerticalAlignment='Stretch'/>" +
                          "</Grid>" +
                          "</DataTemplate>";

        var stringReader = new StringReader(DT);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
    }

    #endregion

    #region Commands

    public RelayCommand CloseProfileCommand { get; private set; }
    public RelayCommand PostToWallCommand { get; private set; }

    #endregion

    public ProfileViewModel(FacebookViewModel model, Messenger messenger)
      : base(model, messenger)
    {
      InitCommands();
    }

    public void UpdateUser(User user)
    {
      StatusWall = string.Empty;
      CurrentUser = new FacebookUser(user);
      ProfileHomeViewModel.UpdateUser(user);
      Refresh();
    }

    protected override void InitCommands()
    {
      CloseProfileCommand = new RelayCommand(() => MessengerInstance.Send("CloseProfile"));

      PostToWallCommand = new RelayCommand(PostToWall, () => !string.IsNullOrEmpty(StatusWall));


      base.InitCommands();
    }

    private void PostToWall()
    {
      try
      {
        var test = Service.Api.Stream.Publish(StatusWall, null, null, CurrentUser.Id, -1);
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        MessengerInstance.Send(new BMessage("ShowError", e.Message));
      }
      StatusWall = string.Empty;
    }

    public override void UpdateAll()
    {
      try
      {
        var lst = new List<long>();
        if (CurrentUser == null)
        {
          EndUpdateAll();
          return;
        }
        lst.Add(long.Parse(CurrentUser.Id));
        Service.Api.Users.GetInfoAsync(lst, GetUserInfoCompleted, null);
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this,
          ex);
      }
    }

    #region Callback

    private void GetUserInfoCompleted(IList<User> users, object state, FacebookException e1)
    {
      try
      {
        if (e1 != null)
        {
          EndUpdateAll();
          MessengerInstance.Send(new BMessage("ShowError", e1.Message));
          return;
        }
        if (users != null && users.Count > 0)
        {
          if (users[0] != null)
          {
            var newUser = users[0];
#if SILVERLIGHT
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
        {
#else
            if (Application.Current == null || Application.Current.Dispatcher == null) return;
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
#endif
              var user = Service.Api.Users.GetInfo((long) newUser.uid);
              CurrentUser = new FacebookUser
              {
                Id = newUser.uid.ToString(),
                Name = newUser.last_name,
                NickName = newUser.name,
                FirstName = newUser.first_name,
                ProfileImgUrl = newUser.pic_big,
                Description = newUser.about_me,
                FacebookActivities = newUser.activities,
                BirthdayDateTimeActu = Convert.ToDateTime(newUser.birthday_date),
                Birthday = newUser.birthday,
                FacebookBook = newUser.books,
                Location =
                  newUser.current_location == null
                    ? null
                    : $"{newUser.current_location.street} {newUser.current_location.zip} {newUser.current_location.city} {newUser.current_location.state} {newUser.current_location.country}",
                FacebookInterest = newUser.interests,
                FacebookMovies = newUser.movies,
                FacebookMusic = newUser.music,
                //Online = user. newUser.online_presence == "active",
                FacebookPolitical = newUser.political,
                ProfileUrl = newUser.profile_url,
                LastStatus =
                  newUser.status == null
                    ? null
                    : new Entry
                    {
                      Title = newUser.status.message,
                      PubDate =
                        FbHelper.ConvertUniversalTimeToDate(newUser.status.time)
                    },
                FacebookTv = newUser.tv,
                WallCount = newUser.wall_count == null
                  ? 0
                  : (int) newUser.wall_count,
              };

            }));
          }
          EndUpdateAll();
        }
        else
        {
          if (users == null) return;
          if (CurrentUser == null)
          {
            EndUpdateAll();
            return;
          }
          var lstFields = new List<string>
          {
            "name",
            "pic_small",
            "pic_big",
            "pic_square",
            "pic",
            "pic_large",
            "page_url",
            "type",
            "website",
            "has_added_app",
            "founded",
            "company_overview",
            "mission",
            "products",
            "location",
            "parking",
            "public_transit",
            "hours",
            "attire",
            "payment_options",
            "culinary_team",
            "general_manager",
            "price_range",
            "restaurant_services",
            "restaurant_specialties",
            "release_date",
            "genre",
            "starring",
            "screenplay_by",
            "directed_by",
            "produced_by",
            "studio",
            "awards",
            "plot_outline",
            "network",
            "season",
            "schedule",
            "written_by",
            "band_members",
            "hometown",
            "current_location",
            "record_label",
            "booking_agent",
            "press_contact",
            "artists_we_like",
            "influences",
            "band_interests",
            "bio",
            "affiliation",
            "birthday",
            "personal_info",
            "personal_interests",
            "members",
            "built",
            "features",
            "mpg",
            "general_info",
            "fan_count"
          };
          Service.Api.Pages.GetInfoAsync(lstFields, new List<long> {long.Parse(CurrentUser.Id)},
            SobeesSettings.Accounts[
              SobeesSettings.Accounts.IndexOf(new UserAccount(Settings.UserName, EnumAccountType.Facebook))].UserId,
            GetPageInfoCompleted, null);
        }
      }
      catch (Exception ex)
      {
        EndUpdateAll();
        TraceHelper.Trace(this,
          ex);
      }
    }

    private void GetPageInfoCompleted(IList<Page> pages, object state, FacebookException e)
    {
      try
      {
        if (e != null)
        {
          EndUpdateAll();
          MessengerInstance.Send(new BMessage("ShowError", e.Message));
          return;
        }
        if (pages != null && pages.Count > 0)
        {
          if (pages[0] != null)
          {
            var newUser = pages[0];
            if (Application.Current == null || Application.Current.Dispatcher == null) return;
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
              CurrentUser = new FacebookUser
              {
                Id = newUser.page_id.ToString(),
                Name = newUser.name,
                NickName = newUser.type,
                ProfileImgUrl = newUser.pic_big,
                Description = newUser.general_info,
                FacebookCompany = newUser.company_overview,
                FacebookFeatures = newUser.features,
                FacebookMission = newUser.mission,
                FacebookProducts = newUser.products,
                Birthday = newUser.birthday,
                Location = newUser.current_location,
                ProfileUrl = newUser.page_url,
                LastStatus =
                  newUser.status == null
                    ? null
                    : new Entry
                    {
                      Title = newUser.status.message,
                      PubDate =
                        FbHelper.ConvertUniversalTimeToDate(newUser.status.time)
                    },
              };
            }));
          }
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this,
          ex);
      }
      EndUpdateAll();
    }

    #endregion
  }
}