#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using System.Xml;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Controls.Twitter.Cls;
using Sobees.Configuration.BGlobals;
using Sobees.Infrastructure.Cls;
using Sobees.Infrastructure.Controls;
using Sobees.Infrastructure.ViewModelBase;
using Sobees.Library.BGenericLib;
using Sobees.Library.BTwitterLib;
using Sobees.Tools.Logging;

#endregion

namespace Sobees.Controls.Twitter.ViewModel
{
  public class ProfileViewModel : TwitterWorkspaceViewModel, IBProfileViewModel
  {
    #region Fields

    private TwitterUser _currentUser;
    private int _selectedTweetIndex = -1;

    #endregion Fields

    #region Properties

    private bool _isFriends;
    public ObservableCollection<TwitterList> ListsOwn { get; set; }

    public ObservableCollection<TwitterList> ListsInside { get; set; }

    public ObservableCollection<TwitterList> ListsFollow { get; set; }

    public ObservableCollection<TwitterList> ListsToShow { get; set; }

    public override DataTemplate DataTemplateView
    {
      get
      {
        const string dt = "<DataTemplate x:Name='dtGroupsView' " +
                          "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                          "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' " +
                          "xmlns:Views='clr-namespace:Sobees.Infrastructure.Controls;assembly=Sobees.Infrastructure'>" +
                          "<Grid HorizontalAlignment='Stretch' VerticalAlignment='Stretch'>" +
                          "<Views:UcProfile HorizontalAlignment='Stretch' VerticalAlignment='Stretch'/>" +
                          "</Grid>" +
                          "</DataTemplate>";

        var stringReader = new StringReader(dt);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
    }

    public DataTemplate Profilcontrol
    {
      get
      {
        const string dt = "<DataTemplate x:Name='dtGroupsView' " +
                          "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                          "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' " +
                          "xmlns:Views='clr-namespace:Sobees.Controls.Twitter.Views;assembly=Sobees.Controls.Twitter'>" +
                          "<Grid HorizontalAlignment='Stretch' VerticalAlignment='Stretch'>" +
                          "<Views:Profile HorizontalAlignment='Stretch' VerticalAlignment='Stretch'/>" +
                          "</Grid>" +
                          "</DataTemplate>";

        var stringReader = new StringReader(dt);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
    }

    public BWorkspaceViewModel CurrentViewModel => this;

    public new Visibility TweetsDetailsVisibility => Visibility.Collapsed;

    public new int SelectedTweetIndex
    {
      get { return _selectedTweetIndex; }
      set { _selectedTweetIndex = value; }
    }

    public bool IsFriends
    {
      get { return _isFriends; }
      set
      {
        _isFriends = value;
        RaisePropertyChanged("IsFriends");
      }
    }

    public TwitterUser CurrentUser
    {
      get { return _currentUser; }
      set
      {
        _currentUser = value;
        RaisePropertyChanged("CurrentUser");
      }
    }

    #endregion Properties

    #region Commands

    public RelayCommand CloseProfileCommand { get; private set; }

    public RelayCommand FollowCommand { get; private set; }

    public RelayCommand ShowTweetsCommand { get; private set; }

    public RelayCommand ShowOwnListCommand { get; private set; }

    public RelayCommand ShowListsFollowingCommand { get; private set; }

    public RelayCommand ShowListsInsideCommand { get; private set; }

    #endregion Commands

    public ProfileViewModel(TwitterViewModel model, Messenger messenger, TwitterWorkspaceSettings settings)
      : base(model, messenger, settings)
    {
      InitCommands();
    }

    public ProfileViewModel(TwitterViewModel model, Messenger messenger)
      : base(model, messenger, null)
    {
      InitCommands();
      ListsFollow = new ObservableCollection<TwitterList>();
      ListsInside = new ObservableCollection<TwitterList>();
      ListsToShow = new ObservableCollection<TwitterList>();
      ListsOwn = new ObservableCollection<TwitterList>();
    }

    protected override void InitCommands()
    {
      ShowTweetsCommand = new RelayCommand(ShowTweets);
      ShowListsFollowingCommand = new RelayCommand(ShowListsFollowing);
      ShowListsInsideCommand = new RelayCommand(ShowListsInside);
      ShowOwnListCommand = new RelayCommand(ShowOwnList);
      FollowCommand = new RelayCommand(Follow);
      CloseProfileCommand = new RelayCommand(() => MessengerInstance.Send("CloseProfile"));
      base.InitCommands();
    }

    /// <summary>
    /// Follow
    /// </summary>
    private void Follow()
    {
      using (var worker = new BackgroundWorker())
      {
        string errorMsg;

        worker.DoWork += async delegate(object s,
                                  DoWorkEventArgs args)
                           {
                             if (worker.CancellationPending)
                             {
                               args.Cancel = true;
                               return;
                             }

                             try
                             {
                               await TwitterLibV11.Follow(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET,
                                                    CurrentAccount.SessionKey, CurrentAccount.Secret,
                                                    CurrentUser.NickName, CurrentUser.IsFollowing);

                               CurrentUser.IsFollowing = TwitterLibV11.FriendshipExists(BGlobals.TWITTER_OAUTH_KEY,
                                                                                        BGlobals.TWITTER_OAUTH_SECRET,
                                                                                        CurrentAccount.SessionKey,
                                                                                        CurrentAccount.Secret,
                                                                                        CurrentAccount.Login,
                                                                                        CurrentUser.NickName,
                                                                                        out errorMsg,
                                                                                        ProxyHelper
                                                                                          .GetConfiguredWebProxy(
                                                                                            SobeesSettings));
                               IsFriends = CurrentUser.IsFollowing;
                             }
                             catch (Exception ex)
                             {
                               TraceHelper.Trace(this, ex);
                             }
                           };

        worker.RunWorkerAsync();
      }
    }

    private void ShowOwnList()
    {
      if (ListsOwn.Count == 0)
      {
        string error;

        var lst = TwitterLibV11.GetListOwn(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET,
                                             CurrentAccount.SessionKey, CurrentAccount.Secret, CurrentUser.NickName,
                                             out error, ProxyHelper.GetConfiguredWebProxy(SobeesSettings));
        foreach (var list in lst)
        {
          ListsOwn.Add(list);
        }
      }
      ListsToShow.Clear();
      foreach (var list in ListsOwn)
      {
        ListsToShow.Add(list);
      }
    }

    /// <summary>
    /// ShowListsInside
    /// </summary>
    private void ShowListsInside()
    {
      if (ListsInside.Count == 0)
      {
        string error;

        var lst = TwitterLibV11.GetListMembership(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET,
                                                    CurrentAccount.SessionKey, CurrentAccount.Secret,
                                                    CurrentUser.NickName, out error,
                                                    ProxyHelper.GetConfiguredWebProxy(SobeesSettings));
        foreach (var list in lst)
        {
          ListsInside.Add(list);
        }
      }
      ListsToShow.Clear();
      foreach (var list in ListsInside)
      {
        ListsToShow.Add(list);
      }
    }

    /// <summary>
    ///   ShowListsFollowing
    /// </summary>
    private void ShowListsFollowing()
    {
      if (ListsFollow.Count == 0)
      {
        string error;

        var lists = TwitterLibV11.GetList(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET,
                                          CurrentAccount.SessionKey, CurrentAccount.Secret, CurrentUser.NickName,
                                          out error, ProxyHelper.GetConfiguredWebProxy(SobeesSettings));
        lists.ForEach(list => ListsFollow.Add(list));
      }

      ListsToShow.Clear();
      foreach (var list in ListsFollow)
        ListsToShow.Add(list);
    }

    /// <summary>
    ///   ShowTweets
    /// </summary>
    private void ShowTweets()
    {
      if (TweetsShow.Count == 0)
      {
        using (var worker = new BackgroundWorker())
        {
          var userTweets = new List<TwitterEntry>();
          string errorMsg = null;

          worker.DoWork += delegate(object s,
                                    DoWorkEventArgs args)
                             {
                               if (worker.CancellationPending)
                               {
                                 args.Cancel = true;
                                 return;
                               }

                               userTweets = TwitterLibV11.GetUser(BGlobals.TWITTER_OAUTH_KEY,
                                                                  BGlobals.TWITTER_OAUTH_SECRET,
                                                                  CurrentAccount.SessionKey, CurrentAccount.Secret,
                                                                  CurrentUser.NickName,
                                                                  BGlobals.DEFAULT_NB_POST_TO_GET_Twitter,
                                                                  out errorMsg,
                                                                  ProxyHelper.GetConfiguredWebProxy(SobeesSettings));
                             };

          worker.RunWorkerCompleted += delegate
                                         {
                                           OnLoadUserTweetsAsyncCompleted(userTweets,
                                                                          errorMsg);
                                         };

          worker.RunWorkerAsync();
        }
      }
    }

    private void OnLoadUserTweetsAsyncCompleted(IEnumerable<TwitterEntry> userTweets, string errorMsg)
    {
      IsWaiting = false;

      if (!string.IsNullOrEmpty(errorMsg))
      {
        MessengerInstance.Send(new BMessage("error", errorMsg));
        return;
      }

      if (userTweets == null) return;
      foreach (var tweet in userTweets.Where(tweet => !TweetsShow.Contains(tweet)))
        TweetsShow.Add(tweet);
      
      IsAnyDataVisibility = TweetsShow.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
    }

    public void ShowUser(TwitterUser user)
    {
      try
      {
        if (Tweets != null) Tweets.Clear();
        if (TweetsShow != null) TweetsShow.Clear();
        CurrentUser = user;
        if (CurrentUser.ProfileImgUrl != null)
          if (!CurrentUser.ProfileImgUrl.ToLower().Contains("default_profile"))
          {
            if (!string.IsNullOrEmpty(CurrentUser.ProfileImgUrl) && CurrentUser.ProfileImgUrl.Contains("_normal"))
              CurrentUser.ProfileImgUrl = CurrentUser.ProfileImgUrl.Replace("_normal",
                                                                            string.Empty);
          }

        using (var worker = new BackgroundWorker())
        {
          var userProfile = new User();
          CurrentUser.IsFollowing = false;
          IsFriends = CurrentUser.IsFollowing;
          string errorMsg = null;
          string errorMsgAreFriends = null;

          worker.DoWork += delegate(object s,
                                    DoWorkEventArgs args)
                             {
                               if (worker.CancellationPending)
                               {
                                 args.Cancel = true;
                                 return;
                               }

                               try
                               {
                                 if (string.IsNullOrEmpty(CurrentUser.Name))
                                 {
                                   //var user2 = TwitterLib.GetUserInfo(CurrentUser.NickName, true, out errorMsgAreFriends) as TwitterUser;
                                   var user2 = TwitterLibV11.GetUserInfo(BGlobals.TWITTER_OAUTH_KEY,
                                                                         BGlobals.TWITTER_OAUTH_SECRET,
                                                                         CurrentAccount.SessionKey,
                                                                         CurrentAccount.Secret, CurrentUser.NickName,
                                                                         true, out errorMsgAreFriends,
                                                                         ProxyHelper.GetConfiguredWebProxy(
                                                                           SobeesSettings));
                                   if (user2 != null)
                                   {
                                     CurrentUser.Birthday = user2.Birthday;
                                     CurrentUser.BirthdayDateTimeActu = user2.BirthdayDateTimeActu;
                                     CurrentUser.CreatedAt = user2.CreatedAt;
                                     CurrentUser.Description = user2.Description;
                                     CurrentUser.FirstName = user2.FirstName;
                                     CurrentUser.FollowersCount = user2.FollowersCount;
                                     CurrentUser.FriendsCount = user2.FriendsCount;
                                     CurrentUser.Id = user2.Id;
                                     CurrentUser.IsSelected = user2.IsSelected;
                                     CurrentUser.Location = user2.Location;
                                     CurrentUser.Name = user2.Name;
                                     CurrentUser.Online = user2.Online;
                                     CurrentUser.ProfileImgUrl = user2.ProfileImgUrl;
                                     CurrentUser.ProfileUrl = user2.ProfileUrl;
                                     CurrentUser.Url = user2.Url;
                                     CurrentUser.IsFollowing = false;
                                     CurrentUser.Geolocation = user2.Geolocation;
                                     CurrentUser.StatusUseCount = user2.StatusUseCount;
                                     CurrentUser.LastStatus = user2.LastStatus;
                                     CurrentUser.NbFavorites = user2.NbFavorites;
                                     CurrentUser.IsProtected = user2.IsProtected;
                                     CurrentUser.IsVerified = user2.IsVerified;
                                     CurrentUser.UserTimeZone = user2.UserTimeZone;
                                     CurrentUser.UtcOffset = user2.UtcOffset;
                                     CurrentUser.Lang = user2.Lang;
                                     IsFriends = CurrentUser.IsFollowing;

                                     RaisePropertyChanged("CurrentUser");
                                   }
                                 }

                                 CurrentUser.IsFollowing = TwitterLibV11.FriendshipExists(BGlobals.TWITTER_OAUTH_KEY,
                                                                                            BGlobals
                                                                                              .TWITTER_OAUTH_SECRET,
                                                                                            CurrentAccount.SessionKey,
                                                                                            CurrentAccount.Secret,
                                                                                            CurrentAccount.Login,
                                                                                            user.NickName,
                                                                                            out errorMsgAreFriends,
                                                                                            ProxyHelper
                                                                                              .GetConfiguredWebProxy(
                                                                                                SobeesSettings));
                                 IsFriends = CurrentUser.IsFollowing;
                               }
                               catch (Exception e)
                               {
                                 Console.WriteLine(e);
                               }
                             };

          worker.RunWorkerAsync();
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }
  }
}