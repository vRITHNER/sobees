#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using BUtility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Configuration.BGlobals;
using Sobees.Controls.Twitter.Cls;
using Sobees.Infrastructure.Cls;
using Sobees.Infrastructure.Commands;
using Sobees.Infrastructure.Controls;
using Sobees.Infrastructure.UI;
using Sobees.Library.BGenericLib;
using Sobees.Library.BLocalizeLib;
using Sobees.Library.BTwitterLib;
using Sobees.Library.BTwitterLib.Response;
using Sobees.Tools.Logging;
using Sobees.Tools.Threading.Extensions;
using Sobees.Tools.Web;

#endregion

namespace Sobees.Controls.Twitter.ViewModel
{
  public class TwitterWorkspaceViewModel : BTwitterViewModel
  {
    #region Fields

    public bool IsSelected;
    private DateTime _doNotTakeTweetOlderThanThis;
    private int _selectedTweetIndex = -1;
    private Entry _tweetToShow;
    private ObservableCollection<Entry> _tweetsShow;

    #endregion Fields

    #region Properties

    private string _title;
    private ObservableCollection<Entry> _tweets;
    private Visibility _tweetsDetailsVisibility = Visibility.Collapsed;
    //private DeferredAction deferredAction;
    //private DeferredAction _deferredActionStats;

    public int SelectedTweetIndex
    {
      get => _selectedTweetIndex;
      set
      {
        _selectedTweetIndex = value;
        if (_selectedTweetIndex <= -1 || _selectedTweetIndex >= TweetsShow.Count()) return;
        try
        {
          TweetToShow = TweetsShow[_selectedTweetIndex];
        }
        catch (Exception ex)
        {
          TraceHelper.Trace(this, ex);
        }
      }
    }

    public Entry TweetToShow
    {
      get => _tweetToShow;
      set
      {
        _tweetToShow = value;
        Conversations.Clear();
        try
        {
          UpdateConversation();
        }
        catch (Exception e)
        {
          Console.WriteLine(e);
        }
        RaisePropertyChanged();
      }
    }

    public bool TweetsDetailOpen => TweetsDetailsVisibility == Visibility.Visible;

    public Visibility TweetsDetailsVisibility
    {
      get => _tweetsDetailsVisibility;
      set
      {
        _tweetsDetailsVisibility = value;
        RaisePropertyChanged();
        RaisePropertyChanged(() => TweetsDetailOpen);
      }
    }

    public ObservableCollection<Entry> NewEntries { get; set; }

    public ObservableCollection<Entry> Tweets
    {
      get => _tweets ?? (_tweets = new ObservableCollection<Entry>());
      set => _tweets = value;
    }

    public string Title
    {
      get
      {
        if (WorkspaceSettings == null) return string.Empty;
        switch (WorkspaceSettings.Type)
        {
          case EnumTwitterType.List:
            _title = WorkspaceSettings.GroupName;
            break;

          case EnumTwitterType.Groups:
            _title = WorkspaceSettings.GroupName;
            break;

          case EnumTwitterType.User:
            _title = new LocText("Sobees.Configuration.BGlobals:Resources:btnSent").ResolveLocalizedValue();
            break;

          case EnumTwitterType.DirectMessages:
            _title = new LocText("Sobees.Configuration.BGlobals:Resources:btnDirectMessages").ResolveLocalizedValue();
            break;

          case EnumTwitterType.Friends:
            _title = new LocText("Sobees.Configuration.BGlobals:Resources:rdTabHome").ResolveLocalizedValue();
            break;

          case EnumTwitterType.Replies:
            _title = new LocText("Sobees.Configuration.BGlobals:Resources:btnReplies").ResolveLocalizedValue();
            break;

          case EnumTwitterType.Favorites:
            _title = new LocText("Sobees.Configuration.BGlobals:Resources:btnFavorites").ResolveLocalizedValue();
            break;

          default:
            _title = WorkspaceSettings.Type.ToString();
            break;
        }
        return _title;
      }
    }

    public ObservableCollection<TwitterList> TwitterListDisplay => _twitterViewModel.TwitterListDisplay;

    public ObservableCollection<Entry> TweetsShow
    {
      get => _tweetsShow ?? (_tweetsShow = new ObservableCollection<Entry>());
      set => _tweetsShow = value;
    }

    //private void UserAction(EnumEventType action)
    //{
    //  try
    //  {
    //    if (TweetToShow == null) return;
    //    var previous = SelectedTweetIndex > 0 ? TweetsShow[_selectedTweetIndex - 1].Id : "0";
    //    var next = SelectedTweetIndex == TweetsShow.Count - 1 ? "0" : TweetsShow[_selectedTweetIndex + 1].Id;

    //    StatsHelper.ReadContent(Settings.UserName, EnumServiceType.TWITTER, TweetToShow.Id, ((TwitterEntry)TweetToShow).User.Id, action, previous,
    //                            TweetToShow.Id, next);
    //  }
    //  catch (Exception)
    //  {
    //  }
    //}

    #endregion Properties

