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
using BUtility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Controls.TwitterSearch.Cls;
using Sobees.Infrastructure.Cache;
using Sobees.Infrastructure.Cls;
using Sobees.Infrastructure.Commands;
using Sobees.Infrastructure.Controls;
using Sobees.Infrastructure.UI;
using Sobees.Infrastructure.ViewModelBase;
using Sobees.Library.BGenericLib;
using Sobees.Library.BTwitterLib;
using Sobees.Tools.Logging;
using Sobees.Tools.Threading;
using Sobees.Tools.Threading.Extensions;
using Sobees.Tools.Web;

#endregion

namespace Sobees.Controls.TwitterSearch.ViewModel
{
  public class TwitterSearchWorkspaceViewModel : BWorkspaceViewModel
  {
    #region Static Fields and Constants

    private new const string APPNAME = "TwitterSearchWorkspaceViewModel";

    #endregion

    #region Fields

    private readonly TwitterSearchViewModel _twitterSearchViewModel;
    private Entry _ads;

    private ObservableCollection<TwitterEntry> _conversations;

    private DateTime _doNotTakeTweetOlderThanThis;
    private bool _isSelected;
    private bool _isUpdatingView;
    private ObservableCollection<Entry> _listAds;
    private bool _mustUpdateView;
    private int _selectedTweetIndex = -1;
    private Visibility _tweetsDetailsVisibility = Visibility.Collapsed;
    private Entry _tweetToShow;

    #endregion

    #region Properties

    public TwitterSearchSettings Settings => (TwitterSearchSettings) _twitterSearchViewModel.Settings;
    //private DeferredAction deferredAction;

    public bool OrderType => false;

    public ObservableCollection<Entry> NewEntries { get; set; }

    public ObservableCollection<Entry> ListAds
    {
      get => _listAds ?? (_listAds = new ObservableCollection<Entry>());
      set => _listAds = value;
    }

    public Entry Ads
    {
      get => _ads;
      set
      {
        _ads = value;
        RaisePropertyChanged(() => Ads);
      }
    }

    public ObservableCollection<Entry> Tweets { get; set; }

    public ObservableCollection<Entry> TweetsShow { get; set; }

    public TwitterSearchWorkspaceSettings WorkspaceSettings { get; set; }

    public bool IsSelected
    {
      get => _isSelected;
      set
      {
        _isSelected = value;
        RaisePropertyChanged(() => IsSelected);
      }
    }

    public int SelectedTweetIndex
    {
      get => _selectedTweetIndex;
      set
      {
        _selectedTweetIndex = value;
        if (_selectedTweetIndex > -1 && _selectedTweetIndex < TweetsShow.Count)
          try
          {
            TweetToShow = TweetsShow[_selectedTweetIndex];
          }
          catch (Exception e)
          {
            Console.WriteLine(e);
          }
      }
    }

    public Entry TweetToShow
    {
      get => _tweetToShow;
      set
      {
        _tweetToShow = value;
        UpdateConversation();
        RaisePropertyChanged(() => TweetToShow);
      }
    }

    public bool TweetsDetailOpen => TweetsDetailsVisibility == Visibility.Visible;

    public Visibility AddsVisibility => SobeesSettings.DisableAds ? Visibility.Collapsed : Visibility.Visible;

    public ObservableCollection<TwitterEntry> Conversations
    {
      get => _conversations ?? (_conversations = new ObservableCollection<TwitterEntry>());
      set
      {
        _conversations = value;
        RaisePropertyChanged(() => Conversations);
      }
    }

    public Visibility TweetsDetailsVisibility
    {
      get => _tweetsDetailsVisibility;
      set
      {
        _tweetsDetailsVisibility = value;
        RaisePropertyChanged(() => TweetsDetailsVisibility);
        RaisePropertyChanged(() => TweetsDetailOpen);
      }
    }

    #endregion

    #region Constructors

