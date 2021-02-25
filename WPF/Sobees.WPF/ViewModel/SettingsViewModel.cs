#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using Sobees.Configuration.BGlobals;
using Sobees.Infrastructure.Cls;
using Sobees.Infrastructure.Commands;
using Sobees.Infrastructure.Controls;
using Sobees.Infrastructure.Mail;
using Sobees.Infrastructure.ViewModelBase;
using Sobees.Library.BGoogleLib.Language;
using Sobees.Library.BLocalizeLib;
using Sobees.Library.BTwitterLib;
using Sobees.Settings;
using Sobees.Themes;
using Sobees.Tools.Crypto;
using Sobees.Tools.Helpers;
using Sobees.Tools.Logging;
using Sobees.Tools.Theme;
using Sobees.Tools.Threading;
using Sobees.Tools.Util;

#endregion

namespace Sobees.ViewModel
{
  public class SettingsViewModel : BSettingsViewModel
  {
    #region Fields

    #region MenuSelection

    private string _settingsTitleSelectedText;
    private bool _isExpandedGeneral = true;
    private bool _isExpandedAccount;
    private bool _isExpandedSync;
    private bool _isExpandedServices;
    private bool _isExpandedAdvanced;
    private bool _isExpandedSupport;
    private int _selectedIndexGeneral;
    private int _selectedIndexAccounts = -1;
    private int _selectedIndexServices;
    private int _selectedIndexAdvanced;
    private int _selectedIndexSupport;

    #endregion

    #region Tab General

    #region General Settings

    private BLanguage _selectedLanguage;
#if SILVERLIGHT
        private string _themeSelectedItem = null;
#else
    private int _themeSelectedIndex = -1;
#endif
    private int _fontSizeValue;
    private bool _showGlobalFilter;
    private bool _disableAds;
    private bool _runAtStartup;
    private bool _alertsEnabled = true;
    private bool _minizeWindowInTray;
    private bool _viewState;

    #endregion

    #endregion

    #region Tab Advanced

    #region Proxy

    private bool _useProxyServer;
    private string _proxyServerName;
    private int _proxyPort;
    private string _proxyUserName;
    private string _proxyPassword;
    private string _proxyUserDomain;

    #endregion

    #region Support

    private string _logFrom = string.Empty;
    private string _logSubject = BGlobals.SUPPORTLOG_DEFAULT_SUBJECT;
    private string _logDescription = string.Empty;
    private string _logAttachment = string.Empty;
    private ImageSource _supportLogCapture;
    private bool _bisSupportLogMailValid;
    private bool _isSupportLogImageSendWithLog = true;

    #endregion

    #endregion

    #region Services

    private bool _closeBoxPublication;
    private bool _isSendByEnter;
    private int _uRLShortenerIndex = -1;
    private ObservableCollection<string> _uRLShorteners = new ObservableCollection<string>();

    #region bitLy

    private string _bitLyUserName;
    private string _bitLyPassword;
    private Visibility _isBitLyChoice = Visibility.Collapsed;

    #endregion

    #endregion

    #region Tab Account

    #region All

    #region Spam

    private string _newSpam;
    private ObservableCollection<string> _spamList = new ObservableCollection<string>();

    #endregion

    #region Alertes

    private bool _isCheckedFBAll;
    private bool _isCheckedFBNo;
    private bool _isCheckedFBAdvanced;
    private string _alertWord;
    private string _alertRemovedWord;
    private string _alertUser;
    private ObservableCollection<string> _alertWordList = new ObservableCollection<string>();
    private ObservableCollection<string> _alertRemovedWordList = new ObservableCollection<string>();
    private ObservableCollection<string> _alertUserList = new ObservableCollection<string>();
    private EnumAlertsFacebookType _alertsFacebookType;
    private Visibility _advancedAlertsSettingsVisibility;
    private bool _isCheckedUseAlertsWords;
    private bool _isCheckedUseAlertsUsers;
    private bool _isCheckedUseAlertsRemovedWords;
    private bool _isCheckedUseAlertsDM;
    private bool _isCheckedUseAlertsReply;
    private bool _isCheckedUseAlertsGroups;
    private bool _isCheckedUseAlertsFriends;

    #endregion

    #endregion

    #endregion

    #endregion

    #region Constructor

    public SettingsViewModel(Messenger messenger)
      : base(messenger)
    {
      if (IsInDesignMode)
      {
      }
      LoadCurrentValue();
      InitCommands();
    }

    public SettingsViewModel()
      : base(Messenger.Default)
    {
      if (IsInDesignMode)
      {
      }

      InitCommands();
      LoadCurrentValue();
    }

    #endregion

    #region Properties

    public SettingsViewType CurrentView { get; set; }

    public ObservableCollection<UserAccount> Accounts { get; set; }

    #region View

    //private SyncViewModel _syncViewModel;