    #region Constructors

    public TwitterWorkspaceViewModel(TwitterViewModel model, TwitterWorkspaceSettings settings, Messenger messenger)
      : base(model, messenger, settings)
    {
      Init();
    }

    public TwitterWorkspaceViewModel(TwitterViewModel model, Messenger messenger, TwitterWorkspaceSettings settings)
      : base(model, messenger, settings)
    {
    }

    #endregion Constructors

    #region Commands

    private ObservableCollection<TwitterEntry> _conversations;
    private BRelayCommand _menuActionCommand;

    /// <summary>
    /// </summary>
    public RelayCommand<TwitterUser> ShowUserProfileCommand { get; set; }

    public RelayCommand ClearTweetsCommand { get; private set; }

    public RelayCommand OpenTweetCommand { get; private set; }

    public RelayCommand<TwitterEntry> ReplyCommand { get; set; }

    public RelayCommand<TwitterEntry> ReTweetCommand { get; set; }

    public RelayCommand<TwitterEntry> GoToReplyToTweetCommand { get; set; }

    public RelayCommand<TwitterEntry> DMCommand { get; set; }

    public RelayCommand<TwitterEntry> GoToTweetCommand { get; set; }

    public RelayCommand CloseDetailsCommand { get; set; }

    public RelayCommand ShowDetailsCommand { get; set; }

    public RelayCommand<TwitterEntry> ShowFullDetailsCommand { get; set; }

    public BRelayCommand MenuActionCommand => _menuActionCommand ?? (_menuActionCommand = new BRelayCommand(MenuAction));

    #endregion Commands

    #region Methods

    private List<Entry> _listAds;

    protected List<Entry> ListAds
    {
      get => _listAds ?? (_listAds = new List<Entry>());
      set => _listAds = value;
    }

    #endregion Ads

    private readonly object _listLock = new object();
    private readonly SemaphoreSlim _syncLock = new SemaphoreSlim(1);
    //private readonly Mutex _syncMutex = new Mutex(false);

    public ObservableCollection<TwitterEntry> Conversations
    {
      get
      {
        if (_conversations != null) return _conversations;
        _conversations = new ObservableCollection<TwitterEntry>();
        BindingOperations.EnableCollectionSynchronization(Conversations, _listLock);
        RaisePropertyChanged();
        return _conversations;
      }
    }

    private void AddToAlerts(Entry entry)
    {
      try
      {
        switch (
          CurrentAccount.TypeAlertsFB)
        {
          case EnumAlertsFacebookType.All:
            NewEntries.Add(entry);
            break;

          case EnumAlertsFacebookType.Advanced:

            //Don't show messagewith removed word
            if ((WorkspaceSettings.Type == EnumTwitterType.DirectMessages && CurrentAccount.IsCheckedUseAlertsDM)
                ||
                (WorkspaceSettings.Type == EnumTwitterType.Friends && CurrentAccount.IsCheckedUseAlertsFriends)
                ||
                (WorkspaceSettings.Type == EnumTwitterType.Groups && CurrentAccount.IsCheckedUseAlertsGroups)
                ||
                (WorkspaceSettings.Type == EnumTwitterType.Replies && CurrentAccount.IsCheckedUseAlertsReply))
            {
              var skip = false;
              if (!CurrentAccount.IsCheckedUseAlertsRemovedWords &&
                  !CurrentAccount.IsCheckedUseAlertsUsers &&
                  !CurrentAccount.IsCheckedUseAlertsWords)
              {
                NewEntries.Add(entry);
                break;
              }
              if (CurrentAccount.IsCheckedUseAlertsRemovedWords)
              {
                if (CurrentAccount.AlertsRemovedWordsList.Any(word => entry.Title.ToLower().Contains(word.ToLower())))
                  skip = true;
              }
              if (!skip && CurrentAccount.IsCheckedUseAlertsWords)
              {
                if (CurrentAccount.AlertsWordsList.Any(word => entry.Title.ToLower().Contains(word.ToLower())))
                {
                  NewEntries.Add(entry);
                  skip = true;
                }
              }
              if (!skip &&
                  CurrentAccount.IsCheckedUseAlertsUsers)
              {
                if (CurrentAccount.AlertsUsersList.Any(user => entry.User.NickName.ToLower().Contains(user.ToLower())))

                  NewEntries.Add(entry);
              }
            }
            break;

          case EnumAlertsFacebookType.No:
            break;
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this,
          ex);
      }
    }

