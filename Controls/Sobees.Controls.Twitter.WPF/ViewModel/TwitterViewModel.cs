#region

using BUtility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Configuration.BGlobals;
using Sobees.Controls.Twitter.Cls;
using Sobees.Infrastructure.Cls;
using Sobees.Infrastructure.Commands;
using Sobees.Infrastructure.Controls;
using Sobees.Infrastructure.Model;
using Sobees.Infrastructure.Tools.Serialization;
using Sobees.Infrastructure.UI;
using Sobees.Infrastructure.ViewModelBase;
using Sobees.Library.BGenericLib;
using Sobees.Library.BLocalizeLib;
using Sobees.Library.BServicesLib;
using Sobees.Library.BTwitterLib;
using Sobees.Library.BTwitterLib.Response;
using Sobees.Tools.Logging;
using Sobees.Tools.Threading.Extensions;
using Sobees.Tools.Web;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Xml;
using Application = System.Windows.Application;
using Binding = System.Windows.Data.Binding;
using Button = System.Windows.Controls.Button;
using Orientation = System.Windows.Controls.Orientation;
using Path = System.Windows.Shapes.Path;

#endregion

namespace Sobees.Controls.Twitter.ViewModel
{
  public class TwitterViewModel : BServiceWorkspaceViewModel
  {
    #region Fields

    private new const string APPNAME = "TwitterViewModel";

    private string _actionToConfirm;
    private CredentialsViewModel _credentialsViewModel;
    private string _cursor = "-1";
    private Visibility _isDelBtnImgShow = Visibility.Collapsed;

    //private bool _isFilterFactory;
    //private bool _isShowFactery;
    //private bool _isShowOnlyFactery;
    private Visibility _isVisibleBtnPhoto = Visibility.Visible;

    private ListViewModel _listViewModel;
    private int _numberNewDm;
    private int _numberNewFavorit;
    private int _numberNewFriends;
    private int _numberNewReplies;
    private int _numberNewUser;
    private ProfileViewModel _profileViewModel;
    private SettingsViewModel _settingsViewModel;
    private TwitterEntry _tweetToShow;
    private TwitterRateLimit _twitterRateLimit;
    private TwitterWorkspaceViewModel _twitterSelectedWorkspace;
    private ObservableCollection<TwitterWorkspaceViewModel> _twitterWorkspaces;

    #region Lists

    private string _confirmationText;
    private ObservableCollection<TwitterEntry> _conversations;
    private string _createNewList;
    private TwitterList _currentList;
    private List<User> _friends;
    private bool _isPrivate = true;
    private string _nameNewList;
    private int _page = 0;
    private TwitterList _selectedList;
    private DispatcherTimer _timerCreateFriendList;
    private ObservableCollection<TwitterList> _twitterList;
    private ObservableCollection<TwitterList> _twitterListDisplay;
    private ObservableCollection<TwitterList> _twitterListOwn;

    #endregion Lists

    #region PostStatus

    private int _cursorPosition;
    private bool _isAddingTweet;
    private bool _moveCursor;
    private int _twitterMsgMaximumLength = 140;
    private Visibility _visibilityButtonRetweet = Visibility.Collapsed;

    #endregion PostStatus

    #endregion Fields

    #region Properties

    private string _imageName;

    public TwitterEntry TweetToShow
    {
      get { return _tweetToShow; }
      set
      {
        _tweetToShow = value;
        UpdateConversation();
        RaisePropertyChanged(() => TweetToShow);
      }
    }

    public int NumberNewReplies
    {
      get { return _numberNewReplies; }
      set
      {
        _numberNewReplies = value;
        RaisePropertyChanged();
      }
    }

    public int NumberNewFriends
    {
      get { return _numberNewFriends; }
      set
      {
        _numberNewFriends = value;
        RaisePropertyChanged();
      }
    }

    public int NumberNewDm
    {
      get { return _numberNewDm; }
      set
      {
        _numberNewDm = value;
        RaisePropertyChanged();
      }
    }

    public int NumberNewFavorit
    {
      get { return _numberNewFavorit; }
      set
      {
        _numberNewFavorit = value;
        RaisePropertyChanged();
      }
    }

    public int NumberNewUser
    {
      get { return _numberNewUser; }
      set
      {
        _numberNewUser = value;
        RaisePropertyChanged();
      }
    }

    public string ImageName
    {
      get { return _imageName; }
      set
      {
        _imageName = value;
        RaisePropertyChanged();
        RaisePropertyChanged(() => IsImgShow);
      }
    }

    public Visibility IsImgShow => String.IsNullOrEmpty(ImageName) ? Visibility.Collapsed : Visibility.Visible;

    public Visibility IsVisibleBtnPhoto
    {
      get { return _isVisibleBtnPhoto; }
      set
      {
        _isVisibleBtnPhoto = value;
        RaisePropertyChanged();
      }
    }

    public Visibility IsDelBtnImgShow
    {
      get { return _isDelBtnImgShow; }
      set
      {
        _isDelBtnImgShow = value;
        RaisePropertyChanged();
      }
    }

    public bool IsUpdate { get; set; }

    #region Activ & UnActiv Buttons

    public bool IsActivFriends
    {
      get
      {
        if (!ViewState && TwitterSelectedWorkspace != null && TwitterSelectedWorkspace.Settings != null)
          return TwitterSelectedWorkspace.WorkspaceSettings.Type.Equals(EnumTwitterType.Friends);

        return !CanAddColumn(new object[3] { EnumTwitterType.Friends.ToString(), null, null });
      }
      set
      {
        if (value)
          AddColumn(new object[] { "Friends", null, null });
      }
    }

    public bool IsActivUser
    {
      get
      {
        if (!ViewState && TwitterSelectedWorkspace != null && TwitterSelectedWorkspace.Settings != null)
          return TwitterSelectedWorkspace.WorkspaceSettings.Type.Equals(EnumTwitterType.User);

        return !CanAddColumn(new object[3] { EnumTwitterType.User.ToString(), null, null });
      }
      set
      {
        if (value)
          AddColumn(new object[] { "User", null, null });
      }
    }

    public bool IsActivFavorites
    {
      get
      {
        if (!ViewState && TwitterSelectedWorkspace != null && TwitterSelectedWorkspace.Settings != null)
          return TwitterSelectedWorkspace.WorkspaceSettings.Type.Equals(EnumTwitterType.Favorites);

        return !CanAddColumn(new object[3] { EnumTwitterType.Favorites.ToString(), null, null });
      }
      set
      {
        if (value)
          AddColumn(new object[] { "Favorites", null, null });
      }
    }

    public bool IsActivReplies
    {
      get
      {
        if (!ViewState && TwitterSelectedWorkspace != null && TwitterSelectedWorkspace.Settings != null)
          return TwitterSelectedWorkspace.WorkspaceSettings.Type.Equals(EnumTwitterType.Replies);

        return !CanAddColumn(new object[3] { EnumTwitterType.Replies.ToString(), null, null });
      }
      set
      {
        if (value)
          AddColumn(new object[] { "Replies", null, null });
      }
    }

    public bool IsActivDirectMessages
    {
      get
      {
        if (!ViewState && TwitterSelectedWorkspace != null && TwitterSelectedWorkspace.Settings != null)
          return TwitterSelectedWorkspace.WorkspaceSettings.Type.Equals(EnumTwitterType.DirectMessages);

        return !CanAddColumn(new object[3] { EnumTwitterType.DirectMessages.ToString(), null, null });
      }
      set
      {
        if (value)
          AddColumn(new object[] { "DirectMessages", null, null });
      }
    }

    public bool IsActivList
    {
      get
      {
        RaisePropertyChanged("IsListVisible");
        if (!ViewState && TwitterSelectedWorkspace != null && TwitterSelectedWorkspace.Settings != null)
        {
          return TwitterSelectedWorkspace.WorkspaceSettings.Type.Equals(EnumTwitterType.List);
        }

        return !CanAddColumn(new object[3] { EnumTwitterType.List.ToString(), null, null });
      }
      set
      {
        if (value)
        {
          AddColumn(new object[] { "List", null, null });
        }
        else
        {
        }
      }
    }

    public List<bool> IsActivLists { get; set; }

    public List<string> ActivLists { get; set; }

    public bool IsOpenLists => true;

    //public bool IsShowOnlyFactery
    //{
    //  get { return _isShowOnlyFactery; }
    //  set
    //  {
    //    _isShowOnlyFactery = value;
    //    ServiceMessenger.Send("UpdateView");
    //    RaisePropertyChanged("IsShowOnlyFactery");
    //  }
    //}

    public override Visibility ClearTweetsVisibility => Visibility.Visible;

    //public Visibility IsFacteryInfoVisible
    //{
    //  get { return _isShowFactery ? Visibility.Visible : Visibility.Collapsed; }
    //  set { return; }
    //}

    //public bool IsShowFactery
    //{
    //  get { return _isShowFactery; }
    //  set
    //  {
    //    _isShowFactery = value;
    //    if (!value)
    //    {
    //      IsFilterFactory = false;
    //      IsShowOnlyFactery = false;
    //    }
    //    ServiceMessenger.Send("UpdateView");
    //    RaisePropertyChanged("IsShowFactery");
    //    RaisePropertyChanged("IsFacteryInfoVisible");
    //  }
    //}

    //public bool IsFilterFactory
    //{
    //  get { return _isFilterFactory; }
    //  set
    //  {
    //    _isFilterFactory = value;
    //    ServiceMessenger.Send("UpdateView");
    //    RaisePropertyChanged("IsFilterFactory");
    //  }
    //}

    #endregion Activ & UnActiv Buttons

    #region Current Account

    public override string ImageUser => CurrentAccount != null ? CurrentAccount.PictureUrl : string.Empty;

    protected internal UserAccount CurrentAccount
    {
      get
      {
        try
        {
          if (SobeesSettings.Accounts != null && SobeesSettings.Accounts.Count > 0 &&
              !string.IsNullOrEmpty(Settings.UserName))
            return SobeesSettings.Accounts[
                SobeesSettings.Accounts.IndexOf(
                    new UserAccount(Settings.UserName,
                        EnumAccountType.Twitter))];
        }
        catch (Exception e)
        {
          Console.WriteLine(e);
        }
        return null;
      }
    }

    protected internal string PasswordHash
    {
      get
      {
        if (CurrentAccount != null) return CurrentAccount.PasswordHash;
        return string.Empty;
      }
    }

    #endregion Current Account

    #region PostStatus

    public string ReplyId { get; set; }

    public string RetweetId { get; set; }

    public string RetweetText { get; set; }

    public int CursorPosition
    {
      get { return _cursorPosition; }
      set
      {
        _cursorPosition = value;
        RaisePropertyChanged("CursorPosition");
      }
    }

    public bool MoveCursor
    {
      get { return _moveCursor; }
      set
      {
        _moveCursor = value;
        RaisePropertyChanged("MoveCursor");
      }
    }

    public Visibility VisibilityButtonRetweet
    {
      get { return _visibilityButtonRetweet; }
      set
      {
        _visibilityButtonRetweet = value;
        RaisePropertyChanged("VisibilityButtonRetweet");
      }
    }

    #endregion PostStatus

    #region Lists

    private static readonly Object CanAdd = new Object();

    public Visibility IsListVisible
    {
      get
      {
        if (ViewState)
          return Visibility.Visible;
        if (TwitterSelectedWorkspace != null)
        {
          return TwitterSelectedWorkspace.WorkspaceSettings.Type == EnumTwitterType.List
              ? Visibility.Visible
              : Visibility.Collapsed;
        }
        return TwitterListDisplay.Count > 0
            ? Visibility.Visible
            : Visibility.Collapsed;
      }
    }

