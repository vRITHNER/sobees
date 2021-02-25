#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml;
using System.Xml.Schema;
using Sobees.Infrastructure.Settings;
using Sobees.Tools.Logging;
using Sobees.Tools.Serialization;

#endregion

namespace Sobees.Controls.Twitter.Cls
{
  public class TwitterSettings : BSettingsControlBase
  {
    private string _cursorFriends = "-1";
    private double _slDmsValue = 10;
    private double _slFriendsValue = 5;
    private double _slListValue = 5;
    private double _slRepliesValue = 10;
    private double _slUserValue = 10;
    private List<TwitterWorkspaceSettings> _workspaceSettings;
    private ObservableCollection<TwitterWorkspaceSettings> _workspaceSettingsCollection;

    public TwitterSettings()
    {
      IsSpellCheckActivated = true;
      ViewStateTweets = true;
    }

    public TwitterSettings(string login, string pwdHash, bool viewState, bool showApiUsage,
                           List<TwitterWorkspaceSettings> workspaceSettings)
    {
      UserName = login;
      PasswordHash = pwdHash;
      ViewState = viewState;
      ViewStateTweets = true;
      ShowApiUsage = showApiUsage;
      WorkspaceSettings = workspaceSettings ?? new List<TwitterWorkspaceSettings>();
    }

    public string PasswordHash { get; set; }

    public string CursorFriends
    {
      get { return _cursorFriends; }
      set { _cursorFriends = value; }
    }

    public bool ViewState { get; set; }

    public bool ViewStateTweets { get; set; }

    public bool ViewRrafIcon { get; set; }

    //private bool _showFactery = true;
    //public bool ShowFactery
    //{
    //  get { return _showFactery; }
    //  set { _showFactery = value; }
    //}

    public ObservableCollection<TwitterWorkspaceSettings> WorkspaceSettingsCollection
    {
      get
      {
        if (_workspaceSettingsCollection == null)
        {
          _workspaceSettingsCollection = new ObservableCollection<TwitterWorkspaceSettings>();
        }
        return _workspaceSettingsCollection;
      }
      set
      {
        _workspaceSettingsCollection = value;
        OnPropertyChanged("WorkspaceSettingsCollection");
      }
    }

    public List<TwitterWorkspaceSettings> WorkspaceSettings
    {
      get { return _workspaceSettings; }
      set
      {
        _workspaceSettings = value;
        if (value == null)
        {
          return;
        }
#if !SILVERLIGHT
        WorkspaceSettingsCollection = new ObservableCollection<TwitterWorkspaceSettings>(_workspaceSettings);

#else
        WorkspaceSettingsCollection = new ObservableCollection<TwitterWorkspaceSettings>();
        foreach (var workspaceSetting in _workspaceSettings)
        {
          WorkspaceSettingsCollection.Add(workspaceSetting);
        }
#endif
      }
    }

    public List<string> SpamList { get; set; }

    public bool ShowApiUsage { get; set; }

    public double SlRepliesValue
    {
      get
      {
        if (_slRepliesValue <= 0)
          _slRepliesValue = 10;
        return _slRepliesValue;
      }
      set { _slRepliesValue = value; }
    }

    public double SlDmsValue
    {
      get
      {
        if (_slDmsValue <= 0)
          _slDmsValue = 10;
        return _slDmsValue;
      }
      set { _slDmsValue = value; }
    }

    public double SlListValue
    {
      get
      {
        if (_slListValue <= 0)
          _slListValue = 5;
        return _slListValue;
      }
      set { _slListValue = value; }
    }

    public double SlFriendsValue
    {
      get
      {
        if (_slFriendsValue <= 0)
          _slFriendsValue = 5;
        return _slFriendsValue;
      }
      set { _slFriendsValue = value; }
    }

    public double SlUserValue
    {
      get
      {
        if (_slUserValue <= 0)
          _slUserValue = 5;
        return _slUserValue;
      }
      set { _slUserValue = value; }
    }

