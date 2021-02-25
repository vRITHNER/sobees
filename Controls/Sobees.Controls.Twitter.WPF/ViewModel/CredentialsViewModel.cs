#region

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Xml;
using BUtility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using LinqToTwitter;
using Sobees.Controls.Twitter.Cls;
using Sobees.Configuration.BGlobals;
using Sobees.Infrastructure.Cls;
using Sobees.Infrastructure.Commands;
using Sobees.Library.BTwitterLib;
using Sobees.Tools.Crypto;
using Sobees.Tools.Logging;
using Sobees.Tools.Threading.Extensions;

#endregion

namespace Sobees.Controls.Twitter.ViewModel
{
  public class CredentialsViewModel : BTwitterViewModel
  {
    #region Fields

    private BRelayCommand _cbxSelectionChangedCommand;
    private bool _isActiv;
    private bool _isStoreAccount;
    private string _login;
    private RelayCommand _loginCommand;
    private RelayCommand _loginCommandBrowser;
    private BRelayCommand _loginOAuthCommand;
    private string _password;
    private string _passwordHash;
    private int _selectedIndex = -1;
    private int _selectedIndexOAuth;
    private string _uRL;

    #endregion

    #region Properties

    private const string APPNAME = "CredentialViewModel";
    private ObservableCollection<UserAccount> _accountsOAuth;