    public ObservableCollection<TwitterList> TwitterList
    {
      get
      {
        if (_twitterList == null)
        {
          _twitterList = new ObservableCollection<TwitterList>();
          RaisePropertyChanged("TwitterList");
        }
        return _twitterList;
      }
    }

    public ObservableCollection<TwitterList> TwitterListDisplay
    {
      get
      {
        if (_twitterListDisplay != null) return _twitterListDisplay;
        _twitterListDisplay = new ObservableCollection<TwitterList>();
        RaisePropertyChanged("TwitterListDisplay");
        return _twitterListDisplay;
      }
    }

    public ObservableCollection<TwitterList> TwitterListOwn
    {
      get
      {
        if (_twitterListOwn != null) return _twitterListOwn;
        _twitterListOwn = new ObservableCollection<TwitterList>();
        RaisePropertyChanged("TwitterListOwn");
        return _twitterListOwn;
      }
    }

    private readonly object _listLock = new object();

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

    public string ConfirmationText
    {
      get { return _confirmationText; }
      set
      {
        _confirmationText = value;
        RaisePropertyChanged("ConfirmationText");
      }
    }

    public TwitterList SelectedList
    {
      get { return _selectedList; }
      set
      {
        _selectedList = value;
        RaisePropertyChanged("SelectedList");
      }
    }

    public TwitterList CurrentList
    {
      get { return _currentList; }
      set
      {
        _currentList = value;
        OpenList(value);
        RaisePropertyChanged();
      }
    }

    public string NameNewList
    {
      get { return _nameNewList; }
      set
      {
        _nameNewList = value;
        RaisePropertyChanged();
      }
    }

    public bool IsPrivate
    {
      get { return _isPrivate; }
      set
      {
        _isPrivate = value;
        RaisePropertyChanged();
      }
    }

    public string CreateNewList
    {
      get { return _createNewList; }
      set
      {
        _createNewList = value;
        RaisePropertyChanged("CreateNewList");
      }
    }

    public List<User> Friends
    {
      get { return _friends ?? (_friends = new List<User>()); }
      set { _friends = value; }
    }

    #region ViewsManaging

    public bool ShowApiUsage
    {
      get
      {
        //if(TwitterRateLimit == null)
        //  return false;

        return ((TwitterSettings)Settings).ShowApiUsage;
      }
      set
      {
        ((TwitterSettings)Settings).ShowApiUsage = value;
        RaisePropertyChanged("ShowApiUsage");
      }
    }

    public bool ViewState
    {
      get
      {
        if (CurrentView == TwitterTypeView.Columns)
        {
          return true;
        }
        return false;
      }
    }

    public TwitterTypeView CurrentView { get; set; }

    public TwitterFrontView CurrentFrontView { get; set; }

    public EnumTwitterProfile CurrentProfilView { get; set; }

    public TwitterMainTypeView CurrentMainView { get; set; }

