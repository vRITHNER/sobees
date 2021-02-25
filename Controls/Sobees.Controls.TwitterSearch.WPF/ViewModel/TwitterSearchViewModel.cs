#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Xml;
using BUtility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Configuration.BGlobals;
using Sobees.Controls.TwitterSearch.Cls;
using Sobees.Infrastructure.Cls;
using Sobees.Infrastructure.Commands;
using Sobees.Infrastructure.Controls;
using Sobees.Infrastructure.Model;
using Sobees.Infrastructure.Tools.Serialization;
using Sobees.Infrastructure.ViewModelBase;
using Sobees.Library.BGenericLib;
using Sobees.Library.BLocalizeLib;
using Sobees.Library.BServicesLib;
using Sobees.Library.BTwitterLib;
using Sobees.Library.BTwitterLib.Response;
using Sobees.Tools.Logging;
using Sobees.Tools.Threading;
using Sobees.Tools.Threading.Extensions;
using Sobees.Tools.Web;
using Application = System.Windows.Application;
using Binding = System.Windows.Data.Binding;
using CheckBox = System.Windows.Controls.CheckBox;
using Orientation = System.Windows.Controls.Orientation;

#endregion

namespace Sobees.Controls.TwitterSearch.ViewModel
{
  public class TwitterSearchViewModel : BServiceWorkspaceViewModel
  {
    #region Fields

    private const string APPNAME = "TwitterSearchViewModel";

    #region Fields Global

    private const int TWITTER_MSG_MAXIMUM_LENGTH = 140;
    private string _actionToConfirm;
    private string _confirmationText;
    private bool _firstSearch;
    private bool _isAddingTweet;
    private Visibility _isDelBtnImgShow = Visibility.Collapsed;
    private Visibility _isVisibleBtnPhoto = Visibility.Visible;
    private ObservableCollection<UserAccount> _listAccount;
    private ObservableCollection<Entry> _listAds;
    private ObservableCollection<Entry> _listTrends;
    private ObservableCollection<UserAccount> _listUserAccount;
    private string _newTweetMsg;
    private int _numberUserAccount;
    private UserAccount _selectedAccount;
    private TwitterList _selectedList;
    private SettingsViewModel _settingsViewModel;
    private string _stringSearch;
    private ObservableCollection<TwitterList> _twitterListOwn;
    private TwitterSearchWorkspaceViewModel _twitterSearchSelectedWorkspace;
    private ObservableCollection<TwitterSearchWorkspaceViewModel> _twitterSearchWorkspaces;

    #endregion

    #region Timer

    // private DispatcherTimer _timerFF;
    //private DispatcherTimer _timerOR;
    //private DispatcherTimer _timerTS;

    #endregion

    #region Fields Visibility

    private bool _isShowFacebookEntry = true;
    private bool _isShowTwitterSearchEntry = true;

    //private bool _isFilterFactory;
    //private bool _isShowFactery = true;
    //private bool _isShowFriendFeedEntry = true;
    //private bool _isShowOneRiotEntry = true;
    //private bool _isShowOnlyFactery;

    private Visibility _visibilityButtonRetweet = Visibility.Collapsed;

    #endregion

    #endregion

    #region Properties

    public ObservableCollection<TwitterSearchWorkspaceViewModel> TwitterSearchWorkspaces => _twitterSearchWorkspaces ?? (_twitterSearchWorkspaces = new ObservableCollection<TwitterSearchWorkspaceViewModel>());

    public SettingsViewModel SettingsViewModel => _settingsViewModel ?? (_settingsViewModel = new SettingsViewModel(this, ServiceMessenger));

    public TweetProfileViewModel TweetProfileViewModel => _tweetProfileViewModel ?? (_tweetProfileViewModel = new TweetProfileViewModel(this, null, ServiceMessenger));

    #region Global

    public Entry AddEntry;
    private BRelayCommand _checkInsideListCommand;
    private int _cursorPosition;
    private string _entryId;

    private string _imageName;
    private bool _moveCursor;
    private TweetProfileViewModel _tweetProfileViewModel;

    public string ImageName
    {
      get => _imageName;
      set
      {
        _imageName = value;
        RaisePropertyChanged();
        RaisePropertyChanged(()=> IsImgShow);
      }
    }

    public Visibility IsImgShow => string.IsNullOrEmpty(ImageName) ? Visibility.Collapsed : Visibility.Visible;

    public Visibility IsVisibleBtnPhoto
    {
      get => _isVisibleBtnPhoto;
      set
      {
        _isVisibleBtnPhoto = value;
        RaisePropertyChanged();
      }
    }

    public Visibility IsDelBtnImgShow
    {
      get => _isDelBtnImgShow;
      set
      {
        _isDelBtnImgShow = value;
        RaisePropertyChanged();
      }
    }

    public bool FirstSearch => TwitterSearchWorkspaces.Count == 0;

    private TwitterSearchFrontView CurrentFrontView { get; set; }

    public int CursorPosition
    {
      get => _cursorPosition;
      set
      {
        _cursorPosition = value;
        RaisePropertyChanged();
      }
    }

    public bool MoveCursor
    {
      get => _moveCursor;
      set
      {
        _moveCursor = value;
        RaisePropertyChanged();
      }
    }

    public TwitterList SelectedList
    {
      get => _selectedList;
      set
      {
        _selectedList = value;
        RaisePropertyChanged();
      }
    }

    public UserAccount SelectedAccount
    {
      get => _selectedAccount;
      set
      {
        _selectedAccount = value;
        RaisePropertyChanged();
      }
    }

    private TwitterSearchTypeView CurrentView { get; set; }

    public ObservableCollection<TwitterList> TwitterListOwn
    {
      get
      {
        if (_twitterListOwn == null)
        {
          _twitterListOwn = new ObservableCollection<TwitterList>();
          RaisePropertyChanged();
        }
        return _twitterListOwn;
      }
    }

    public new bool IsBtnStatuszoneOpen => false;

    public new bool IsBtnSpecialzoneOpen => true;

    public int NumberUserAccount
    {
      get => _numberUserAccount;
      set
      {
        _numberUserAccount = value;
        RaisePropertyChanged();
      }
    }

    public ObservableCollection<Entry> Tweets { get; set; }

    public ObservableCollection<UserAccount> ListAccount
    {
      get => _listAccount;
      set
      {
        _listAccount = value;
        RaisePropertyChanged();
      }
    }

    public ObservableCollection<UserAccount> ListUserAccount
    {
      get => _listUserAccount ?? (_listUserAccount = new ObservableCollection<UserAccount>());
      set
      {
        _listUserAccount = value;
        RaisePropertyChanged();
      }
    }

    public ObservableCollection<Entry> ListAds
    {
      get => _listAds ?? (_listAds = new ObservableCollection<Entry>());
      set => _listAds = value;
    }

    public ObservableCollection<Entry> ListTrends
    {
      get => _listTrends;
      set
      {
        _listTrends = value;
        RaisePropertyChanged();
      }
    }

