#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using BUtility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using LinqToTwitter;
using Sobees.Configuration.BGlobals;
using Sobees.Infrastructure.Cls;
using Sobees.Infrastructure.Controls;
using Sobees.Infrastructure.ViewModelBase;
using Sobees.Library.BGenericLib;
using Sobees.Library.BTwitterLib;
using Sobees.Library.BTwitterLib.Cls;
using Sobees.Tools.Logging;
using Sobees.Tools.Threading.Extensions;
using Sobees.Tools.Web;
using User = Sobees.Library.BGenericLib.User;

#endregion

namespace Sobees.ViewModel
{
  #region

  

  #endregion

  public class FirstLaunchControlViewModel : BWorkspaceViewModel
  {
    public ObservableCollection<EnumAccountType> Services { get; set; }

    public FirstLaunchControlViewModel()
    {
      Services = new ObservableCollection<EnumAccountType>
      {
        //EnumAccountType.Facebook,
        //EnumAccountType.LinkedIn,
        EnumAccountType.Twitter,
        EnumAccountType.TwitterSearch
        //EnumAccountType.MySpace,
        //EnumAccountType.Rss,
        //EnumAccountType.NyTimes
      };

      //LoadDefault();
      LoadTrends();
      InitCommands();
      Messenger.Default.Register<BMessage>(this, DoActionMessage);
    }

    #region Command

    public RelayCommand CloseFirstUseCommand { get; set; }

    #endregion

    #region Properties

    public Visibility MaxServicesVisibility
    {
      get
      {
        var i = BViewModelLocator.SobeesViewModelStatic.BServiceWorkspaces1.Count(model => model.IsServiceWorkspace);
        return i == 5 ? Visibility.Visible : Visibility.Collapsed;
      }
    }

    private string _uRL;

    public string URL
    {
      get => _uRL;
        set
      {
        _uRL = value;
        RaisePropertyChanged();
      }
    }

    #endregion

    //#region Facebook

    //#region Fields

    //private bool _isFacebookConnected = true;

    //private ObservableCollection<string> _accountsFacebook;
    //private readonly string FacebookApplicationKey = BGlobals.FACEBOOK_WPF_API;
    ////private readonly string FacebookSecret = BGlobals.FACEBOOK_WPF_SECRET;
    ////private DesktopSession _session;

    //#endregion

    //#region Commands

    ////public RelayCommand ConnectToFBCommand { get; set; }

    ////public RelayCommand<string> OpenFBCommand { get; set; }

    //#endregion

    //#region Properties

    ////public DesktopSession CurrentSession
    ////{
    ////  get
    ////  {
    ////    if (_session != null) return _session;
    ////    var perms = FacebookScopeCollection.GetPermissionList();
    ////   _session = new DesktopSession(FacebookApplicationKey, perms);
    ////    return _session;
    ////  }
    ////}

    //private void fbLoginDlg_Closed(object sender, EventArgs e)
    //{
    //  throw new NotImplementedException();
    //}

    //public bool IsFacebookConnected
    //{
    //  get => _isFacebookConnected;
    //    set
    //  {
    //    _isFacebookConnected = value;
    //    RaisePropertyChanged();
    //  }
    //}

    ////public ObservableCollection<string> AccountsFacebook
    ////{
    ////  get => _accountsFacebook ?? (_accountsFacebook = new ObservableCollection<string>());
    ////    set => _accountsFacebook = value;
    ////}

    //private string _fbUrl = string.Empty;

    //public string FbUrl
    //{
    //  get => _fbUrl;
    //    set
    //  {
    //    _fbUrl = value;
    //    RaisePropertyChanged();
    //  }
    //}

    //private string _liUrl = string.Empty;

    //public string LiUrl
    //{
    //  get => _liUrl;
    //    set
    //  {
    //    _liUrl = value;
    //    RaisePropertyChanged();
    //  }
    //}

    //#region Methods

    ////private void ConnectToFB()
    ////{
    ////  CurrentSession.LoginCompleted += SessionLoginCompleted;
    ////  CurrentSession.Login();
    ////}

    ////private void OpenFB(string account)
    ////{
    ////  foreach (var acc in SobeesSettings.Accounts)
    ////  {
    ////    if (acc.Type != EnumAccountType.Facebook || acc.Login != account) continue;
    ////    Application.Current.Dispatcher.InvokeAsync(() => Messenger.Default.Send(new BMessage("NewAccountFirstUse", acc)));
    ////    return;
    ////  }
    ////}