    public bool ShowRepliesHome { get; set; }

    public bool ShowDMHome { get; set; }

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion INotifyPropertyChanged Members

    #region IXmlSerializable Members

    public override XmlSchema GetSchema()
    {
      return null;
    }

    public override void ReadXml(XmlReader reader)
    {
      try
      {
        reader.MoveToContent();
        reader.Read();
        if (reader.Name.Equals("Login"))
        {
          if (reader.IsEmptyElement)
          {
            reader.ReadStartElement("Login");
          }
          else
          {
            reader.ReadStartElement("Login");
            UserName = reader.ReadContentAsString();
            reader.ReadEndElement();
          }
        }
        if (reader.Name.Equals("PasswordHash"))
        {
          if (!reader.HasValue)
          {
            reader.ReadStartElement("PasswordHash");
          }
          else
          {
            reader.ReadStartElement("PasswordHash");
            PasswordHash = reader.ReadContentAsString();
            reader.ReadEndElement();
          }
        }
        reader.ReadStartElement("ViewState");
        ViewState = bool.Parse(reader.ReadContentAsString());
        reader.ReadEndElement();

        reader.ReadStartElement("WorkspaceSettings");
        WorkspaceSettings =
          GenericCollectionSerializer.DeserializeObject<List<TwitterWorkspaceSettings>>(reader.ReadContentAsString());
        reader.ReadEndElement();

        reader.ReadStartElement("SpamList");
        SpamList = GenericCollectionSerializer.DeserializeObject<List<string>>(reader.ReadContentAsString());
        reader.ReadEndElement();

        reader.ReadStartElement("IsSpellCheckActivated");
        IsSpellCheckActivated = bool.Parse(reader.ReadContentAsString());
        reader.ReadEndElement();

        reader.ReadStartElement("ShowApiUsage");
        ShowApiUsage = bool.Parse(reader.ReadContentAsString());
        reader.ReadEndElement();

        reader.ReadStartElement("Rpp");
        NbPostToGet = int.Parse(reader.ReadContentAsString());
        reader.ReadEndElement();

        reader.ReadStartElement("MaxTweets");
        NbMaxPosts = int.Parse(reader.ReadContentAsString());
        reader.ReadEndElement();

        reader.ReadStartElement("ViewStateTweets");
        ViewStateTweets = bool.Parse(reader.ReadContentAsString());
        reader.ReadEndElement();

        reader.ReadStartElement("ViewRrafIcon");
        ViewRrafIcon = bool.Parse(reader.ReadContentAsString());
        reader.ReadEndElement();

        reader.ReadStartElement("SlRepliesValue");
        SlRepliesValue = double.Parse(reader.ReadContentAsString());
        reader.ReadEndElement();

        reader.ReadStartElement("SlDmsValue");
        SlDmsValue = double.Parse(reader.ReadContentAsString());
        reader.ReadEndElement();

        reader.ReadStartElement("SlListValue");
        SlListValue = double.Parse(reader.ReadContentAsString());
        reader.ReadEndElement();

        reader.ReadStartElement("SlFriendsValue");
        SlFriendsValue = double.Parse(reader.ReadContentAsString());
        reader.ReadEndElement();

        reader.ReadStartElement("SlUserValue");
        SlUserValue = double.Parse(reader.ReadContentAsString());
        reader.ReadEndElement();
        if (reader.Name.Equals("ShowFactery"))
        {
          reader.ReadStartElement("ShowFactery");

          //ShowFactery = bool.Parse(reader.ReadContentAsString());
          reader.ReadEndElement();
        }
        if (reader.Name.Equals("ShowDMHome"))
        {
          reader.ReadStartElement("ShowDMHome");
          ShowDMHome = bool.Parse(reader.ReadContentAsString());
          reader.ReadEndElement();
        }

        if (reader.Name.Equals("ShowRepliesHome"))
        {
          reader.ReadStartElement("ShowRepliesHome");
          ShowRepliesHome = bool.Parse(reader.ReadContentAsString());
          reader.ReadEndElement();
        }
        if (reader.Name.Equals("DateLastUpdate"))
        {
          reader.ReadStartElement("DateLastUpdate");
          DateLastUpdate = DateTime.Parse(reader.ReadContentAsString());
          reader.ReadEndElement();
        }
        if (reader.Name.Equals("CursorFriends"))
        {
          reader.ReadStartElement("CursorFriends");
          CursorFriends = reader.ReadContentAsString();
          reader.ReadEndElement();
        }

        reader.ReadEndElement();
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    public override void WriteXml(XmlWriter writer)
    {
      try
      {
        writer.WriteElementString("Login", UserName);
        writer.WriteElementString("PasswordHash", PasswordHash);
        writer.WriteElementString("ViewState", ViewState.ToString());
        var workspaceSettings = GenericCollectionSerializer.SerializeObject(GetTwitterWorkspaceSettingsList());
        writer.WriteElementString("WorkspaceSettings", workspaceSettings);
        var spamList = GenericCollectionSerializer.SerializeObject(SpamList);
        writer.WriteElementString("SpamList", spamList);
        writer.WriteElementString("IsSpellCheckActivated", IsSpellCheckActivated.ToString());
        writer.WriteElementString("ShowApiUsage", ShowApiUsage.ToString());
        writer.WriteElementString("Rpp", NbPostToGet.ToString());
        writer.WriteElementString("MaxTweets", NbMaxPosts.ToString());
        writer.WriteElementString("ViewStateTweets", ViewStateTweets.ToString());
        writer.WriteElementString("ViewRrafIcon", ViewRrafIcon.ToString());

        writer.WriteElementString("SlRepliesValue", SlRepliesValue.ToString());
        writer.WriteElementString("SlDmsValue", SlDmsValue.ToString());
        writer.WriteElementString("SlListValue", SlListValue.ToString());
        writer.WriteElementString("SlFriendsValue", SlFriendsValue.ToString());
        writer.WriteElementString("SlUserValue", SlUserValue.ToString());

        //writer.WriteElementString("ShowFactery", ShowFactery.ToString());
        writer.WriteElementString("ShowDMHome", ShowDMHome.ToString());
        writer.WriteElementString("ShowRepliesHome", ShowRepliesHome.ToString());
        writer.WriteElementString("DateLastUpdate", DateLastUpdate.ToString());
        writer.WriteElementString("CursorFriends", CursorFriends);
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    #endregion IXmlSerializable Members

    private List<TwitterWorkspaceSettings> GetTwitterWorkspaceSettingsList()
    {
      _workspaceSettings = new List<TwitterWorkspaceSettings>();
      foreach (var wsettings in WorkspaceSettingsCollection)
      {
        if (wsettings != null)
        {
          var ws = new TwitterWorkspaceSettings
                     {
                       Type = wsettings.Type,
                       DateLastUpdate = wsettings.DateLastUpdate,
                       RefreshTime = wsettings.RefreshTime,
                       GroupName = wsettings.GroupName,
                       GroupMembers = wsettings.GroupMembers,
                       Count = wsettings.Count,
                       PageNb = wsettings.PageNb,
                       UserToGet = wsettings.UserToGet,
                       ColumnInGrid = wsettings.ColumnInGrid,
                       ColumnInGridWidth = wsettings.ColumnInGridWidth,
                       MaxTweets = wsettings.MaxTweets
                     };
          _workspaceSettings.Add(ws);
        }
      }

      return _workspaceSettings;
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
      var handler = PropertyChanged;
      if (handler != null)
      {
        var e = new PropertyChangedEventArgs(propertyName);
        handler(this, e);
      }
    }
  }
}