    public string Title
    {
      get
      {
        var title = string.Empty;
        title = new LocText("Sobees.Configuration.BGlobals:Resources:RTS").ResolveLocalizedValue();
        return title;
      }
    }

    public TwitterSearchWorkspaceViewModel TwitterSearchSelectedWorkspace
    {
      get
      {
        if (TwitterSearchWorkspaces?.Count > 0)
          if (_twitterSearchSelectedWorkspace == null)
            _twitterSearchSelectedWorkspace = TwitterSearchWorkspaces[0];
        return _twitterSearchSelectedWorkspace;
      }
      set
      {
        _twitterSearchSelectedWorkspace = value;
        foreach (var tWorkspace in TwitterSearchWorkspaces)
        {
          tWorkspace.IsSelected = tWorkspace == _twitterSearchSelectedWorkspace;
        }
        RaisePropertyChanged();
      }
    }

    public string ReplyId { get; set; }

    public string RetweetId { get; set; }

    public string RetweetText { get; set; }

    public string StringSearch
    {
      get => _stringSearch;
      set
      {
        _stringSearch = value;
        RaisePropertyChanged();
      }
    }

    public User User { get; set; }

    public string ConfirmationText
    {
      get => _confirmationText;
      set
      {
        _confirmationText = value;
        RaisePropertyChanged();
      }
    }

    public override BWorkspaceViewModel ProfileView
    {
      get
      {
        if (_tweetProfileViewModel == null)
        {
          return null;
        }
        return TweetProfileViewModel;
      }
      set => base.ProfileView = value;
    }

    public override DataTemplate ProfileTemplate
    {
      get
      {
        if (_tweetProfileViewModel == null)
        {
          return null;
        }
        return TweetProfileViewModel.DataTemplateView;
      }
    }

    public override object MenuItems
    {
      get
      {
        var stackPanel = new StackPanel {Orientation = Orientation.Vertical};

        //Create ToggleButton
        var tgbtnTwitter = new ToggleButton
                           {
                             Content = new Image {Style = (Style) (Application.Current.TryFindResource("ImageStyleRTSTwitterSearchH1"))},
                             Style = (Style) (Application.Current.TryFindResource("BtntColumnLeftH1"))
                           };
        var bindingTwitter = new Binding("IsShowTwitterSearchEntry") {Source = this, Mode = BindingMode.TwoWay};
        tgbtnTwitter.ToolTip = new LocText("Sobees.Configuration.BGlobals:Resources:btntShowTS").ResolveLocalizedValue();
        tgbtnTwitter.SetBinding(ToggleButton.IsCheckedProperty, bindingTwitter);
        var tgbtnFf = new ToggleButton
                      {
                        Content = new Image {Style = (Style) (Application.Current.TryFindResource("ImageStyleRTSFriendsFeedH1"))},
                        Style = (Style) (Application.Current.TryFindResource("BtntColumnLeftH1"))
                      };
        var bindingFf = new Binding("IsShowFriendFeedEntry") {Source = this, Mode = BindingMode.TwoWay};
        tgbtnFf.ToolTip = new LocText("Sobees.Configuration.BGlobals:Resources:btntShowFF").ResolveLocalizedValue();
        tgbtnFf.SetBinding(ToggleButton.IsCheckedProperty, bindingFf);
        var tgbtnOneRiot = new ToggleButton
                           {
                             Content = new Image {Style = (Style) (Application.Current.TryFindResource("ImageStyleRTSOneriotH1"))},
                             Style = (Style) (Application.Current.TryFindResource("BtntColumnLeftH1"))
                           };
        var bindingOneRiot = new Binding("IsShowOneRiotEntry") {Source = this, Mode = BindingMode.TwoWay};
        tgbtnOneRiot.ToolTip = new LocText("Sobees.Configuration.BGlobals:Resources:btntShowOR").ResolveLocalizedValue();
        tgbtnOneRiot.SetBinding(ToggleButton.IsCheckedProperty, bindingOneRiot);
        var tgbtnOnlyFactery = new ToggleButton
                               {
                                 Content = new Image {Style = (Style) (Application.Current.TryFindResource("ImageStyleRTSOnlyFacteryH1"))},
                                 Style = (Style) (Application.Current.TryFindResource("BtntColumnLeftH1"))
                               };
        var bindingOnlyFactery = new Binding("IsShowOnlyFactery") {Source = this, Mode = BindingMode.TwoWay};
        tgbtnOnlyFactery.ToolTip = new LocText("Sobees.Configuration.BGlobals:Resources:btnOnlyFactery").ResolveLocalizedValue();
        tgbtnOnlyFactery.SetBinding(ToggleButton.IsCheckedProperty, bindingOnlyFactery);
        var tgbtnOrderFactery = new ToggleButton
                                {
                                  Content = new Image {Style = (Style) (Application.Current.TryFindResource("ImageStyleRTSSortFacteryH1"))},
                                  Style = (Style) (Application.Current.TryFindResource("BtntColumnLeftH1"))
                                };
        var bindingOrderFactery = new Binding("IsFilterFactory") {Source = this, Mode = BindingMode.TwoWay};
        tgbtnOrderFactery.ToolTip = new LocText("Sobees.Configuration.BGlobals:Resources:btnFilterRanking").ResolveLocalizedValue();
        tgbtnOrderFactery.SetBinding(ToggleButton.IsCheckedProperty, bindingOrderFactery);

        if (((TwitterSearchSettings) Settings).ShowTwitter)
        {
          stackPanel.Children.Add(tgbtnTwitter);
        }

        //if (((TwitterSearchSettings)Settings).ShowFriendFeed)
        //{
        //  stackPanel.Children.Add(tgbtnFF);
        //}
        //if (((TwitterSearchSettings)Settings).ShowOneRiot)
        //{
        //  stackPanel.Children.Add(tgbtnOneRiot);
        //}
        //if (((TwitterSearchSettings)Settings).ShowFactery)
        //{
        //  stackPanel.Children.Add(tgbtnOnlyFactery);
        //  stackPanel.Children.Add(tgbtnOrderFactery);
        //}
        //var btnShowtrends = new Button
        //                      {
        //                        Content =
        //                          new Path
        //                            {Style = (Style) (Application.Current.TryFindResource("PathStyleRTSTop10H1"))},
        //                        Style = (Style) (Application.Current.TryFindResource("BtnColumnLeftH1")),
        //                        ToolTip =
        //                          new LocText("Sobees.Configuration.BGlobals:Resources:btnTrends").ResolveLocalizedValue
        //                          (),
        //                        Command = ShowTrendsCommand
        //                      };
        //stackPanel.Children.Add(btnShowtrends);
        return stackPanel;
      }
    }

    #endregion

    #region Visibility Properties

    public bool IsShowTwitterSearchEntry
    {
      get => _isShowTwitterSearchEntry;
      set
      {
        _isShowTwitterSearchEntry = value;
        ServiceMessenger.Send("UpdateView");
        RaisePropertyChanged();
      }
    }