    //#endregion

    //#region Callback

    //private void SessionLoginCompleted(object sender, AsyncCompletedEventArgs e)
    //{
    //  try
    //  {
    //    if (e.Error == null)
    //    {
    //      //if (_service == null)
    //      //{
    //      //  _service.Api.Users.GetLoggedInUserAsync(GetLoggedInUsersCompleted, null);
    //      //}
    //    }
    //  }
    //  catch (Exception ex)
    //  {
    //    ErrorMsgVisibility = Visibility.Visible;
    //    ErrorMsg = ex.Message;
    //  }
    //}

    //private void GetLoggedInUsersCompleted(long userid, object state, FacebookException e)
    //{
    //  try
    //  {
    //    if (userid != 0)
    //    {
    //      //_service.Api.Users.GetInfoAsync(userid, GetUserCompleted, null);
    //    }
    //  }
    //  catch (Exception ex)
    //  {
    //    ShowHideErrorMsg(ex.Message);
    //  }
    //}

    ///// <summary>
    /////   GetUserCompleted
    ///// </summary>
    ///// <param name="users"></param>
    ///// <param name="state"></param>
    ///// <param name="e"></param>
    //private void GetUserCompleted(IList<User> users, object state, FacebookException e)
    //{
    //  try
    //  {
    //    if (users != null && users.Any())
    //    {
    //      var user = users[0];
    //      var userAccount = new UserAccount(user.Name, EnumAccountType.Facebook) {PictureUrl = user.ProfileImgUrl};
    //      if (!SobeesSettings.Accounts.Contains(userAccount))
    //      {
    //        SobeesSettings.Accounts.Add(userAccount);
    //        Application.Current.Dispatcher.BeginInvokeIfRequired(() => AccountsFacebook.Add(userAccount.Login));
    //        Messenger.Default.Send("AccountAdded");
    //      }
    //      SobeesSettings.Accounts[SobeesSettings.Accounts.IndexOf(userAccount)].AuthToken = CurrentSession.AccessToken;
    //      SobeesSettings.Accounts[SobeesSettings.Accounts.IndexOf(userAccount)].Secret = CurrentSession.SessionSecret;
    //      SobeesSettings.Accounts[SobeesSettings.Accounts.IndexOf(userAccount)].SessionKey = CurrentSession.SessionKey;

    //      SobeesSettings.Accounts[SobeesSettings.Accounts.IndexOf(userAccount)].UserId = Convert.ToInt64(user.Id);

    //      Application.Current.Dispatcher.BeginInvokeIfRequired(() => IsFacebookConnected = false);
    //      Application.Current.Dispatcher.BeginInvokeIfRequired(() => Messenger.Default.Send(new BMessage("NewAccountFirstUse", userAccount)));
    //    }
    //  }
    //  catch (Exception ex)
    //  {
    //    ShowHideErrorMsg(ex.Message);
    //  }
    //}

    //#endregion

    //#endregion

    //#endregion

    #region Twitter

    #region Fields

    private bool _isTwitterConnected;
    private ObservableCollection<string> _accountsTwitter;

    #endregion

    #region Commands

    public RelayCommand<TextBox> LaunchTwitterBrowserForPinCodeCommand { get; set; }

    public RelayCommand<TextBox> CheckTwitterSignInCommand { get; set; }

    public RelayCommand<string> OpenTWCommand { get; set; }

    #endregion

    #region Properties

    private string _loginTwitter;

    public string LoginTwitter
    {
      get => _loginTwitter;
        set
      {
        _loginTwitter = value;
        RaisePropertyChanged();
      }
    }

    public string Pincode { get; set; }

    public bool IsTwitterConnected
    {
      get => _isTwitterConnected;
        set
      {
        _isTwitterConnected = value;
        RaisePropertyChanged();
      }
    }

    public ObservableCollection<string> AccountsTwitter
    {
      get => _accountsTwitter ?? (_accountsTwitter = new ObservableCollection<string>());
        set => _accountsTwitter = value;
    }

    #endregion

    #region Methods

    private PinAuthorizer _mPinAuth;
    private TwitterContext _mTwitterCtx;

