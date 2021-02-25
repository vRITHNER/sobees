#region Includes

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Sobees.Library.BGoogleLib.Language;
using Sobees.Tools.Crypto;
using Sobees.Tools.Logging;
using Sobees.Tools.Serialization;
using Sobees.Configuration.BGlobals;

#endregion

namespace Sobees.Infrastructure.Cls
{
  public class SobeesSettings : IXmlSerializable, INotifyPropertyChanged
  {
    #region Fields

    private bool _alertsEnabled = BGlobals.ENABLE_ALERTS;
    private bool _alertsTwitterSearchEnabled = BGlobals.ENABLE_ALERTS;
    private bool _minimizeWindowInTray = BGlobals.ENABLE_MINIMIZEWINDOWINTRAY;
    private bool _closeBoxPublication = true;
    private bool _isSendByEnter = true;
    private bool _viewState;
    private double _fontSizeValue = BGlobals.DEFAULT_FONTSIZE;
    private string _language = "";
    private bool _runAtStartup = BGlobals.ENABLE_RUNASSTARTUP;
    private bool _showGlobalFilter = BGlobals.ENABLE_GLOBALFILTER;
    private bool _disableAds = BGlobals.ENABLE_DISABLEADS;
    private bool _showTwitterFrom = BGlobals.ENABLE_TWITTERFROM;

    private string _theme = BGlobals.DEFAULT_THEME;
    private string _twitterService;
    private string _utilService;
    private UrlShorteners _urlShortener = (UrlShorteners)BGlobals.DEFAULT_URLSHORTENER;

    private string _wordSearch = "";

    #region Services
    private Language _languageTranslator = Sobees.Library.BGoogleLib.Language.Language.English;

    private bool _isUseTwitterSearch = BGlobals.DEFAULT_USESEARCH_TWITTERSEARCH;

    #endregion

    #region Proxy

    private string _bitLyPassword = "";
    private string _bitLyUserName = "";
    private bool _isEnabledProxy;
    private string _proxyPassword = "";
    private string _SyncUser = "";
    private int _proxyPort;
    private string _proxyServer = "";
    private string _proxyUserDomain = "";
    private string _proxyUserName = "";

    #endregion

    #endregion

    #region Properties

    //public string MySpaceService
    //{
    //  get
    //  {
    //    if (string.IsNullOrEmpty(_mySpaceService))
    //    {
    //      if (BGlobals.MYSPACE_WS_URL_LIST.Count > 0)
    //      {
    //        _mySpaceService = BGlobals.MYSPACE_WS_URL_LIST[0];
    //      }
    //    }
    //    return _mySpaceService;
    //  }
    //  set { _mySpaceService = value; }
    //}

    public string TwitterService
    {
      get
      {
        if (string.IsNullOrEmpty(_twitterService))
        {
          if (BGlobals.TWITTER_WS_URL_LIST.Count > 0)
          {
            _twitterService = BGlobals.TWITTER_WS_URL_LIST[0];
          }
        }
        return _twitterService;
      }
      set { _twitterService = value; }
    }

    public string UtilService
    {
      get
      {
        if (string.IsNullOrEmpty(_utilService))
        {
          if (BGlobals.UTIL_WS_URL_LIST.Count > 0)
          {
            _utilService = BGlobals.UTIL_WS_URL_LIST[0];
          }
        }
        return _utilService;
      }
      set { _utilService = value; }
    }


    public ObservableCollection<UserAccount> Accounts { get; set; }

    public bool IsFirstLaunch { get; set; }

    public string Theme
    {
      get { return _theme; }
      set
      {
        _theme = value;
        OnPropertyChanged("Theme");
      }
    }

    public Language LanguageTranslator
    {
      get { return _languageTranslator; }
      set
      {

        _languageTranslator = value;
        OnPropertyChanged("LanguageTranslator");
      }
    }

    public string SupportLogEmail { get; set; }
    public string Language
    {
      get { return _language; }
      set
      {
        _language = value;
        OnPropertyChanged("Language");
      }
    }