    /// <summary>
    ///   UpdateConversation
    /// </summary>
    private void UpdateConversation()
    {
      try
      {
        Conversations.Clear();
        var entry = TweetToShow as TwitterEntry;
        if (TweetToShow == null || entry == null) return;
        if (!string.IsNullOrEmpty(entry.InReplyTo))
        {
          var id = entry.InReplyTo;

          Action mainAction = async () =>
          {
            try
            {
              while (!string.IsNullOrEmpty(id) && id != "0")
              {
                if (Conversations.Count() > 4) break;

                var result = await TwitterLibV11.GetTweetInfo(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET,
                  CurrentAccount.SessionKey, CurrentAccount.Secret, id);
                if (!result.BisSuccess)
                  return;

                var tweets = result.DataResult;
                if (tweets == null || !tweets.Any())
                  return;

                //TraceHelper.Trace(APPNAME, string.Format("TwitterWorkSpaceViewModel::UpdateConversations::Id:{0}||Add tweet to conversation feed:{1}", id, tweets[0].Title), true);

                Conversations.Add(tweets[0]);
                id = tweets[0].InReplyTo;
              }
            }
            catch (Exception ex)
            {
              TraceHelper.Trace(this, ex);
            }
          };

          TraceHelper.Trace(APPNAME, $"TwitterWorkSpaceViewModel::UpdateConversations::Id:{id}", true);

          var task = Task.Factory.StartNew(mainAction, CancellationToken.None, TaskCreationOptions.AttachedToParent, UiContext.Instance.Current);
          task.Wait();
        }
        else if (!string.IsNullOrEmpty(entry.InReplyToUserName))
        {
          Action mainAction = () =>
          {
            try
            {
              string errorMsg;
              var tweets =
                TwitterLib.SearchSummize(
                  string.Format("{0} OR {1}", $"from:{entry.User.NickName} to:{entry.InReplyToUserName}",
                                $"from:{entry.InReplyToUserName} to:{entry.User.NickName}"), EnumLanguages.all,
                  Settings.NbPostToGet, string.Empty, out errorMsg);
              if (!string.IsNullOrEmpty(errorMsg))
              {
                MessengerInstance.Send(new BMessage("ShowError", errorMsg));
                return;
              }
              if (tweets == null || !tweets.Any())
                return;

              foreach (var tweet in tweets)
                Conversations.Insert(0, tweet);
            }
            catch (Exception ex)
            {
              TraceHelper.Trace(this, ex);
            }
          };

          var task = Task.Factory.StartNew(mainAction, CancellationToken.None, TaskCreationOptions.AttachedToParent, UiContext.Instance.Current);
          task.Wait();
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    public override void ShowUserProfile(User user)
    {
      if (user == null)
      {
        TraceHelper.Trace(this, "User was NULL!");
        return;
      }
      if (user.Id == null)
      {
        Task.Run(() => WebHelper.NavigateToUrl(user.ProfileUrl));
        return;
      }
      MessengerInstance.Send(new BMessage("ShowProfile", user));
    }

    public override void ShowUserProfile(string user)
    {
      MessengerInstance.Send(new BMessage("ShowProfile", new TwitterUser {NickName = user}));
    }

    /// <summary>
    ///   UpdateView
    /// </summary>
    public void UpdateView()
    {
      BLogManager.LogEntry(APPNAME, "UpdateView", "START", true);

      Action mainAction = async () =>
      {
        try
        {
          if (Tweets == null) return;

          if (!Tweets.Any())
          {
            BLogManager.LogEntry(APPNAME, "UpdateView", "TweetsShow.Clear()", true);
            TweetsShow.Clear();
            return;
          }

          BLogManager.LogEntry(APPNAME, "UpdateView", "UpdateNumberPost(Tweets)", true);
          //Update number post list Tweets

          lock(_syncLock)
          {
            //await _syncLock.WaitAsync(); //Only 1 thread can access the function or functions that use this lock, others trying to access - will wait until the first one released.
            //Do your stuff..
            UpdateNumberPost(Tweets);

            var i = 0;
            while (i < TweetsShow.Count())
            {
              if (!Tweets.Contains(TweetsShow[i]) || (!string.IsNullOrEmpty(SobeesSettings.Filter) &&
                                                      !(TweetsShow[i].Title.IndexOf(SobeesSettings.Filter,
                                                        StringComparison.InvariantCultureIgnoreCase) >= 0)))
              {
                TweetsShow.RemoveAt(i);
              }
              else
              {
                TweetsShow[i].PubDate = TweetsShow[i].PubDate;
                i++;
              }
            }

            i = 0;
            while (i < Tweets.Count())
            {
              if (string.IsNullOrEmpty(SobeesSettings.Filter) ||
                  Tweets[i].Title.IndexOf(SobeesSettings.Filter,
                    StringComparison.InvariantCultureIgnoreCase) >= 0)
              {
                if (!TweetsShow.Contains(Tweets[i]))
                {
                  var pos = 0;
                  while (pos < TweetsShow.Count() && Tweets[i].PubDate < TweetsShow[pos].PubDate)
                    pos++;

                  var i1 = i;
                  TweetsShow.Insert(pos, Tweets[i1]);
                }
              }
              i++;
            }

            IsAnyDataVisibility = TweetsShow.Any() ? Visibility.Collapsed : Visibility.Visible;

            //UpdateVisibility of details view
            if (TweetsDetailsVisibility != Visibility.Visible) return;
            if (TweetToShow == null || !TweetsShow.Any())
              TweetsDetailsVisibility = Visibility.Collapsed;
            else
            {
              if (!TweetsShow.Contains(TweetToShow))
                SelectedTweetIndex = 0;
            }
          }
        }
        catch (Exception e)
        {
          TraceHelper.Trace(this, e);
          EndUpdateAll();
        }
      };

      
      var task = Task.Factory.StartNew(mainAction, CancellationToken.None, TaskCreationOptions.AttachedToParent, UiContext.Instance.Current);
      task.Wait();

      BLogManager.LogEntry(APPNAME, "UpdateView", "END", true);
    }

    private void Init()
    {
      try
      {
        Tweets = new ObservableCollection<Entry>();
        NewEntries = new ObservableCollection<Entry>();

        BindingOperations.EnableCollectionSynchronization(Tweets, _listLock);
        BindingOperations.EnableCollectionSynchronization(TweetsShow, _listLock);
        BindingOperations.EnableCollectionSynchronization(NewEntries, _listLock);

        Refresh();
        InitCommands();
        MessengerInstance.Register<string>(this, DoAction);
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    protected override void InitCommands()
    {
      try
      {
        ClearTweetsCommand = new RelayCommand(ClearTweets);
        ShowUserProfileCommand = new RelayCommand<TwitterUser>(ShowUserProfile);
        ReTweetCommand = new RelayCommand<TwitterEntry>(id => MessengerInstance.Send(new BMessage("Retweet", id)));
        ReplyCommand = new RelayCommand<TwitterEntry>(id => MessengerInstance.Send(new BMessage("Reply", id)));
        DMCommand = new RelayCommand<TwitterEntry>(entry => MessengerInstance.Send(new BMessage("DM", entry)));
        CloseDetailsCommand = new RelayCommand(() => { TweetsDetailsVisibility = Visibility.Collapsed; });
        ShowDetailsCommand = new RelayCommand(() =>
        {
          //UserAction(EnumEventType.MORE);
          TweetsDetailsVisibility = TweetsDetailsVisibility == Visibility.Visible
            ? Visibility.Collapsed
            : Visibility.Visible;
        });
        ShowFullDetailsCommand = new RelayCommand<TwitterEntry>(entry =>
        {
          TweetsDetailsVisibility = Visibility.Visible;
          MessengerInstance.Send(new BMessage("ShowTweet", entry));
        });
        GoToTweetCommand = new RelayCommand<TwitterEntry>(GoToTweet);
        GoToReplyToTweetCommand = new RelayCommand<TwitterEntry>(entry =>
        {
          if (string.IsNullOrEmpty(entry.InReplyToUserId)) return;
          var url = $"http://www.twitter.com/{entry.InReplyToUserName}/status/{entry.InReplyTo}";
          WebHelper.NavigateToUrl(url);
        });
        OpenTweetCommand = new RelayCommand(() => MessengerInstance.Send(new BMessage("ShowTweet", TweetToShow)));

        base.InitCommands();
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this,
          ex);
      }
    }

    private void GoToTweet(TwitterEntry entry)
    {
      try
      {
        if (entry == null) return;
        var url = $"{entry.User.ProfileUrl}/status/{entry.Id}";
        Task.Run(() => WebHelper.NavigateToUrl(url));
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this,
          ex);
      }
    }

    public override double GetRefreshTime()
    {
      switch (WorkspaceSettings.Type)
      {
        case EnumTwitterType.Replies:
          if (Settings != null) return Settings.SlRepliesValue;
          break;

        case EnumTwitterType.Search:
          if (Settings != null) return Settings.SlUserValue;
          break;

        case EnumTwitterType.DirectMessages:
          if (Settings != null) return Settings.SlDmsValue;
          break;

        case EnumTwitterType.Friends:
          if (Settings != null) return Settings.SlFriendsValue;
          break;

        case EnumTwitterType.Groups:
          if (Settings != null) return Settings.SlListValue;
          break;

        case EnumTwitterType.List:
          if (Settings != null) return Settings.SlListValue;
          break;

        case EnumTwitterType.User:
          if (Settings != null) return Settings.SlUserValue;
          break;

        case EnumTwitterType.Favorites:
          if (Settings != null) return Settings.SlRepliesValue;
          break;

        default:
          throw new ArgumentOutOfRangeException();
      }

      return 0;
    }

    public override void DoAction(string param)
    {
      base.DoAction(param);
      switch (param)
      {
        case "UpdateView":
          Task.Run(() => UpdateView());
          break;

        case "SettingsUpdated":
          Task.Run(() => UpdateView());
          break;
      }
    }

    public override void DoActionMessage(BMessage message)
    {
      switch (message.Action)
      {
        case "DeleteCompleted":
          try
          {
            var entry = message.Parameter as TwitterEntry;
            Tweets.Remove(entry);
            UpdateView();
          }
          catch (Exception e)
          {
            Console.WriteLine(e);
          }
          break;
      }
      base.DoActionMessage(message);
    }

    protected override void OnSettingsUpdated()
    {
      StopTimer();
      StartTimer();
    }

    public void ClearTweets()
    {
      try
      {
        if (Tweets == null || !Tweets.Any()) return;
        _doNotTakeTweetOlderThanThis = Tweets[0].PubDate;
        Tweets.Clear();
        UpdateView();
        IsAnyDataVisibility = Tweets.Any() ? Visibility.Collapsed : Visibility.Visible;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this,
          ex);
      }
    }

    #region UPDATE NUMBER POST

    private int MaxPost
    {
      get
      {
        if (Settings == null) return 200;
        return Settings.NbMaxPosts > 5 ? Settings.NbMaxPosts : 200;
      }
    }

    private void UpdateNumberPost(ObservableCollection<Entry> listEntry)
    {
      try
      {
        if (listEntry == null || !listEntry.Any()) return;

        var lst = listEntry.OrderByDescending(entry => entry.PubDate);
        for (var j = MaxPost - 1; j < lst.Count(); j++)
          listEntry.Remove(lst.ElementAt(j));
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this,
          ex);
      }
    }

    #endregion UPDATE NUMBER POST

    #region Callback

    protected int NumberTweetSinceLastAds { get; set; }

    protected void OnGetNewEntriesAsyncCompleted(List<TwitterEntry> entries, string errorMsg, string listName)
    {
      //var dispatcher = Dispatcher.CurrentDispatcher;
      try
      {
        Action newEntryCompletedAction = async () =>
        {
          try
          {
            if (NewEntries != null)
            {
              //We mark all old tweet as read
              for (var index = 0; index < Tweets.Count; index++)
              {
                var tweet = Tweets[index];
                if (tweet == null)
                  Tweets.RemoveAt(index);
                else
                  tweet.HasBeenViewed = true;
              }
              NewEntries.Clear();
              NumberNewMessage = 0;
              if (String.IsNullOrEmpty(errorMsg))
              {
                if (entries == null || !entries.Any())
                {
                  TraceHelper.Trace(this, "There is no tweet in this category.");
                  EndUpdateAll();
                  MessengerInstance.Send("UpdateAPI");
                  UpdateView();
                  return;
                }

                var toAdd = new List<Entry>();

                foreach (var entry in entries)
                {
                  try
                  {
                    if (entry == null) continue;

                    entry.HasBeenViewed = true;
                    if (Settings != null)
                    {
                      if (!string.IsNullOrEmpty(Settings.UserName))
                        entry.CanPost = Settings.UserName.ToUpper() == entry.User.NickName.ToUpper()
                          ? 1
                          : 0;
                      if (entry.Title == null) continue;
                      if (entry.Title.Contains($"@{Settings.UserName}"))
                        entry.PostType = 1;
                    }

                    var addIt = Tweets.FirstOrDefault(t => t.Id == entry.Id) == null;

                    if (!_doNotTakeTweetOlderThanThis.Equals(new DateTime()) && entry.PubDate <= _doNotTakeTweetOlderThanThis)
                      addIt = false;

                    if (addIt && CurrentAccount.SpamList != null)
                      if (CurrentAccount.SpamList.Any(spam => entry.Title.ToLower().Contains(spam.ToLower())))
                        addIt = false;

                    if (!addIt) continue;

                    //
                    toAdd.Add(entry);
                    if (Settings.DateLastUpdate >= entry.PubDate) continue;
                    if (Settings.DateLastUpdate == DateTime.MinValue) continue;
                    entry.HasBeenViewed = false;

                    var currentEntry = entry;
                    await Task.Factory.StartNew(() => AddToAlerts(currentEntry));
                  }
                  catch (Exception e)
                  {
                    Console.WriteLine(e);
                  }
                }

                try
                {
                  foreach (var entry in toAdd)
                  {
                    if (Tweets.Contains(entry)) continue;
                    var pos = 0;
                    while (pos < Tweets.Count && entry.PubDate < Tweets[pos].PubDate)
                      pos++;

                    Tweets.Insert(pos, entry);
                  }
                  if (toAdd.Any())
                    UpdateView();
                }
                catch (Exception e)
                {
                  UpdateView();
                  TraceHelper.Trace(this, e);
                }

                NumberNewMessage = NewEntries.Count;
                if (SobeesSettings.AlertsEnabled && Settings.DateLastUpdate != DateTime.MinValue && NewEntries.Any())
                {
                  Application.Current.Dispatcher.BeginInvokeIfRequired(() => ShowAlerts(NewEntries, Settings.UserName, EnumAccountType.Twitter));
                }
                if (Tweets.Any())
                  Settings.DateLastUpdate = Tweets[0].PubDate;

                if (WorkspaceSettings.Type != EnumTwitterType.Groups) return;

                //Delete all tweets in groups when the user in not any more inside the group.
                var toRemove = new ObservableCollection<Entry>();
                foreach (var tweet in Tweets.Where(tweet => !WorkspaceSettings.GroupMembers.Contains(tweet.User.NickName)))
                  toRemove.Add(tweet);

                foreach (var tweet in toRemove)
                  Tweets.Remove(tweet);
              }
              else
              {
                MessengerInstance.Send(new BMessage("ShowError", errorMsg));
                TraceHelper.Trace(this, errorMsg);
                EndUpdateAll();
              }
            }

            //FacterySearchMeta();
            MessengerInstance.Send("UpdateAPI");
          }
          catch (Exception ex)
          {
            TraceHelper.Trace(this, ex);
            EndUpdateAll();
          }
        };

        //Action newEntryCompletedBackgroundAction = async () => await dispatcher.BeginInvoke(newEntryCompletedAction);
        var newEntryCompletedTask = Task.Factory.StartNew(newEntryCompletedAction).ContinueWith(_ =>
        {
          if (_twitterViewModel != null)
          {
            // Update remaining API
            if (WorkspaceSettings.Type != EnumTwitterType.Friends) return;
            foreach (var workspace in _twitterViewModel.TwitterWorkspaces.Where(workspace => workspace.WorkspaceSettings.Type == EnumTwitterType.Groups))
              workspace.Refresh();
          }

          EndUpdateAll();
          UpdateView();
        }, CancellationToken.None, TaskContinuationOptions.AttachedToParent, UiContext.Instance.Current);
      }
      catch (Exception ex)
      {
        EndUpdateAll();

        Task.Run(() => UpdateView());
        TraceHelper.Trace(this, ex);
      }
      EndUpdateAll();
    }

    #endregion Callback

    #region MenuAction

    private void MenuAction(object obj)
    {
      try
      {
        IsWaiting = true;
        var objs = BRelayCommand.CheckParams(obj);
        var cbx = objs[1] as ComboBox;
        var entry = objs[0] as TwitterEntry;
        if (cbx != null && entry != null)
        {
          var cbxItem = cbx.SelectedItem as ComboBoxItem;
          if (cbxItem != null)
          {
            var param = cbxItem.Tag as string;
            const string error = "";
            switch (param)
            {
              case "Follow":
                Follow(entry, error, false);
                break;

              case "UnFollow":
                Follow(entry, error, true);
                break;

              case "Block":
                MessengerInstance.Send(new BMessage("BlockUser", entry));
                break;

              case "ReportSpam":
                MessengerInstance.Send(new BMessage("ReportSpam", entry));
                break;

              case "ReplyToAll":
                MessengerInstance.Send(new BMessage("ReplyToAll", entry));
                break;

              case "UnFavorit":
                Removefavorite(entry);
                break;

              case "Favorit":
                MessengerInstance.Send(new BMessage("AddToFavorite", entry));
                break;

              case "AddToList":
                MessengerInstance.Send(new BMessage("AddToList", entry));
                break;

              case "DeleteTweet":
                try
                {
                  Tweets.Remove(entry);
                  TweetsShow.Remove(entry);
                  MessengerInstance.Send(new BMessage("DeleteStatus", entry));
                }
                catch (Exception e)
                {
                  Console.WriteLine(e);
                }
                break;
            }
          }
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this,
          ex);
      }
    }

    private void Removefavorite(TwitterEntry entry)
    {
      try
      {
        if (entry == null) return;
        try
        {
          StartWaiting();

          using (var worker = new BackgroundWorker())
          {
            var result = BTwitterResponseResult<TwitterEntry>.CreateInstance();

            worker.DoWork += async delegate(object s,
              DoWorkEventArgs args)
            {
              if (worker.CancellationPending)
              {
                args.Cancel = true;
                return;
              }

              result = await TwitterLibV11.AddFavorite(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET, CurrentAccount.SessionKey, CurrentAccount.Secret, entry.Id, true);
            };

            worker.RunWorkerCompleted += delegate { OnAddToFavoriteAsyncCompleted(result.DataResult, result.ErrorMessage); };

            worker.RunWorkerAsync();
          }
        }
        catch (Exception ex)
        {
          TraceHelper.Trace(this, ex);
          StopWaiting();
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    private void OnAddToFavoriteAsyncCompleted(TwitterEntry newFavorite, string errorMsg)
    {
      StopWaiting();
      try
      {
        if (!string.IsNullOrEmpty(errorMsg))
        {
          MessengerInstance.Send(new BMessage("ShowError", errorMsg));
          return;
        }

        Tweets.Remove(newFavorite);
        UpdateView();
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this,
          ex);
      }
    }

    private void Follow(TwitterEntry entry, string error, bool destroy)
    {
      try
      {
        using (var worker = new BackgroundWorker())
        {
          worker.DoWork += async delegate(object s,
            DoWorkEventArgs args)
          {
            if (worker.CancellationPending)
            {
              args.Cancel = true;
              return;
            }

            await TwitterLibV11.Follow(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET, CurrentAccount.SessionKey, CurrentAccount.Secret,
              entry.User.Id, destroy);
          };

          worker.RunWorkerCompleted += delegate { MessengerInstance.Send(new BMessage("ShowError", error)); };
          worker.RunWorkerAsync();
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this,
          ex);
      }
    }

    #endregion MenuAction

    #region UpdateAll

    private const string APPNAME = "TwitterWorkspaceViewModel";

    /// <summary>
    ///   UpdateAll
    /// </summary>
    public override async void UpdateAll()
    {
      try
      {
        string errorMsg = null;
        BLogManager.LogEntry(APPNAME, "UpdateAll", "START", true);

        if (CurrentAccount == null)
        {
          BLogManager.LogEntry(APPNAME, "CurrentAccount IS NULL (TwitterWorkspaceViewModel.cs)!!!", true);
          EndUpdateAll();
          return;
        }

        switch (WorkspaceSettings.Type)
        {
          case EnumTwitterType.Replies:

            var repliesNewEntries = new List<TwitterEntry>();

            Action repliesAction = () =>
            {
              var result = TwitterLibV11.GetReplies(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET,
                CurrentAccount.AuthToken ?? CurrentAccount.SessionKey, CurrentAccount.Secret,
                Settings.NbPostToGet,
                out errorMsg, ProxyHelper.GetConfiguredWebProxy(SobeesSettings));
              if (result != null)
              {
                foreach (var twitterEntry in result)
                  repliesNewEntries.Insert(0, twitterEntry);
              }
              foreach (var entry in repliesNewEntries)
                entry.PostType = 1;
            };

            var repliesTask = Task.Factory.StartNew(repliesAction).ContinueWith(_ => OnGetNewEntriesAsyncCompleted(repliesNewEntries, errorMsg, ""), UiContext.Instance.Current);
            break;

          case EnumTwitterType.DirectMessages:
            var directNewEntries = new List<TwitterEntry>();

            Action directAction = () =>
            {
              var result = TwitterLibV11.GetDirectMessages(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET,
                CurrentAccount.SessionKey, CurrentAccount.Secret,
                Settings.NbPostToGet, out errorMsg,
                ProxyHelper.GetConfiguredWebProxy(SobeesSettings));
              if (result == null)
              {
                var textConnection =
                  new LocText("Sobees.Configuration.BGlobals:Resources:TwOauth2DMWarning").ResolveLocalizedValue();
                Messenger.Default.Send(new BMessage("DisplayPopupTw", textConnection));
              }

              if (result != null && result.Any())
              {
                foreach (var twitterEntry in result)
                  directNewEntries.Insert(0, twitterEntry);
              }
              foreach (var entry in directNewEntries)
                entry.PostType = 2;
            };

            //Action directBackgroundAction = () => dispatcher.BeginInvoke(directAction);
            var directTask = Task.Factory.StartNew(directAction).ContinueWith(_ => OnGetNewEntriesAsyncCompleted(directNewEntries, errorMsg, ""), UiContext.Instance.Current);
            break;

          case EnumTwitterType.Friends:

            var friendsNewEntries = new List<TwitterEntry>();

            Action friendsAction = () =>
            {
              if (Tweets == null) Tweets = new ObservableCollection<Entry>();
              if (Tweets.Any())
              {
                var result = TwitterLibV11.GetHomeTimeline(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET,
                  CurrentAccount.SessionKey, CurrentAccount.Secret,
                  Settings.NbPostToGet, -1, Convert.ToInt64(Tweets[0].Id),
                  out errorMsg, ProxyHelper.GetConfiguredWebProxy(SobeesSettings));

                if (result == null) return;
                foreach (var twitterEntry in result)
                  friendsNewEntries.Insert(0, twitterEntry);
              }
              else
              {
                var result =
                  TwitterLibV11.GetHomeTimeline(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET,
                    CurrentAccount.SessionKey,
                    CurrentAccount.Secret, Settings.NbPostToGet, -1, -1,
                    out errorMsg, ProxyHelper.GetConfiguredWebProxy(SobeesSettings));

                if (result == null) return;
                foreach (var twitterEntry in result)
                  friendsNewEntries.Insert(0, twitterEntry);
              }

              if (!Settings.ShowDMHome && !Settings.ShowRepliesHome) return;
              var isDmOpen = false;
              var isRepliesOpen = false;
              foreach (var model in _twitterViewModel.TwitterWorkspaces)
              {
                if (model.WorkspaceSettings.Type == EnumTwitterType.DirectMessages)

                  isDmOpen = true;

                if (model.WorkspaceSettings.Type == EnumTwitterType.Replies)

                  isRepliesOpen = true;
              }
              if (!isDmOpen)
              {
                var result = TwitterLibV11.GetDirectMessages(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET,
                  CurrentAccount.SessionKey, CurrentAccount.Secret,
                  Settings.NbPostToGet,
                  out errorMsg, ProxyHelper.GetConfiguredWebProxy(SobeesSettings));
                foreach (var entry in result)
                {
                  friendsNewEntries.Insert(0, entry);
                  entry.PostType = 2;
                }
              }
              if (!isRepliesOpen)
              {
                var result = TwitterLibV11.GetReplies(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET,
                  CurrentAccount.SessionKey, CurrentAccount.Secret,
                  Settings.NbPostToGet, out errorMsg,
                  ProxyHelper.GetConfiguredWebProxy(SobeesSettings));
                result.ToList().ForEach(entry =>
                {
                  entry.PostType = 1;
                  friendsNewEntries.Insert(0, entry);
                }
                  );
              }
            };

            //Action backgroundAction = () => dispatcher.BeginInvoke(friendsAction);
            var task = Task.Factory.StartNew(friendsAction).ContinueWith(_ => OnGetNewEntriesAsyncCompleted(friendsNewEntries, errorMsg, ""), UiContext.Instance.Current);
            task.Wait();
            break;

          case EnumTwitterType.Groups:
            if (WorkspaceSettings.GroupMembers == null || !WorkspaceSettings.GroupMembers.Any())
              break;

            var groupsNewEntries = new List<TwitterEntry>();

            Action groupsAction = () =>
            {
              var isFriendsOpen = false;
              foreach (var workspace in _twitterViewModel.TwitterWorkspaces.Where(workspace => workspace.WorkspaceSettings.Type == EnumTwitterType.Friends))
              {
                isFriendsOpen = true;
                foreach (var tweet in workspace.Tweets.Cast<TwitterEntry>().Where(tweet => WorkspaceSettings.GroupMembers.Contains(tweet.User.NickName)))
                {
                  groupsNewEntries.Insert(0, tweet);
                }
              }

              if (isFriendsOpen) return;
              var result = TwitterLibV11.GetFriendsTimeline(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET,
                CurrentAccount.SessionKey, CurrentAccount.Secret,
                CurrentAccount.Login, Settings.NbPostToGet, 0,
                out errorMsg, ProxyHelper.GetConfiguredWebProxy(SobeesSettings));
              if (result == null) return;
              foreach (var tweet in result.Where(tweet => WorkspaceSettings.GroupMembers.Contains(tweet.User.NickName)))
              {
                groupsNewEntries.Insert(0, tweet);
              }
            };

            //Action groupsBackgroundAction = () => dispatcher.BeginInvoke(groupsAction);
            var groupstask = Task.Factory.StartNew(groupsAction).ContinueWith(_ => OnGetNewEntriesAsyncCompleted(groupsNewEntries, errorMsg, ""), UiContext.Instance.Current);
            break;

          case EnumTwitterType.User:

            var userNewEntries = new List<TwitterEntry>();

            Action userAction = () =>
            {
              var result = TwitterLibV11.GetUser(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET, CurrentAccount.SessionKey,
                CurrentAccount.Secret,
                Settings.UserName,
                Settings.NbPostToGet,
                out errorMsg, ProxyHelper.GetConfiguredWebProxy(SobeesSettings));

              if (result == null) return;
              userNewEntries.AddRange(result);
            };

            //Action userBackgroundAction = () => dispatcher.BeginInvoke(userAction);
            var usertask = Task.Factory.StartNew(userAction).ContinueWith(_ => OnGetNewEntriesAsyncCompleted(userNewEntries, errorMsg, ""), UiContext.Instance.Current);
            usertask.Wait();
            break;

          case EnumTwitterType.List:

            var listNewEntries = new List<TwitterEntry>();

            Action listAction = () =>
            {
              try
              {
                if (string.IsNullOrEmpty(WorkspaceSettings.GroupName)) return;
                var result = TwitterLibV11.GetListStatuse(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET,
                  CurrentAccount.SessionKey, CurrentAccount.Secret,
                  WorkspaceSettings.GroupName.Substring(1).Split('/')[0], WorkspaceSettings.UserToGet,
                  Settings.NbPostToGet, out errorMsg, ProxyHelper.GetConfiguredWebProxy(SobeesSettings));

                if (result == null) return;
                listNewEntries.AddRange(result);
              }
              catch (Exception e)
              {
                // Console.WriteLine(e);
              }
            };

            //Action listBackgroundAction = () => dispatcher.BeginInvoke(listAction);
            var listTask = Task.Factory.StartNew(listAction).ContinueWith(_ => OnGetNewEntriesAsyncCompleted(listNewEntries, errorMsg, ""), UiContext.Instance.Current);
            break;

          case EnumTwitterType.Favorites:

            var favNewEntries = new List<TwitterEntry>();

            Action favAction = () =>
            {
              var result = TwitterLibV11.GetFavorites(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET,
                CurrentAccount.SessionKey, CurrentAccount.Secret,
                Settings.UserName,
                Settings.NbPostToGet,
                out errorMsg, ProxyHelper.GetConfiguredWebProxy(SobeesSettings));

              if (result == null) return;
              favNewEntries.AddRange(result);
            };

            //Action favBackgroundAction = () => dispatcher.BeginInvoke(favAction);
            var favTask = Task.Factory.StartNew(favAction).ContinueWith(_ => OnGetNewEntriesAsyncCompleted(favNewEntries, errorMsg, ""), UiContext.Instance.Current);
            break;

          default:
            break;
        }
      }
      catch (Exception ex)
      {
        BLogManager.LogEntry(APPNAME, ex);
        EndUpdateAll();
      }
    }

    #endregion UpdateAll
  }
}