    public TwitterSearchWorkspaceViewModel(TwitterSearchViewModel model, TwitterSearchWorkspaceSettings settings,
                                           Messenger messenger)
    {
      MessengerInstance = messenger;
      MessengerInstance.Register<string>(this, DoAction);
      _twitterSearchViewModel = model;
      WorkspaceSettings = settings;
      NewEntries = new ObservableCollection<Entry>();
      Tweets = new ObservableCollection<Entry>();
      TweetsShow = new ObservableCollection<Entry>();
      Refresh();
      InitCommands();
    }

    #endregion

    #region Overrides

    public override double GetRefreshTime()
    {
      return Settings.RefreshTimeTS;
    }

    #endregion

    #region Command

    private BRelayCommand _menuActionCommand;

    public RelayCommand CloseDetailsCommand { get; set; }

    public RelayCommand ClearTweetsCommand { get; private set; }

    public RelayCommand<TwitterEntry> ReplyCommand { get; set; }

    public RelayCommand OpenTweetCommand { get; private set; }

    public RelayCommand<TwitterEntry> ReTweetCommand { get; set; }

    public RelayCommand<TwitterEntry> DmCommand { get; set; }

    public RelayCommand<Entry> GoToTweetCommand { get; set; }

    public RelayCommand<Entry> ShowFullDetailsCommand { get; set; }

    public RelayCommand ShowDetailsCommand { get; set; }

    public BRelayCommand MenuActionCommand => _menuActionCommand ?? (_menuActionCommand = new BRelayCommand(MenuAction));

    #endregion

    #region Methods

    protected new void Dispose()
    {
      IsClosed = true;
      DeleteTimer();
      base.Dispose();
    }

    private void GoToTweet(Entry entry)
    {
      try
      {
        if (entry == null) return;

        var url = "";
        if (entry.Type == EnumType.TwitterSearch)
        {
          var split = entry.Id.Split(Convert.ToChar(":"));
          url = $"{((TwitterEntry) entry).User.ProfileUrl}/status/{split[2]}";
        }
        if (entry.Type == EnumType.Facebook)
          url = entry.User.ProfileUrl;
        if (!string.IsNullOrEmpty(url))
          WebHelper.NavigateToUrl(url);
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }
    }

    public override void UpdateAll()
    {
      if (ListAds.Any())
        try
        {
          Ads = ListAds[(ListAds.IndexOf(Ads) + 1) % ListAds.Count];
        }
        catch (Exception ex)
        {
          BLogManager.LogEntry(APPNAME, "UpdateAll", ex);
        }

      if (Settings.ShowTwitter)
        LoadTweets();
    }

    /// <summary>
    ///   LoadTweets
    /// </summary>
    public void LoadTweets()
    {
      try
      {
        string errorMsg = null;

        var newEntries = new ObservableCollection<Entry>();

        Action mainAction = () =>
                            {
                              var accounts = SobeesSettings.Accounts.Where(a => a.Type == EnumAccountType.Twitter).ToList();
                              if (!accounts.Any())
                              {
                                MessengerInstance.Send(new BMessage("ShowError", "You need to add a Twitter account to allow to make Search queries!"));
                                return;
                              }

                              var firstAccount = accounts.FirstOrDefault(l => l.Login != null);

                              var result =
                                new ObservableCollection<TwitterEntry>(
                                                                       TwitterLib.SearchSummize2(firstAccount.AuthToken, firstAccount.Secret,
                                                                                                 WorkspaceSettings.SearchQuery, Settings.Language,
                                                                                                 Settings.NbPostToGet, Settings.GeoCode, out errorMsg));
                              foreach (var entry in result)
                              {
                                entry.Type = EnumType.TwitterSearch;
                                newEntries.Add(entry);
                              }
                            };
        var task = Task.Factory.StartNew(mainAction, CancellationToken.None, TaskCreationOptions.AttachedToParent, UiContext.Instance.Current);
        task.ContinueWith(_ => OnGetNewEntriesAsyncCompleted(newEntries, errorMsg));
      }
      catch (Exception ex)
      {
        BLogManager.LogEntry(APPNAME, "LoadTweets", ex);
      }
    }