    //public SyncViewModel SyncViewModel
    //{
    //  get { return _syncViewModel ?? (_syncViewModel = new SyncViewModel { IsMaxiMize = true }); }
    //  set { _syncViewModel = value; }
    //}
    public DataTemplate View
    {
      get
      {
        RaisePropertyChanged("SettingsTitleSelectedText");
        var view = string.Empty;
        switch (CurrentView)
        {
          case SettingsViewType.General:
            view = "UcGeneral";
            break;
          case SettingsViewType.Accounts:
            if (SelectedIndexAccounts > -1 && Accounts.Count > 0)
            {
              switch (Accounts[SelectedIndexAccounts].Type)
              {
                case EnumAccountType.Twitter:
                  view = "UcTwitter";
                  break;
                case EnumAccountType.Facebook:
                  view = "UcFacebook";
                  break;
                case EnumAccountType.TwitterSearch:
                  break;
                //case EnumAccountType.MySpace:
                //  view = "UcMySpace";
                //  break;
                case EnumAccountType.LinkedIn:
                  view = "UcLinkedIn";
                  break;
                default:
                  throw new ArgumentOutOfRangeException();
              }
            }
            else
            {
              view = "UcNoAccounts";
            }
            break;
          case SettingsViewType.Services:
            view = "UcServices";
            break;
          case SettingsViewType.AdvancedProxy:
#if SILVERLIGHT
            view = "UcAdvancedOthers";
#else
            switch (SelectedIndexAdvanced)
            {
              case 1:
                view = "UcAdvancedOthers";
                break;
              default:
                view = "UcAdvancedProxy";
                break;
            }
#endif
            break;
          case SettingsViewType.Support:
            switch (SelectedIndexSupport)
            {
              case 0:
                view = "UcAdvancedSupport";
                break;
              default:
                view = "UcAdvancedSupport";
                break;
            }
            break;
          case SettingsViewType.Sync:
            view = "UcSync";
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
        var dt =
          string.Format(
            "<DataTemplate x:Name='dtMainView' xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:Views='clr-namespace:Sobees.Settings.Views;assembly=Sobees' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' ><Views:{0}/> </DataTemplate>",
            view);

#if SILVERLIGHT
        return XamlReader.Load(dt) as DataTemplate;
#else
        var stringReader = new StringReader(dt);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
#endif
      }
    }

    #endregion

    #region MenuSelection

    public string SettingsTitleSelectedText
    {
      get
      {
#if !SILVERLIGHT
        switch (CurrentView)
        {
          case SettingsViewType.General:
            return new LocText("Sobees.Configuration.BGlobals:Resources:tabItemGeneral").ResolveLocalizedValue();

          case SettingsViewType.Accounts:
            return new LocText("Sobees.Configuration.BGlobals:Resources:tabCredentials").ResolveLocalizedValue();
          case SettingsViewType.Services:
            return new LocText("Sobees.Configuration.BGlobals:Resources:tabItemServices").ResolveLocalizedValue();
          case SettingsViewType.AdvancedProxy:
            return new LocText("Sobees.Configuration.BGlobals:Resources:tabItemProxy").ResolveLocalizedValue();
          case SettingsViewType.Support:
            return new LocText("Sobees.Configuration.BGlobals:Resources:tabItemSupport").ResolveLocalizedValue();
          case SettingsViewType.Sync:
            return new LocText("Sobees.Configuration.BGlobals:Resources:tabItemSync").ResolveLocalizedValue();
          default:
            throw new ArgumentOutOfRangeException();
        }
#else
        switch (CurrentView)
        {
          case SettingsViewType.General:
            return LocalizationManager.GetString("tabItemGeneral");

          case SettingsViewType.Accounts:
            return LocalizationManager.GetString("tabCredentials");
          case SettingsViewType.Services:
            return LocalizationManager.GetString("tabItemServices");
          case SettingsViewType.AdvancedProxy:
            return LocalizationManager.GetString("tabItemProxy");
          case SettingsViewType.Support:
            return LocalizationManager.GetString("tabItemSupport");
          case SettingsViewType.Sync:
            return LocalizationManager.GetString("tabItemSync");
          default:
            throw new ArgumentOutOfRangeException();
        }
            

#endif
        return _settingsTitleSelectedText;
      }
      set { _settingsTitleSelectedText = value; }
    }

    public bool IsExpandedGeneral
    {
      get => _isExpandedGeneral;
        set
      {
        _isExpandedGeneral = value;
        if (_isExpandedGeneral)
        {
          CurrentView = SettingsViewType.General;
          UpdateView();
        }
        RaisePropertyChanged();
      }
    }

    public bool IsExpandedAccount
    {
      get => _isExpandedAccount;
        set
      {
        _isExpandedAccount = value;
        if (value)
        {
          CurrentView = SettingsViewType.Accounts;
          UpdateView();
        }
        RaisePropertyChanged();
      }
    }

    public bool IsExpandedSync
    {
      get => _isExpandedSync;
        set
      {
        _isExpandedSync = value;
        if (value)
        {
          CurrentView = SettingsViewType.Sync;
          UpdateView();
        }
        RaisePropertyChanged();
      }
    }

    public bool IsExpandedServices
    {
      get => _isExpandedServices;
        set
      {
        _isExpandedServices = value;
        if (value)
        {
          CurrentView = SettingsViewType.Services;
          UpdateView();
        }
        RaisePropertyChanged();
      }
    }

    public bool IsExpandedAdvanced
    {
      get => _isExpandedAdvanced;
        set
      {
        _isExpandedAdvanced = value;
        if (value)
        {
          CurrentView = SettingsViewType.AdvancedProxy;
          UpdateView();
        }
        RaisePropertyChanged();
      }
    }

    public bool IsExpandedSupport
    {
      get => _isExpandedSupport;
        set
      {
        _isExpandedSupport = value;
        if (value)
        {
          CurrentView = SettingsViewType.Support;
          UpdateView();
        }
        RaisePropertyChanged();
      }
    }

    public int SelectedIndexGeneral
    {
      get => _selectedIndexGeneral;
        set
      {
        _selectedIndexGeneral = value;
        UpdateView();
        RaisePropertyChanged();
      }
    }

    public int SelectedIndexSupport
    {
      get => _selectedIndexSupport;
        set
      {
        _selectedIndexSupport = value;
        UpdateView();
        RaisePropertyChanged();
      }
    }


    public int SelectedIndexAccounts
    {
      get => _selectedIndexAccounts;
        set
      {
        if (_selectedIndexAccounts != -1)
          SaveValueAccount();
        _selectedIndexAccounts = value;
        ChangeAccountSelected();
        UpdateView();
        RaisePropertyChanged();
        RaisePropertyChanged("Account");
      }
    }

    public int SelectedIndexServices
    {
      get => _selectedIndexServices;
        set
      {
        _selectedIndexServices = value;
        UpdateView();
        RaisePropertyChanged();
      }
    }


    public int SelectedIndexAdvanced
    {
      get => _selectedIndexAdvanced;
        set
      {
        _selectedIndexAdvanced = value;
        UpdateView();
        RaisePropertyChanged();
      }
    }

    #endregion

    #region Tab Services

    public ICollectionView LcvURLShortener { get; set; }
    public UrlShorteners URLShortener { get; set; }

    public ObservableCollection<string> URLShorteners
    {
      get => _uRLShorteners;
        set => _uRLShorteners = value;
    }

    public int URLShortenerIndex
    {
      get => _uRLShortenerIndex;
        set
      {
        if (value == -1)
        {
          return;
        }
        _uRLShortenerIndex = value;
        ChangeURLShortener();
        IsBitLyChoice = _uRLShortenerIndex != 0 ? Visibility.Collapsed : Visibility.Visible;

        RaisePropertyChanged("IsBitLyChoice");
        RaisePropertyChanged();
      }
    }

    public bool IsSendByEnter
    {
      get => _isSendByEnter;
        set
      {
        _isSendByEnter = value;

        RaisePropertyChanged();
      }
    }

    public bool CloseBoxPublication
    {
      get => _closeBoxPublication;
        set
      {
        _closeBoxPublication = value;

        RaisePropertyChanged();
      }
    }

    public List<Language> LanguagesTraductor => LanguageHelper.GetAllLanguages();

    private Language _currentLanguageTraductor;

    public Language CurrentLanguageTraductor
    {
      get => _currentLanguageTraductor;
        set
      {
        RaisePropertyChanged();
        _currentLanguageTraductor = value;
      }
    }

    #region BitLy

    public string BitLyUserName
    {
      get => _bitLyUserName;
        set
      {
        _bitLyUserName = value;
        RaisePropertyChanged();
      }
    }

    public Visibility IsBitLyChoice
    {
      get => _isBitLyChoice;
        set
      {
        _isBitLyChoice = value;
        RaisePropertyChanged();
      }
    }

    public string BitLyPassword
    {
      get => _bitLyPassword;
        set
      {
        _bitLyPassword = value;
        RaisePropertyChanged();
      }
    }

    #endregion

    #endregion

    #region Tab General

    #region General Settings

    public ObservableCollection<BLanguage> Languages { get; set; }



    public ListCollectionView Themes => BThemeHelper.GetThemes();

    public int FontSizeValue
    {
      get => _fontSizeValue;
        set
      {
        _fontSizeValue = value;
        RaisePropertyChanged();
      }
    }

    public int ThemeSelectedIndex
    {
      get => _themeSelectedIndex;
        set
      {
        _themeSelectedIndex = value;
        RaisePropertyChanged();
      }
    }

    public bool ShowGlobalFilter
    {
      get => _showGlobalFilter;
        set
      {
        _showGlobalFilter = value;
        RaisePropertyChanged();
      }
    }

    public bool DisableAds
    {
      get => _disableAds;
        set
      {
        _disableAds = value;
        RaisePropertyChanged();
      }
    }

    public bool RunAtStartup
    {
      get => _runAtStartup;
        set
      {
        _runAtStartup = value;
        RaisePropertyChanged();
      }
    }

    public BLanguage SelectedLanguage
    {
      get => _selectedLanguage;
        set
      {
        if (value == null)
        {
          return;
        }
        _selectedLanguage = value;
        RaisePropertyChanged();
      }
    }

    public bool AlertsEnabled
    {
      get => _alertsEnabled;
        set
      {
        _alertsEnabled = value;
        RaisePropertyChanged();
      }
    }

    public bool MinimizeWindowInTray
    {
      get => _minizeWindowInTray;
        set
      {
        _minizeWindowInTray = value;
        RaisePropertyChanged();
      }
    }

    public bool ViewState
    {
      get => _viewState;
        set
      {
        _viewState = value;
        RaisePropertyChanged();
      }
    }

    #endregion

    #endregion

    #region Tab Advanced

    #region Proxy

    public bool UseProxyServer
    {
      get => _useProxyServer;
        set
      {
        _useProxyServer = value;
        RaisePropertyChanged();
      }
    }


    public string ProxyServerName
    {
      get => _proxyServerName;
        set
      {
        _proxyServerName = value;
        RaisePropertyChanged();
      }
    }

    public int ProxyPort
    {
      get => _proxyPort;
        set
      {
        _proxyPort = value;
        RaisePropertyChanged();
      }
    }

    public string ProxyUserName
    {
      get => _proxyUserName;
        set
      {
        _proxyUserName = value;
        RaisePropertyChanged();
      }
    }

    public string ProxyUserDomain
    {
      get => _proxyUserDomain;
        set
      {
        _proxyUserDomain = value;
        RaisePropertyChanged();
      }
    }

    public string ProxyPassword
    {
      get => _proxyPassword;
        set
      {
        _proxyPassword = value;
        RaisePropertyChanged();
      }
    }

    #endregion

    #region Log

    public string SupportLogFrom
    {
      get => SobeesSettings.SupportLogEmail;
        set
      {
        SobeesSettings.SupportLogEmail = value;
        RaisePropertyChanged();
        RaisePropertyChanged("BisSupportLogMailValid");
      }
    }

    public string SupportLogSubject
    {
      get => _logSubject;
        set
      {
        _logSubject = value;
        RaisePropertyChanged();
      }
    }

    public string SupportLogDescription
    {
      get => _logDescription;
        set
      {
        _logDescription = value;
        RaisePropertyChanged();
        RaisePropertyChanged("BisSupportLogMailValid");
      }
    }

    public string SupportLogAttachment
    {
      get
      {
        if (string.IsNullOrEmpty(_logAttachment))
          SupportLogCapture();

        return _logAttachment;
      }
      set
      {
        _logAttachment = value;
        RaisePropertyChanged();
      }
    }

    public ImageSource SupportLogCaptureImage
    {
      get => _supportLogCapture;
        set
      {
        _supportLogCapture = value;
        RaisePropertyChanged();
      }
    }

#if !SILVERLIGHT
    public bool BisSupportLogImageSendWithLog
    {
      get => _isSupportLogImageSendWithLog;
        set
      {
        try
        {
          _isSupportLogImageSendWithLog = value;
          if (!_isSupportLogImageSendWithLog)
          {
            SupportLogCaptureImage = null;
            File.Delete(_supportLogCapturePathFile);
          }
          else
          {
            SupportLogCapture();
          }
        }
        catch (Exception ex)
        {
          TraceHelper.Trace(this,
                            (ex));
        }

        RaisePropertyChanged();
      }
    }
#endif

    public bool BisSupportLogMailValid
    {
      get
      {
        if (SupportLogDescription != null)
          _bisSupportLogMailValid = !String.IsNullOrEmpty(SupportLogFrom) &&
                                    MailHelper.BisValidEmail(SupportLogFrom) && SupportLogDescription.Trim().Length != 0;

        return _bisSupportLogMailValid;
      }
      //set { _bisSupportLogMailValid = value; }
    }

    #endregion Log

    #endregion

    #region Tab Account

    #region All

    #region Spam

    public string NewSpam
    {
      get => _newSpam;
        set
      {
        _newSpam = value;
        RaisePropertyChanged();
      }
    }

    public UserAccount Account
    {
      get
      {
        if (_selectedIndexAccounts > -1 && _selectedIndexAccounts < Accounts.Count)
          return Accounts[_selectedIndexAccounts];
        return null;
      }
    }

    public ObservableCollection<string> Spam
    {
      get => _spamList;
        set
      {
        _spamList = value;
        RaisePropertyChanged();
      }
    }

    #endregion

    #region Alerts

    public EnumAlertsFacebookType AlertsFacebookType
    {
      get
      {
        if (IsCheckedFBNo)
          return EnumAlertsFacebookType.No;
        if (IsCheckedFBAll)
          return EnumAlertsFacebookType.All;
        return EnumAlertsFacebookType.Advanced;
      }
      set
      {
        _alertsFacebookType = value;
        RaisePropertyChanged("IsCheckedFBAll");
        RaisePropertyChanged("IsCheckedFBNo");
        RaisePropertyChanged("IsCheckedFBAdvanced");
        RaisePropertyChanged();
        RaisePropertyChanged("Account");
      }
    }

    public bool IsCheckedFBAll
    {
      get => _isCheckedFBAll;
        set
      {
        if (value)
        {
          AlertsFacebookType = EnumAlertsFacebookType.All;
          IsCheckedFBNo = false;
          IsCheckedFBAdvanced = false;
          AdvancedAlertsSettingsVisibility = Visibility.Collapsed;
        }
        _isCheckedFBAll = value;
        RaisePropertyChanged();
      }
    }

    public bool IsCheckedFBAdvanced
    {
      get => _isCheckedFBAdvanced;
        set
      {
        if (value)
        {
          AlertsFacebookType = EnumAlertsFacebookType.Advanced;
          IsCheckedFBNo = false;
          IsCheckedFBAll = false;
          AdvancedAlertsSettingsVisibility = Visibility.Visible;
        }
        _isCheckedFBAdvanced = value;
        RaisePropertyChanged();
      }
    }

    public bool IsCheckedFBNo
    {
      get => _isCheckedFBNo;
        set
      {
        if (value)
        {
          AlertsFacebookType = EnumAlertsFacebookType.No;
          IsCheckedFBAll = false;
          IsCheckedFBAdvanced = false;
          AdvancedAlertsSettingsVisibility = Visibility.Collapsed;
        }
        _isCheckedFBNo = value;
        RaisePropertyChanged();
      }
    }

    public bool IsCheckedUseAlertsWords
    {
      get => _isCheckedUseAlertsWords;
        set
      {
        _isCheckedUseAlertsWords = value;
        RaisePropertyChanged();
      }
    }

    public bool IsCheckedUseAlertsUsers
    {
      get => _isCheckedUseAlertsUsers;
        set
      {
        _isCheckedUseAlertsUsers = value;
        RaisePropertyChanged();
      }
    }

    public bool IsCheckedUseAlertsRemovedWords
    {
      get => _isCheckedUseAlertsRemovedWords;
        set
      {
        _isCheckedUseAlertsRemovedWords = value;
        RaisePropertyChanged();
      }
    }

    public bool IsCheckedAlertsDM
    {
      get => _isCheckedUseAlertsDM;
        set
      {
        _isCheckedUseAlertsDM = value;
        RaisePropertyChanged();
      }
    }

    public bool IsCheckedAlertsFriends
    {
      get => _isCheckedUseAlertsFriends;
        set
      {
        _isCheckedUseAlertsFriends = value;
        RaisePropertyChanged();
      }
    }

    public bool IsCheckedAlertsGroups
    {
      get => _isCheckedUseAlertsGroups;
        set
      {
        _isCheckedUseAlertsGroups = value;
        RaisePropertyChanged();
      }
    }

    public bool IsCheckedAlertsReply
    {
      get => _isCheckedUseAlertsReply;
        set
      {
        _isCheckedUseAlertsReply = value;
        RaisePropertyChanged();
      }
    }

    public Visibility AdvancedAlertsSettingsVisibility
    {
      get => _advancedAlertsSettingsVisibility;
        set
      {
        _advancedAlertsSettingsVisibility = value;
        RaisePropertyChanged();
      }
    }

    public string AlertWord
    {
      get => _alertWord ?? "";
        set
      {
        _alertWord = value;
        RaisePropertyChanged();
        //RaisePropertyChanged("IsActivAddAlertsWords");
      }
    }

    public string AlertRemovedWord
    {
      get => _alertRemovedWord ?? "";
        set
      {
        _alertRemovedWord = value;
        RaisePropertyChanged();
        //RaisePropertyChanged("IsActivAddAlertsRemovedWords");
      }
    }

    public string AlertUser
    {
      get => _alertUser ?? "";
        set
      {
        _alertUser = value;
        RaisePropertyChanged();
        //RaisePropertyChanged("IsActivAddAlertsUsers");
      }
    }

    public ObservableCollection<string> AlertWordList
    {
      get => _alertWordList;
        set
      {
        _alertWordList = value;
        RaisePropertyChanged();
      }
    }

    public ObservableCollection<string> AlertUserList
    {
      get => _alertUserList;
        set
      {
        _alertUserList = value;
        RaisePropertyChanged();
      }
    }

    public ObservableCollection<string> AlertRemovedWordList
    {
      get => _alertRemovedWordList;
        set
      {
        _alertRemovedWordList = value;
        RaisePropertyChanged();
      }
    }

    #endregion

    #endregion

    #endregion

    #endregion

    #region Commands

    public RelayCommand AddSpamCommand { get; private set; }
    public RelayCommand<string> DeleteSpamCommand { get; private set; }
    public RelayCommand SaveSettingsCommand { get; set; }
    public RelayCommand DeleteAccountCommand { get; set; }
    public RelayCommand CloseSettingsCommand { get; set; }
    public RelayCommand AddAlertsWordsCommand { get; set; }

    public RelayCommand<string> DeleteRemovedWordCommand { get; set; }

    public RelayCommand<string> DeleteUserWordCommand { get; set; }

    public RelayCommand<string> DeleteWordCommand { get; set; }

    public RelayCommand AddAlertsRemovedWordsCommand { get; set; }

    public RelayCommand AddAlertsUsersCommand { get; set; }
    public RelayCommand<PasswordBox> CheckAccountCommand { get; set; }
    public RelayCommand ImportConfigCommand { get; set; }
    public RelayCommand ClearCacheCommand { get; set; }


    public RelayCommand SupportLoadedCommand { get; set; }

    public RelayCommand SupportLogSubmitCommand { get; set; }
    public RelayCommand SupportLogCancelCommand { get; set; }

    public RelayCommand SupportLogCaptureCommand { get; set; }

    public RelayCommand ExportConfigCommand { get; set; }

    private BRelayCommand _changeThemeCommand;


    public BRelayCommand ChangeThemeCommand => _changeThemeCommand ?? (_changeThemeCommand = new BRelayCommand(ChangeTheme));

    #endregion

    #region Methods

    #region Public Methods

    /// <summary>
    ///   Used when a string arrived into the Messenger
    /// </summary>
    /// <param name="param">A string that represents the fonction to execute.</param>
    protected new virtual void DoAction(string param)
    {
      switch (param)
      {
        case "Offline":
          Messenger.Default.Send("SaveSobeesSettings");
          break;
        default:
          break;
      }
      base.DoAction(param);
    }

    #endregion

    #region Private Methods

    private void UpdateView()
    {
      RaisePropertyChanged("View");
    }

    protected override void InitCommands()
    {
      AddSpamCommand = new RelayCommand(AddSpam, () => !string.IsNullOrEmpty(NewSpam) && !Spam.Contains(NewSpam));
      DeleteSpamCommand = new RelayCommand<string>(DeleteSpam);
      AddAlertsWordsCommand = new RelayCommand(AddAlertsWords, () => !string.IsNullOrEmpty(AlertWord) && !AlertWordList.Contains(AlertWord));
      AddAlertsUsersCommand = new RelayCommand(AddAlertsUsers, () => !string.IsNullOrEmpty(AlertUser) && !AlertWordList.Contains(AlertUser));
      AddAlertsRemovedWordsCommand = new RelayCommand(AddAlertsRemovedWords,
                                                      () => !string.IsNullOrEmpty(AlertRemovedWord) && !AlertRemovedWordList.Contains(AlertRemovedWord));
      CheckAccountCommand = new RelayCommand<PasswordBox>(CheckLoginNew);
      ImportConfigCommand = new RelayCommand(ImportConfig);
      ExportConfigCommand = new RelayCommand(ExportConfig);
      ClearCacheCommand = new RelayCommand(ClearCache);
      SupportLogSubmitCommand = new RelayCommand(SupportLogSubmit);
      SupportLogCancelCommand = new RelayCommand(SupportLogCancel);
      SupportLogCaptureCommand = new RelayCommand(SupportLogCapture);
      CloseCommand = new RelayCommand(() => Messenger.Default.Send("CloseSettings"));

      SaveCommand = new RelayCommand(() =>
                                       {
                                         SaveValueAccount();
                                         Messenger.Default.Send("CloseSaveSettings");
                                       });
      DeleteAccountCommand = new RelayCommand(() =>
                                                {
                                                  Accounts.Remove(Account);

                                                  if (Accounts.Count > 0)
                                                  {
                                                    SelectedIndexAccounts = 0;
                                                  }
                                                  else
                                                  {
                                                    SelectedIndexAccounts = -1;
                                                  }

                                                  //Messenger.Default.Send("AccountAdded");
                                                });
      DeleteRemovedWordCommand = new RelayCommand<string>(str => AlertRemovedWordList.Remove(str));
      DeleteUserWordCommand = new RelayCommand<string>(str => AlertUserList.Remove(str));
      DeleteWordCommand = new RelayCommand<string>(str => AlertWordList.Remove(str));
      base.InitCommands();
    }

    /// <summary>
    /// </summary>
    /// <param name="box"></param>
    public void CheckLoginNew(PasswordBox box)
    {
      try
      {
        var hashKey = EncryptionHelper.GetHashKey(BGlobals.CIPHER_KEY);
        var passwordHash = EncryptionHelper.Encrypt(hashKey,
                                                    box.Password);

        // Check credentials
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

                               ErrorMsg = TwitterLib.CheckCredentials(Account.Login, passwordHash, BGlobals.CIPHER_KEY,
                                                                      ProxyHelper.GetConfiguredWebProxy(SobeesSettings));
                             };

          worker.RunWorkerCompleted += delegate
                                         {
                                           if (string.IsNullOrEmpty(ErrorMsg))
                                           {
                                             Account.PasswordHash = passwordHash;
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

    private void SaveValueAccount()
    {
      if (Account == null) return;
      //Save Spam Properties
      Account.SpamList.Clear();
      if (Spam != null)
        foreach (var spamList in Spam)
        {
          Account.SpamList.Add(spamList);
        }
      //Load Alerts Properties
      Account.TypeAlertsFB = AlertsFacebookType;
      Account.AlertsWordsList.Clear();
      foreach (var word in AlertWordList)
      {
        Account.AlertsWordsList.Add(word);
      }
      Account.AlertsUsersList.Clear();
      foreach (var word in AlertUserList)
      {
        Account.AlertsUsersList.Add(word);
      }
      Account.AlertsRemovedWordsList.Clear();
      foreach (var word in AlertRemovedWordList)
      {
        Account.AlertsRemovedWordsList.Add(word);
      }
      Account.IsCheckedUseAlertsRemovedWords = IsCheckedUseAlertsRemovedWords;
      Account.IsCheckedUseAlertsUsers = IsCheckedUseAlertsUsers;
      Account.IsCheckedUseAlertsWords = IsCheckedUseAlertsWords;
      if (Account.Type == EnumAccountType.Twitter)
      {
        Account.IsCheckedUseAlertsDM = IsCheckedAlertsDM;
        Account.IsCheckedUseAlertsFriends = IsCheckedAlertsFriends;
        Account.IsCheckedUseAlertsGroups = IsCheckedAlertsGroups;
        Account.IsCheckedUseAlertsReply = IsCheckedAlertsReply;
        //TODO         Account.IsSignatureActivated = IsSignatureActivated;
        //TODO Account.Signature = Signature;
      }
    }

    private void ChangeAccountSelected()
    {
      if (Account == null)
      {
        return;
      }
      //Load Spam Properties
      Spam.Clear();
      if (Account.SpamList != null)
        foreach (var spamList in Account.SpamList)
        {
          Spam.Add(spamList);
        }
      NewSpam = string.Empty;
      //Load Alerts Properties
      switch (Account.TypeAlertsFB)
      {
        case EnumAlertsFacebookType.All:
          IsCheckedFBAll = true;
          IsCheckedFBNo = false;
          IsCheckedFBAdvanced = false;
          break;
        case EnumAlertsFacebookType.Advanced:
          IsCheckedFBAll = false;
          IsCheckedFBNo = false;
          IsCheckedFBAdvanced = true;
          break;
        case EnumAlertsFacebookType.No:
          IsCheckedFBAll = false;
          IsCheckedFBNo = true;
          IsCheckedFBAdvanced = false;
          break;
      }
      AlertWord = string.Empty;
      AlertWordList.Clear();
      foreach (var word in Account.AlertsWordsList)
      {
        AlertWordList.Add(word);
      }
      AlertRemovedWordList.Clear();
      foreach (var word in Account.AlertsRemovedWordsList)
      {
        AlertRemovedWordList.Add(word);
      }
      AlertUserList.Clear();
      foreach (var word in Account.AlertsUsersList)
      {
        AlertUserList.Add(word);
      }
      IsCheckedUseAlertsRemovedWords = Account.IsCheckedUseAlertsRemovedWords;
      IsCheckedUseAlertsUsers = Account.IsCheckedUseAlertsUsers;
      IsCheckedUseAlertsWords = Account.IsCheckedUseAlertsWords;
      if (Account.Type == EnumAccountType.Twitter)
      {
        IsCheckedAlertsDM = Account.IsCheckedUseAlertsDM;
        IsCheckedAlertsFriends = Account.IsCheckedUseAlertsFriends;
        IsCheckedAlertsGroups = Account.IsCheckedUseAlertsGroups;
        IsCheckedAlertsReply = Account.IsCheckedUseAlertsReply;
        //TODO        IsSignatureActivated = Account.IsSignatureActivated;
        //TODO        Signature = Account.Signature;
      }
    }

    private void ChangeURLShortener()
    {
      switch (URLShortenerIndex)
      {
        case 0:
          URLShortener = UrlShorteners.BitLy;
          break;
        case 1:
          URLShortener = UrlShorteners.Digg;
          break;
        case 2:
          URLShortener = UrlShorteners.IsGd;
          break;
        case 3:
          URLShortener = UrlShorteners.TinyUrl;
          break;
          URLShortener = UrlShorteners.MigreMe;
          break;
      }
    }

    private void CreateListURLShorteners()
    {
      switch (URLShortener)
      {
        case UrlShorteners.BitLy:
          URLShortenerIndex = 0;
          break;
        case UrlShorteners.Digg:
          URLShortenerIndex = 1;
          break;
        case UrlShorteners.IsGd:
          URLShortenerIndex = 2;
          break;
        case UrlShorteners.TinyUrl:
          URLShortenerIndex = 3;
          break;
        case UrlShorteners.MigreMe:
          URLShortenerIndex = 4;
          break;
      }
      URLShorteners.Add("Bit.ly");
      URLShorteners.Add("Digg");
      URLShorteners.Add("Is.gd");
      URLShorteners.Add("TinyUrl");
      //URLShorteners.Add("Tr.im");
      //URLShorteners.Add("Twurl");
      URLShorteners.Add("MigreMe");
    }

    private void LoadCurrentValue()
    {
      //Load General
      Accounts = new ObservableCollection<UserAccount>();
      {
        foreach (var account in
          SobeesSettings.Accounts.Where(account => account != null && !string.IsNullOrEmpty(account.Login)))
        {
          Accounts.Add(new UserAccount(account));
        }
      }
      FontSizeValue = (int)SobeesSettings.FontSizeValue;
      if (Accounts.Count > 0)
      {
        _selectedIndexAccounts = 0;
        ChangeAccountSelected();
      }
      AlertsEnabled = SobeesSettings.AlertsEnabled;
      MinimizeWindowInTray = SobeesSettings.MinimizeWindowInTray;
      CloseBoxPublication = SobeesSettings.CloseBoxPublication;
      IsSendByEnter = SobeesSettings.IsSendByEnter;
      RunAtStartup = SobeesSettings.RunAtStartup;
      ShowGlobalFilter = SobeesSettings.ShowGlobalFilter;
      DisableAds = SobeesSettings.DisableAds;
      UseProxyServer = SobeesSettings.IsEnabledProxy;
      ProxyPassword = SobeesSettings.ProxyPassword;
      ProxyPort = SobeesSettings.ProxyPort;
      ProxyServerName = SobeesSettings.ProxyServer;
      ProxyUserDomain = SobeesSettings.ProxyUserDomain;
      ProxyUserName = SobeesSettings.ProxyUserName;
      Languages = new ObservableCollection<BLanguage>
                    {
                      new BLanguage("de", "Deutsch"),
                      new BLanguage("en", "English"),
                      new BLanguage("es", "Español"),
                      new BLanguage("fr", "Français"),
                      new BLanguage("it", "Italiano"),
                      //new bLanguage("ko", "한국어"),
                      //new bLanguage("ja", "日本語"),
                      //new bLanguage("zh", "中文"),
                    };
      if (SobeesSettings.Language == "")
      {
        SobeesSettings.Language = "en";
      }


      foreach (var language in Languages.Where(language => SobeesSettings.Language == language.ShortName))
      {
        SelectedLanguage = language;
        break;
      }

      UpdateThemeSelection();
      LcvURLShortener = CollectionViewSource.GetDefaultView(URLShorteners);


      //Services
      _currentLanguageTraductor = SobeesSettings.LanguageTranslator;
      URLShortener = SobeesSettings.UrlShortener;
      CreateListURLShorteners();
      BitLyUserName = SobeesSettings.BitLyUserName;
      BitLyPassword = SobeesSettings.BitLyPassword;
      ViewState = !SobeesSettings.ViewState;
    }

    private void ClearCache()
    {
      Messenger.Default.Send("ClearCacheAll");
    }

    private static DispatcherFrame frame = new DispatcherFrame();

    private void SupportLogSubmit()
    {
      try
      {
        {
          StartWaiting();
          ThreadHelper.DoEvents();

          var result = "The application wasn't able to send the mail; please contact sobees support -> help@sobees.com";
          try
          {
            var productInfo = string.Format("ProductInfo:[{0}]:", AssemblyInfoDetails.ProductName);
            var logdenvironment = string.Format("Environement:{0}|{1}|{2}|{3}|{4}", Environment.OSVersion.Platform, Environment.OSVersion,
                                                Environment.OSVersion.ServicePack, Environment.OSVersion.Version,
                                                Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE"));
            var logdescription = string.Format("From:{0}\r\n{1}\r\n{2}\r\nDescription:{3}", SupportLogFrom, productInfo, logdenvironment, SupportLogDescription);

            SupportLogAttachment = GetLogAttachment();
            result = BMailHelper.SendEmail(SupportLogFrom,
                                           BGlobals.SUPPORTLOG_DEFAULT_MAIL_TO,
                                           SupportLogSubject,
                                           logdescription,
                                           SupportLogAttachment);
          }
          catch (Exception ex)
          {
            TraceHelper.Trace(this,
                              (ex));
          }

          MessageBox.Show(result, "Status", MessageBoxButton.OK, MessageBoxImage.Information);
          SupportLogDescription = string.Empty;
          StopWaiting();
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this,
                          (ex));
      }
    }


    private string GetLogAttachment()
    {
      var logfilesAsString = string.Empty;
      try
      {
        var logPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var destpath = Path.Combine(logPath, BGlobals.SUPPORTLOG_FILENAME);
        var gz = CompressionHelper.ZipFiles(logPath, destpath, BGlobals.SUPPORTLOG_FILE_PATTERN, false, string.Empty);
        logfilesAsString = gz;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this,
                          (ex));
      }
      return logfilesAsString;
    }

    private void SupportLogCancel()
    {
      //SupportLogSubject = string.Empty;
      SupportLogDescription = string.Empty;
      SupportLogAttachment = string.Empty;
      SupportLogCaptureImage = null;
    }

    private static readonly string _supportLogPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    private static readonly string _supportLogCapturePathFile = Path.Combine(_supportLogPath, BGlobals.SUPPORTLOG_CAPTURE_FILENAME);

    private void SupportLogCapture()
    {
      BitmapSource bitmapSource = null;
      try
      {
        bitmapSource = CaptureScreenHelper.CaptureScreenToFile(_supportLogCapturePathFile);
        SupportLogAttachment = _supportLogCapturePathFile;
      }
      catch (Exception ex)
      {
        //To avoid recursive call between attachement and capture cause field is null, I put it as "" in case of error
        SupportLogAttachment = string.Empty;
        TraceHelper.Trace(this,
                          (ex));
      }
      //Display the Capture in the control
      SupportLogCaptureImage = bitmapSource;
      RaisePropertyChanged("SupportLogCaptureImage");
    }

    private void ExportConfig()
    {
      var dlg = new SaveFileDialog
                  {
                    FileName = "sobees",
                    DefaultExt = ".xml",
                    Filter = "XML documents (.xml)|*.xml"
                  };
      var result = dlg.ShowDialog();
      if (result == true)
      {
        // Save document
        BViewModelLocator.SobeesViewModelStatic.Settings.ExportSettings(dlg.FileName);
      }
    }

    private void ImportConfig()
    {
      var dlg = new OpenFileDialog
                  {
                    FileName = "sobees_bDule",
                    DefaultExt = ".xml",
                    Filter = "XML documents (.xml)|*.xml"
                  };

      // Show open file dialog box
      var result = dlg.ShowDialog();

      // Process open file dialog box results
      if (result != true) return;
      // Open document
      BViewModelLocator.SobeesViewModelStatic.Settings.ImportSettingsXml(dlg.FileName);
      Messenger.Default.Send("CloseSettings");
    }

    #region Alertes

    private void AddAlertsWords()
    {
      if (!AlertWordList.Contains(AlertWord))
      {
        AlertWordList.Add(AlertWord);
      }
      AlertWord = string.Empty;
    }

    private void AddAlertsRemovedWords()
    {
      if (!AlertRemovedWordList.Contains(AlertRemovedWord))
      {
        AlertRemovedWordList.Add(AlertRemovedWord);
      }
      AlertRemovedWord = string.Empty;
    }

    private void AddAlertsUsers()
    {
      if (!AlertUserList.Contains(AlertUser))
      {
        AlertUserList.Add(AlertUser);
      }
      AlertUser = string.Empty;
    }

    #endregion

    #region Spam

    private void AddSpam()
    {
      if (!Spam.Contains(NewSpam))
      {
        Spam.Add(NewSpam);
      }
      NewSpam = string.Empty;
    }

    private void DeleteSpam(string spamToDelete)
    {
      Spam.Remove(spamToDelete);
    }

    #endregion

    #region theme

    private void ChangeTheme(object param)
    {
      var objs = BRelayCommand.CheckParams(param);
      if (objs == null || objs[1] == null) return;
      if (objs[1].GetType() == typeof(Button))
      {
        Messenger.Default.Send(new BMessage("ChangeTheme", objs[0].ToString()));
      }
      else if (objs[1].GetType() == typeof(ComboBox))
      {
        var lst = objs[1] as ComboBox;
        if (lst != null)
        {
          var select = lst.SelectedItem as BThemeInfo;
          if (select != null)
          {
            Messenger.Default.Send(new BMessage("ChangeTheme", select.SkinName));
          }
        }
      }
      UpdateThemeSelection();
    }

    private void UpdateThemeSelection()
    {
      var i = 0;
      foreach (var view in Themes)
      {
        if (((BThemeInfo)view).SkinName.ToLower().Equals(BThemeHelper.CurrentTheme))
        {
          ThemeSelectedIndex = i;
          break;
        }
        i++;
      }
    }

    #endregion

    #endregion

    #endregion

    #region Event Handler

    #endregion
  }
}