    public double FontSizeValue
    {
      get { return _fontSizeValue; }
      set
      {
        _fontSizeValue = value;
        OnPropertyChanged("FontSizeValue");
      }
    }

    public bool AlertsEnabled
    {
      get { return _alertsEnabled; }
      set
      {
        _alertsEnabled = value;
        OnPropertyChanged("AlertsEnabled");
      }
    }

    public string WordSearch
    {
      get { return _wordSearch; }
      set
      {
        _wordSearch = value;
        OnPropertyChanged("WordSearch");
      }
    }

    public bool MinimizeWindowInTray
    {
      get { return _minimizeWindowInTray; }
      set
      {
        _minimizeWindowInTray = value;
        OnPropertyChanged("MinimizeWindowInTray");
      }
    }

    public bool RunAtStartup
    {
      get { return _runAtStartup; }
      set
      {
        _runAtStartup = value;
        OnPropertyChanged("RunAtStartup");
      }
    }

    public bool ShowTwitterFrom
    {
      get { return _showTwitterFrom; }
      set
      {
        _showTwitterFrom = value;
        OnPropertyChanged("ShowTwitterFrom");
      }
    }

    public bool ShowGlobalFilter
    {
      get { return _showGlobalFilter; }
      set
      {
        _showGlobalFilter = value;
        OnPropertyChanged("ShowGlobalFilter");
      }
    }

    public bool DisableAds
    {
      get { return _disableAds; }
      set
      {
        _disableAds = value;
        OnPropertyChanged("DisableAds");
      }
    }

    public bool CloseBoxPublication
    {
      get { return _closeBoxPublication; }
      set
      {
        _closeBoxPublication = value;
        OnPropertyChanged("CloseBoxPublication");
      }
    }
    public bool IsSendByEnter
    {
      get { return _isSendByEnter; }
      set
      {
        _isSendByEnter = value;
        OnPropertyChanged("IsSendByEnter");
      }
    }
    public bool ViewState
    {
      get { return _viewState; }
      set
      {
        _viewState = value;
        OnPropertyChanged("ViewState");
      }
    }

    public UrlShorteners UrlShortener
    {
      get { return _urlShortener; }
      set
      {
        _urlShortener = value;
        OnPropertyChanged("UrlShortener");
      }
    }

    #region Services

    public bool AlertsTwitterSearchEnabled
    {
      get { return _alertsTwitterSearchEnabled; }
      set
      {
        _alertsTwitterSearchEnabled = value;
        OnPropertyChanged("AlertsTwitterSearchEnabled");
      }
    }

    public bool IsUseTwitterSearch
    {
      get { return _isUseTwitterSearch; }
      set
      {
        _isUseTwitterSearch = value;
        OnPropertyChanged("IsUseTwitterSearch");
      }
    }

    #endregion

    #region Proxy

    public bool IsEnabledProxy
    {
      get { return _isEnabledProxy; }
      set
      {
        _isEnabledProxy = value;
        OnPropertyChanged("IsEnabledProxy");
      }
    }

    public string ProxyServer
    {
      get { return _proxyServer; }
      set
      {
        _proxyServer = value;
        OnPropertyChanged("ProxyServer");
      }
    }

    public int ProxyPort
    {
      get { return _proxyPort; }
      set
      {
        _proxyPort = value;
        OnPropertyChanged("ProxyIP");
      }
    }

    public string ProxyUserName
    {
      get { return _proxyUserName; }
      set
      {
        _proxyUserName = value;
        OnPropertyChanged("ProxyUserName");
      }
    }

    public string ProxyUserDomain
    {
      get { return _proxyUserDomain; }
      set
      {
        _proxyUserDomain = value;
        OnPropertyChanged("ProxyUserDomain");
      }
    }

    public string SyncUser
    {
      get
      {
        byte[] key = EncryptionHelper.GetHashKey(BGlobals.CIPHER_KEY);
        return EncryptionHelper.Decrypt(key,
                                        _SyncUser);
      }
      set
      {
        byte[] hashKey = EncryptionHelper.GetHashKey(BGlobals.CIPHER_KEY);
        _SyncUser = EncryptionHelper.Encrypt(hashKey,
                                                  value);
        OnPropertyChanged("SyncUser");
      }
    }