    public DataTemplate ColumnsDataTemplate
    {
      get
      {
        const string dt = "<DataTemplate x:Name='dtMainView' " +
            "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
            "xmlns:Views='clr-namespace:Sobees.Controls.Twitter.Views;assembly=Sobees.Controls.Twitter' " +
            "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >" +
            "<Views:UcColumsView/> " +
            "</DataTemplate>";

        var stringReader = new StringReader(dt);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
    }

    public DataTemplate ProfileTweetDataTemplate
    {
      get
      {
        const string dt = "<DataTemplate x:Name='dtMainView' " +
            "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
            "xmlns:Views='clr-namespace:Sobees.Controls.Twitter.Views;assembly=Sobees.Controls.Twitter' " +
            "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >" +
            "<Views:Tweet/> " +
            "</DataTemplate>";

        var stringReader = new StringReader(dt);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
    }

    public DataTemplate TabDataTemplate
    {
      get
      {
        const string dt = "<DataTemplate x:Name='dtMainView' " +
            "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
            "xmlns:Views='clr-namespace:Sobees.Controls.Twitter.Views;assembly=Sobees.Controls.Twitter' " +
            "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >" +
            "<Views:UcTabView/> " +
            "</DataTemplate>";

        var stringReader = new StringReader(dt);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
    }

    public DataTemplate AddToListDataTemplate
    {
      get
      {
        const string dt = "<DataTemplate x:Name='dtMainView' " +
            "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
            "xmlns:Controls='clr-namespace:Sobees.Controls.Twitter.Controls;assembly=Sobees.Controls.Twitter' " +
            "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >" +
            "<Controls:UcAddToList/> " +
            "</DataTemplate>";

        var stringReader = new StringReader(dt);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
    }

    public DataTemplate FollowSobeesDataTemplate
    {
      get
      {
        const string dt = "<DataTemplate x:Name='dtMainView' " +
            "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
            "xmlns:Controls='clr-namespace:Sobees.Controls.Twitter.Controls;assembly=Sobees.Controls.Twitter' " +
            "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >" +
            "<Controls:UcFollowSobees/> " +
            "</DataTemplate>";

        var stringReader = new StringReader(dt);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
    }

    public DataTemplate BlockUserDataTemplate
    {
      get
      {
        const string dt = "<DataTemplate x:Name='dtMainView' " +
            "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
            "xmlns:Controls='clr-namespace:Sobees.Controls.Twitter.Controls;assembly=Sobees.Controls.Twitter' " +
            "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >" +
            "<Controls:UcBlockUser/> " +
            "</DataTemplate>";

        var stringReader = new StringReader(dt);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
    }

    public override DataTemplate MainTemplateView
    {
      get
      {
        switch (CurrentView)
        {
        case TwitterTypeView.Credentials:
          break;

        case TwitterTypeView.Columns:
          return ColumnsDataTemplate;

        case TwitterTypeView.Tab:
          return TabDataTemplate;

        case TwitterTypeView.Profile:
          break;

        case TwitterTypeView.OneStream:
          break;

        default:
          break;
        }
        return null;
      }
      set { base.MainTemplateView = value; }
    }

    public override object MenuItems
    {
      get
      {
        var stackPanel = new StackPanel { Orientation = Orientation.Vertical };

        //Create ToggleButton

        var tgbtnFriends = new BToggleButton
        {
          BPath = "PathStyleTwitterHomeH1",
          Command = new RelayCommand(() => AddColumn(new object[] { "Friends", null, null })),
          IsChecked = true
        };

        var bindingFriends = new Binding("IsActivFriends")
        {
          Source = this,
          Mode = BindingMode.TwoWay,
          FallbackValue = true,
        };
        tgbtnFriends.ToolTip =
            new LocText("Sobees.Configuration.BGlobals:Resources:rdTabHome").ResolveLocalizedValue();
        tgbtnFriends.SetBinding(BToggleButton.IsCheckedProperty, bindingFriends);
        var bindingFriendsNb = new Binding("NumberNewFriends") { Source = this, Mode = BindingMode.TwoWay };
        tgbtnFriends.SetBinding(BToggleButton.BNumberNewProperty, bindingFriendsNb);
        var tgbtnDirectMessages = new BToggleButton
        {
          BPath = "PathStyleTwitterDmH1",
          Command =
              new RelayCommand(() => AddColumn(new object[] { "DirectMessages", null, null }))
        };
        var bindingDirectMessages = new Binding("IsActivDirectMessages")
        {
          Source = this,
          Mode = BindingMode.TwoWay
        };
        tgbtnDirectMessages.ToolTip =
            new LocText("Sobees.Configuration.BGlobals:Resources:btnDMb").ResolveLocalizedValue();
        tgbtnDirectMessages.SetBinding(BToggleButton.IsCheckedProperty, bindingDirectMessages);
        var bindingMessagesNb = new Binding("NumberNewDm") { Source = this, Mode = BindingMode.TwoWay };
        tgbtnDirectMessages.SetBinding(BToggleButton.BNumberNewProperty, bindingMessagesNb);
        var tgbtnUser = new BToggleButton
        {
          BPath = "PathStyleTwitterUserH1",
          Command = new RelayCommand(() => AddColumn(new object[] { "User", null, null })),
        };
        var bindingUser = new Binding("IsActivUser") { Source = this, Mode = BindingMode.TwoWay };

        tgbtnUser.ToolTip =
            new LocText("Sobees.Configuration.BGlobals:Resources:btnSent").ResolveLocalizedValue();

        tgbtnUser.SetBinding(BToggleButton.IsCheckedProperty, bindingUser);
        var bindingUserNb = new Binding("NumberNewUser") { Source = this, Mode = BindingMode.TwoWay };
        tgbtnUser.SetBinding(BToggleButton.BNumberNewProperty, bindingUserNb);
        var tgbtnReplies = new BToggleButton
        {
          BPath = "PathStyleTwitterRepliesH1",
          Command = new RelayCommand(() => AddColumn(new object[] { "Replies", null, null })),
        };
        var bindingReplies = new Binding("IsActivReplies") { Source = this, Mode = BindingMode.TwoWay };

        tgbtnReplies.ToolTip =
            new LocText("Sobees.Configuration.BGlobals:Resources:btnReplies").ResolveLocalizedValue();

        tgbtnReplies.SetBinding(BToggleButton.IsCheckedProperty, bindingReplies);
        var bindingRepliesNb = new Binding("NumberNewReplies") { Source = this, Mode = BindingMode.TwoWay };
        tgbtnReplies.SetBinding(BToggleButton.BNumberNewProperty, bindingRepliesNb);
        var tgbtnFavorites = new BToggleButton
        {
          BPath = "PathStyleTwitterFavoritH1",
          Command = new RelayCommand(() => AddColumn(new object[] { "Favorites", null, null })),
        };
        var bindingFavorites = new Binding("IsActivFavorites") { Source = this, Mode = BindingMode.TwoWay };

        tgbtnFavorites.ToolTip =
            new LocText("Sobees.Configuration.BGlobals:Resources:btnFavorites").ResolveLocalizedValue();

        tgbtnFavorites.SetBinding(BToggleButton.IsCheckedProperty, bindingFavorites);
        var bindingFavoritesNb = new Binding("NumberNewFavorit") { Source = this, Mode = BindingMode.TwoWay };
        tgbtnFavorites.SetBinding(BToggleButton.BNumberNewProperty, bindingFavoritesNb);
        stackPanel.Children.Add(tgbtnFriends);
        stackPanel.Children.Add(tgbtnReplies);
        stackPanel.Children.Add(tgbtnDirectMessages);
        stackPanel.Children.Add(tgbtnUser);
        stackPanel.Children.Add(tgbtnFavorites);

        var btnLists = new ToggleButton
        {
          Content =
              new Path { Style = (Style)(Application.Current.TryFindResource("PathStyleTwitterListH1")) },
          ToolTip =
              new LocText("Sobees.Configuration.BGlobals:Resources:btnManageLists").ResolveLocalizedValue
                  (),
          Style = (Style)(Application.Current.TryFindResource("BtntColumnLeftH1")),
          Command = new RelayCommand(ShowLists),
        };

        var bindingLists = new Binding("IsActivList") { Source = this, Mode = BindingMode.TwoWay };
        btnLists.SetBinding(ToggleButton.IsCheckedProperty, bindingLists);
        stackPanel.Children.Add(btnLists);

        return stackPanel;
      }
    }

    protected List<Button> LstItemLists { get; set; }

    public override DataTemplate DataTemplateView
    {
      get
      {
        switch (CurrentMainView)
        {
        case TwitterMainTypeView.Credentials:
          return CredentialsViewModel.DataTemplateView;

        case TwitterMainTypeView.Base:
          return base.DataTemplateView;

        default:
          return base.DataTemplateView;
        }
      }
      set { base.DataTemplateView = value; }
    }

    public override BWorkspaceViewModel MainViewModel
    {
      get { return this; }
      set { base.MainViewModel = value; }
    }

    public override DataTemplate FrontTemplateView
    {
      get
      {
        switch (CurrentFrontView)
        {
        case TwitterFrontView.None:
          return null;

        case TwitterFrontView.List:
          return ListViewModel.DataTemplateView;

        case TwitterFrontView.Profile:
          return ProfileViewModel.DataTemplateView;

        case TwitterFrontView.Settings:
          return SettingsViewModel.DataTemplateView;

        case TwitterFrontView.Confirmation:
          return BlockUserDataTemplate;

        case TwitterFrontView.AddToList:
          return AddToListDataTemplate;

        case TwitterFrontView.FollowSobees:
          return FollowSobeesDataTemplate;

        default:
          throw new ArgumentOutOfRangeException();
        }
      }
      set { base.FrontTemplateView = value; }
    }

    public override BWorkspaceViewModel ProfileView
    {
      get
      {
        switch (CurrentProfilView)
        {
        case EnumTwitterProfile.User:
          return _profileViewModel;

        case EnumTwitterProfile.Tweet:
          return TweetToShow != null ? this : null;

        default:
          throw new ArgumentOutOfRangeException();
        }
      }
      set { base.ProfileView = value; }
    }

    public override DataTemplate ProfileTemplate
    {
      get
      {
        switch (CurrentProfilView)
        {
        case EnumTwitterProfile.User:
          return _profileViewModel == null ? null : _profileViewModel.DataTemplateView;

        case EnumTwitterProfile.Tweet:
          return TweetToShow != null ? ProfileTweetDataTemplate : null;

        default:
          throw new ArgumentOutOfRangeException();
        }
      }
    }

    public override BWorkspaceViewModel FrontViewModel
    {
      get
      {
        switch (CurrentFrontView)
        {
        case TwitterFrontView.None:
          return null;

        case TwitterFrontView.List:
          return ListViewModel;

        case TwitterFrontView.Profile:
          return ProfileViewModel;

        case TwitterFrontView.Settings:
          return SettingsViewModel;

        case TwitterFrontView.Confirmation:
          return this;

        case TwitterFrontView.AddToList:
          return this;

        case TwitterFrontView.FollowSobees:
          return this;

        default:
          throw new ArgumentOutOfRangeException();
        }
      }
      set { base.FrontViewModel = value; }
    }

    #endregion ViewsManaging

    #endregion Lists

    #region Others

    public User User { get; set; }

    public TwitterRateLimit TwitterRateLimit
    {
      get { return _twitterRateLimit; }
      set
      {
        _twitterRateLimit = value;
        RaisePropertyChanged("TwitterRateLimit");
        RaisePropertyChanged("ShowApiUsage");
      }
    }

    public TwitterRateLimit TwitterRateLimitIP { get; set; }

    #endregion Others

    #endregion Properties

    #region Workspaces

    public CredentialsViewModel CredentialsViewModel => _credentialsViewModel ??
                                                        (_credentialsViewModel = new CredentialsViewModel(this, ServiceMessenger));

    public ProfileViewModel ProfileViewModel => _profileViewModel ?? (_profileViewModel = new ProfileViewModel(this, ServiceMessenger));

    public SettingsViewModel SettingsViewModel => _settingsViewModel ?? (_settingsViewModel = new SettingsViewModel(this, ServiceMessenger));

    public ListViewModel ListViewModel => _listViewModel ?? (_listViewModel = new ListViewModel(this, ServiceMessenger));

    #endregion Workspaces

    #region Twitter Workspaces

    /// <summary>
    ///     Returns the collection of available workspaces to display.
    ///     A 'workspace' is a ViewModel that can request to be closed.
    /// </summary>
    public ObservableCollection<TwitterWorkspaceViewModel> TwitterWorkspaces
    {
      get { return _twitterWorkspaces ?? (_twitterWorkspaces = new ObservableCollection<TwitterWorkspaceViewModel>()); }
      set
      {
        _twitterWorkspaces = value;
        RaisePropertyChanged("TwitterWorkspaces");
      }
    }

    public TwitterWorkspaceViewModel TwitterSelectedWorkspace
    {
      get { return _twitterSelectedWorkspace; }
      set
      {
        _twitterSelectedWorkspace = value;
        RaisePropertyChanged("TwitterSelectedWorkspace");

        foreach (var tWorkspace in TwitterWorkspaces)
        {
          tWorkspace.IsSelected = tWorkspace == _twitterSelectedWorkspace;
          var propertyToNotify = string.Format("IsActiv{0}", tWorkspace.WorkspaceSettings.Type);
          RaisePropertyChanged(propertyToNotify);
        }
      }
    }

    #endregion Twitter Workspaces

    #region Commands

    /// <summary>
    ///     Returns the command that, when invoked, attempts
    ///     to update the content of the control
    /// </summary>
    public RelayCommand AddColumnCommand { get; private set; }

    /// <summary>
    ///     Commd to use the old type of retweet
    /// </summary>
    public RelayCommand ReTweetOldCommand { get; private set; }

    /// <summary>
    ///     Commd to use the old type of retweet
    /// </summary>
    public RelayCommand<TwitterWorkspaceViewModel> CloseColumnCommand { get; private set; }

    /// <summary>
    ///     Commd to use the old type of retweet
    /// </summary>
    public RelayCommand ClearTweetsCommand { get; private set; }

    /// <summary>
    ///     Commd to use the old type of retweet
    /// </summary>
    public RelayCommand OpenListEditorCommand { get; private set; }

    public RelayCommand CloseDetailsCommand { get; private set; }

    public RelayCommand YesFollowingSobeesCommand { get; private set; }

    public RelayCommand NoFollowingSobeesCommand { get; private set; }

    /// <summary>
    ///     Command for shorten URL
    /// </summary>
    public RelayCommand ShortenUrlCommand { get; private set; }

    /// <summary>
    ///     Command for Upload an image on Twitpic
    /// </summary>
    public RelayCommand UploadImageCommand { get; private set; }

    public RelayCommand DeleteImageCommand { get; set; }

    /// <summary>
    ///     Command for tweetshrink the text.
    /// </summary>
    public RelayCommand TweetShrinkCommand { get; private set; }

    /// <summary>
    ///     Command for close cancel confirmation
    /// </summary>
    public RelayCommand CancelConfirmationCommand { get; private set; }

    /// <summary>
    ///     Command for close confirmation
    /// </summary>
    public RelayCommand ConfirmationCommand { get; private set; }

    protected override void InitCommands()
    {
      base.InitCommands();
      ClearTweetsCommand = new RelayCommand(ClearTweets);
      ShortenUrlCommand = new RelayCommand(ShortenUrl, CanShortenUrl);
      //UploadImageCommand = new RelayCommand(UploadImage);
      DeleteImageCommand = new RelayCommand(DeleteImage);
      OpenListEditorCommand = new RelayCommand(ShowListsEditor);
      TweetShrinkCommand = new RelayCommand(TweetShrink);
      CloseColumnCommand = new RelayCommand<TwitterWorkspaceViewModel>(CloseColumn);
      CancelConfirmationCommand = new RelayCommand(CloseConfirmation);
      ConfirmationCommand = new RelayCommand(() =>
      {
        if (_actionToConfirm == "Block")
        {
          BlockUserConfirmation();
        }
        if (_actionToConfirm == "Report")
        {
          ReportConfirmation();
        }
        if (_actionToConfirm == "AddToList")
        {
          AddToListConfirmation();
        }
      }, CanConfirmCommand);
      ReTweetOldCommand = new RelayCommand(() =>
      {
        VisibilityButtonRetweet = Visibility.Collapsed;
        RetweetId = string.Empty;
        CursorPosition = (RetweetText + Status).Length;
        MoveCursor = true;
        Status = RetweetText + Status;
      });
      CloseDetailsCommand = new RelayCommand(CloseProfile);
      YesFollowingSobeesCommand = new RelayCommand(FollowSobees);
      NoFollowingSobeesCommand = new RelayCommand(() =>
      {
        CurrentAccount.IsFanAsk = true;
        CurrentFrontView = TwitterFrontView.None;
        RaisePropertyChanged(() => FrontTemplateView);
        RaisePropertyChanged(() => FrontViewModel);
      });
    }

    private bool CanConfirmCommand()
    {
      if (String.Equals(_actionToConfirm, "AddToList", StringComparison.CurrentCulture) && SelectedList == null)
        return false;

      return true;
    }

    private void CloseColumn(TwitterWorkspaceViewModel view)
    {
      TwitterWorkspaces.Remove(view);
      ((TwitterSettings)Settings).WorkspaceSettingsCollection.Remove(view.WorkspaceSettings);
      view.Cleanup();
      view = null;
    }

    private void AddToListConfirmation()
    {
      Action mainAction = () => TwitterLibV11.AddToList(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET, CurrentAccount.SessionKey, CurrentAccount.Secret, Settings.UserName, SelectedList.Slug, User.Id, ProxyHelper.GetConfiguredWebProxy(SobeesSettings));

      var task = Task.Factory.StartNew(mainAction, CancellationToken.None, TaskCreationOptions.None, UiContext.Instance.Current);
      task.ContinueWith(_ => OnGetMenuActionAsyncCompleted(null));
    }

    /// <summary>
    /// CloseConfirmation
    /// </summary>
    private void CloseConfirmation()
    {
      CurrentFrontView = TwitterFrontView.None;
      RaisePropertyChanged(() => FrontTemplateView);
      RaisePropertyChanged(() => FrontViewModel);
      User = null;
      _actionToConfirm = string.Empty;
    }

    /// <summary>
    /// OnGetMenuActionAsyncCompleted
    /// </summary>
    /// <param name="error"></param>
    private void OnGetMenuActionAsyncCompleted(string error)
    {
      try
      {
        CloseConfirmation();
        if (!string.IsNullOrEmpty(error))
          ShowErrorMsg(error);
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    #endregion Commands

    #region Constructors

    public TwitterViewModel()
    {
    }

    public TwitterViewModel(BPosition bposition, BServiceWorkspace serviceWorkspace, string settings)
      : base(bposition, serviceWorkspace)
    {
      CurrentView = TwitterTypeView.Credentials;
      CurrentMainView = TwitterMainTypeView.Credentials;

      //Register to the Messenger
      ServiceMessenger.Register<string>(this, DoAction);
      ServiceMessenger.Register<BMessage>(this, DoActionMessage);

      //restore Settings
      if (string.IsNullOrEmpty(settings))
      {
        Settings = new TwitterSettings();
      }
      else
      {
        Settings = GenericSerializer.DeserializeObject(settings, typeof(TwitterSettings)) as TwitterSettings;
      }

      _timerCreateFriendList = new DispatcherTimer { Interval = new TimeSpan(0, 5, 0) };
    }

    #endregion Constructors

    #region Methods

    #region Overriden Methods

    public override bool UseTimer => false;

    public override void DoAction(string param)
    {
      switch (param)
      {
      case "Connected":
        LoadDefaultworkSpace();
        SwitchViews();
        CurrentMainView = TwitterMainTypeView.Base;
        RaisePropertyChanged("DataTemplateView");
        UpdateTwitterList(null);
        UpdateCurrentUserData();
        UpdateRemainingApiAsync();
        LoadFriendsList();
        FollowingSobees();
        Messenger.Default.Send("SaveSettings");
        break;

      case "ConnectedStored":
        LoadWorkSpace();
        CurrentMainView = TwitterMainTypeView.Base;
        RaisePropertyChanged("DataTemplateView");
        UpdateTwitterList(null);
        UpdateCurrentUserData();
        UpdateRemainingApiAsync();
        LoadFriendsList();
        FollowingSobees();
        break;

      case "CloseList":
        CloseLists();
        break;

      case "CloseProfile":
        CloseProfile();
        break;

      case "SaveSettingsTW":
        SaveSettings();
        CloseSettings();
        Messenger.Default.Send("SaveSettings");
        break;

      case "CloseSettingsTW":
        CloseSettings();
        break;

      case "UpdateList":
        UpdateTwitterList(null);
        break;

      case "UpdateAPI":
        if (ShowApiUsage)
        {
          UpdateRemainingApiAsync();
        }

        //Update the number of newmessage
        if (SobeesSettings.AlertsEnabled)
        {
          UpdatenumberNewMessage();
        }
        break;

      case "UpdateViewFilter":
        ServiceMessenger.Send("UpdateView");
        break;
      }
      base.DoAction(param);
    }

    protected override void OnSettingsUpdated()
    {
      try
      {
        base.OnSettingsUpdated();

        foreach (var workspace in TwitterWorkspaces)
          workspace.UpdateView();
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    public override void DoActionMessage(BMessage message)
    {
      switch (message.Action)
      {
      case "ShowError":
        ShowErrorMsg(message.Parameter as string);
        break;

      case "OpenList":
        OpenList(message.Parameter as TwitterListShow);
        break;

      case "CloseList":
        CloseList(message.Parameter as TwitterListShow);
        break;

      case "BlockUser":
        BlockUser(message.Parameter as TwitterEntry);
        break;

      case "ReportSpam":
        ReportSpam(message.Parameter as TwitterEntry);
        break;

      case "ReplyToAll":
        ReplyToAll(message.Parameter as TwitterEntry);
        break;

      case "AddToFavorite":
        AddToFavorite(message.Parameter as TwitterEntry);
        break;

      case "AddToList":
        AddToList(message.Parameter as TwitterEntry);
        break;

      case "DeleteStatus":
        DeleteStatus(message.Parameter as TwitterEntry);
        break;

      case "Retweet":
        Retweet(message.Parameter as TwitterEntry);
        break;

      case "Reply":
        Reply(message.Parameter as TwitterEntry);
        break;

      case "DM":
        DM(message.Parameter as TwitterEntry);
        break;

      case "ShowProfile":
        ShowProfile(message.Parameter as TwitterUser);
        break;

      case "ShowTweet":
        ShowTweet(message.Parameter as TwitterEntry);
        break;
      }
      base.DoActionMessage(message);
    }

    public override void DeleteUser(UserAccount account)
    {
      if (account.Type == EnumAccountType.Twitter && account.Login == Settings.UserName)
      {
        IsClosed = true;
        CloseControl();
      }
    }

    public override void ShowSettings()
    {
      CloseProfile();
      CurrentFrontView = TwitterFrontView.Settings;
      RaisePropertyChanged(() => FrontTemplateView);
      RaisePropertyChanged(() => FrontViewModel);
    }

    public override void SetAccount(UserAccount account)
    {
      Settings.UserName = account.Login;
      LoadDefaultworkSpace();
      SwitchViews();
      CurrentMainView = TwitterMainTypeView.Base;
      RaisePropertyChanged("DataTemplateView");
      UpdateTwitterList(null);
      UpdateCurrentUserData();
      UpdateRemainingApiAsync();
      LoadFriendsList();
      FollowingSobees();
    }

    public override void RaiseStatusChanged()
    {
      base.RaiseStatusChanged();
      ShortenUrlCommand.RaiseCanExecuteChanged();
    }

    #endregion Overriden Methods

    /// <summary>
    ///     Ask User if he want to follow sobees
    /// </summary>
    private void FollowingSobees()
    {
      if (CurrentAccount.IsFanAsk) return;

      string error;
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

          if (TwitterLibV11.FriendshipExists(BGlobals.TWITTER_OAUTH_KEY,
              BGlobals.TWITTER_OAUTH_SECRET,
              CurrentAccount.SessionKey, CurrentAccount.Secret,
              Settings.UserName, "sobees", out error,
              ProxyHelper.GetConfiguredWebProxy(SobeesSettings)))
          {
            CurrentAccount.IsFanAsk = true;
            return;
          }
          CurrentFrontView = TwitterFrontView.FollowSobees;
          RaisePropertyChanged(() => FrontTemplateView);
          RaisePropertyChanged(() => FrontViewModel);
        };

        worker.RunWorkerAsync();
      }
    }

    private void FollowSobees()
    {
      //string error;
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
          CurrentAccount.IsFanAsk = true;
          await TwitterLibV11.Follow(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET, CurrentAccount.SessionKey, CurrentAccount.Secret, "sobees", false);
          CurrentFrontView = TwitterFrontView.None;
          RaisePropertyChanged(() => FrontTemplateView);
          RaisePropertyChanged(() => FrontViewModel);
        };

        worker.RunWorkerAsync();
      }
    }

    private void CloseProfile()
    {
      TweetToShow = null;
      CurrentProfilView = EnumTwitterProfile.User;
      if (_profileViewModel != null) _profileViewModel.Cleanup();
      ;
      _profileViewModel = null;
      RaisePropertyChanged(() => ProfileTemplate); ;
      RaisePropertyChanged(() => ProfileView); ;
    }

    private void UpdatenumberNewMessage()
    {
      NumberNewMessage = 0;
      foreach (var workspace in TwitterWorkspaces)
      {
        NumberNewMessage += workspace.NumberNewMessage;
        switch (workspace.WorkspaceSettings.Type)
        {
        case EnumTwitterType.Replies:
          NumberNewReplies = workspace.NumberNewMessage;
          break;

        case EnumTwitterType.DirectMessages:
          NumberNewDm = workspace.NumberNewMessage;
          break;

        case EnumTwitterType.Friends:
          NumberNewFriends = workspace.NumberNewMessage;
          break;

        case EnumTwitterType.Groups:
          break;

        case EnumTwitterType.User:
          NumberNewUser = workspace.NumberNewMessage;
          break;

        case EnumTwitterType.Favorites:
          NumberNewFavorit = workspace.NumberNewMessage;
          break;

        case EnumTwitterType.List:
          break;

        default:
          throw new ArgumentOutOfRangeException();
        }
      }
    }

    /// <summary>
    ///     UpdateCurrentUserData
    /// </summary>
    private void UpdateCurrentUserData()
    {
      string error;
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

          var result = TwitterLibV11.GetUserInfo(BGlobals.TWITTER_OAUTH_KEY,
              BGlobals.TWITTER_OAUTH_SECRET,
              CurrentAccount.SessionKey, CurrentAccount.Secret,
              Settings.UserName, out error,
              ProxyHelper.GetConfiguredWebProxy(SobeesSettings));
          if (result == null) return;
          CurrentAccount.PictureUrl = result.ProfileImgUrl;
          CurrentAccount.UserId = Convert.ToInt64(result.Id);
          RaisePropertyChanged("ImageUser");
        };

        worker.RunWorkerAsync();
      }
    }

    private void SwitchViews()
    {
      CurrentView = ((TwitterSettings)Settings).ViewState ? TwitterTypeView.Columns : TwitterTypeView.Tab;
      RaisePropertyChanged(() => MainTemplateView);
      foreach (var tWorkspace in TwitterWorkspaces)
      {
        tWorkspace.IsSelected = tWorkspace == _twitterSelectedWorkspace;
        var propertyToNotify = string.Format("IsActiv{0}", tWorkspace.WorkspaceSettings.Type);
        RaisePropertyChanged(propertyToNotify);
      }
    }

    private void CloseSettings()
    {
      CurrentFrontView = TwitterFrontView.None;
      RaisePropertyChanged(() => FrontTemplateView);
      RaisePropertyChanged(() => FrontViewModel);
      _settingsViewModel.Cleanup();
      _settingsViewModel = null;
    }

    private void SaveSettings()
    {
      if (!SettingsViewModel.IsDirty)
        return;
      try
      {
        if (Settings.NbMaxPosts != _settingsViewModel.MaxTweets)
        {
          Settings.NbMaxPosts = _settingsViewModel.MaxTweets;
        }
        if (((TwitterSettings)Settings).ViewState != _settingsViewModel.ViewState)
        {
          ((TwitterSettings)Settings).ViewState = _settingsViewModel.ViewState;
          SwitchViews();
        }

        if (((TwitterSettings)Settings).ViewStateTweets != _settingsViewModel.ViewStateTweets)
        {
          ((TwitterSettings)Settings).ViewStateTweets = _settingsViewModel.ViewStateTweets;
        }

        if (((TwitterSettings)Settings).ViewRrafIcon != _settingsViewModel.ViewRrafIcon)
        {
          ((TwitterSettings)Settings).ViewRrafIcon = _settingsViewModel.ViewRrafIcon;
        }

        ((TwitterSettings)Settings).SlRepliesValue = _settingsViewModel.SlRepliesValue;
        ((TwitterSettings)Settings).SlDmsValue = _settingsViewModel.SlDmsValue;
        ((TwitterSettings)Settings).SlListValue = _settingsViewModel.SlListValue;
        ((TwitterSettings)Settings).SlFriendsValue = _settingsViewModel.SlFriendsValue;
        ((TwitterSettings)Settings).SlUserValue = _settingsViewModel.SlUserValue;
        ((TwitterSettings)Settings).ShowDMHome = _settingsViewModel.ShowDMHome;
        ((TwitterSettings)Settings).ShowRepliesHome = _settingsViewModel.ShowRepliesHome;

        UpdateRemainingApiAsync();
        if (((TwitterSettings)Settings).ShowApiUsage != _settingsViewModel.ShowApiUsage)
        {
          ((TwitterSettings)Settings).ShowApiUsage = _settingsViewModel.ShowApiUsage;
        }

        // Add SPAMs entered by the user!
        if (CurrentAccount.SpamList == null)
          CurrentAccount.SpamList = new List<string>();

        // Remove existant spams from current lists!
        CurrentAccount.SpamList.Clear();
        foreach (var spam in _settingsViewModel.Spams)
        {
          CurrentAccount.SpamList.Add(spam);
          TraceHelper.Trace(this, "Following SPAM was added: " + spam);

          foreach (var tWorkspace in TwitterWorkspaces)
          {
            var toRemove = new List<Entry>();
            foreach (var tweet in tWorkspace.Tweets)
            {
              if (!tweet.Title.ToLower().Contains(spam.ToLower())) continue;
              toRemove.Add(tweet);
              TraceHelper.Trace(this, string.Format("Tweet marked 'to be removed': {0}", tweet));
            }

            foreach (var tweet in toRemove)
            {
              TraceHelper.Trace(this, string.Format("Tweet 'removed': {0}", tweet));
              tWorkspace.Tweets.Remove(tweet);
              tWorkspace.UpdateView();
            }
            toRemove.Clear();
          }
        }

        if (Settings.NbPostToGet != _settingsViewModel.Rpp)
        {
          Settings.NbPostToGet = _settingsViewModel.Rpp;
        }
        UpdateActivLists();
        ServiceMessenger.Send("SettingsUpdated");
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    private void UpdateActivLists()
    {
      if (ActivLists == null)
        ActivLists = new List<string>();

      var i = 0;
      if (ViewState)
      {
        foreach (var list in ActivLists)
        {
          IsActivLists[i] = false;
          LstItemLists[i].IsEnabled = false;
          i++;
        }
        return;
      }
      foreach (var list in ActivLists)
      {
        foreach (var model in TwitterWorkspaces)
        {
          if (model.WorkspaceSettings.Type == EnumTwitterType.List &&
              model.WorkspaceSettings.GroupName == list &&
              model == TwitterSelectedWorkspace)
          {
            IsActivLists[i] = false;
            LstItemLists[i].IsEnabled = false;
            break;
          }
          IsActivLists[i] = true;
          LstItemLists[i].IsEnabled = true;
        }
        RaisePropertyChanged(string.Format("IsActivLists[{0}]", i));
        i++;
      }
    }

    private void CloseLists()
    {
      CurrentFrontView = TwitterFrontView.None;
      RaisePropertyChanged(() => FrontViewModel);
      RaisePropertyChanged(() => FrontTemplateView);
      _listViewModel.Cleanup();
      _listViewModel = null;
      RaisePropertyChanged(() => CurrentList);
    }

    private void ShowProfile(TwitterUser user)
    {
      CurrentProfilView = EnumTwitterProfile.User;
      ProfileViewModel.ShowUser(user);
      RaisePropertyChanged(() => ProfileTemplate); ;
      RaisePropertyChanged(() => ProfileView); ;
    }

    private void ShowTweet(TwitterEntry entry)
    {
      CurrentProfilView = EnumTwitterProfile.Tweet;
      TweetToShow = entry;
      RaisePropertyChanged(() => ProfileTemplate); ;
      RaisePropertyChanged(() => ProfileView); ;
    }

    private void LoadWorkSpace()
    {
      try
      {
        // Login successfull, restore settings
        CurrentView = ((TwitterSettings)Settings).ViewState ? TwitterTypeView.Columns : TwitterTypeView.Tab;

        foreach (var twitterSetting in ((TwitterSettings)Settings).WorkspaceSettingsCollection)
          TwitterWorkspaces.Add(InitializeTwitterWorkspaceViewModel(twitterSetting));

        // Select friend column if available
        foreach (var twitterWorkspace in TwitterWorkspaces)
        {
          if (twitterWorkspace.WorkspaceSettings.Type.Equals(EnumTwitterType.Friends))
          {
            TwitterSelectedWorkspace = twitterWorkspace;
            break;
          }
        }

        SettingsButtonVisibility = Visibility.Visible;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
      LoadDefaultworkSpace();
    }

    private void LoadDefaultworkSpace()
    {
      // We load the 4 defaults Twitter views
      if (((TwitterSettings)Settings).WorkspaceSettingsCollection == null)
        ((TwitterSettings)Settings).WorkspaceSettingsCollection = new ObservableCollection<TwitterWorkspaceSettings>();

      if (((TwitterSettings)Settings).WorkspaceSettingsCollection.Count != 0) return;

      ((TwitterSettings)Settings).WorkspaceSettingsCollection.Add(new TwitterWorkspaceSettings(EnumTwitterType.Friends, BGlobals.DEFAULT_NB_POST_TO_GET_Twitter, ((TwitterSettings)Settings).SlFriendsValue, 0, -1));
      ((TwitterSettings)Settings).WorkspaceSettingsCollection.Add(new TwitterWorkspaceSettings(EnumTwitterType.Replies, BGlobals.DEFAULT_NB_POST_TO_GET_Twitter, ((TwitterSettings)Settings).SlRepliesValue, 1, -1));
      ((TwitterSettings)Settings).WorkspaceSettingsCollection.Add(new TwitterWorkspaceSettings(EnumTwitterType.DirectMessages, BGlobals.DEFAULT_NB_POST_TO_GET_Twitter, ((TwitterSettings)Settings).SlDmsValue, 2, -1));
      ((TwitterSettings)Settings).WorkspaceSettingsCollection.Add(new TwitterWorkspaceSettings(EnumTwitterType.User, BGlobals.DEFAULT_NB_POST_TO_GET_Twitter, ((TwitterSettings)Settings).SlUserValue, null, null, Settings.UserName, 3, -1));

      foreach (var twitterSetting in ((TwitterSettings)Settings).WorkspaceSettingsCollection)
        TwitterWorkspaces.Add(InitializeTwitterWorkspaceViewModel(twitterSetting));

      TwitterSelectedWorkspace = TwitterWorkspaces[0];
    }

    public TwitterWorkspaceViewModel InitializeTwitterWorkspaceViewModel(TwitterWorkspaceSettings ts)
    {
      try
      {
        var twitterViewModel = new TwitterWorkspaceViewModel(this, ts, ServiceMessenger);
        return twitterViewModel;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
        return null;
      }
    }

    private bool CanAddColumn(object param)
    {
      try
      {
        var objs = BRelayCommand.CheckParams(param);
        if (objs == null) return false;

        var twitterType =
            (EnumTwitterType)Enum.Parse(typeof(EnumTwitterType), objs[0].ToString(), true);

        if (Settings == null)
          Settings = new TwitterSettings();

        if (((TwitterSettings)Settings).WorkspaceSettingsCollection == null)
          ((TwitterSettings)Settings).WorkspaceSettingsCollection =
              new ObservableCollection<TwitterWorkspaceSettings>();

        var canAddColumn =
            ((TwitterSettings)Settings).WorkspaceSettingsCollection.All(
                tSettings => !tSettings.Type.Equals(twitterType));
        return canAddColumn;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
      return false;
    }

    private void AddColumn(object param)
    {
      try
      {
        lock (CanAdd)
        {
          var objs = param as object[];

          TwitterWorkspaceSettings ts = null;
          if (objs != null)
            if (!(objs[0].GetType() == typeof(TwitterWorkspaceSettings)))
            {
              var canAddColumn = true;
              var twitterType =
                  (EnumTwitterType)Enum.Parse(typeof(EnumTwitterType), objs[0].ToString(), true);

              if (Settings == null)
                Settings = new TwitterSettings();

              if (((TwitterSettings)Settings).WorkspaceSettingsCollection == null)
                ((TwitterSettings)Settings).WorkspaceSettingsCollection =
                    new ObservableCollection<TwitterWorkspaceSettings>();

              foreach (var tWorkspace in TwitterWorkspaces)
              {
                if (!tWorkspace.WorkspaceSettings.Type.Equals(twitterType)) continue;
                canAddColumn = false;

                // Select the corresponding Workspace
                TwitterSelectedWorkspace = tWorkspace;

                break;
              }

              if (canAddColumn)
              {
                var addToColumn = ((TwitterSettings)Settings).WorkspaceSettingsCollection.Count;

                switch (twitterType)
                {
                case EnumTwitterType.Replies:
                  ts = new TwitterWorkspaceSettings(EnumTwitterType.Replies,
                      BGlobals.DEFAULT_NB_POST_TO_GET_Twitter,
                      ((TwitterSettings)Settings).SlRepliesValue,
                      addToColumn, -1);
                  break;

                case EnumTwitterType.DirectMessages:
                  ts = new TwitterWorkspaceSettings(EnumTwitterType.DirectMessages,
                      BGlobals.DEFAULT_NB_POST_TO_GET_Twitter,
                      ((TwitterSettings)Settings).SlDmsValue,
                      addToColumn, -1);
                  break;

                case EnumTwitterType.Friends:
                  ts = new TwitterWorkspaceSettings(EnumTwitterType.Friends,
                      BGlobals.DEFAULT_NB_POST_TO_GET_Twitter,
                      ((TwitterSettings)Settings).SlFriendsValue,
                      addToColumn, -1);
                  break;

                case EnumTwitterType.Groups:
                  break;

                case EnumTwitterType.User:
                  ts = new TwitterWorkspaceSettings(EnumTwitterType.User,
                      BGlobals.DEFAULT_NB_POST_TO_GET_Twitter,
                      ((TwitterSettings)Settings).SlUserValue, null, null,
                      Settings.UserName, addToColumn, -1);
                  break;

                case EnumTwitterType.Favorites:
                  ts = new TwitterWorkspaceSettings(EnumTwitterType.Favorites,
                      BGlobals.DEFAULT_NB_POST_TO_GET_Twitter,
                      double.NaN,
                      addToColumn, -1);
                  break;

                default:
                  break;
                }
              }
            }
            else
            {
              ts = objs[0] as TwitterWorkspaceSettings;
            }

          if (ts != null)
          {
            ((TwitterSettings)Settings).WorkspaceSettingsCollection.Add(ts);
            var wk = InitializeTwitterWorkspaceViewModel(ts);
            TwitterWorkspaces.Add(wk);
            TwitterSelectedWorkspace = wk;
          }
        }
        UpdateActivLists();
      }
      catch
          (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    #endregion Methods

    #region Api

    public void UpdateRemainingApiAsync()
    {
      try
      {
        TraceHelper.Trace(this, "Try to update remaining Twitter APIs");
        TraceHelper.Trace(this, "Updating remaining Twitter APIs");

        TwitterRateLimit trl = null;
        string errorMsg = null;

        Action mainAction = () =>
        {
          trl = TwitterLibV11.RemainingApi(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET, CurrentAccount.SessionKey, CurrentAccount.Secret, out errorMsg, ProxyHelper.GetConfiguredWebProxy(SobeesSettings));
        };

        var task = Task.Factory.StartNew(mainAction, CancellationToken.None, TaskCreationOptions.None, UiContext.Instance.Current);
        task.ContinueWith(_ => OnUpdateRemainingApiAsyncCompleted(trl, errorMsg));
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    private void OnUpdateRemainingApiAsyncCompleted(TwitterRateLimit limit, string msg)
    {
      if (!string.IsNullOrEmpty(msg))
        TraceHelper.Trace(this, msg);
      else if (limit != null)
      {
        TwitterRateLimit = null;
        TwitterRateLimit = limit;
        TwitterRateLimitIP = limit;
      }

      TraceHelper.Trace(this, "Remaining Twitter APIs UPDATED");
    }

    #endregion Api

    #region User Actions

    /// <summary>
    /// DeleteStatus
    /// </summary>
    /// <param name="entry"></param>
    private void DeleteStatus(Entry entry)
    {
      if (entry == null) return;
      try
      {
        StartWaiting();
        var result = BTwitterResponseResult<TwitterEntry>.CreateInstance();

        Action mainAction = async () =>
        {
          result = await TwitterLibV11.DeleteStatus(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET, CurrentAccount.SessionKey, CurrentAccount.Secret, entry.Id);
        };
        var task = Task.Factory.StartNew(mainAction, CancellationToken.None, TaskCreationOptions.None, UiContext.Instance.Current);
        task.ContinueWith(_ => OnDeleteStatusAsyncCompleted(result.DataResult, result.ErrorMessage));
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
        StopWaiting();
      }
    }

    private void OnDeleteStatusAsyncCompleted(TwitterEntry newDelete, string errorMsg)
    {
      StopWaiting();

      if (!string.IsNullOrEmpty(errorMsg))
      {
        ShowErrorMsg(errorMsg);
        return;
      }

      ServiceMessenger.Send(new BMessage("DeleteCompleted", newDelete));
    }

    private void AddToList(TwitterEntry entry)
    {
      CurrentFrontView = TwitterFrontView.AddToList;
      User = entry.User;
      ConfirmationText =
          new LocText("Sobees.Configuration.BGlobals:Resources:txtblSelectListTwitter").ResolveLocalizedValue();
      _actionToConfirm = "AddToList";
      RaisePropertyChanged(() => FrontTemplateView);
      RaisePropertyChanged(() => FrontViewModel);
    }

    /// <summary>
    /// AddToFavorite
    /// </summary>
    /// <param name="entry"></param>
    private void AddToFavorite(TwitterEntry entry)
    {
      if (entry == null) return;
      try
      {
        StartWaiting();

        var result = BTwitterResponseResult<TwitterEntry>.CreateInstance();

        Action mainAction = async () =>
        {
          result = await TwitterLibV11.AddFavorite(BGlobals.TWITTER_OAUTH_KEY,
            BGlobals.TWITTER_OAUTH_SECRET,
            CurrentAccount.SessionKey,
            CurrentAccount.Secret, entry.Id,
            false);
        };

        var task = Task.Factory.StartNew(mainAction, CancellationToken.None, TaskCreationOptions.None, UiContext.Instance.Current);
        task.ContinueWith(_ => OnAddToFavoriteAsyncCompleted(result.DataResult, result.ErrorMessage));
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
        StopWaiting();
      }
    }

    /// <summary>
    /// OnAddToFavoriteAsyncCompleted
    /// </summary>
    /// <param name="newFavorite"></param>
    /// <param name="errorMsg"></param>
    private void OnAddToFavoriteAsyncCompleted(TwitterEntry newFavorite, string errorMsg)
    {
      StopWaiting();
      try
      {
        if (!string.IsNullOrEmpty(errorMsg))
        {
          ShowErrorMsg(errorMsg);
          return;
        }

        // Add new favorite to favorite's columns (if available)
        foreach (var twitterWorkspace in TwitterWorkspaces)
        {
          if (!twitterWorkspace.WorkspaceSettings.Type.Equals(EnumTwitterType.Favorites)) continue;

          var i = 0;
          var find = false;
          foreach (var tweetFav in twitterWorkspace.Tweets)
          {
            if (tweetFav.Id.Equals(newFavorite.Id))
            {
              find = true;
              break;
            }
            i++;
          }

          if (!find)
            twitterWorkspace.Tweets.Add(newFavorite);
          else
            twitterWorkspace.Tweets.RemoveAt(i);

          twitterWorkspace.UpdateView();
          break;
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
        StopWaiting();
      }
    }

    /// <summary>
    /// ReplyToAll
    /// </summary>
    /// <param name="entry"></param>
    private void ReplyToAll(TwitterEntry entry)
    {
      VisibilityButtonRetweet = Visibility.Collapsed;
      RetweetId = string.Empty;
      ReplyId = entry.Id;
      IsStatusZoneOpen = true;
      Status = string.Empty;

      CursorPosition = string.Format("@{0} {1}", entry.User.NickName, FindAllReply(entry.Title)).Length;
      MoveCursor = true;
      Status = string.Format("@{0} {1}", entry.User.NickName, FindAllReply(entry.Title));
    }

    private static string FindAllReply(string title)
    {
      var splited = title.Split((" :").ToCharArray());
      var lst = new StringBuilder();
      foreach (var s in splited.Where(s => s.StartsWith("@")))
      {
        lst.Append(s);
        lst.Append(" ");
      }
      return lst.ToString();
    }

    /// <summary>
    /// ReportSpam
    /// </summary>
    /// <param name="entry"></param>
    private void ReportSpam(TwitterEntry entry)
    {
      CurrentFrontView = TwitterFrontView.Confirmation;
      User = entry.User;

      ConfirmationText =
          new LocText("Sobees.Configuration.BGlobals:Resources:txtblConfirmationReportSpam").ResolveLocalizedValue
              ();

      _actionToConfirm = "Report";
      RaisePropertyChanged(() => FrontTemplateView);
      RaisePropertyChanged(() => FrontViewModel);
    }

    /// <summary>
    /// BlockUser
    /// </summary>
    /// <param name="entry"></param>
    private void BlockUser(TwitterEntry entry)
    {
      CurrentFrontView = TwitterFrontView.Confirmation;
      _actionToConfirm = "Block";
      User = entry.User;

      ConfirmationText =
          new LocText("Sobees.Configuration.BGlobals:Resources:txtblConfirmationBlock").ResolveLocalizedValue();

      RaisePropertyChanged(() => FrontTemplateView);
      RaisePropertyChanged(() => FrontViewModel);
    }

    /// <summary>
    /// ReportConfirmation
    /// </summary>
    private void ReportConfirmation()
    {
      using (var worker = new BackgroundWorker())
      {
        var result = BTwitterResponseResult<LinqToTwitter.User>.CreateInstance();
        worker.DoWork += async delegate(object s,
            DoWorkEventArgs args)
        {
          if (worker.CancellationPending)
          {
            args.Cancel = true;
            return;
          }

          //TwitterLib.ReportSpam(Settings.UserName, PasswordHash, Globals.CIPHER_KEY, User.NickName, out error, ProxyHelper.GetConfiguredWebProxy(SobeesSettings));
          result = await TwitterLibV11.ReportSpam(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET, CurrentAccount.SessionKey, CurrentAccount.Secret, User.NickName);
        };

        worker.RunWorkerCompleted += delegate { OnGetMenuActionAsyncCompleted(result.ErrorMessage); };
        worker.RunWorkerAsync();
      }
    }

    /// <summary>
    /// BlockUserConfirmation
    /// </summary>
    private void BlockUserConfirmation()
    {
      using (var worker = new BackgroundWorker())
      {
        var result = BTwitterResponseResult<LinqToTwitter.User>.CreateInstance();
        worker.DoWork += async delegate(object s, DoWorkEventArgs args)
        {
          if (worker.CancellationPending)
          {
            args.Cancel = true;
            return;
          }

          result = await TwitterLibV11.Block(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET, CurrentAccount.SessionKey, CurrentAccount.Secret, User.NickName);
        };

        worker.RunWorkerCompleted += delegate
        {
          CloseConfirmation();
          if (result.BisSuccess)
          {
            UpdateTwitterList(NameNewList);
            return;
          }
          ShowErrorMsg(result.ErrorMessage);
        };
        worker.RunWorkerAsync();
      }
    }

    /// <summary>
    /// DM
    /// </summary>
    /// <param name="entry"></param>
    private void DM(TwitterEntry entry)
    {
      VisibilityButtonRetweet = Visibility.Collapsed;
      if (entry == null) return;
      RetweetId = string.Empty;
      ReplyId = string.Empty;
      IsStatusZoneOpen = true;
      Status = string.Empty;
      CursorPosition = string.Format("D {0} ", entry.User.NickName).Length;
      MoveCursor = true;
      Status = string.Format("D {0} ", entry.User.NickName);
    }

    private void Reply(TwitterEntry entry)
    {
      VisibilityButtonRetweet = Visibility.Collapsed;
      if (entry == null) return;
      ReplyId = entry.Id;
      RetweetId = string.Empty;
      IsStatusZoneOpen = true;
      Status = string.Empty;
      CursorPosition = string.Format("@{0} ", entry.User.NickName).Length;
      MoveCursor = true;
      Status = string.Format("@{0} ", entry.User.NickName);
    }

    /// <summary>
    /// Retweet
    /// </summary>
    /// <param name="entry"></param>
    private void Retweet(TwitterEntry entry)
    {
      VisibilityButtonRetweet = Visibility.Visible;
      if (entry == null) return;
      IsStatusZoneOpen = true;
      RetweetId = entry.Id;
      Status = entry.Title;
      ReplyId = string.Empty;
      Status = string.Empty;
      CursorPosition = entry.Title.Length;
      MoveCursor = true;
      Status = entry.Title;
      RetweetText = string.Format("RT @{0}: ", entry.User.NickName);
    }

    #endregion User Actions

    #region Lists Twitter

    /// <summary>
    /// UpdateTwitterList
    /// </summary>
    /// <param name="name"></param>
    private void UpdateTwitterList(string name)
    {
      if (CurrentAccount == null)
      {
        TraceHelper.Trace(this, "CurrentAccount IS NULL !!! (TwitterViewModel.cs)");
        return;
      }

      using (var worker = new BackgroundWorker())
      {
        var result = new List<TwitterList>();
        worker.DoWork += delegate(object s, DoWorkEventArgs args)
        {
          if (worker.CancellationPending)
          {
            args.Cancel = true;
            return;
          }

          string error;

          result = TwitterLibV11.GetList(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET,
              CurrentAccount.SessionKey, CurrentAccount.Secret,
              CurrentAccount.Login, out error,
              ProxyHelper.GetConfiguredWebProxy(
                  SobeesSettings));

          var result2 = TwitterLibV11.GetListOwn(BGlobals.TWITTER_OAUTH_KEY,
              BGlobals.TWITTER_OAUTH_SECRET,
              CurrentAccount.SessionKey, CurrentAccount.Secret,
              CurrentAccount.Login, out error,
              ProxyHelper.GetConfiguredWebProxy(SobeesSettings));

          if (result2 == null || !result2.Any()) return;
          if (result == null)
            result = new List<TwitterList>();

          result.AddRange(result2);
        };

        worker.RunWorkerCompleted += delegate { OnGetListsAsyncCompleted(result, name); };

        worker.RunWorkerAsync();
      }

      using (var worker = new BackgroundWorker())
      {
        var result = new List<TwitterList>();
        worker.DoWork += delegate(object s,
            DoWorkEventArgs args)
        {
          if (worker.CancellationPending)
          {
            args.Cancel = true;
            return;
          }

          string error;
          result =
              TwitterLibV11.GetListOwn(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET,
                  CurrentAccount.SessionKey, CurrentAccount.Secret,
                  CurrentAccount.Login, out error,
                  ProxyHelper.GetConfiguredWebProxy(
                      SobeesSettings));
        };

        worker.RunWorkerCompleted += delegate { OnGetListsAsyncCompleted(result, name); };

        worker.RunWorkerAsync();
      }
    }

    private void OnGetListsAsyncCompleted(IEnumerable<TwitterList> twitters, string name)
    {
      if (twitters == null) return;

      foreach (var list in twitters)
      {
        if (!TwitterList.Contains(list))
        {
          if (list.Id == "0") continue;
          TwitterList.Add(list);
        }

        if (!TwitterListOwn.Contains(list) &&
            list.FullName.ToLower().Contains(string.Format("@{0}", CurrentAccount.Login.ToLower())))
        {
          TwitterListOwn.Add(list);
        }
      }
      if (!string.IsNullOrEmpty(name))
      {
        foreach (var list in TwitterList)
        {
          if (list.Name != name) continue;

          foreach (
            var workspace in
              TwitterWorkspaces.Where(
                workspace => workspace.WorkspaceSettings.Type == EnumTwitterType.List)
                .Where(workspace => workspace.WorkspaceSettings.GroupName == list.FullName))
          {
            TwitterSelectedWorkspace = workspace;
            return;
          }
          try
          {
            // We must pass 3 args to AddColumn because of the Command HACK
            var ts = new TwitterWorkspaceSettings(EnumTwitterType.List,
                                                  BGlobals.DEFAULT_NB_POST_TO_GET_Twitter,
                                                  ((TwitterSettings)Settings).SlListValue,
                                                  ((TwitterSettings)Settings).WorkspaceSettingsCollection.Count, list, -1);
            AddColumn(new object[] { ts, null, null });
            return;
          }
          catch (Exception ex)
          {
            TraceHelper.Trace(this, ex);
          }
        }
      }

      TwitterListDisplay.Clear();
      foreach (var list in TwitterListOwn)
      {
        if (list.Id == "0") continue;
        TwitterListDisplay.Add(list);
      }

      foreach (var list in TwitterList)
      {
        if (list.Id == "0") continue;
        if (!list.FullName.ToLower().Contains(string.Format("@{0}", CurrentAccount.Login.ToLower())))
        {
          TwitterListDisplay.Add(list);
        }
      }
      if (TwitterListDisplay.Count == 0)
      {
        TwitterListDisplay.Add(new TwitterList { FullName = "" });
      }
      RaisePropertyChanged(() => IsListVisible);
      ServiceMessenger.Send("ListUpdated");
    }

    private void OpenList(TwitterList list)
    {
      if (list == null) return;
      foreach (var workspace in TwitterWorkspaces.Where(workspace => workspace.WorkspaceSettings.Type == EnumTwitterType.List).Where(workspace => workspace.WorkspaceSettings.GroupName == list.FullName))
      {
        TwitterSelectedWorkspace = workspace;
        return;
      }

      try
      {
        // We must pass 3 args to AddColumn because of the Command HACK
        var ts = new TwitterWorkspaceSettings(EnumTwitterType.List, BGlobals.DEFAULT_NB_POST_TO_GET_Twitter, ((TwitterSettings)Settings).SlListValue, ((TwitterSettings)Settings).WorkspaceSettingsCollection.Count, list, -1);
        AddColumn(new object[] { ts, null, null });
        RaisePropertyChanged(() => MenuItems);
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    private void OpenList(TwitterListShow list)
    {
      foreach (var workspace in TwitterWorkspaces)
      {
        if (workspace.WorkspaceSettings.Type != EnumTwitterType.List) continue;
        if (workspace.WorkspaceSettings.GroupName != list.FullName) continue;
        TwitterSelectedWorkspace = workspace;
        workspace.WorkspaceSettings.Color = list.ColorIcon;
        RaisePropertyChanged(() => MenuItems);
        return;
      }

      try
      {
        // We must pass 3 args to AddColumn because of the Command HACK
        var ts = new TwitterWorkspaceSettings(EnumTwitterType.List,
            BGlobals.DEFAULT_NB_POST_TO_GET_Twitter,
            ((TwitterSettings)Settings).SlListValue,
            ((TwitterSettings)Settings).WorkspaceSettingsCollection.Count,
            list,
            -1);
        AddColumn(new object[] { ts, null, null });
        RaisePropertyChanged(() => MenuItems);
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    private void CloseList(TwitterListShow list)
    {
      foreach (var workspace in TwitterWorkspaces.Where(workspace => workspace.WorkspaceSettings.Type == EnumTwitterType.List).Where(workspace => workspace.WorkspaceSettings.GroupName == list.FullName))
      {
        if (TwitterSelectedWorkspace == workspace)
          TwitterSelectedWorkspace = TwitterWorkspaces[0];

        TwitterWorkspaces.Remove(workspace);
        break;
      }

      try
      {
        RaisePropertyChanged(() => MenuItems);
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    //private void OpenSearch()
    //{
    //  foreach (var workspace in TwitterWorkspaces.Where(workspace => workspace.WorkspaceSettings.Type == EnumTwitterType.Search))
    //  {
    //    TwitterSelectedWorkspace = workspace;
    //    return;
    //  }

    //  try
    //  {
    //    // We must pass 3 args to AddColumn because of the Command HACK
    //    var ts = new TwitterWorkspaceSettings(EnumTwitterType.Search, BGlobals.DEFAULT_NB_POST_TO_GET_Twitter,
    //        double.NaN, ((TwitterSettings)Settings).WorkspaceSettingsCollection.Count, -1);
    //    AddColumn(new object[] { ts, null, null });
    //  }
    //  catch (Exception ex)
    //  {
    //    TraceHelper.Trace(this, ex);
    //  }
    //}

    /// <summary>
    /// ShowLists
    /// </summary>
    private void ShowLists()
    {
      if (TwitterListDisplay.Any())
        CurrentList = TwitterListDisplay[0];
    }

    /// <summary>
    /// ShowListsEditor
    /// </summary>
    private void ShowListsEditor()
    {
      CurrentFrontView = TwitterFrontView.List;
      RaisePropertyChanged(() => FrontTemplateView);
      RaisePropertyChanged(() => FrontViewModel);
    }

    //private void ShowList(string name)
    //{
    //  foreach (var model in TwitterWorkspaces)
    //  {
    //    if (model.WorkspaceSettings.Type != EnumTwitterType.List || model.WorkspaceSettings.GroupName != name)
    //      continue;
    //    TwitterSelectedWorkspace = model;
    //    UpdateActivLists();
    //    return;
    //  }
    //}

    #endregion Lists Twitter

    #region Status

    protected override void CancelPostStatus()
    {
      Status = " ";
      ImageName = "";
      CursorPosition = 0;
      MoveCursor = true;
      Status = "";
      RetweetId = string.Empty;
      ReplyId = string.Empty;
      VisibilityButtonRetweet = Visibility.Collapsed;

      if (
          SobeesSettings.Accounts[SobeesSettings.Accounts.IndexOf(new UserAccount((Settings).UserName))].
              IsSignatureActivated)
      {
        MoveCursor = true;
        Status +=
            SobeesSettings.Accounts[SobeesSettings.Accounts.IndexOf(new UserAccount((Settings).UserName))]
                .Signature;
      }
      IsStatusZoneOpen = false;
    }

    /// <summary>
    ///     PostStatus
    /// </summary>
    protected override void PostStatus()
    {
      _isAddingTweet = true;
      {
        try
        {
          StartWaiting();
          var result = BTwitterResponseResult<TwitterEntry>.CreateInstance();

          var userAccount = SobeesSettings.Accounts[SobeesSettings.Accounts.IndexOf(new UserAccount((Settings).UserName))];
          var bUserSig = userAccount.IsSignatureActivated;
          var userSig = userAccount.Signature;

          Action mainAction = async () =>
          {
            if (!string.IsNullOrEmpty(RetweetId))
            {
              result = await TwitterLibV11.Retweet(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET, CurrentAccount.SessionKey, CurrentAccount.Secret, RetweetId);
              if (!result.BisSuccess) return;
              //Application.Current.Dispatcher.BeginInvokeIfRequired(() =>
              //{
              if (SobeesSettings.CloseBoxPublication)
                IsStatusZoneOpen = false;

              VisibilityButtonRetweet = Visibility.Collapsed;
              RetweetId = string.Empty;
              Status = string.Empty;

              if (!bUserSig) return;

              MoveCursor = true;
              Status += userSig;
              //});
            }
            else
            {
              if (string.IsNullOrEmpty(ImageName))
              {
                result =
                  await TwitterLibV11.AddTweet(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET, CurrentAccount.SessionKey, CurrentAccount.Secret, Status, false, ReplyId);
                OnPostStatusCompleted(result.DataResult, result.ErrorMessage);
              }
              else
              {
                var fileinfo = new FileInfo(ImageName);
                var twitterCtx = TwitterLibV11.GetTwitterContext(TwitterUserCredentials);
                result = await TwitterLibV11.PostTweetWithMedia(twitterCtx, Status, fileinfo.FullName, fileinfo.Name);
                OnPostStatusCompleted(result.DataResult, result.ErrorMessage);
              }
            }
          };

          var task = Task.Factory.StartNew(mainAction, CancellationToken.None, TaskCreationOptions.AttachedToParent, UiContext.Instance.Current);
          task.Wait();
        }
        catch (Exception ex)
        {
          TraceHelper.Trace(this, ex);
          _isAddingTweet = false;
          StopWaiting();
        }
      }
    }

    protected override bool CanPostStatus()
    {
      if (VisibilityButtonRetweet == Visibility.Visible)
        return true;

      return !string.IsNullOrEmpty(Status) && Status.Length <= _twitterMsgMaximumLength && !_isAddingTweet;
    }

    /// <summary>
    /// OnPostStatusCompleted
    /// </summary>
    /// <param name="result"></param>
    /// <param name="errorMsg"></param>
    private void OnPostStatusCompleted(TwitterEntry result, string errorMsg)
    {
      try
      {
        _isAddingTweet = false;
        StopWaiting();
        var userAccount = SobeesSettings.Accounts[SobeesSettings.Accounts.IndexOf(new UserAccount((Settings).UserName))];
        var bUserSig = userAccount.IsSignatureActivated;
        var userSig = userAccount.Signature;
        if (string.IsNullOrEmpty(errorMsg) && result != null)
        {
          if (Application.Current == null || Application.Current.Dispatcher == null) return;

          //Application.Current.Dispatcher.BeginInvoke(new Action(() =>
          //{
          // Message sent successfully
          CursorPosition = 0;
          Status = "";
          IsVisibleBtnPhoto = Visibility.Visible;
          IsDelBtnImgShow = Visibility.Collapsed;
          //}));
          result.CanPost = Settings.UserName.ToUpper() == result.User.NickName.ToUpper()
              ? 1
              : 0;
          if (bUserSig)
          {
            MoveCursor = true;
            Status += userSig;
          }
          ErrorMsg = string.Empty;
          if (SobeesSettings.CloseBoxPublication)
            IsStatusZoneOpen = false;

          foreach (var twitterWorkspace in TwitterWorkspaces)
          {
            if (twitterWorkspace.WorkspaceSettings.Type.Equals(EnumTwitterType.Friends))
            {
              if (!twitterWorkspace.Tweets.Contains(result) && result.ToUser == null)
                twitterWorkspace.Tweets.Insert(0, result);
            }
            else if (twitterWorkspace.WorkspaceSettings.Type.Equals(EnumTwitterType.DirectMessages))
            {
              if (result.Content.StartsWith("d ", StringComparison.CurrentCultureIgnoreCase) || result.ToUser != null)
              {
                if (!twitterWorkspace.Tweets.Contains(result))
                  twitterWorkspace.Tweets.Insert(0, result);
              }
            }
            Application.Current.Dispatcher.BeginInvokeIfRequired(twitterWorkspace.UpdateView);
          }
        }
        else
        {
          ShowErrorMsg(errorMsg);
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    /// <summary>
    /// UpdateAll
    /// </summary>
    public override void UpdateAll()
    {
      if (!ViewState)
        TwitterSelectedWorkspace.Refresh();
      else
      {
        foreach (var twitterWorkspace in TwitterWorkspaces)
          twitterWorkspace.Refresh();
      }
    }

    /// <summary>
    /// ClearTweets
    /// </summary>
    private void ClearTweets()
    {
      try
      {
        if (!ViewState)
          TwitterSelectedWorkspace.ClearTweets();

        else
        {
          foreach (var model in TwitterWorkspaces)
            model.ClearTweets();
        }
      }
      catch (Exception ex)
      {
        BLogManager.LogEntry(APPNAME, ex);
      }
    }

    #region Buttons

    #region ShortenUrl

    private void ShortenUrl()
    {
      StartWaiting();
      try
      {
        using (var worker = new BackgroundWorker())
        {
          var result = string.Empty;

          worker.DoWork += delegate(object s,
              DoWorkEventArgs args)
          {
            if (worker.CancellationPending)
            {
              args.Cancel = true;
              return;
            }

            switch (SobeesSettings.UrlShortener)
            {
            case UrlShorteners.BitLy:
              result =
                  BitLyHelper.ConvertUrlsToTinyUrls(Status,
                      ProxyHelper.GetConfiguredWebProxy(
                          SobeesSettings), SobeesSettings.BitLyUserName,
                      SobeesSettings.BitLyPassword);
              break;

            case UrlShorteners.Digg:
              result =
                  DiggHelper.ConvertUrlsToTinyUrls(Status,
                      ProxyHelper.GetConfiguredWebProxy(
                          SobeesSettings));
              break;

            case UrlShorteners.IsGd:
              result =
                  IsGdHelper.ConvertUrlsToTinyUrls(Status,
                      ProxyHelper.GetConfiguredWebProxy(
                          SobeesSettings));
              break;

            case UrlShorteners.TinyUrl:
              result =
                  TinyUrlHelper.ConvertUrlsToTinyUrls(Status,
                      ProxyHelper.GetConfiguredWebProxy(
                          SobeesSettings));
              break;

            case UrlShorteners.TrIm:
              result =
                  TrImHelper.ConvertUrlsToTinyUrls(Status,
                      ProxyHelper.GetConfiguredWebProxy(
                          SobeesSettings));
              break;

            case UrlShorteners.Twurl:
              result =
                  TwurlHelper.ConvertUrlsToTinyUrls(Status,
                      ProxyHelper.GetConfiguredWebProxy(
                          SobeesSettings));
              break;

            case UrlShorteners.MigreMe:
              result =
                  MigreMeHelper.ConvertUrlsToTinyUrls(Status,
                      ProxyHelper.GetConfiguredWebProxy(
                          SobeesSettings));
              break;
            }
          };

          worker.RunWorkerCompleted += delegate { OnShortenUrlAsyncCompleted(result); };

          worker.RunWorkerAsync();
        }
      }
      catch (Exception ex)
      {
        StopWaiting();
        TraceHelper.Trace(this, ex);
      }
    }

    private bool CanShortenUrl()
    {
      if (!string.IsNullOrEmpty(Status))
      {
        var words = Status.Split(' ');
        return words.Any(HyperLinkHelper.IsHyperlink);
      }
      return false;
    }

    private void OnShortenUrlAsyncCompleted(string result)
    {
      StopWaiting();

      if (!string.IsNullOrEmpty(result))
      {
        Status = result;
      }
      else
      {
        ShowErrorMsg("Shorten URL has failed. Please try again later...");
      }
    }

    #endregion ShortenUrl

    #region UploadImage

    private TwitterCredentials _twitterUserCredentials;

    public TwitterCredentials TwitterUserCredentials => _twitterUserCredentials ?? (_twitterUserCredentials = new TwitterCredentials
    {
      ConsumerKey = BGlobals.TWITTER_OAUTH_KEY,
      ConsumerSecret = BGlobals.TWITTER_OAUTH_SECRET,
      OAuthToken = CurrentAccount.AuthToken,
      OAuthSecret = CurrentAccount.Secret
    });

    private void DeleteImage()
    {
      try
      {
        ImageName = null;

        //FileName = null;
        IsDelBtnImgShow = Visibility.Collapsed;
        IsVisibleBtnPhoto = Visibility.Visible;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    //private void UploadImage()
    //{
    //  try
    //  {
    //    StartWaiting();
    //    const string filter = "Images |*.jpg;*.gif;*.png";

    //    var ofd = new OpenFileDialog
    //    {
    //      InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
    //      Filter = filter,
    //      RestoreDirectory = true
    //    };

    //    var dr = ofd.ShowDialog();
    //    if (dr == DialogResult.OK)
    //    {
    //      using (var worker = new BackgroundWorker())
    //      {
    //        string imgLink = null;
    //        string errorMsg = null;

    //        worker.DoWork += delegate(object s,
    //            DoWorkEventArgs args)
    //        {
    //          if (worker.CancellationPending)
    //          {
    //            args.Cancel = true;
    //            return;
    //          }

    //          //var binaryImageData = File.ReadAllBytes(ofd.FileName);
    //          //LinqToTwitterHelper.

    //          ImageName = ofd.FileName;
    //          IsVisibleBtnPhoto = Visibility.Collapsed;
    //          IsDelBtnImgShow = Visibility.Visible;

    //          //imgLink = TwitPicHelper.UploadPhoto(Settings.UserName, PasswordHash,
    //          //                                    binaryImageData, null, ofd.FileName,
    //          //                                    Globals.CIPHER_KEY,
    //          //                                    Globals.CLIENTNAME, out errorMsg,
    //          //                                    ProxyHelper.GetConfiguredWebProxy(SobeesSettings));
    //          //var credentials = new TPCredentials(CurrentAccount.SessionKey, CurrentAccount.Secret, true, BGlobals.TWEETPHOTO_OAUTH_KEY, TweetPhotoServicesEnum.Twitter);
    //          //TPResponse<TweetPhotoResponse> result = TPWrapper.TPWrapper.Upload(credentials, ofd.FileName, false,
    //          //                                     null, null, string.Empty, string.Empty);
    //          //if (result.Status == HttpStatusCode.OK)
    //          //{
    //          //  imgLink = result.Response.MediaUrl;
    //          //}
    //          //else
    //          //{
    //          //  errorMsg = result.Status.Value.ToString();
    //          //}

    //          //const string photoStartTag = "<Photo>|";
    //          //const string photoEndTag = "|</Photo>";
    //          //imgLink = string.Format("{0}{1}{2}", photoStartTag, ofd.FileName, photoEndTag);
    //        };

    //        worker.RunWorkerCompleted += delegate { OnUploadImageAsyncCompleted(imgLink, errorMsg); };
    //        worker.RunWorkerAsync();
    //      }
    //    }
    //    else
    //    {
    //      StopWaiting();
    //    }
    //  }
    //  catch (Exception ex)
    //  {
    //    IsWaiting = false;
    //    TraceHelper.Trace(this, ex);
    //  }
    //}

    //private void OnUploadImageAsyncCompleted(string imgLink, string errorMsg)
    //{
    //  StopWaiting();
    //}

    #endregion UploadImage

    #region TweetShrink

    private void TweetShrink()
    {
      var result = "";
      StartWaiting();

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
          result = TweetShrinkHelper.GetNewTweetShrink(Status, ProxyHelper.GetConfiguredWebProxy(
              SobeesSettings));
        };

        worker.RunWorkerCompleted += delegate { OnTweetShrinkAsyncCompleted(result); };

        worker.RunWorkerAsync();
      }
    }

    private void OnTweetShrinkAsyncCompleted(string result)
    {
      Status = result;
      StopWaiting();
    }

    #endregion TweetShrink

    #endregion Buttons

    #region Friends List

    /// <summary>
    ///     LoadFriendsList
    /// </summary>
    private void LoadFriendsList()
    {
      try
      {
        BLogManager.LogEntry(APPNAME, "LoadFriendsList", "START", true);
        Friends = FileHelper.LoadFriendsList(Settings.UserName);
        RaisePropertyChanged(() => Friends);

        //Application.Current.Dispatcher.BeginInvokeIfRequired(LoadFriendsAsync);
        BLogManager.LogEntry(APPNAME, "LoadFriendsList", "Call Task => LoadFriendsAsync", true);
        //LoadFriendsAsync();

        BLogManager.LogEntry(APPNAME, "LoadFriendsList", "END", true);
      }
      catch (Exception ex)
      {
        BLogManager.LogEntry(APPNAME, ex);
      }
    }

    /// <summary>
    ///     LoadFriendsAsync
    /// </summary>
    private void LoadFriendsAsync()
    {
      try
      {
        BLogManager.LogEntry(APPNAME, "LoadFriendsAsync", "START", true);

        string errorMsg;
        string nextCursor;
        var result = TwitterLibV11.GetFriendsCursor(BGlobals.TWITTER_OAUTH_KEY,
            BGlobals.TWITTER_OAUTH_SECRET,
            CurrentAccount.SessionKey,
            CurrentAccount.Secret,
            CurrentAccount.Login,
            ((TwitterSettings)Settings).CursorFriends,
            out errorMsg,
            out nextCursor,
            ProxyHelper.GetConfiguredWebProxy(
                SobeesSettings));

        if (result == null) return;
        ((TwitterSettings)Settings).CursorFriends = nextCursor;
        foreach (var user in result.Where(user => !Friends.Contains(user)))
          Friends.Add(user);

        BLogManager.LogEntry(APPNAME, "LoadFriendsAsync", "SaveFriendsList", true);
        SaveFriendsList();
      }
      catch (Exception ex)
      {
        BLogManager.LogEntry(APPNAME, ex);
      }

      BLogManager.LogEntry(APPNAME, "LoadFriendsAsync", "END", true);
    }

    //private void TimerTickUpdateFriendsList(object sender, EventArgs e)
    //{
    //    Application.Current.Dispatcher.BeginInvokeIfRequired(() =>
    //    {
    //        _timerCreateFriendList.Tick -= TimerTickUpdateFriendsList;
    //        _timerCreateFriendList.Stop();
    //        Task.Factory.StartNew(LoadFriendsAsync);
    //    });
    //}

    private void SaveFriendsList()
    {
      RaisePropertyChanged(() => Friends);
      if (Settings != null) FileHelper.SaveFriendsList(Settings.UserName, Friends);
    }

    #endregion Friends List

    #region TweetDetails

    /// <summary>
    ///     UpdateConversation
    /// </summary>
    private void UpdateConversation()
    {
      Conversations.Clear();
      var entry = TweetToShow as TwitterEntry;
      if (TweetToShow == null || entry == null) return;
      if (!string.IsNullOrEmpty(TweetToShow.InReplyTo))
      {
        var id = TweetToShow.InReplyTo;

        Action mainAction = async () =>
        {
          try
          {
            while (!string.IsNullOrEmpty(id) && id != "0")
            {
              if (Conversations.Count() > 4) break;
              var result = await TwitterLibV11.GetTweetInfo(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET, CurrentAccount.SessionKey, CurrentAccount.Secret, id);
              if (!result.BisSuccess)
              {
                ShowErrorMsg(result.ErrorMessage);
                return;
              }

              var tweets = result.DataResult;
              if (tweets == null || !tweets.Any())
                return;

              //TraceHelper.Trace(APPNAME, string.Format("TwitterViewModel::UpdateConversations::Id:{0}||Add tweet to conversation feed:{1}", id, tweets[0].Title), true);
              Conversations.Add(tweets[0]);
              id = tweets[0].InReplyTo;
            }
          }
          catch (Exception ex)
          {
            TraceHelper.Trace(this, ex);
          }
        };

        TraceHelper.Trace(APPNAME, "TwitterViewModel::UpdateConversations::Id:" + id, true);

        var task = Task.Factory.StartNew(mainAction, CancellationToken.None, TaskCreationOptions.AttachedToParent, UiContext.Instance.Current);
        task.Wait();
      }
      else if (!string.IsNullOrEmpty(TweetToShow.InReplyToUserName))
      {
        TraceHelper.Trace(APPNAME, "TwitterViewModel::UpdateConversations::TweetToShow.InReplyToUserName:" + TweetToShow.InReplyToUserName, true);
        Action mainAction = () =>
              {
                try
                {
                  string errorMsg;
                  var tweets =
                    TwitterLib.SearchSummize(
                      string.Format("{0} OR {1}",
                        string.Format("from:{0} to:{1}", TweetToShow.User.NickName,
                          TweetToShow.InReplyToUserName),
                        string.Format("from:{0} to:{1}", TweetToShow.InReplyToUserName,
                          TweetToShow.User.NickName)), EnumLanguages.all,
                      Settings.NbPostToGet, string.Empty, out errorMsg);
                  if (!string.IsNullOrEmpty(errorMsg))
                  {
                    ShowErrorMsg(errorMsg);
                    return;
                  }
                  if (tweets == null || !tweets.Any())
                    return;

                  foreach (var tweet in tweets)
                    Conversations.Add(tweet);
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

    #endregion TweetDetails

    #endregion Status
  }
}