    private async void LaunchTwitterBrowserForPinCode(object param)
    {
      _mPinAuth = new PinAuthorizer
      {
        CredentialStore = new InMemoryCredentialStore {ConsumerKey = BGlobals.TWITTER_OAUTH_KEY, ConsumerSecret = BGlobals.TWITTER_OAUTH_SECRET},
        //UseCompression = true,
        GoToTwitterAuthorization = pageLink =>
        {
          BLogManager.LogEntry(APPNAME + "::CheckTwitterSignIn", "pageLink:" + pageLink, true);
          Process.Start(pageLink);
        }
      };

      await _mPinAuth.BeginAuthorizeAsync();
    }

    /// <summary>
    /// </summary>
    /// <param name="param"></param>
    private async void CheckTwitterSignIn(object param)
    {
      var pincodeControl = param as TextBox;
      if (pincodeControl == null) return;
      var pincode = pincodeControl.Text;
      if (string.IsNullOrEmpty(pincode)) return;
      PinCode = pincode;
      IsWaiting = true;

      BLogManager.LogEntry(APPNAME + "CheckTwitterSignIn", $"Pincode:{PinCode}", true);

      await _mPinAuth.CompleteAuthorizeAsync(PinCode);

      var credentials = _mPinAuth.CredentialStore;
      var tokenSecret = credentials.OAuthTokenSecret;
      var token = credentials.OAuthToken;
      _loginTwitter = credentials.ScreenName;

      CheckTwitterCredentialsCompleted(tokenSecret, token);

      BLogManager.LogEntry(APPNAME + "CheckTwitterSignIn:1", $"Token|tokenSecret:{token}|{tokenSecret}", true);

      //using (var worker = new BackgroundWorker())
      //{
      //  worker.DoWork += delegate(object s, DoWorkEventArgs args)
      //                   {
      //                     if (worker.CancellationPending)
      //                     {
      //                       args.Cancel = true;
      //                       return;
      //                     }

      //                     CheckTwitterCredentialsCompleted(tokenSecret, token);
      //                   };
      //  worker.RunWorkerAsync();
      //}
    }

    /// <summary>
    ///   CheckTwitterCredentialsCompleted
    /// </summary>
    /// <param name="result"></param>
    /// <param name="token"></param>
    private void CheckTwitterCredentialsCompleted(string result, string token)
    {
      IsWaiting = false;

      if (string.IsNullOrEmpty(result) || string.IsNullOrEmpty(token))
      {
        ShowHideErrorMsg("Wrong Username/Email and password combination");
      }

      try
      {
        var account = new UserAccount(LoginTwitter, EnumAccountType.Twitter) {Secret = result, SessionKey = token, AuthToken = token};
        if (!SobeesSettings.Accounts.Contains(account))
        {
          SobeesSettings.Accounts.Add(account);
          Messenger.Default.Send("AccountAdded");
          AccountsTwitter.Add(account.Login);
        }

        IsTwitterConnected = true;
        LoginTwitter = null;
        Messenger.Default.Send(new BMessage("NewAccountFirstUse", account));
        ShowHideErrorMsg();
      }
      catch (Exception ex)
      {
        BLogManager.LogEntry(APPNAME + "::CredantialsViewModel -> ", ex);
      }
    }

    private void OpenTW(string account)
    {
      foreach (var acc in SobeesSettings.Accounts)
      {
        if (acc.Type == EnumAccountType.Twitter && acc.Login == account)
        {
          using (var worker = new BackgroundWorker())
          {
            string errorMsg = null;
            worker.DoWork += delegate(object s, DoWorkEventArgs args)
            {
              if (worker.CancellationPending)
              {
                args.Cancel = true;
                return;
              }
              TwitterLibV11.GetDirectMessages(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET, acc.SessionKey, acc.Secret, 1, out errorMsg,
                ProxyHelper.GetConfiguredWebProxy(SobeesSettings));
            };

            worker.RunWorkerCompleted += delegate
            {
              IsWaiting = false;
              if (string.IsNullOrEmpty(errorMsg))
              {
                Application.Current.Dispatcher.BeginInvokeIfRequired(() => Messenger.Default.Send(new BMessage("NewAccountFirstUse", acc)));
              }
              else
              {
                ErrorMsgVisibility = Visibility.Visible;
                ErrorMsg = $"{acc.Login}, please your credentials again. Your Session isn't anymore valid. ";
              }
            };

            worker.RunWorkerAsync();
          }
          return;
        }
      }
    }

    #endregion

    #endregion

    #region LinkedIn

    #region Fields

    private bool _isLinkedInConnected;