    private bool _asAskSync;

    public bool AsAskSync
    {
      get { return _asAskSync; }
      set { _asAskSync = value; }
    }

    public string ProxyPassword
    {
      get
      {
        byte[] key = EncryptionHelper.GetHashKey(BGlobals.CIPHER_KEY);
        return EncryptionHelper.Decrypt(key,
                                        _proxyPassword);
      }
      set
      {
        byte[] hashKey = EncryptionHelper.GetHashKey(BGlobals.CIPHER_KEY);
        _proxyPassword = EncryptionHelper.Encrypt(hashKey,
                                                  value);
        OnPropertyChanged("ProxyPassword");
      }
    }

    public string BitLyUserName
    {
      get { return _bitLyUserName; }
      set
      {
        _bitLyUserName = value;
        OnPropertyChanged("BitLyUserName");
      }
    }


    public string BitLyPassword
    {
      get
      {
        byte[] key = EncryptionHelper.GetHashKey(BGlobals.CIPHER_KEY);
        return EncryptionHelper.Decrypt(key,
                                        _bitLyPassword);
      }
      set
      {
        byte[] hashKey = EncryptionHelper.GetHashKey(BGlobals.CIPHER_KEY);
        _bitLyPassword = EncryptionHelper.Encrypt(hashKey,
                                                  value);
        OnPropertyChanged("BitLyUserName");
      }
    }

    public string Filter
    {
      get;
      set;
    }

    #endregion

    #endregion

    #region Constructor

    public SobeesSettings()
    {
      Accounts = new ObservableCollection<UserAccount>();
    }

    #endregion

    #region INotifyPropertyChanged Members

    /// <summary>
    /// Raised when a property on this object has a new value.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    #region IXmlSerializable Members

    public XmlSchema GetSchema()
    {
      return null;
    }