    //public bool IsShowOneRiotEntry
    //{
    //  get { return _isShowOneRiotEntry; }
    //  set
    //  {
    //    _isShowOneRiotEntry = value;
    //    ServiceMessenger.Send("UpdateView");
    //    RaisePropertyChanged("IsShowOneRiotEntry");
    //  }
    //}

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

    //public bool IsShowFriendFeedEntry
    //{
    //  get { return _isShowFriendFeedEntry; }
    //  set
    //  {
    //    _isShowFriendFeedEntry = value;
    //    ServiceMessenger.Send("UpdateView");
    //    RaisePropertyChanged("IsShowFriendFeedEntry");
    //  }
    //}

    public bool IsShowFacebookEntry
    {
      get => _isShowFacebookEntry;
      set
      {
        _isShowFacebookEntry = value;
        ServiceMessenger.Send("UpdateView");
        RaisePropertyChanged();
      }
    }

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

    public Visibility VisibilityButtonRetweet
    {
      get => _visibilityButtonRetweet;
      set
      {
        _visibilityButtonRetweet = value;
        RaisePropertyChanged();
      }
    }

    #endregion

    #region Activ & UnActiv Buttons

    #endregion

    #region Overriden Properties

    public override string DisplayName => ServiceType;

    public override string ServiceType
    {
      get
      {
        if (!string.IsNullOrEmpty(Title))
          return Title;

        return base.DisplayName;
      }
    }

    #endregion

    #region Viewving Manager