    private Visibility _waitingCodeLiVisibility = Visibility.Collapsed;
    private ObservableCollection<string> _accountsLinkedIn;

    #endregion

    #region Commands

    public RelayCommand ConnectToLICommand { get; set; }

    public RelayCommand<string> EnterSecurityCodeCommand { get; set; }

    public RelayCommand<string> OpenLICommand { get; set; }

    #endregion

    #region Properties

    public Visibility WaitingCodeLiVisibility
    {
      get => _waitingCodeLiVisibility;
        set
      {
        _waitingCodeLiVisibility = value;
        RaisePropertyChanged();
      }
    }

    public bool IsLinkedInConnected
    {
      get => _isLinkedInConnected;
        set
      {
        _isLinkedInConnected = value;
        RaisePropertyChanged();
      }
    }

    public ObservableCollection<string> AccountsLinkedIn
    {
      get => _accountsLinkedIn ?? (_accountsLinkedIn = new ObservableCollection<string>());
        set => _accountsLinkedIn = value;
    }

    #endregion

    #region Methods

    private void OpenLI(string account)
    {
      foreach (var acc in SobeesSettings.Accounts)
      {
        if (acc.Type == EnumAccountType.LinkedIn && acc.Login == account)
        {
          Application.Current.Dispatcher.BeginInvokeIfRequired(() => Messenger.Default.Send(new BMessage("NewAccountFirstUse", acc)));
          return;
        }
      }
    }

    //private IConsumerTokenManager TokenManager
    //{
    //  get
    //  {
    //    if (this._tokenManager == null)
    //    {
    //      string consumerKey = BGlobals.LINKEDIN_WPF_KEY;
    //      string consumerSecret = BGlobals.LINKEDIN_WPF_SECRET;
    //      _tokenManager = new WindowsCredentialStoreTokenManager(consumerKey, consumerSecret);
    //    }

    //    return this._tokenManager;
    //  }
    //}
    //private IConsumerTokenManager _tokenManager;
    //private DesktopOAuthAuthorization _authorization;
    //private DesktopOAuthAuthorization Authorization
    //{
    //  get
    //  {
    //    if (this._authorization == null)
    //    {
    //      string OAuthSecret = string.Empty; // this can be retrieve from your own storage solution

    //      this._authorization = new DesktopOAuthAuthorization(TokenManager, OAuthSecret);
    //      //this.authorization.GetVerifier = GetVerifier;
    //     // OAuthSecret = this.authorization.Authorize();

    //      // Store OAuthSecret for further use
    //    }

    //    return this._authorization;
    //  }
    //}

    //private void ConnectToLI()
    //{
    //  if (_linkedInLibV2 == null)
    //    _linkedInLibV2 = new OAuthLinkedInV2(BGlobals.LINKEDIN_WPF_KEY, BGlobals.LINKEDIN_WPF_SECRET);

    //  WaitingCodeLiVisibility = Visibility.Visible;
    //  using (var worker = new BackgroundWorker())
    //  {
    //    var result = "";
    //    worker.DoWork += delegate(object s, DoWorkEventArgs args)
    //    {
    //      if (worker.CancellationPending)
    //      {
    //        args.Cancel = true;
    //        return;
    //      }

    //      result = _linkedInLibV2.AuthorizationLinkGet();

    //      worker.RunWorkerCompleted += delegate { WebHelper.NavigateToUrl(result); };
    //    };
    //    worker.RunWorkerAsync();
    //  }
    //}

    //private void EnterSecurityCode(string txt)
    //{
    //  try
    //  {
    //    using (var worker = new BackgroundWorker())
    //    {
    //      var result = "";
    //      worker.DoWork += delegate(object s, DoWorkEventArgs args)
    //      {
    //        if (worker.CancellationPending)
    //        {
    //          args.Cancel = true;
    //          return;
    //        }

    //        PinCode = txt;
    //        _linkedInLibV2.Verifier = PinCode;
    //        result = _linkedInLibV2.AccessTokenGet(_linkedInLibV2.Token);
    //      };

    //      worker.RunWorkerCompleted += delegate { OnAuthCompleted(result); };
    //      worker.RunWorkerAsync();
    //    }
    //  }
    //  catch (Exception ex)
    //  {
    //    TraceHelper.Trace(this, ex);
    //  }
    //}

    public string PinCode { get; set; }

