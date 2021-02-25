#region Includes

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Sobees.Configuration.BGlobals;
using Sobees.Tools.Logging;
using Sobees.Tools.Serialization;

#endregion

namespace Sobees.Infrastructure.Cls
{

  #region Enum

  public enum EnumAccountType
  {
    Twitter,
    Facebook,
    TwitterSearch,
    MySpace,
    LinkedIn,
    Rss,
    NyTimes,
    FacebookPage,
    SmartFeed
   }

  public enum EnumAlertsFacebookType
  {
    All,
    Advanced,
    No,
  }

  #endregion

  public class UserAccount : IXmlSerializable,
                             INotifyPropertyChanged
  {
    #region Fields

    private List<string> _alertsRemovedWordsList;
    private List<string> _alertsUsersList;
    private List<string> _alertsWordsList;
    private int _nbPostToGet;
    private int _nbMaxPosts;
    private List<string> _spamlist;

    #endregion

    #region Properties

    #region All Accounts Type

    public string Login { get; set; }
    private string _pictureURL;
    public string PictureUrl
    {
      get { return _pictureURL; }
      set
      {
        if (Type ==EnumAccountType.Twitter && !string.IsNullOrEmpty(value) && value.Contains("_normal"))
        {
          _pictureURL = value.Replace("_normal",string.Empty);
          return;
        }
        _pictureURL = value;
      }
    }

    public string PasswordHash { get; set; }

    public List<string> SpamList
    {
      get
      {
        if (_spamlist == null)
        {
          _spamlist = new List<string>();
        }
        return _spamlist;
      }
      set
      {
        _spamlist = value;
        OnPropertyChanged("SpamList");
      }
    }

    public long UserId { get; set; }

    public EnumAccountType Type { get; set; }

    public int NbPostToGet
    {
      get
      {
        if (_nbPostToGet <= 0)
        {
          _nbPostToGet = BGlobals.DEFAULT_NB_POST_TO_GET;
        }
        return _nbPostToGet;
      }
      set { _nbPostToGet = value; }
    }

    public int NbMaxPosts
    {
      get
      {
        if (_nbMaxPosts <= 0)
        {
          _nbMaxPosts = 200;
        }
        return _nbMaxPosts;
      }
      set { _nbMaxPosts = value; }
    }


    #region Signature

    public string Signature { get; set; }
    public bool IsSignatureActivated { get; set; }

    #endregion

    #endregion

    #region Activ / Unactiv Option

    public bool IsSpellCheckActivated { get; set; }
    public bool IsFanAsk { get; set; }
    public bool IsStatusClosedOk { get; set; }

    #endregion

    #region Facebook Property

    public string SessionKey { get; set; }
    public string Secret { get; set; }
    public string AuthToken { get; set; }

    #endregion

    #region Alerts

    #region all alerts

    public bool IsCheckedUseAlertsRemovedWords;
    public bool IsCheckedUseAlertsUsers;
    public bool IsCheckedUseAlertsWords;
    public EnumAlertsFacebookType TypeAlertsFB = EnumAlertsFacebookType.No;


    public List<string> AlertsWordsList
    {
      get
      {
        if (_alertsWordsList == null)
        {
          _alertsWordsList = new List<string>();
        }
        return _alertsWordsList;
      }
      set
      {
        _alertsWordsList = value;
        OnPropertyChanged("AlertsWordsList");
      }
    }

    public List<string> AlertsUsersList
    {
      get
      {
        if (_alertsUsersList == null)
        {
          _alertsUsersList = new List<string>();
        }
        return _alertsUsersList;
      }
      set
      {
        _alertsUsersList = value;
        OnPropertyChanged("AlertsUsersList");
      }
    }

    public List<string> AlertsRemovedWordsList
    {
      get
      {
        if (_alertsRemovedWordsList == null)
        {
          _alertsRemovedWordsList = new List<string>();
        }
        return _alertsRemovedWordsList;
      }
      set
      {
        _alertsRemovedWordsList = value;
        OnPropertyChanged("AlertsRemovedWordsList");
      }
    }

    #endregion

    #region Twitter Alert

    public bool IsCheckedUseAlertsDM = true;
    public bool IsCheckedUseAlertsFriends = true;
    public bool IsCheckedUseAlertsGroups = true;
    public bool IsCheckedUseAlertsReply = true;

    #endregion

    #endregion

    #endregion

    #region Constructors

    public UserAccount(string login, string pwdHash)
    {
      Login = login;
      Type = EnumAccountType.Twitter;
      PasswordHash = pwdHash;
    }

    public UserAccount()
    {
      IsSpellCheckActivated = true;
    }

    public UserAccount(string login, string pwdHash, EnumAccountType type)
    {
      Login = login;
      Type = type;
      PasswordHash = pwdHash;
    }

    public UserAccount(string login)
    {
      Login = login;
    }
    public UserAccount(UserAccount account)
    {
      Login = account.Login;
      PasswordHash = account.PasswordHash;
      UserId = account.UserId;
      SpamList = new List<string>();
      foreach (var spam in account.SpamList)
      {
        SpamList.Add(spam);
      }
      Type = account.Type;
      NbMaxPosts = account.NbMaxPosts;
      NbPostToGet = account.NbPostToGet;
      Signature = account.Signature;
      IsSignatureActivated = account.IsSignatureActivated;
      IsSpellCheckActivated = account.IsSignatureActivated;
      IsStatusClosedOk = account.IsStatusClosedOk;
      SessionKey = account.SessionKey;
      Secret = account.Secret;
      AuthToken = account.AuthToken;
      AlertsWordsList = new List<string>();
      foreach (var spam in account.AlertsWordsList)
      {
        AlertsWordsList.Add(spam);
      }
      AlertsUsersList = new List<string>();
      foreach (var spam in account.AlertsUsersList)
      {
        AlertsUsersList.Add(spam);
      }
      AlertsRemovedWordsList = new List<string>();
      foreach (var spam in account.AlertsRemovedWordsList)
      {
        AlertsRemovedWordsList.Add(spam);
      }
      TypeAlertsFB = account.TypeAlertsFB;
      PictureUrl = account.PictureUrl;

    }
    public UserAccount(string login, EnumAccountType type)
    {
      Login = login;
      Type = type;
    }

    #endregion

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    #region IXmlSerializable Members

    public void ReadXml(XmlReader reader)
    {
      try
      {
        reader.MoveToContent();
        reader.Read();

        if (!reader.IsEmptyElement)
        {
          reader.ReadStartElement("Login");
          Login = reader.ReadContentAsString();
          reader.ReadEndElement();
        }
        else
        {
          reader.ReadStartElement("Login");
        }
        if (!reader.IsEmptyElement)
        {
          reader.ReadStartElement("PasswordHash");
          PasswordHash = reader.ReadContentAsString();
          reader.ReadEndElement();
        }
        else
        {
          reader.ReadStartElement("PasswordHash");
        }



        reader.ReadStartElement("SpamList");
        SpamList = GenericCollectionSerializer.DeserializeObject<List<string>>(reader.ReadContentAsString());
        reader.ReadEndElement();

        reader.ReadStartElement("Type");
        Type = GenericCollectionSerializer.DeserializeObject<EnumAccountType>(reader.ReadContentAsString());
        reader.ReadEndElement();
        if (!reader.IsEmptyElement)
        {
          reader.ReadStartElement("SessionKey");
          SessionKey = reader.ReadContentAsString();
          reader.ReadEndElement();
        }
        else
        {
          reader.ReadStartElement("SessionKey");
        }
        if (!reader.IsEmptyElement)
        {
          reader.ReadStartElement("Secret");
          Secret = reader.ReadContentAsString();
          reader.ReadEndElement();
        }
        else
        {
          reader.ReadStartElement("Secret");
        }

        if (!reader.IsEmptyElement)
        {
          reader.ReadStartElement("AuthToken");
          AuthToken = reader.ReadContentAsString();
          reader.ReadEndElement();
        }
        else
        {
          reader.ReadStartElement("AuthToken");
        }

        if (!reader.IsEmptyElement)
        {
          reader.ReadStartElement("IsSpellCheckActivated");
          IsSpellCheckActivated = bool.Parse(reader.ReadContentAsString());
          reader.ReadEndElement();
        }
        else
        {
          reader.ReadStartElement("IsSpellCheckActivated");
        }

        if (!reader.IsEmptyElement)
        {
          reader.ReadStartElement("IsStatusClosedOk");
          IsStatusClosedOk = bool.Parse(reader.ReadContentAsString());
          reader.ReadEndElement();
        }
        else
        {
          reader.ReadStartElement("IsStatusClosedOk");
        }
        if (!reader.IsEmptyElement)
        {
          reader.ReadStartElement("NbPostToGet");
          NbPostToGet = int.Parse(reader.ReadContentAsString());
          reader.ReadEndElement();
        }
        else
        {
          reader.ReadStartElement("NbPostToGet");
        }

        reader.ReadStartElement("TypeAlertsFB");
        TypeAlertsFB =
          GenericCollectionSerializer.DeserializeObject<EnumAlertsFacebookType>(reader.ReadContentAsString());
        reader.ReadEndElement();

        reader.ReadStartElement("AlertsUsersList");
        AlertsUsersList = GenericCollectionSerializer.DeserializeObject<List<string>>(reader.ReadContentAsString());
        reader.ReadEndElement();

        reader.ReadStartElement("AlertsWordsList");
        AlertsWordsList = GenericCollectionSerializer.DeserializeObject<List<string>>(reader.ReadContentAsString());
        reader.ReadEndElement();

        reader.ReadStartElement("AlertsRemovedWordsList");
        AlertsRemovedWordsList =
          GenericCollectionSerializer.DeserializeObject<List<string>>(reader.ReadContentAsString());
        reader.ReadEndElement();

        if (!reader.IsEmptyElement)
        {
          reader.ReadStartElement("IsCheckedUseAlertsWords");
          IsCheckedUseAlertsWords = bool.Parse(reader.ReadContentAsString());
          reader.ReadEndElement();
        }
        else
        {
          reader.ReadStartElement("IsCheckedUseAlertsWords");
        }

        if (!reader.IsEmptyElement)
        {
          reader.ReadStartElement("IsCheckedUseAlertsUsers");
          IsCheckedUseAlertsUsers = bool.Parse(reader.ReadContentAsString());
          reader.ReadEndElement();
        }
        else
        {
          reader.ReadStartElement("IsCheckedUseAlertsUsers");
        }

        if (!reader.IsEmptyElement)
        {
          reader.ReadStartElement("IsCheckedUseAlertsRemovedWords");
          IsCheckedUseAlertsRemovedWords = bool.Parse(reader.ReadContentAsString());
          reader.ReadEndElement();
        }
        else
        {
          reader.ReadStartElement("IsCheckedUseAlertsRemovedWords");
        }

        if (!reader.IsEmptyElement)
        {
          reader.ReadStartElement("IsCheckedUseAlertsDM");
          IsCheckedUseAlertsDM = bool.Parse(reader.ReadContentAsString());
          reader.ReadEndElement();
        }
        else
        {
          reader.ReadStartElement("IsCheckedUseAlertsDM");
        }
        if (!reader.IsEmptyElement)
        {
          reader.ReadStartElement("IsCheckedUseAlertsFriends");
          IsCheckedUseAlertsFriends = bool.Parse(reader.ReadContentAsString());
          reader.ReadEndElement();
        }
        else
        {
          reader.ReadStartElement("IsCheckedUseAlertsFriends");
        }
        if (!reader.IsEmptyElement)
        {
          reader.ReadStartElement("IsCheckedUseAlertsGroups");
          IsCheckedUseAlertsGroups = bool.Parse(reader.ReadContentAsString());
          reader.ReadEndElement();
        }
        else
        {
          reader.ReadStartElement("IsCheckedUseAlertsGroups");
        }
        if (!reader.IsEmptyElement)
        {
          reader.ReadStartElement("IsCheckedUseAlertsReply");
          IsCheckedUseAlertsReply = bool.Parse(reader.ReadContentAsString());
          reader.ReadEndElement();
        }
        else
        {
          reader.ReadStartElement("IsCheckedUseAlertsReply");
        }
        if (!reader.IsEmptyElement)
        {
          reader.ReadStartElement("IsSignatureActivated");
          IsSignatureActivated = bool.Parse(reader.ReadContentAsString());
          reader.ReadEndElement();
        }
        else
        {
          reader.ReadStartElement("IsSignatureActivated");
        }
        if (!reader.IsEmptyElement)
        {
          reader.ReadStartElement("Signature");
          Signature = reader.ReadContentAsString();
          reader.ReadEndElement();
        }
        else
        {
          reader.ReadStartElement("Signature");
        }

        if (reader.Name.Equals("UserId"))
        {
          if (!reader.IsEmptyElement)
          {
            reader.ReadStartElement("UserId");
            UserId = long.Parse(reader.ReadContentAsString());
            reader.ReadEndElement();
          }
          else
          {
            reader.ReadStartElement("UserId");
          }
        }


        if (!reader.IsEmptyElement)
        {
          reader.ReadStartElement("NbMaxPosts");
          NbMaxPosts = int.Parse(reader.ReadContentAsString());
          reader.ReadEndElement();
        }
        else
        {
          reader.ReadStartElement("NbMaxPosts");
        }
        if (reader.Name.Equals("PictureUrl"))
        {
          if (!reader.IsEmptyElement)
          {
            reader.ReadStartElement("PictureUrl");
            PictureUrl = reader.ReadContentAsString();
            reader.ReadEndElement();
          }
          else
          {
            reader.ReadStartElement("PictureUrl");
          }
        }
        if (reader.Name.Equals("IsFanAsk"))
        {
          if (!reader.IsEmptyElement)
          {
            reader.ReadStartElement("IsFanAsk");
            IsFanAsk = bool.Parse(reader.ReadContentAsString());
            reader.ReadEndElement();
          }
          else
          {
            reader.ReadStartElement("IsFanAsk");
          }
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
        writer.WriteElementString("Login",
                                  Login);
        writer.WriteElementString("PasswordHash",
                                  PasswordHash);
        var spamList = GenericCollectionSerializer.SerializeObject(SpamList);
        writer.WriteElementString("SpamList",
                                  spamList);
        var type = GenericCollectionSerializer.SerializeObject(Type);
        writer.WriteElementString("Type", type);
        writer.WriteElementString("SessionKey", SessionKey);
        writer.WriteElementString("Secret", Secret);
        writer.WriteElementString("AuthToken", AuthToken);
        writer.WriteElementString("IsSpellCheckActivated", IsSpellCheckActivated.ToString());
        writer.WriteElementString("IsStatusClosedOk", IsStatusClosedOk.ToString());
        writer.WriteElementString("NbPostToGet", NbPostToGet.ToString());
        var typeAlertsFB = GenericCollectionSerializer.SerializeObject(TypeAlertsFB);
        writer.WriteElementString("TypeAlertsFB", typeAlertsFB);
        var alertsUserList = GenericCollectionSerializer.SerializeObject(AlertsUsersList);
        writer.WriteElementString("AlertsUsersList", alertsUserList);
        var alertsWordsList = GenericCollectionSerializer.SerializeObject(AlertsWordsList);
        writer.WriteElementString("AlertsWordsList", alertsWordsList);
        var alertsRemovedWordsList = GenericCollectionSerializer.SerializeObject(AlertsRemovedWordsList);
        writer.WriteElementString("AlertsRemovedWordsList", alertsRemovedWordsList);
        writer.WriteElementString("IsCheckedUseAlertsWords", IsCheckedUseAlertsWords.ToString());
        writer.WriteElementString("IsCheckedUseAlertsUsers", IsCheckedUseAlertsUsers.ToString());
        writer.WriteElementString("IsCheckedUseAlertsRemovedWords", IsCheckedUseAlertsRemovedWords.ToString());
        writer.WriteElementString("IsCheckedUseAlertsDM", IsCheckedUseAlertsDM.ToString());
        writer.WriteElementString("IsCheckedUseAlertsFriends", IsCheckedUseAlertsFriends.ToString());
        writer.WriteElementString("IsCheckedUseAlertsGroups", IsCheckedUseAlertsGroups.ToString());
        writer.WriteElementString("IsCheckedUseAlertsReply", IsCheckedUseAlertsReply.ToString());
        writer.WriteElementString("IsSignatureActivated", IsSignatureActivated.ToString());
        writer.WriteElementString("Signature", Signature);

        writer.WriteElementString("UserId", UserId.ToString());

        writer.WriteElementString("NbMaxPosts", NbMaxPosts.ToString());
        writer.WriteElementString("PictureUrl", PictureUrl);
        writer.WriteElementString("IsFanAsk", IsFanAsk.ToString());

      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this,
                          ex);
      }
    }

    public XmlSchema GetSchema()
    {
      return GetSchema();
    }

    #endregion

    #region Methods

    protected void OnPropertyChanged(string propertyName)
    {
      var handler = PropertyChanged;
      if (handler != null)
        handler(this,
                new PropertyChangedEventArgs(propertyName));
    }

    public bool Equals(UserAccount other)
    {
      if (ReferenceEquals(null,
                          other)) return false;
      if (ReferenceEquals(this,
                          other)) return true;
      return Equals(other.Login,
                    Login) && Equals(other.Type,
                                     Type);
    }

    public override bool Equals(object obj)
    {
      if (obj != null)
      {
        if (obj.GetType().Equals(typeof(UserAccount)))
        {
          var e = obj as UserAccount;
          if (e != null && !string.IsNullOrEmpty(e.Login))
          {
            return (e.Login.Equals(Login) && e.Type.Equals(Type));
          }
        }
      }
      return false;
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return ((Login != null ? Login.GetHashCode() : 0) * 397) ^ Type.GetHashCode();
      }
    }

    public void DisconnectAccountFB()
    {
#if !SILVERLIGHT
      //FacebookHelper.FacebookService.LogOff();
      Secret = string.Empty;
      SessionKey = string.Empty;
      AuthToken = string.Empty;
#endif
    }

    #endregion
    public override string ToString()
    {
        return Login;
    }
  }

  public class UserAccountComparer : IEqualityComparer<UserAccount>
  {
    #region IEqualityComparer<UserAccount> Members

    public bool Equals(UserAccount x,
                       UserAccount y)
    {
      if (x.Login.Equals(y.Login) && x.Type.Equals(y.Type))
        return true;
      return false;
    }


    public int GetHashCode(UserAccount e)
    {
      return e.Login.ToLower().Trim().GetHashCode();
    }

    #endregion
  }
}