    public void ReadXml(XmlReader reader)
    {
      try
      {
        reader.MoveToContent();
        reader.Read();

        reader.ReadStartElement("Accounts");
        Accounts =
          GenericCollectionSerializer.DeserializeObject<ObservableCollection<UserAccount>>(reader.ReadContentAsString());
        reader.ReadEndElement();

        reader.ReadStartElement("Theme");
        Theme = reader.ReadContentAsString();
        reader.ReadEndElement();

        reader.ReadStartElement("FontSizeValue");
        FontSizeValue = double.Parse(reader.ReadContentAsString());
        reader.ReadEndElement();

        reader.ReadStartElement("AlertsEnabled");
        AlertsEnabled = bool.Parse(reader.ReadContentAsString());
        reader.ReadEndElement();

        reader.ReadStartElement("RunAtStartup");
        RunAtStartup = bool.Parse(reader.ReadContentAsString());
        reader.ReadEndElement();

        reader.ReadStartElement("UrlShortener");
        UrlShortener = (UrlShorteners)Enum.Parse(typeof(UrlShorteners), reader.ReadContentAsString(), true);
        reader.ReadEndElement();

        reader.ReadStartElement("ShowTwitterFrom");
        ShowTwitterFrom = bool.Parse(reader.ReadContentAsString());
        reader.ReadEndElement();
        reader.ReadStartElement("CloseBoxPublication");
        CloseBoxPublication = bool.Parse(reader.ReadContentAsString());
        reader.ReadEndElement();
        reader.ReadStartElement("AlertsTwitterSearchEnabled");
        AlertsTwitterSearchEnabled = bool.Parse(reader.ReadContentAsString());
        reader.ReadEndElement();


        if (!reader.IsEmptyElement)
        {
          reader.ReadStartElement("ProxyPort");
          ProxyPort = int.Parse(reader.ReadContentAsString());
          reader.ReadEndElement();
        }
        else
        {
          reader.ReadStartElement("ProxyPort");
        }
        if (!reader.IsEmptyElement)
        {
          reader.ReadStartElement("ProxyServer");
          ProxyServer = reader.ReadContentAsString();
          reader.ReadEndElement();
        }
        else
        {
          reader.ReadStartElement("ProxyServer");
        }
        if (!reader.IsEmptyElement)
        {
          reader.ReadStartElement("ProxyUserName");
          ProxyUserName = reader.ReadContentAsString();
          reader.ReadEndElement();
        }
        else
        {
          reader.ReadStartElement("ProxyUserName");
        }
        if (!reader.IsEmptyElement)
        {
          reader.ReadStartElement("ProxyUserDomain");
          ProxyUserDomain = reader.ReadContentAsString();
          reader.ReadEndElement();
        }
        else
        {
          reader.ReadStartElement("ProxyUserDomain");
        }
        if (!reader.IsEmptyElement)
        {
          reader.ReadStartElement("_proxyPassword");
          _proxyPassword = reader.ReadContentAsString();
          reader.ReadEndElement();
        }
        else
        {
          reader.ReadStartElement("_proxyPassword");
        }
        if (!reader.IsEmptyElement)
        {
          reader.ReadStartElement("IsEnabledProxy");
          IsEnabledProxy = bool.Parse(reader.ReadContentAsString());
          reader.ReadEndElement();
        }
        else
        {
          reader.ReadStartElement("IsEnabledProxy");
        }
        if (!reader.IsEmptyElement)
        {
          reader.ReadStartElement("IsUseTwitterSearch");
          IsUseTwitterSearch = bool.Parse(reader.ReadContentAsString());
          reader.ReadEndElement();
        }
        else
        {
          reader.ReadStartElement("IsUseTwitterSearch");
        }
        if (!reader.IsEmptyElement)
        {
          reader.ReadStartElement("ShowGlobalFilter");
          ShowGlobalFilter = bool.Parse(reader.ReadContentAsString());
          reader.ReadEndElement();
        }
        else
        {
          reader.ReadStartElement("Language");
        }
        if (!reader.IsEmptyElement)
        {
          reader.ReadStartElement("Language");
          Language = reader.ReadContentAsString();
          reader.ReadEndElement();
        }
        else
        {
          reader.ReadStartElement("Language");
        }
        if (!reader.IsEmptyElement)
        {
          reader.ReadStartElement("BitLyUserName");
          BitLyUserName = reader.ReadContentAsString();
          reader.ReadEndElement();
        }
        else
        {
          reader.ReadStartElement("BitLyUserName");
        }
        if (!reader.IsEmptyElement)
        {
          reader.ReadStartElement("_bitLyPassword");
          _bitLyPassword = reader.ReadContentAsString();
          reader.ReadEndElement();
        }
        else
        {
          reader.ReadStartElement("_bitLyPassword");
        }
        if (reader.Name.Equals("ViewState"))
        {
          reader.ReadStartElement("ViewState");
          ViewState = Convert.ToBoolean(reader.ReadContentAsString());
          reader.ReadEndElement();
        }
        if (reader.Name.Equals("MinimizeWindowInTray"))
        {
          reader.ReadStartElement("MinimizeWindowInTray");
          if (!reader.IsEmptyElement)
          {
            MinimizeWindowInTray = Convert.ToBoolean(reader.ReadContentAsString());
            reader.ReadEndElement();
          }
        }
        if (reader.Name.Equals("LanguageTranslator"))
        {

          if (!reader.IsEmptyElement)
          {
            reader.ReadStartElement("LanguageTranslator");
            LanguageTranslator = (Language)Enum.Parse(typeof(Language), reader.ReadContentAsString(), true);
            reader.ReadEndElement();
          }
          else
          {
            reader.ReadStartElement("LanguageTranslator");
          }
        }
        if (reader.Name.Equals("SyncUser"))
        {

          if (!reader.IsEmptyElement)
          {
            reader.ReadStartElement("SyncUser");
            SyncUser = reader.ReadContentAsString();
            reader.ReadEndElement();
          }
          else
          {
            reader.ReadStartElement("SyncUser");
          }
        }

        if (reader.Name.Equals("WordSearch"))
        {

          if (!reader.IsEmptyElement)
          {
            reader.ReadStartElement("WordSearch");
            WordSearch = reader.ReadContentAsString();
            reader.ReadEndElement();
          }
          else
          {
            reader.ReadStartElement("WordSearch");
          }
        }
        if (reader.Name.Equals("SupportLogEmail"))
        {

          if (!reader.IsEmptyElement)
          {
            reader.ReadStartElement("SupportLogEmail");
            SupportLogEmail = reader.ReadContentAsString();
            reader.ReadEndElement();
          }
          else
          {
            reader.ReadStartElement("SupportLogEmail");
          }
        }
        if (reader.Name.Equals("IsSendByEnter"))
        {

          reader.ReadStartElement("IsSendByEnter");
          IsSendByEnter = bool.Parse(reader.ReadContentAsString());
          reader.ReadEndElement();

        }
        if (reader.Name.Equals("AsAskSync"))
        {

          reader.ReadStartElement("AsAskSync");
          AsAskSync = bool.Parse(reader.ReadContentAsString());
          reader.ReadEndElement();

        }
        if (reader.Name.Equals("DisableAds"))
        {
                reader.ReadStartElement("DisableAds");
                DisableAds = bool.Parse(reader.ReadContentAsString());
                reader.ReadEndElement();
        }
        reader.ReadEndElement();
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this,
                          ex);
      }
    }