    //private void OnAuthCompleted(string result)
    //{
    //  var user = _linkedInLibV2.GetCurrentProfile();
    //  var response = _linkedInLibV2.ApiWebRequest("GET", "https://api.linkedin.com/v1/people/~", null);
    //  BLogManager.LogEntry(APPNAME + "::onAuthCompleted:Response:", response, true);

    //  try
    //  {
    //    if (user != null)
    //    {
    //      var account = new UserAccount
    //      {
    //        Type = EnumAccountType.LinkedIn,
    //        Secret = _linkedInLibV2.TokenSecret,
    //        SessionKey = _linkedInLibV2.Token,
    //        Login = user.NickName,
    //        PictureUrl = user.ProfileImgUrl
    //      };
    //      if (SobeesSettings.Accounts.Any(userAccount => userAccount.Login == account.Login && userAccount.Type == account.Type))
    //      {
    //        IsLinkedInConnected = true;
    //        Messenger.Default.Send(new BMessage("NewAccountFirstUse", account));
    //        WaitingCodeLiVisibility = Visibility.Collapsed;
    //        return;
    //      }
    //      SobeesSettings.Accounts.Add(account);
    //      Messenger.Default.Send("AccountAdded");
    //      WaitingCodeLiVisibility = Visibility.Collapsed;

    //      AccountsLinkedIn.Add(account.Login);
    //      IsLinkedInConnected = true;
    //      Messenger.Default.Send(new BMessage("NewAccountFirstUse", account));
    //    }
    //  }
    //  catch (Exception ex)
    //  {
    //    TraceHelper.Trace("CredantialsViewModel -> ", ex);
    //  }
    //}

    #region Callback Silverlight

    #endregion

    #endregion

    #endregion

    #region MySpace

    #region Fields

    #endregion







    #endregion

    #region Search

    #region Fields

    private ObservableCollection<Entry> _listTrends;
    private string _stringSearch;

    #endregion

    #region Commands

    public RelayCommand SaveKeywordsCommand { get; set; }

    public RelayCommand<string> PostSearchTrendsCommand { get; set; }

    #endregion

    #region Properties

    public string StringSearch
    {
      get => _stringSearch;
        set
      {
        _stringSearch = value;
        RaisePropertyChanged();
      }
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

    #endregion

    #region Methods

    private void Search()
    {
      var search =
        BViewModelLocator.SobeesViewModelStatic.BServiceWorkspaces1.Where(service => service.BServiceWorkspace != null)
          .FirstOrDefault(service => service.BServiceWorkspace.ClassName == "TwitterSearchViewModel");
      if (search == null)
      {
        Messenger.Default.Send(new BMessage("NewAccountFirstUse", new UserAccount(StringSearch, EnumAccountType.TwitterSearch)));
      }
      else
      {
        search.SetAccount(new UserAccount(StringSearch));
        Messenger.Default.Send("SaveSettings");
      }
      StringSearch = "";
    }

    private void OpenTrend(string search)
    {
      StringSearch = search;
      Search();
    }

    private async void LoadTrends()
    {
      IsWaiting = true;
      try
      {
        string msg = null;
        var dispatcher = Dispatcher.CurrentDispatcher;

        ObservableCollection<Entry> newEntries = null;

        await Task.Factory.StartNew(() =>
        {
          var entries = TwitterLib.SearchTrends(BTwitterLibGlobal.TESTUSER1015_ACCESS_TOKEN, BTwitterLibGlobal.TESTUSER1015_ACCESS_TOKEN_SECRET,
            out msg, BGlobals.TWITTER_TREND_NUMBER, ProxyHelper.GetConfiguredWebProxy(SobeesSettings));
          newEntries = new ObservableCollection<Entry>(entries);
        });
        await dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() => ListTrends = newEntries));
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
      finally
      {
        IsWaiting = false;
      }
    }

    #endregion

    #endregion

    #region Rss

    #region Fields

    private string _urlRss;
    private string errorMsg;
    private const string APPNAME = "FirtLaunchControlViewModel";

    #endregion

    #region Commands

    public RelayCommand OpenRssUrlCommand { get; set; }
    public RelayCommand<string> OpenRssCommand { get; set; }

    #endregion

    #region Properties

    public string UrlRss
    {
      get => _urlRss;
        set
      {
        _urlRss = value;
        RaisePropertyChanged();
      }
    }

    #endregion

    #region Methods