    private void OnGetNewEntriesAsyncCompleted(ICollection<Entry> entries, string errorMsg)
    {
      if (IsClosed)
        return;

      if (!string.IsNullOrEmpty(errorMsg))
      {
        MessengerInstance.Send(new BMessage("ShowError", errorMsg));
        EndUpdateAll();
      }
      foreach (var list in entries)
        if (list.User != null)
          BCacheImage.WebGetter.QueueImageRequest(new SmallUri(list.User.ProfileImgUrl, ImageKind.Small),
                                                  ImageKind.Small);

      Action mainAction = () =>
                          {
                            try
                            {
                              if (NewEntries == null)
                                NewEntries = new ObservableCollection<Entry>();
                              NewEntries.Clear();
                              foreach (var entry in entries)
                              {
                                entry.HasBeenViewed = true;
                                if (Tweets == null) continue;
                                if (!Tweets.Contains(entry))
                                {
                                  if (_doNotTakeTweetOlderThanThis.Equals(new DateTime()) ||
                                      entry.PubDate > _doNotTakeTweetOlderThanThis)
                                  {
                                    var item = entry;
                                    var pos = Tweets.TakeWhile(tweet => item.PubDate <= tweet.PubDate).Count();
                                    Tweets.Insert(pos, item);

                                    if (WorkspaceSettings.DateLastUpdate < entry.PubDate)
                                    {
                                      if (WorkspaceSettings.DateLastUpdate != DateTime.MinValue)
                                        entry.HasBeenViewed = false;

                                      //Alerts
                                      if (SobeesSettings.AlertsEnabled && SobeesSettings.AlertsTwitterSearchEnabled)
                                        NewEntries.Add(entry);
                                    }
                                  }
                                }
                                else
                                {
                                  Tweets[Tweets.IndexOf(entry)].PubDate = entry.PubDate;
                                  Tweets[Tweets.IndexOf(entry)].HasBeenViewed = true;
                                }
                              }

                              //Show Alerts
                              NumberNewMessage = NewEntries.Count;
                              if (SobeesSettings.AlertsEnabled && SobeesSettings.AlertsTwitterSearchEnabled)
                                ShowAlerts(NewEntries, WorkspaceSettings.SearchQuery, EnumAccountType.TwitterSearch);
                              if (Tweets != null)
                                if (Tweets.Any())
                                  WorkspaceSettings.DateLastUpdate = Tweets[0].PubDate;

                              entries.Clear();
                              MessengerInstance.Send("UpdateView");
                            }
                            catch (Exception ex)
                            {
                              TraceHelper.Trace(this, ex);
                            }
                          };

      var task = Task.Factory.StartNew(mainAction, CancellationToken.None, TaskCreationOptions.AttachedToParent, UiContext.Instance.Current);
      task.ContinueWith(_ => EndUpdateAll());
    }

    public override void DoAction(string param)
    {
      base.DoAction(param);
      switch (param)
      {
        case "UpdateView":
          Application.Current.Dispatcher.BeginInvokeIfRequired(UpdateView);
          break;

        case "SettingsUpdated":
          Application.Current.Dispatcher.BeginInvokeIfRequired(UpdateView);
          break;
      }
    }

    public void UpdateAdsVisibility()
    {
      RaisePropertyChanged(() => AddsVisibility);
    }