    public override DataTemplate DataTemplateView
    {
      get
      {
        var dt =
          "<DataTemplate x:Name='dtCredentialsView' " +
          "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
          "xmlns:Views='clr-namespace:Sobees.Controls.Twitter.Views;assembly=Sobees.Controls.Twitter' " +
          "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >" +
          "<Views:Credentials/> " +
          "</DataTemplate>";

        var stringReader = new StringReader(dt);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
      set { base.DataTemplateView = value; }
    }

    public ObservableCollection<UserAccount> Accounts { get; set; }

    public ObservableCollection<UserAccount> AccountsOAuth
    {
      get { return _accountsOAuth ?? (_accountsOAuth = new ObservableCollection<UserAccount>()); }
      set { _accountsOAuth = value; }
    }

    public string Login
    {
      get { return _login; }
      set
      {
        _login = value;
        RaisePropertyChanged("Login");
        RaisePropertyChanged("IsActiv");
      }
    }

    public string URL
    {
      get { return _uRL; }
      set
      {
        _uRL = value;
        RaisePropertyChanged("URL");
      }
    }

    public string Password
    {
      get { return _password; }
      set
      {
        _password = value;
        RaisePropertyChanged("Password");
        RaisePropertyChanged("IsActiv");
      }
    }

    public int SelectedIndex
    {
      set
      {
        _selectedIndex = value;
        RaisePropertyChanged("SelectedIndex");
      }
      get { return _selectedIndex; }
    }

    public Visibility OAuthAccountVisibility => AccountsOAuth.Count > 0 ? Visibility.Visible : Visibility.Collapsed;

    public bool IsActiv
    {
      set
      {
        _isActiv = value;
        RaisePropertyChanged("IsActiv");
      }
      get { return _isActiv; }
    }

    public bool OAuthLoginEnabled
    {
      get
      {
        if (SelectedIndexOAuth > -1)
        {
          return true;
        }
        return false;
      }
    }

    public int SelectedIndexOAuth
    {
      set
      {
        _selectedIndexOAuth = value;
        RaisePropertyChanged("SelectedIndexOAuth");
        RaisePropertyChanged("OAuthLoginEnabled");
      }
      get
      {
        if (AccountsOAuth.Count == 0)
        {
          _selectedIndexOAuth = -1;
        }
        return _selectedIndexOAuth;
      }
    }

    public bool IsStoreAccount
    {
      set
      {
        _isStoreAccount = value;
        RaisePropertyChanged("IsStoreAccount");
      }
      get { return _isStoreAccount; }
    }

    #endregion

    #region Commands

    public BRelayCommand CbxSelectionChangedCommand
    {
      get
      {
        try
        {
          return _cbxSelectionChangedCommand ?? (_cbxSelectionChangedCommand = new BRelayCommand(CbxSelectionChanged));
        }
        catch (Exception ex)
        {
          TraceHelper.Trace(this,
                            ex);
        }
        return null;
      }
    }

    public RelayCommand<TextBox> LoginCommand { get; set; }

    public RelayCommand<TextBox> LaunchTwitterBrowserForPinCodeCommand { get; set; }

    public RelayCommand<TextBox> CheckTwitterSignInCommand { get; set; }

    public RelayCommand<UserAccount> LoginOAuthOldCommand { get; set; }

    public BRelayCommand LoginOAuthCommand
    {
      get
      {
        try
        {
          return _loginOAuthCommand ?? (_loginOAuthCommand = new BRelayCommand(LoginOAuth));
        }
        catch (Exception ex)
        {
          TraceHelper.Trace(this,
                            ex);
        }
        return null;
      }
    }

    #endregion

    #region Constructors

    public CredentialsViewModel(TwitterViewModel model, Messenger messenger)
      : base(model, messenger)
    {
      Accounts = new ObservableCollection<UserAccount>();
      foreach (var account in SobeesSettings.Accounts.Where(account => account.Type == EnumAccountType.Twitter))
      {
        Accounts.Add(account);
        if (!string.IsNullOrEmpty(account.Secret))
        {
          AccountsOAuth.Add(account);
        }
      }
      RaisePropertyChanged("OAuthAccountVisibility");

      if (Settings != null && !string.IsNullOrEmpty(Settings.UserName))
      {
        CheckLoginWithParam(Settings.UserName);
      }
      InitCommands();
    }

    #endregion

    #region Commands

    private bool CanRequestLogin(TextBox textBox)
    {
      try
      {
        return !String.IsNullOrEmpty(Login);
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
        return false;
      }
    }

    #endregion

    #region Methods

    private string _loginTwitter = string.Empty;
    private string _pincode = string.Empty;
    private PinAuthorizer _mPinAuth;
    private TwitterContext _mTwitterCtx;

    protected override void InitCommands()
    {
      CloseCommand = new RelayCommand(() => _twitterViewModel.CloseControl());
      LoginCommand = new RelayCommand<TextBox>(CheckTwitterSignIn, CanRequestLogin);
      LaunchTwitterBrowserForPinCodeCommand = new RelayCommand<TextBox>(LaunchTwitterBrowserForPinCode);
      CheckTwitterSignInCommand = new RelayCommand<TextBox>(CheckTwitterSignIn);
      LoginOAuthOldCommand = new RelayCommand<UserAccount>(LoginOAuthOld);
      base.InitCommands();
    }

    public void UseStoreAccount(UserAccount account)
    {
      try
      {
        if (account == null) throw new NullReferenceException("'account' parameter was NULL!");

        IsWaiting = true;

        //string token = null;
        // Check credentials
        using (var worker = new BackgroundWorker())
        {
          worker.DoWork += delegate
                             {
                               if (!string.IsNullOrEmpty(account.PasswordHash))
                               {
                                 string secret;
                                 try
                                 {
                                   var key = EncryptionHelper.GetHashKey(BGlobals.CIPHER_KEY);
                                   var pwd = EncryptionHelper.Decrypt(key,
                                                                      account.PasswordHash);
                                   var token = TwitterLibV11.ConnectXAuth(BGlobals.TWITTER_OAUTH_KEY,
                                                                          BGlobals.TWITTER_OAUTH_SECRET, account.Login,
                                                                          account.AuthToken, account.Secret,
                                                                          ProxyHelper.GetConfiguredWebProxy(
                                                                            SobeesSettings));

                                   Application.Current.Dispatcher.BeginInvokeIfRequired(() =>
                                                                                          {
                                                                                            if (
                                                                                              !string.IsNullOrEmpty(
                                                                                                token))
                                                                                            {
                                                                                              account.PasswordHash =
                                                                                                string.Empty;

                                                                                              //account.Secret = secret;
                                                                                              account.SessionKey = token;
                                                                                              MessengerInstance.Send(
                                                                                                "ConnectedStored");
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                              ErrorMsgVisibility =
                                                                                                Visibility.Visible;
                                                                                              ErrorMsg = string.Format("{0}, please your credentials again. Your Session isn't anymore valid. ", account.Login);
                                                                                            }
                                                                                          });
                                 }
                                 catch (Exception e)
                                 {
                                   Application.Current.Dispatcher.BeginInvokeIfRequired(() =>
                                                                                          {
                                                                                            ErrorMsgVisibility =
                                                                                              Visibility.Visible;

                                                                                            ErrorMsg = account.Login +
                                                                                                       ", please your credentials again. Your Session isn't anymore valid. ";
                                                                                          });
                                   Console.WriteLine(e);
                                 }
                               }
                               else
                               {
                                 string errorMsg;
                                 TwitterLibV11.GetDirectMessages(BGlobals.TWITTER_OAUTH_KEY,
                                                                 BGlobals.TWITTER_OAUTH_SECRET,
                                                                 account.SessionKey, account.Secret, 1, out errorMsg,
                                                                 ProxyHelper.GetConfiguredWebProxy(SobeesSettings));
                                 Application.Current.Dispatcher.BeginInvokeIfRequired(() =>
                                                                                        {
                                                                                          if (
                                                                                            string.IsNullOrEmpty(
                                                                                              errorMsg))
                                                                                          {
                                                                                            MessengerInstance.Send(
                                                                                              "ConnectedStored");
                                                                                          }
                                                                                          else
                                                                                          {
                                                                                            ErrorMsg = errorMsg;
                                                                                            AccountsOAuth.Remove(account);
                                                                                          }
                                                                                        })
                                   ;
                               }
                               Application.Current.Dispatcher.BeginInvokeIfRequired(() => { IsWaiting = false; });
                             };

          worker.RunWorkerAsync();
        }
      }
      catch (Exception ex)
      {
        IsWaiting = false;
        TraceHelper.Trace(this, ex);
      }
    }

    public void CheckLoginWithParam(object login)
    {
      try
      {
        var account = new UserAccount(login.ToString(), EnumAccountType.Twitter);

        //An existing account
        if (SobeesSettings.Accounts.Contains(account))
        {
          UseStoreAccount(SobeesSettings.Accounts[SobeesSettings.Accounts.IndexOf(account)]);
        }
      }
      catch (Exception ex)
      {
        IsWaiting = false;
        TraceHelper.Trace(this, ex);
      }
    }

    public void CbxSelectionChanged(object param)
    {
      var objs = BRelayCommand.CheckParams(param);
      if (objs == null) return;
      var cbx = objs[1] as ComboBox;
      if (cbx != null)
      {
        var accout = cbx.SelectedItem as UserAccount;
        if (accout != null) Login = accout.Login;
      }
      Password = "password";
      IsActiv = true;
    }

    /// <summary>
    /// LaunchTwitterBrowserForPinCode
    /// </summary>
    /// <param name="param"></param>
    private async void LaunchTwitterBrowserForPinCode(object param)
    {
      _mPinAuth = new PinAuthorizer
                    {
                      CredentialStore = 
                        new InMemoryCredentialStore()
                          {
                            ConsumerKey = BGlobals.TWITTER_OAUTH_KEY,
                            ConsumerSecret = BGlobals.TWITTER_OAUTH_SECRET
                          },
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
    /// CheckTwitterSignIn
    /// </summary>
    /// <param name="param"></param>
    private async void CheckTwitterSignIn(object param)
    {
      var pincodeControl = param as TextBox;
      if (pincodeControl == null) return;
      var pincode = pincodeControl.Text;
      if (string.IsNullOrEmpty(pincode)) return;
      _pincode = pincode;
      IsWaiting = true;

      BLogManager.LogEntry(APPNAME + "CheckTwitterSignIn", string.Format("Pincode:{0}", _pincode), true);

      await _mPinAuth.CompleteAuthorizeAsync(_pincode); 
      
      var credentials = _mPinAuth.CredentialStore;
      var tokenSecret = credentials.OAuthTokenSecret;
      var token = credentials.OAuthToken;
      _loginTwitter = credentials.ScreenName;

      CheckTwitterCredentialsCompleted(tokenSecret, token);

    }


    private void CheckTwitterCredentialsCompleted(string result, string token)
    {
      IsWaiting = false;

      if (string.IsNullOrEmpty(result) || string.IsNullOrEmpty(token))
      {
        ShowHideErrorMsg("Wrong Username/Email and password combination");
      }

      try
      {
        var account = new UserAccount(_loginTwitter, EnumAccountType.Twitter)
                        {
                          Secret = result,
                          SessionKey = token,
                          AuthToken = token,
                        };
        if (!SobeesSettings.Accounts.Contains(account))
        {
          Messenger.Default.Send("AccountAdded");
          SobeesSettings.Accounts.Add(account);
          Settings.UserName = account.Login;
          MessengerInstance.Send("Connected");
        }
        else
        {
          var currentaccount = SobeesSettings.Accounts[SobeesSettings.Accounts.IndexOf(account)];
          currentaccount.Secret = result;
          currentaccount.AuthToken = token;
          currentaccount.SessionKey = token;
          UseStoreAccount(SobeesSettings.Accounts[SobeesSettings.Accounts.IndexOf(account)]);
          MessengerInstance.Send("Connected");
        }
      }
      catch (Exception ex)
      {
        BLogManager.LogEntry(APPNAME + "::CredantialsViewModel -> ", ex);
      }
    }

    private void LoginOAuth(object obj)
    {
    }

    #endregion

    private void LoginOAuthOld(UserAccount obj)
    {
      Settings.UserName = obj.Login;
      IsWaiting = true;
      string errorMsg = null;

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
                             TwitterLibV11.GetDirectMessages(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET,obj.SessionKey, obj.Secret, 1, out errorMsg,ProxyHelper.GetConfiguredWebProxy(SobeesSettings));
                           };

        worker.RunWorkerCompleted += delegate
                                       {
                                         IsWaiting = false;
                                         if (string.IsNullOrEmpty(errorMsg))
                                         {
                                           MessengerInstance.Send("Connected");
                                         }
                                         else
                                         {
                                           ErrorMsgVisibility = Visibility.Visible;
                                           ErrorMsg = string.Format("{0}, please sign in again ; your credentials again. Your Session isn't anymore valid. ", obj.Login);
                                         }
                                       };

        worker.RunWorkerAsync();
      }
    }
  }
}