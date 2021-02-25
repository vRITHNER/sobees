#region

using BUtility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Controls.TwitterSearch.Cls;
using Sobees.Infrastructure.UI;
using Sobees.Library.BGenericLib;
using Sobees.Library.BTwitterLib;
using Sobees.Tools.Threading.Extensions;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Xml;

#endregion

namespace Sobees.Controls.TwitterSearch.ViewModel
{
  public class TweetProfileViewModel : TwitterSearchWorkspaceViewModel
  {
    private const string APPNAME = "TweetProfileViewModel";

    private ObservableCollection<TwitterEntry> _conversations;
    private TwitterEntry _tweetToShowProfile;
    private Entry _tweetToShowProfileOther;

    public TweetProfileViewModel(TwitterSearchViewModel model, TwitterSearchWorkspaceSettings settings, Messenger messenger)
      : base(model, settings, messenger)
    {
    }

    public ObservableCollection<TwitterEntry> Conversations
    {
      get
      {
        if (_conversations != null) return _conversations;
        _conversations = new ObservableCollection<TwitterEntry>();
        RaisePropertyChanged();
        return _conversations;
      }
    }

    public TwitterEntry TweetToShowProfile
    {
      get => _tweetToShowProfile;
      set
      {
        _tweetToShowProfile = value;
        UpdateConversation();
        RaisePropertyChanged();
      }
    }

    public Entry TweetToShowProfileOther
    {
      get => _tweetToShowProfileOther;
      set
      {
        _tweetToShowProfileOther = value;
        RaisePropertyChanged();
      }
    }

    public override DataTemplate DataTemplateView
    {
      get
      {
        const string dt = "<DataTemplate x:Name='dtGroupsView' " +
                          "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                          "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' " +
                          "xmlns:Views='clr-namespace:Sobees.Controls.TwitterSearch.Views;assembly=Sobees.Controls.TwitterSearch'>" +
                          "<Grid HorizontalAlignment='Stretch' VerticalAlignment='Stretch'>" +
                          "<Views:Tweet HorizontalAlignment='Stretch' VerticalAlignment='Stretch'/>" +
                          "</Grid>" +
                          "</DataTemplate>";

        var stringReader = new StringReader(dt);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
    }

    protected override void InitCommands()
    {
      base.InitCommands();
      CloseCommand = new RelayCommand(() => MessengerInstance.Send("CloseProfile"));
    }

    private void UpdateConversation()
    {
      Conversations.Clear();
      if (TweetToShowProfile == null) return;
      if (string.IsNullOrEmpty(TweetToShowProfile.InReplyToUserName)) return;

      Action mainAction = () =>
      {
        try
        {
          string errorMsg;
          var tweets =
            TwitterLib.SearchSummize(
              string.Format("{0} OR {1}",
                            $"from:{TweetToShowProfile.User.NickName} to:{TweetToShowProfile.InReplyToUserName}",
                            $"from:{TweetToShowProfile.InReplyToUserName} to:{TweetToShowProfile.User.NickName}"),
              EnumLanguages.all, Settings.NbPostToGet, string.Empty, out errorMsg);

          if (!string.IsNullOrEmpty(errorMsg) || tweets == null || !tweets.Any())
            return;

          //Application.Current.Dispatcher.BeginInvokeIfRequired(() =>
          foreach (var tweet in tweets)
            Conversations.Add(tweet);
        }
        catch (Exception ex)
        {
          BLogManager.LogEntry(APPNAME, "UpdateConversation", ex);
        }
      };

      var task = Task.Factory.StartNew(mainAction, CancellationToken.None, TaskCreationOptions.AttachedToParent, UiContext.Instance.Current);
    }

    public override void UpdateAll()
    {
      EndUpdateAll();
    }

    public void ShowTweet(TwitterEntry entry)
    {
      TweetToShowProfile = entry;
    }

    public void ShowTweetOther(Entry entry)
    {
      ShowTweet(new TwitterEntry
                  {
                    User = new TwitterUser
                             {
                               Id = entry.User.Id,
                               Name = entry.User.Name,
                               Online = entry.User.Online,
                               FirstName = entry.User.FirstName,
                               NickName = entry.User.NickName,
                               Description = entry.User.Description,
                               Location = entry.User.Location,
                               ProfileUrl = entry.User.ProfileUrl,
                               ProfileImgUrl = entry.User.ProfileImgUrl,
                               Url = entry.User.Url
                             },
                    Id = entry.Id,
                    Title = entry.Title,
                    Section = entry.Section,
                    Link = entry.Link,
                    Comments = entry.Comments,
                    OrigLink = entry.OrigLink,
                    DisplayLink = entry.DisplayLink,
                    PubDate = entry.PubDate,
                    UpdateDate = entry.UpdateDate,
                    Type = entry.Type,
                  });
      TweetToShowProfileOther = entry;
    }
  }
}