    public override void UpdateView()
    {
      try
      {
        if (_isUpdatingView)
        {
          _mustUpdateView = true;
          return;
        }
        _isUpdatingView = true;
        _mustUpdateView = false;

        if (Tweets != null)
        {
          //Update number post list Tweets
          if (Tweets.Count > MaxPost)
            UpdateNumberPost(Tweets);

          var list = (from tweet in Tweets
                      where
                      tweet != null &&
                      (tweet.Type == EnumType.TwitterSearch
                         ? _twitterSearchViewModel.IsShowTwitterSearchEntry
                         : tweet.Type == EnumType.Facebook && _twitterSearchViewModel.IsShowFacebookEntry)
                      &&
                      (string.IsNullOrEmpty(SobeesSettings.Filter) || tweet.Title.IndexOf(SobeesSettings.Filter, StringComparison.InvariantCultureIgnoreCase) >= 0)
                      orderby tweet.PubDate descending
                      select tweet).ToList();

          var i = 0;
          while (i < TweetsShow.Count)
            if (list.Contains(TweetsShow[i]))
            {
              list.Remove(TweetsShow[i]);
              i++;
            }
            else
            {
              TweetsShow.RemoveAt(i);
            }
          i = 0;
          while (i < TweetsShow.Count)
          {
            if (OrderType)
            {
              {
                var j = 0;
                var i1 = i;
                foreach (var entryShow in TweetsShow.TakeWhile(entryShow => j != i1))
                  j++;
              }
            }
            else
            {
              var j = 0;
              while (j < TweetsShow.Count)
              {
                if (j == i)
                  break;

                if (TweetsShow[j].PubDate < TweetsShow[i].PubDate)
                {
                  TweetsShow.Insert(j, TweetsShow[i]);
                  TweetsShow.RemoveAt(i + 1);
                  break;
                }
                j++;
              }
            }
            i++;
          }

          //Add new tweet
          if (OrderType)
            foreach (var entry in list)
            {
              var isAdd = false;
              var nbTweets = TweetsShow.Count;

              i = 0;
              while (i < nbTweets)
              {
                if (TweetsShow[i].PubDate > entry.PubDate)
                {
                  TweetsShow.Insert(i, entry);
                  isAdd = true;
                  break;
                }
                i++;
              }
              if (!isAdd)
                TweetsShow.Add(entry);
            }
          else
            foreach (var entry in list)
            {
              var isAdd = false;
              var nbTweets = TweetsShow.Count;
              i = 0;
              while (i < nbTweets)
              {
                if (TweetsShow[i].PubDate < entry.PubDate)
                {
                  TweetsShow.Insert(i, entry);
                  isAdd = true;
                  break;
                }
                i++;
              }
              if (!isAdd)
                TweetsShow.Add(entry);
            }

          //UpdateVisibility of details view
          if (TweetsDetailsVisibility == Visibility.Visible)
            if (TweetToShow == null || TweetsShow.Count == 0)
            {
              TweetsDetailsVisibility = Visibility.Collapsed;
            }
            else
            {
              if (!TweetsShow.Contains(TweetToShow))
                SelectedTweetIndex = 0;
            }
        }

        ProcessHelper.GcCollect();

        if (_mustUpdateView)
          UpdateView();

        IsAnyDataVisibility = Tweets != null && Tweets.Any() ? Visibility.Collapsed : Visibility.Visible;
        _isUpdatingView = false;
      }
      catch (Exception ex)
      {
        _isUpdatingView = false;
        UpdateView();
        IsAnyDataVisibility = Tweets != null && Tweets.Any() ? Visibility.Collapsed : Visibility.Visible;
        BLogManager.LogEntry(APPNAME, ex);
      }
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

    protected override void InitCommands()
    {
      ClearTweetsCommand = new RelayCommand(ClearTweets);
      CloseDetailsCommand = new RelayCommand(() =>
                                             {
                                               TweetToShow = null;
                                               RaisePropertyChanged(() => TweetsDetailsVisibility);
                                             });
      ReTweetCommand = new RelayCommand<TwitterEntry>(id => MessengerInstance.Send(new BMessage("Retweet", id)));
      ReplyCommand = new RelayCommand<TwitterEntry>(id => MessengerInstance.Send(new BMessage("Reply", id)));
      DmCommand = new RelayCommand<TwitterEntry>(entry => MessengerInstance.Send(new BMessage("DM", entry)));
      GoToTweetCommand = new RelayCommand<Entry>(GoToTweet);
      CloseCommand =
        new RelayCommand(() => MessengerInstance.Send(new BMessage("CloseSearch", WorkspaceSettings.SearchQuery)));
      CloseDetailsCommand = new RelayCommand(() => { TweetsDetailsVisibility = Visibility.Collapsed; });
      ShowFullDetailsCommand = new RelayCommand<Entry>(entry =>
                                                       {
                                                         TweetsDetailsVisibility = Visibility.Visible;
                                                         MessengerInstance.Send(new BMessage("ShowTweet", TweetToShow));
                                                       });
      ShowDetailsCommand =
        new RelayCommand(
                         () =>
                         {
                           TweetsDetailsVisibility = TweetsDetailsVisibility == Visibility.Visible
                                                       ? Visibility.Collapsed
                                                       : Visibility.Visible;
                         });
      OpenTweetCommand = new RelayCommand(() => MessengerInstance.Send(new BMessage("ShowTweet", TweetToShow)));
      base.InitCommands();
    }

    public override void StartWaiting()
    {
      _twitterSearchViewModel.StartWaiting();
      IsWaiting = true;
    }

    public override void StopWaiting()
    {
      _twitterSearchViewModel.EndUpdateAll();
      _twitterSearchViewModel.StopWaiting();
      IsWaiting = false;
    }

    private void MenuAction(object obj)
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
          //var error = "";
          switch (param)
          {
            case "Follow":
              MessengerInstance.Send(new BMessage("Follow", entry));
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

            case "Favorit":
              MessengerInstance.Send(new BMessage("AddToFavorite", entry));
              break;

            case "AddToList":
              MessengerInstance.Send(new BMessage("AddToList", entry));
              break;

            case "DeleteTweet":
              MessengerInstance.Send(new BMessage("DeleteStatus", entry));
              break;
          }
        }
      }
    }

    private void UpdateConversation()
    {
      Conversations.Clear();
      var entry = TweetToShow as TwitterEntry;
      if (!string.IsNullOrEmpty(entry?.InReplyToUserName))
        using (var worker = new BackgroundWorker())
        {
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
                               string errorMsg;
                               var tweets =
                                 TwitterLib.SearchSummize(
                                                          $"from:{entry.User.NickName} to:{entry.InReplyToUserName} OR from:{entry.InReplyToUserName} to:{entry.User.NickName}",
                                                          EnumLanguages.all,
                                                          Settings.NbPostToGet, string.Empty, out errorMsg);
                               if (!string.IsNullOrEmpty(errorMsg))
                                 return;

                               if (tweets == null || tweets.Count == 0)
                                 return;

                               Application.Current.Dispatcher.BeginInvokeIfRequired(() =>
                                                                                    {
                                                                                      foreach (var tweet in tweets)
                                                                                        Conversations.Insert(0,
                                                                                                             tweet);
                                                                                    });
                             }
                             catch (Exception ex)
                             {
                               TraceHelper.Trace(this, ex);
                             }
                           };

          worker.RunWorkerAsync();
        }
    }

    #endregion

    #region UPDATE NUMBER POST

    private int MaxPost => Settings?.NbMaxPosts > 5 ? Settings.NbMaxPosts : 200;

    private void UpdateNumberPost(IList<Entry> listEntry)
    {
      try
      {
        if (listEntry == null || listEntry.Count < 1)
          return;

        var max = MaxPost;
        var i = 0;

        while (i < listEntry.Count)
          if (i >= max)
            listEntry.Remove(listEntry[i]);

          else
            i++;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    #endregion
  }
}