    public DataTemplate ColumnsDataTemplate
    {
      get
      {
        const string DT =
          "<DataTemplate x:Name='dtMainView' " + "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
          "xmlns:Views='clr-namespace:Sobees.Controls.TwitterSearch.Views;assembly=Sobees.Controls.TwitterSearch' " + "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >" +
          "<Views:UcColumsView/> " + "</DataTemplate>";

        var stringReader = new StringReader(DT);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
    }

    public DataTemplate TabDataTemplate
    {
      get
      {
        const string DT =
          "<DataTemplate x:Name='dtMainView' " + "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
          "xmlns:Views='clr-namespace:Sobees.Controls.TwitterSearch.Views;assembly=Sobees.Controls.TwitterSearch' " + "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >" +
          "<Views:UcTabView/> " + "</DataTemplate>";
        var stringReader = new StringReader(DT);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
    }

    public DataTemplate TrendsDataTemplate
    {
      get
      {
        const string DT =
          "<DataTemplate x:Name='dtMainView' " + "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
          "xmlns:Views='clr-namespace:Sobees.Controls.TwitterSearch.Views;assembly=Sobees.Controls.TwitterSearch' " + "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >" +
          "<Views:UcTrends/> " + "</DataTemplate>";
        var stringReader = new StringReader(DT);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
    }

    public DataTemplate AddToListTemplate
    {
      get
      {
        const string DT =
          "<DataTemplate x:Name='dtMainView' " + "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
          "xmlns:Views='clr-namespace:Sobees.Controls.TwitterSearch.Controls;assembly=Sobees.Controls.TwitterSearch' " + "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >" +
          "<Views:UcAddToList/> " + "</DataTemplate>";
        var stringReader = new StringReader(DT);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
    }

    public DataTemplate FollowTemplate
    {
      get
      {
        const string DT =
          "<DataTemplate x:Name='dtMainView' " + "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
          "xmlns:Views='clr-namespace:Sobees.Controls.TwitterSearch.Controls;assembly=Sobees.Controls.TwitterSearch' " + "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >" +
          "<Views:UcFollow/> " + "</DataTemplate>";

        var stringReader = new StringReader(DT);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
    }

    public DataTemplate ConfirmationTemplate
    {
      get
      {
        const string DT =
          "<DataTemplate x:Name='dtMainView' " + "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
          "xmlns:Views='clr-namespace:Sobees.Controls.TwitterSearch.Controls;assembly=Sobees.Controls.TwitterSearch' " + "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >" +
          "<Views:UcConfirmation/> " + "</DataTemplate>";
        var stringReader = new StringReader(DT);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
    }

    public DataTemplate AddToFavoriteTemplate
    {
      get
      {
        const string DT =
          "<DataTemplate x:Name='dtMainView' " + "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
          "xmlns:Views='clr-namespace:Sobees.Controls.TwitterSearch.Controls;assembly=Sobees.Controls.TwitterSearch' " + "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >" +
          "<Views:UcAddToFavorite/> " + "</DataTemplate>";

        var stringReader = new StringReader(DT);
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
          case TwitterSearchTypeView.Columns:
            return ColumnsDataTemplate;

          case TwitterSearchTypeView.Tab:
            return TabDataTemplate;

          default:
            throw new ArgumentOutOfRangeException();
        }
      }
      set { base.MainTemplateView = value; }
    }

    public override BWorkspaceViewModel MainViewModel
    {
      get => this;
      set => base.MainViewModel = value;
    }

    public override DataTemplate FrontTemplateView
    {
      get
      {
        switch (CurrentFrontView)
        {
          case TwitterSearchFrontView.None:
            return null;

          case TwitterSearchFrontView.Trends:
            return TrendsDataTemplate;

          case TwitterSearchFrontView.Settings:
            return SettingsViewModel.DataTemplateView;

          case TwitterSearchFrontView.AddToList:
            return AddToListTemplate;

          case TwitterSearchFrontView.Follow:
            return FollowTemplate;

          case TwitterSearchFrontView.Confirmation:
            return ConfirmationTemplate;

          case TwitterSearchFrontView.AddToFavorite:
            return AddToFavoriteTemplate;

          default:
            throw new ArgumentOutOfRangeException();
        }
        return base.FrontTemplateView;
      }
      set => base.FrontTemplateView = value;
    }

    public override BWorkspaceViewModel FrontViewModel
    {
      get
      {
        switch (CurrentFrontView)
        {
          case TwitterSearchFrontView.None:
            return null;

          case TwitterSearchFrontView.Trends:
            return this;

          case TwitterSearchFrontView.Settings:
            return SettingsViewModel;

          case TwitterSearchFrontView.AddToList:
            return this;

          case TwitterSearchFrontView.Follow:
            return this;

          case TwitterSearchFrontView.Confirmation:
            return this;

          case TwitterSearchFrontView.AddToFavorite:
            return this;

          default:
            throw new ArgumentOutOfRangeException();
        }
        return base.FrontViewModel;
      }
      set => base.FrontViewModel = value;
    }

    #endregion

    #endregion

    #region Commands

    public BRelayCommand CheckInsideListCommand
    {
      get
      {
        try
        {
          return _checkInsideListCommand ?? (_checkInsideListCommand = new BRelayCommand(LoginChoice));
        }
        catch (Exception ex)
        {
          TraceHelper.Trace(this, ex);
        }
        return null;
      }
    }

    public RelayCommand ShortenUrlCommand { get; private set; }

    /// <summary>
    ///   Command for Upload an image on Twitpic
    /// </summary>
    public RelayCommand UploadImageCommand { get; private set; }

    public RelayCommand DeleteImageCommand { get; private set; }

    /// <summary>
    ///   Command for tweetshrink the text.
    /// </summary>
    public RelayCommand TweetShrinkCommand { get; private set; }

    public RelayCommand SaveKeywordsCommand { get; set; }

    public RelayCommand CloseTrendsCommand { get; set; }

    public RelayCommand<string> PostSearchTrendsCommand { get; set; }

    public Visibility AddsVisibility => SobeesSettings.DisableAds ? Visibility.Collapsed : Visibility.Visible;

    public RelayCommand CancelConfirmationCommand { get; private set; }

    /// <summary>
    ///   Command for close confirmation
    /// </summary>
    public RelayCommand ConfirmationCommand { get; private set; }

    /// <summary>
    ///   Commd to use the old type of retweet
    /// </summary>
    public RelayCommand ReTweetOldCommand { get; private set; }

    public RelayCommand ShowTrendsCommand { get; set; }

    #endregion

    #region Constructors

    public TwitterSearchViewModel(BPosition bposition, BServiceWorkspace serviceWorkspace, string settings) : base(bposition, serviceWorkspace)
    {
      //Register to the Messenger
      //Messenger.Default.Register<string>(this, DoAction);
      ServiceMessenger.Register<string>(this, DoAction);
      ServiceMessenger.Register<BMessage>(this, DoActionMessage);

      //restore Settings
      if (string.IsNullOrEmpty(settings))
      {
        Settings = new TwitterSearchSettings();
      }
      else
      {
        Settings = GenericSerializer.DeserializeObject(settings, typeof (TwitterSearchSettings)) as TwitterSearchSettings;
      }

      ListAccount = new ObservableCollection<UserAccount>();
      UpdateAccountList();
      LoadWorkspace();
      InitCommands();
    }

    private void UpdateAccountList()
    {
      try
      {
        foreach (var account in SobeesSettings.Accounts)
        {
          if (account.Type != EnumAccountType.Twitter) continue;
          if (account.Login == null) continue;
          if (!ListAccount.Contains(account))
            ListAccount.Add(account);
        }
        var i = 0;
        while (i < ListAccount.Count)
        {
          if (!SobeesSettings.Accounts.Contains(ListAccount[i]))
          {
            ListAccount.RemoveAt(i);
          }
          else
          {
            i++;
          }
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    #endregion

    #region Methods

    private TwitterCredentials _twitterUserCredentials;

    public TwitterCredentials TwitterUserCredentials
    {
      get
      {
        if (_twitterUserCredentials == null)
        {
          if (ListAccount.Any())
          {
            var goodAccount = ListAccount.FirstOrDefault(l => l.Login != null);
            if (goodAccount != null)
              _twitterUserCredentials = new TwitterCredentials
                                        {
                                          ConsumerKey = BGlobals.TWITTER_OAUTH_KEY,
                                          ConsumerSecret = BGlobals.TWITTER_OAUTH_SECRET,
                                          OAuthToken = goodAccount.AuthToken,
                                          OAuthSecret = goodAccount.Secret
                                        };
          }
        }
        return _twitterUserCredentials;
      }
    }

    public override void NewSearch(string newSearch)
    {
      OpenTrend(newSearch);
    }

    private void LoadWorkspace()
    {
      try
      {
        if (Settings != null)
        {
          if (((TwitterSearchSettings) Settings).TwitterSearchWorkspaceSettings.Count < 1)
          {
            CurrentFrontView = TwitterSearchFrontView.Trends;
            LoadTrends();
            return;
          }

          foreach (var twitterSearchWorkspaceSetting in
            ((TwitterSearchSettings) Settings).TwitterSearchWorkspaceSettings)
          {
            var tswv = new TwitterSearchWorkspaceViewModel(this, twitterSearchWorkspaceSetting, ServiceMessenger);
            TwitterSearchWorkspaces.Add(tswv);
            if (TwitterSearchSelectedWorkspace == null)
              TwitterSearchSelectedWorkspace = tswv;
          }
          CurrentFrontView = TwitterSearchFrontView.None;
          CurrentView = TwitterSearchTypeView.Tab;
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    public override void DoAction(string param)
    {
      switch (param)
      {
        case "SaveSettingsTS":
          SaveSettings();
          CloseSettings();
          break;

        case "CloseSettingsTS":
          CloseSettings();
          break;

        case "AccountAdded":
          UpdateAccountList();
          break;

        case "CloseProfile":
          CloseProfile();
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
        RaisePropertyChanged("AddsVisibility");

        base.OnSettingsUpdated();

        foreach (var workspace in TwitterSearchWorkspaces)
        {
          workspace.UpdateAdsVisibility();
        }
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

        case "SaveSettingsTS":
          SaveSettings();
          CloseSettings();
          break;

        case "CloseSettingsTS":
          CloseSettings();
          break;

        case "ReportSpam":
          ReportSpam(message.Parameter as Entry);
          break;

        case "ReplyToAll":
          ReplyToAll(message.Parameter as Entry);
          break;

        case "AddToFavorite":
          AddToFavorite(message.Parameter as Entry);
          break;

        case "AddToList":
          AddToList(message.Parameter as Entry);
          break;

          //case "DeleteStatus":
          //  DeleteStatus(message.Parameter as TwitterEntry);
          //  break;
        case "Retweet":
          Retweet(message.Parameter as Entry);
          break;

        case "Reply":
          Reply(message.Parameter as Entry);
          break;

        case "Follow":
          Follow(message.Parameter as Entry);
          break;

        case "CloseSearch":
          CloseSearch(message.Parameter as string);
          break;

        case "ShowTweet":
          var tweetEntry = message.Parameter as TwitterEntry;
          if (tweetEntry != null)
          {
            ShowTweet((TwitterEntry) message.Parameter);
            break;
          }

          var otherEntry = message.Parameter as Entry;
          if (otherEntry != null)
          {
            ShowTweetOther(message.Parameter as Entry);
          }

          break;
      }
      base.DoActionMessage(message);
    }

    private void ShowTweet(TwitterEntry entry)
    {
      TweetProfileViewModel.ShowTweet(entry);
      RaisePropertyChanged("ProfileTemplate");
      RaisePropertyChanged("ProfileView");
    }

    private void ShowTweetOther(Entry entry)
    {
      TweetProfileViewModel.ShowTweetOther(entry);
      RaisePropertyChanged("ProfileTemplate");
      RaisePropertyChanged("ProfileView");
    }

    private void CloseSearch(string search)
    {
      try
      {
        foreach (var model in TwitterSearchWorkspaces)
        {
          if (model.WorkspaceSettings.SearchQuery != search) continue;
          TwitterSearchWorkspaces.Remove(model);
          model.Cleanup();
          foreach (var setting in ((TwitterSearchSettings) Settings).TwitterSearchWorkspaceSettings.Where(setting => setting.SearchQuery == search))
          {
            ((TwitterSearchSettings) Settings).TwitterSearchWorkspaceSettings.Remove(setting);
            break;
          }
          if (FirstSearch)
          {
            LoadTrends();
            CurrentFrontView = TwitterSearchFrontView.Trends;
            CurrentView = TwitterSearchTypeView.Tab;
            RaisePropertyChanged("FrontTemplateView");
            RaisePropertyChanged("FrontViewModel");
            RaisePropertyChanged("MainTemplateView");
            RaisePropertyChanged("MainViewModel");
          }

          ProcessHelper.PerformAggressiveCleanup();
          return;
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    protected void Dispose()
    {
      while (TwitterSearchWorkspaces.Any())
      {
        var model = TwitterSearchWorkspaces[0];
        TwitterSearchWorkspaces.Remove(model);
        model.Cleanup();
        ProcessHelper.PerformAggressiveCleanup();
        return;
      }
    }

    public override void DeleteUser(UserAccount account)
    {
      UpdateAccountList();
    }

    private void Follow(Entry entry)
    {
      try
      {
        CurrentFrontView = TwitterSearchFrontView.Follow;
        User = entry.User ?? ((TwitterEntry) entry).User;
        ConfirmationText = new LocText("Sobees.Configuration.BGlobals:Resources:txtblSelectFollow").ResolveLocalizedValue();
        _actionToConfirm = "Follow";
        RaisePropertyChanged("FrontTemplateView");
        RaisePropertyChanged("FrontViewModel");
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    private void AddToFavorite(Entry entry)
    {
      try
      {
        CurrentFrontView = TwitterSearchFrontView.AddToFavorite;
        _entryId = entry.OrigLink;
        var j = _entryId.IndexOf("statuses/") + 9;
        _entryId = _entryId.Substring(j, _entryId.Length - j);
        User = entry.User ?? ((TwitterEntry) entry).User;
        ConfirmationText = new LocText("Sobees.Configuration.BGlobals:Resources:txtblSelectFavorites").ResolveLocalizedValue();
        _actionToConfirm = "AddToFavorite";
        RaisePropertyChanged("FrontTemplateView");
        RaisePropertyChanged("FrontViewModel");
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    private void AddToList(Entry entry)
    {
      try
      {
        UpdateList();
        CurrentFrontView = TwitterSearchFrontView.AddToList;
        User = entry.User ?? ((TwitterEntry) entry).User;
        ConfirmationText = new LocText("Sobees.Configuration.BGlobals:Resources:txtblSelectListTwitter").ResolveLocalizedValue();
        _actionToConfirm = "AddToList";
        RaisePropertyChanged("FrontTemplateView");
        RaisePropertyChanged("FrontViewModel");
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    private void UpdateList()
    {
      try
      {
        if (TwitterListOwn.Count != 0)
        {
          return;
        }
        foreach (var account in ListAccount)
        {
          using (var worker = new BackgroundWorker())
          {
            var error = "";
            List<TwitterList> result = null;

            var userAccount = account;
            worker.DoWork += delegate(object s, DoWorkEventArgs args)
                             {
                               if (worker.CancellationPending)
                               {
                                 args.Cancel = true;
                                 return;
                               }
                               result = TwitterLibV11.GetListOwn(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET, userAccount.SessionKey, userAccount.Secret,
                                                                 userAccount.Login, out error, ProxyHelper.GetConfiguredWebProxy(SobeesSettings));
                             };
            worker.RunWorkerCompleted += delegate { OnGetListAsyncCompleted(result, error); };
            worker.RunWorkerAsync();
          }
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    private void Reply(Entry entry)
    {
      try
      {
        VisibilityButtonRetweet = Visibility.Collapsed;
        if (entry == null) return;
        if (entry.Id.Contains(":"))
        {
          var split = entry.Id.Split(Convert.ToChar(":"));
          ReplyId = split[2];
        }
        else
        {
          ReplyId = entry.Id;
        }
        RetweetId = string.Empty;
        IsStatusZoneOpen = true;
        if (entry.User != null)
        {
          Status = string.Empty;
          CursorPosition = $"@{entry.User.NickName} ".Length;
          MoveCursor = true;
          Status = $"@{entry.User.NickName} ";
        }
        else
        {
          if (((TwitterEntry) entry).User == null) return;
          Status = string.Empty;
          CursorPosition = $"@{((TwitterEntry) entry).User.NickName} ".Length;
          MoveCursor = true;
          Status = $"@{((TwitterEntry) entry).User.NickName} ";
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    private void Retweet(Entry entry)
    {
      try
      {
        VisibilityButtonRetweet = Visibility.Visible;
        if (entry == null) return;
        IsStatusZoneOpen = true;
        if (entry.Id.Contains(":"))
        {
          var split = entry.Id.Split(Convert.ToChar(":"));
          RetweetId = split[2];
        }
        else
        {
          RetweetId = entry.Id;
        }
        Status = entry.Title;
        ReplyId = string.Empty;
        CursorPosition = entry.Title.Length;
        MoveCursor = true;
        Status = entry.Title;
        if (entry.User != null)
        {
          RetweetText = $"RT @{entry.User.NickName}: ";
        }
        else
        {
          if (((TwitterEntry) entry).User != null)
          {
            RetweetText = $"RT @{((TwitterEntry) entry).User.NickName}: ";
          }
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    private void ReplyToAll(Entry entry)
    {
      try
      {
        VisibilityButtonRetweet = Visibility.Collapsed;
        RetweetId = string.Empty;
        if (entry.Id.Contains(":"))
        {
          var split = entry.Id.Split(Convert.ToChar(":"));
          ReplyId = split[2];
        }
        else
        {
          ReplyId = entry.Id;
        }

        IsStatusZoneOpen = true;
        Status = string.Empty;
        if (entry.User != null)
        {
          CursorPosition = $"@{entry.User.NickName} {FindAllReply(entry.Title)}".Length;
          MoveCursor = true;
          Status = $"@{entry.User.NickName} {FindAllReply(entry.Title)}";
        }
        else
        {
          if (((TwitterEntry) entry).User == null) return;
          CursorPosition = $"@{((TwitterEntry) entry).User.NickName} {FindAllReply(entry.Title)}".Length;
          MoveCursor = true;
          Status = $"@{((TwitterEntry) entry).User.NickName} {FindAllReply(entry.Title)}";
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    private static string FindAllReply(string title)
    {
      var splited = title.Split((" :").ToCharArray());
      var lst = new StringBuilder();
      foreach (var s in splited)
      {
        if (s.StartsWith("@"))
        {
          lst.Append(s);
          lst.Append(" ");
        }
      }

      return lst.ToString();
    }

    private void ReportSpam(Entry entry)
    {
      try
      {
        CurrentFrontView = TwitterSearchFrontView.Confirmation;
        User = entry.User;
        ConfirmationText = new LocText("Sobees.Configuration.BGlobals:Resources:txtblConfirmationReportSpam").ResolveLocalizedValue();
        _actionToConfirm = "Report";
        RaisePropertyChanged("FrontTemplateView");
        RaisePropertyChanged("FrontViewModel");
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    private void CloseSettings()
    {
      try
      {
        CurrentFrontView = TwitterSearchFrontView.None;
        RaisePropertyChanged("FrontTemplateView");
        RaisePropertyChanged("FrontViewModel");
        _settingsViewModel.Cleanup();
        _settingsViewModel = null;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    /// <summary>
    ///   SaveSettings
    /// </summary>
    private void SaveSettings()
    {
      if (!SettingsViewModel.IsDirty)
        return;

      try
      {
        if (Settings.NbMaxPosts != _settingsViewModel.MaxTweet)
        {
          Settings.NbMaxPosts = _settingsViewModel.MaxTweet;
        }
        if (((TwitterSearchSettings) Settings).ViewStateTweets != _settingsViewModel.ViewStateTweets)
        {
          ((TwitterSearchSettings) Settings).ViewStateTweets = _settingsViewModel.ViewStateTweets;
        }

        if (((TwitterSearchSettings) Settings).ViewRrafIcon != _settingsViewModel.ViewRrafIcon)
        {
          ((TwitterSearchSettings) Settings).ViewRrafIcon = _settingsViewModel.ViewRrafIcon;
        }

        ((TwitterSearchSettings) Settings).RefreshTimeFF = _settingsViewModel.SlRefreshTimeFF;
        ((TwitterSearchSettings) Settings).RefreshTimeOR = _settingsViewModel.SlRefreshTimeOR;
        ((TwitterSearchSettings) Settings).RefreshTimeTS = _settingsViewModel.SlRefreshTimeTS;
        ((TwitterSearchSettings) Settings).ShowTwitter = _settingsViewModel.IsUseTwitterSearch;

        ((TwitterSearchSettings) Settings).ShowFacebook = _settingsViewModel.IsUseFacebook;
        if (_settingsViewModel.IsUseFacebook != IsShowFacebookEntry)
          IsShowFacebookEntry = _settingsViewModel.IsUseFacebook;

        RaisePropertyChanged("MenuItems");

        if (Settings.NbPostToGet != _settingsViewModel.Rpp)
        {
          Settings.NbPostToGet = _settingsViewModel.Rpp;
        }
        ServiceMessenger.Send("SettingsUpdated");
        Refresh();
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
      Messenger.Default.Send("SaveSettings");
    }

    /// <summary>
    ///   UpdateAll
    /// </summary>
    public override void UpdateAll()
    {
      try
      {
        foreach (var twitterSearchWorkspace in TwitterSearchWorkspaces)
        {
          twitterSearchWorkspace.Refresh();
        }
      }
      catch (Exception ex)
      {
        BLogManager.LogEntry(APPNAME, "UpdateAll", ex);
      }
    }

    public override void ShowSettings()
    {
      try
      {
        CurrentFrontView = TwitterSearchFrontView.Settings;
        RaisePropertyChanged("FrontTemplateView");
        RaisePropertyChanged("FrontViewModel");
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    private void Search()
    {
      try
      {
        var exist = false;
        for (var i = 0; (i < TwitterSearchWorkspaces.Count) && (!exist); i++)
        {
          if (!TwitterSearchWorkspaces[i].WorkspaceSettings.SearchQuery.Equals(StringSearch)) continue;
          TwitterSearchSelectedWorkspace = TwitterSearchWorkspaces[i];
          exist = true;
        }

        if (FirstSearch)
        {
          CurrentFrontView = TwitterSearchFrontView.None;
          CurrentView = TwitterSearchTypeView.Tab;
          RaisePropertyChanged("FrontTemplateView");
          RaisePropertyChanged("FrontViewModel");
          RaisePropertyChanged("MainTemplateView");
          RaisePropertyChanged("MainViewModel");
        }
        if (!exist)
        {
          var tsws = new TwitterSearchWorkspaceSettings(StringSearch, 0, 0, EnumLanguages.all, Settings.RefreshTime, Settings.NbPostToGet, string.Empty);
          ((TwitterSearchSettings) Settings).TwitterSearchWorkspaceSettings.Add(tsws);
          var tswvm = new TwitterSearchWorkspaceViewModel(this, tsws, ServiceMessenger);
          TwitterSearchWorkspaces.Add(tswvm);
          TwitterSearchSelectedWorkspace = tswvm;
        }
        StringSearch = "";
        RaisePropertyChanged("FirstSearch");
        Messenger.Default.Send("SaveSettings");
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    protected override void CancelPostStatus()
    {
      try
      {
        Status = " ";
        Status = "";
        RetweetId = string.Empty;
        ReplyId = string.Empty;
        VisibilityButtonRetweet = Visibility.Collapsed;
        IsStatusZoneOpen = false;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    protected override bool CanPostStatus()
    {
      return !string.IsNullOrEmpty(Status) && ListUserAccount.Any();
    }

    protected override void PostStatus()
    {
      _isAddingTweet = true;
      {
        try
        {
          StartWaiting();

          var result = BTwitterResponseResult<TwitterEntry>.CreateInstance();
          //TwitterEntry result = null;
          string errorMsg = null;

          using (var worker = new BackgroundWorker())
          {
            worker.DoWork += async delegate(object s, DoWorkEventArgs args)
                                   {
                                     if (worker.CancellationPending)
                                     {
                                       args.Cancel = true;
                                       return;
                                     }
                                     foreach (var account in ListUserAccount)
                                     {
                                       if (!string.IsNullOrEmpty(RetweetId))
                                       {
                                         result =
                                           await TwitterLibV11.Retweet(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET, account.SessionKey, account.Secret, RetweetId);
                                         if (result.BisSuccess)
                                         {
                                           Application.Current.Dispatcher.BeginInvokeIfRequired(() =>
                                                                                                {
                                                                                                  IsStatusZoneOpen = false;
                                                                                                  VisibilityButtonRetweet = Visibility.Collapsed;
                                                                                                  RetweetId = string.Empty;
                                                                                                  Status = string.Empty;
                                                                                                });
                                         }
                                         else
                                         {
                                           ShowErrorMsg(result.ErrorMessage);
                                         }
                                       }
                                       else
                                       {
                                         if (string.IsNullOrEmpty(ImageName))
                                         {
                                           result =
                                             await
                                             TwitterLibV11.AddTweet(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET, account.SessionKey, account.Secret, Status, false,
                                                                    ReplyId);
                                         }
                                         else
                                         {
                                           _twitterUserCredentials = new TwitterCredentials
                                                                     {
                                                                       ConsumerKey = BGlobals.TWITTER_OAUTH_KEY,
                                                                       ConsumerSecret = BGlobals.TWITTER_OAUTH_SECRET,
                                                                       OAuthToken = account.AuthToken,
                                                                       OAuthSecret = account.Secret
                                                                     };

                                           var fileinfo = new FileInfo(ImageName);
                                           var twitterCtx = TwitterLibV11.GetTwitterContext(TwitterUserCredentials);
                                           var twitterEntry = await TwitterLibV11.PostTweetWithMedia(twitterCtx, Status, fileinfo.FullName, fileinfo.Name);
                                           result = twitterEntry;
                                         }
                                         if (result.BisSuccess)
                                         {
                                           Application.Current.Dispatcher.BeginInvokeIfRequired(() =>
                                                                                                {
                                                                                                  IsStatusZoneOpen = false;
                                                                                                  VisibilityButtonRetweet = Visibility.Collapsed;
                                                                                                  RetweetId = string.Empty;
                                                                                                  Status = string.Empty;
                                                                                                });
                                         }
                                         else
                                         {
                                           ShowErrorMsg(result.ErrorMessage);
                                         }
                                       }
                                     }
                                   };
            worker.RunWorkerCompleted += delegate { Refresh(); };
            worker.RunWorkerAsync();
          }
        }
        catch (Exception ex)
        {
          TraceHelper.Trace(this, ex);
          _isAddingTweet = false;
          StopWaiting();
        }
      }
    }

    protected override void InitCommands()
    {
      try
      {
        ShortenUrlCommand = new RelayCommand(ShortenUrl, CanShortenUrl);
        UploadImageCommand = new RelayCommand(UploadImage, CanUploadImage);
        DeleteImageCommand = new RelayCommand(DeleteImage);
        TweetShrinkCommand = new RelayCommand(TweetShrink);
        SaveKeywordsCommand = new RelayCommand(Search);

        PostSearchTrendsCommand = new RelayCommand<string>(OpenTrend);
        CloseTrendsCommand = new RelayCommand(() =>
                                              {
                                                CurrentFrontView = TwitterSearchFrontView.None;
                                                RaisePropertyChanged("FrontTemplateView");
                                                RaisePropertyChanged("FrontViewModel");
                                              });
        ShowTrendsCommand = new RelayCommand(OpenTrendsBox);
        CancelConfirmationCommand = new RelayCommand(CloseConfirmation);
        ConfirmationCommand = new RelayCommand(() =>
                                               {
                                                 if (_actionToConfirm == "AddToFavorite")
                                                 {
                                                   AddToFavoriteConfirmation();
                                                 }
                                                 if (_actionToConfirm == "AddToList")
                                                 {
                                                   AddToListConfirmation();
                                                 }
                                                 if (_actionToConfirm == "Follow")
                                                 {
                                                   FollowConfirmation();
                                                 }
                                               }, CanConfirmation);
        ReTweetOldCommand = new RelayCommand(() =>
                                             {
                                               VisibilityButtonRetweet = Visibility.Collapsed;
                                               RetweetId = string.Empty;
                                               CursorPosition = (RetweetText + Status).Length;
                                               MoveCursor = true;
                                               Status = RetweetText + Status;
                                             });
        base.InitCommands();
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    private void CloseProfile()
    {
      try
      {
        if (_tweetProfileViewModel != null) _tweetProfileViewModel.Cleanup();
        _tweetProfileViewModel = null;
        RaisePropertyChanged("ProfileTemplate");
        RaisePropertyChanged("ProfileView");
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    private bool CanUploadImage()
    {
      return ListUserAccount.Any();
    }

    private bool CanConfirmation()
    {
      if (_actionToConfirm == "AddToList")
      {
        return SelectedList != null;
      }

      if (_actionToConfirm == "Follow")
      {
        return SelectedAccount != null;
      }
      if (_actionToConfirm == "AddToFavorite")
      {
        return SelectedAccount != null;
      }

      return false;
    }

    /// <summary>
    ///   FollowConfirmation
    /// </summary>
    private void FollowConfirmation()
    {
      try
      {
        var result = BTwitterResponseResult<TwitterUser>.CreateInstance();

        using (var worker = new BackgroundWorker())
        {
          worker.DoWork += async delegate(object s, DoWorkEventArgs args)
                                 {
                                   if (worker.CancellationPending)
                                   {
                                     args.Cancel = true;
                                     return;
                                   }
                                   result.DataResult =
                                     await
                                     TwitterLibV11.Follow(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET, SelectedAccount.SessionKey, SelectedAccount.Secret,
                                                          User.NickName, false);
                                 };

          worker.RunWorkerCompleted += delegate { OnGetMenuActionAsyncCompleted(result.ErrorMessage); };
          worker.RunWorkerAsync();
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    /// <summary>
    ///   AddToListConfirmation
    /// </summary>
    private void AddToListConfirmation()
    {
      try
      {
        using (var worker = new BackgroundWorker())
        {
          var error = string.Empty;
          worker.DoWork += delegate(object s, DoWorkEventArgs args)
                           {
                             if (worker.CancellationPending)
                             {
                               args.Cancel = true;
                               return;
                             }

                             //Find the Correct account
                             foreach (var account in ListAccount)
                             {
                               if (account.Login != SelectedList.Creator.NickName) continue;

                               //var user = TwitterLib.GetUserInfo(User.NickName, out error);
                               var user = TwitterLibV11.GetUserInfo(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET, account.SessionKey, account.Secret, User.NickName,
                                                                    out error, ProxyHelper.GetConfiguredWebProxy(SobeesSettings));
                               if (user != null)
                                 TwitterLibV11.AddToList(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET, account.SessionKey, account.Secret, account.Login,
                                                         SelectedList.Slug, user.Id, ProxyHelper.GetConfiguredWebProxy(SobeesSettings));
                               break;
                             }
                           };

          worker.RunWorkerCompleted += delegate { OnGetMenuActionAsyncCompleted(error); };
          worker.RunWorkerAsync();
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    /// <summary>
    ///   AddToFavoriteConfirmation
    /// </summary>
    private void AddToFavoriteConfirmation()
    {
      try
      {
        var result = BTwitterResponseResult<TwitterEntry>.CreateInstance();

        using (var worker = new BackgroundWorker())
        {
          worker.DoWork += async delegate(object s, DoWorkEventArgs args)
                                 {
                                   if (worker.CancellationPending)
                                   {
                                     args.Cancel = true;
                                     return;
                                   }

                                   result =
                                     await
                                     TwitterLibV11.AddFavorite(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET, SelectedAccount.SessionKey, SelectedAccount.Secret,
                                                               _entryId, false);
                                 };

          worker.RunWorkerCompleted += delegate { OnGetMenuActionAsyncCompleted(result.ErrorMessage); };
          worker.RunWorkerAsync();
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    private void CloseConfirmation()
    {
      try
      {
        CurrentFrontView = TwitterSearchFrontView.None;
        RaisePropertyChanged("FrontTemplateView");
        RaisePropertyChanged("FrontViewModel");
        User = null;
        _actionToConfirm = string.Empty;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    private void OnGetMenuActionAsyncCompleted(string error)
    {
      try
      {
        CloseConfirmation();
        if (!string.IsNullOrEmpty(error))
        {
          ShowErrorMsg(error);
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    private void OpenTrendsBox()
    {
      try
      {
        LoadTrends();
        CurrentFrontView = TwitterSearchFrontView.Trends;
        RaisePropertyChanged("FrontTemplateView");
        RaisePropertyChanged("FrontViewModel");
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    private void OpenTrend(string search)
    {
      try
      {
        StringSearch = search;
        Search();
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    public void LoginChoice(object param)
    {
      try
      {
        var objs = BRelayCommand.CheckParams(param);
        if (objs == null) return;
        if (objs[1].GetType() == typeof (CheckBox))
        {
          var checkBox = objs[1] as CheckBox;

          if (checkBox == null) return;
          var isCheck = checkBox.IsChecked;
          if (checkBox.DataContext.GetType() == typeof (UserAccount))
          {
            var item = checkBox.DataContext as UserAccount;
            if ((bool) isCheck && !ListUserAccount.Contains(item))
            {
              ListUserAccount.Add(item);
            }
            else
            {
              ListUserAccount.Remove(item);
            }
            NumberUserAccount = ListUserAccount.Count;
          }
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    /// <summary>
    ///   LoadTrends
    /// </summary>
    private void LoadTrends()
    {
      IsWaiting = true;
      UpdateListAds();
      try
      {
        string errorMsg = null;
        using (var worker = new BackgroundWorker())
        {
          var newEntries = new ObservableCollection<Entry>();

          worker.DoWork += delegate(object s, DoWorkEventArgs args)
                           {
                             if (worker.CancellationPending)
                             {
                               args.Cancel = true;
                               return;
                             }

                             newEntries =
                               new ObservableCollection<Entry>(TwitterLib.SearchTrends(TwitterUserCredentials.OAuthToken, TwitterUserCredentials.OAuthSecret, out errorMsg,
                                                                                       BGlobals.TWITTER_TREND_NUMBER, ProxyHelper.GetConfiguredWebProxy(SobeesSettings)));
                           };

          worker.RunWorkerCompleted += delegate { OnGetTrendsAsyncCompleted(newEntries, errorMsg); };
          worker.RunWorkerAsync();
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    public override void SetAccount(UserAccount account)
    {
      try
      {
        StringSearch = account.Login;
        Search();
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    public override void RaiseStatusChanged()
    {
      base.RaiseStatusChanged();
      ShortenUrlCommand.RaiseCanExecuteChanged();
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

          worker.DoWork += delegate(object s, DoWorkEventArgs args)
                           {
                             if (worker.CancellationPending)
                             {
                               args.Cancel = true;
                               return;
                             }

                             switch (SobeesSettings.UrlShortener)
                             {
                               case UrlShorteners.BitLy:
                                 result = BitLyHelper.ConvertUrlsToTinyUrls(Status, ProxyHelper.GetConfiguredWebProxy(SobeesSettings), SobeesSettings.BitLyUserName,
                                                                            SobeesSettings.BitLyPassword);
                                 break;

                               case UrlShorteners.Digg:
                                 result = DiggHelper.ConvertUrlsToTinyUrls(Status, ProxyHelper.GetConfiguredWebProxy(SobeesSettings));
                                 break;

                               case UrlShorteners.IsGd:
                                 result = IsGdHelper.ConvertUrlsToTinyUrls(Status, ProxyHelper.GetConfiguredWebProxy(SobeesSettings));
                                 break;

                               case UrlShorteners.TinyUrl:
                                 result = TinyUrlHelper.ConvertUrlsToTinyUrls(Status, ProxyHelper.GetConfiguredWebProxy(SobeesSettings));
                                 break;

                               case UrlShorteners.TrIm:
                                 result = TrImHelper.ConvertUrlsToTinyUrls(Status, ProxyHelper.GetConfiguredWebProxy(SobeesSettings));
                                 break;

                               case UrlShorteners.Twurl:
                                 result = TwurlHelper.ConvertUrlsToTinyUrls(Status, ProxyHelper.GetConfiguredWebProxy(SobeesSettings));
                                 break;

                               case UrlShorteners.MigreMe:
                                 result = MigreMeHelper.ConvertUrlsToTinyUrls(Status, ProxyHelper.GetConfiguredWebProxy(SobeesSettings));
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
        if (words.Any(HyperLinkHelper.IsHyperlink))
          return true;
      }
      return true;
    }

    private void OnShortenUrlAsyncCompleted(string result)
    {
      try
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
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    #endregion

    #region UploadImage

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

    private void UploadImage()
    {
      try
      {
        StartWaiting();
        const string FILTER = "Images |*.jpg;*.gif;*.png";

        var ofd = new OpenFileDialog {InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), Filter = FILTER, RestoreDirectory = true};

        var dr = ofd.ShowDialog();
        if (dr == DialogResult.OK)
        {
          using (var worker = new BackgroundWorker())
          {
            string imgLink = null;
            string errorMsg = null;

            worker.DoWork += delegate(object s, DoWorkEventArgs args)
                             {
                               if (worker.CancellationPending)
                               {
                                 args.Cancel = true;
                                 return;
                               }

                               ImageName = ofd.FileName;
                               IsVisibleBtnPhoto = Visibility.Collapsed;
                               IsDelBtnImgShow = Visibility.Visible;
                             };

            worker.RunWorkerCompleted += delegate { OnUploadImageAsyncCompleted(imgLink, errorMsg); };
            worker.RunWorkerAsync();
          }
        }
        else
        {
          StopWaiting();
        }
      }
      catch (Exception ex)
      {
        IsWaiting = false;
        TraceHelper.Trace(this, ex);
      }
    }

    private void OnUploadImageAsyncCompleted(string imgLink, string errorMsg)
    {
      StopWaiting();
    }

    #endregion

    #region TweetShrink

    private void TweetShrink()
    {
      var result = "";
      StartWaiting();
      using (var worker = new BackgroundWorker())
      {
        worker.DoWork += delegate(object s, DoWorkEventArgs args)
                         {
                           if (worker.CancellationPending)
                           {
                             args.Cancel = true;
                             return;
                           }
                           result = TweetShrinkHelper.GetNewTweetShrink(Status, ProxyHelper.GetConfiguredWebProxy(SobeesSettings));
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

    #endregion

    #endregion

    #region Lists

    public void UpdateListAds()
    {
    }

    #endregion

    #endregion

    #region Callback

    private void OnGetTrendsAsyncCompleted(ObservableCollection<Entry> entries, string errorMsg)
    {
      try
      {
        ListTrends = entries;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
        ShowErrorMsg(errorMsg);
      }
      IsWaiting = false;
    }

    private void OnGetListAsyncCompleted(IEnumerable<TwitterList> result, string error)
    {
      try
      {
        if (!string.IsNullOrEmpty(error))
        {
          ShowErrorMsg(error);
          return;
        }
        foreach (var list in result)
        {
          TwitterListOwn.Add(list);
        }
      }
      catch (Exception e)
      {
        TraceHelper.Trace(this, e);
      }
    }

    #endregion
  }
}