    private void OpenRssUrl()
    {
      try
      {
        var rss =
          BViewModelLocator.SobeesViewModelStatic.BServiceWorkspaces1.Where(service => service.BServiceWorkspace != null)
            .FirstOrDefault(service => service.BServiceWorkspace.ClassName == "RssViewModel");
        if (rss == null)
        {
          Messenger.Default.Send(new BMessage("NewAccountFirstUse", new UserAccount(UrlRss, EnumAccountType.Rss)));
        }

        else
        {
          rss.SetAccount(new UserAccount(UrlRss));
          Messenger.Default.Send("SaveSettings");
        }
        UrlRss = "";
      }
      catch (Exception e)
      {
        TraceHelper.Trace(this, e);
      }
    }

    private void OpenRssUrl(string url)
    {
      try
      {
        var rss =
          BViewModelLocator.SobeesViewModelStatic.BServiceWorkspaces1.Where(service => service.BServiceWorkspace != null)
            .FirstOrDefault(service => service.BServiceWorkspace.ClassName == "RssViewModel");
        if (rss == null)
        {
          Messenger.Default.Send(new BMessage("NewAccountFirstUse", new UserAccount(url, EnumAccountType.Rss)));
        }
        else
        {
          rss.SetAccount(new UserAccount(url));
          Messenger.Default.Send("SaveSettings");
        }
      }
      catch (Exception e)
      {
        TraceHelper.Trace(this, e);
      }
    }

    #endregion

    #endregion

    //#region NY-Times

    //#region Commands

    //public RelayCommand OpenNYTimesCommand { get; set; }

    //#endregion

    #region Methods

    public void OpenNYTimes()
    {
      if (
        BViewModelLocator.SobeesViewModelStatic.BServiceWorkspaces1.Where(service => service.BServiceWorkspace != null)
          .Any(service => service.BServiceWorkspace.ClassName == "NyTimesViewModel"))
      {
        return;
      }

      Messenger.Default.Send(new BMessage("NewAccountFirstUse", new UserAccount {Type = EnumAccountType.NyTimes}));
    }

    #endregion

    #region Methods

    protected override void InitCommands()
    {
      //ConnectToFBCommand = new RelayCommand(ConnectToFB);
      //OpenFBCommand = new RelayCommand<string>(OpenFB);
      //ConnectToLICommand = new RelayCommand(ConnectToLI);
      //OpenLICommand = new RelayCommand<string>(OpenLI);
      OpenTWCommand = new RelayCommand<string>(OpenTW);
      CloseFirstUseCommand = new RelayCommand(() => Messenger.Default.Send("CloseFirstUse"));
      //EnterSecurityCodeCommand = new RelayCommand<string>(EnterSecurityCode);
      LaunchTwitterBrowserForPinCodeCommand = new RelayCommand<TextBox>(LaunchTwitterBrowserForPinCode);
      CheckTwitterSignInCommand = new RelayCommand<TextBox>(CheckTwitterSignIn);
      PostSearchTrendsCommand = new RelayCommand<string>(OpenTrend);
      SaveKeywordsCommand = new RelayCommand(Search);
      OpenRssUrlCommand = new RelayCommand(OpenRssUrl, CanOpenRssUrl);
      OpenRssCommand = new RelayCommand<string>(OpenRssUrl);
      //OpenNYTimesCommand = new RelayCommand(OpenNYTimes);
      base.InitCommands();
    }

    private bool CanOpenRssUrl()
    {
      return HyperLinkHelper.IsHyperlink(UrlRss);
    }

    public override void DoActionMessage(BMessage obj)
    {
      switch (obj.Action)
      {
        case "NewAccountFirstUse":
          RaisePropertyChanged(()=> MaxServicesVisibility);
          break;
      }
      base.DoActionMessage(obj);
    }

    private void LoadDefault()
    {
      foreach (var account in SobeesSettings.Accounts)
      {
        switch (account.Type)
        {
          //case EnumAccountType.Facebook:
          //  AccountsFacebook.Add(account.Login);
          //  IsFacebookConnected = false;
          //  break;

          case EnumAccountType.Twitter:
            AccountsTwitter.Add(account.Login);
            IsTwitterConnected = true;
            break;

          case EnumAccountType.LinkedIn:
            AccountsLinkedIn.Add(account.Login);
            IsLinkedInConnected = true;
            break;
        }
      }

      //var tab = FacebookScopeCollection.GetPermissionList();
      //_session = new DesktopSession(FacebookApplicationKey, tab);
    }

    #endregion

    #region Utilities

    #endregion
  }
}