    public void WriteXml(XmlWriter writer)
    {
      try
      {
        string accounts = GenericCollectionSerializer.SerializeObject(Accounts);
        writer.WriteElementString("Accounts", accounts);
        writer.WriteElementString("Theme", Theme);
        writer.WriteElementString("FontSizeValue", FontSizeValue.ToString());
        writer.WriteElementString("AlertsEnabled", AlertsEnabled.ToString());
        writer.WriteElementString("RunAtStartup", RunAtStartup.ToString());
        writer.WriteElementString("UrlShortener", UrlShortener.ToString());
        writer.WriteElementString("ShowTwitterFrom", ShowTwitterFrom.ToString());
        writer.WriteElementString("CloseBoxPublication", CloseBoxPublication.ToString());
        writer.WriteElementString("AlertsTwitterSearchEnabled", AlertsTwitterSearchEnabled.ToString());
        writer.WriteElementString("ProxyPort", ProxyPort.ToString());
        writer.WriteElementString("ProxyServer", ProxyServer);
        writer.WriteElementString("ProxyUserName", ProxyUserName);
        writer.WriteElementString("ProxyUserDomain", ProxyUserDomain);
        writer.WriteElementString("_proxyPassword", _proxyPassword);
        writer.WriteElementString("IsEnabledProxy", IsEnabledProxy.ToString());
        writer.WriteElementString("IsUseTwitterSearch", IsUseTwitterSearch.ToString());
        writer.WriteElementString("ShowGlobalFilter", ShowGlobalFilter.ToString());
        writer.WriteElementString("Language", Language);
        writer.WriteElementString("BitLyUserName", BitLyUserName);
        writer.WriteElementString("_bitLyPassword", _bitLyPassword);
        writer.WriteElementString("ViewState", ViewState.ToString());
        writer.WriteElementString("MinimizeWindowInTray", MinimizeWindowInTray.ToString());
        writer.WriteElementString("LanguageTranslator", LanguageTranslator.ToString());
        writer.WriteElementString("SyncUser", SyncUser);
        writer.WriteElementString("WordSearch", WordSearch);
        writer.WriteElementString("SupportLogEmail", SupportLogEmail);
        writer.WriteElementString("IsSendByEnter", IsSendByEnter.ToString());
        writer.WriteElementString("AsAskSync", AsAskSync.ToString());
        writer.WriteElementString("DisableAds", DisableAds.ToString());
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this,
                          ex);
      }
    }

    #endregion

    /// <summary>
    /// Raises this object's PropertyChanged event.
    /// </summary>
    /// <param name="propertyName">The property that has a new value.</param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
      //this.VerifyPropertyName(propertyName);

      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null)
      {
        var e = new PropertyChangedEventArgs(propertyName);
        handler(this, e);
      }
    }
  }
}