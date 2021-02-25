using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using Sobees.Infrastructure.Cls;
using Sobees.Infrastructure.Settings;
using Sobees.Library.BGenericLib;
using Sobees.Tools.Serialization;

namespace Sobees.Controls.TwitterSearch.Cls
{
  public class TwitterSearchSettings : BSettingsControlBase
  {
    #region Fields

    private string _geoCode;
    private double _refreshTimeFF;
    private double _refreshTimeOR;
    private double _refreshTimeTS;
    private List<TwitterSearchWorkspaceSettings> _twitterSearchWorkspaceSettings;

    #endregion

    #region Properties

    private EnumLanguages _language = EnumLanguages.all;
    public EnumLanguages Language
    {
      get { return _language; }
      set { _language = value; }
    }

    private bool _showTwitter = SobeesSettingsLocator.SobeesSettingsStatic.IsUseTwitterSearch;

    private bool _showFacebook = true;

    public bool ShowTwitter
    {
      get { return _showTwitter; }
      set { _showTwitter = value; }
    }

    public bool ShowFacebook
    {
      get { return _showFacebook; }
      set { _showFacebook = value; }
    }

    public double RefreshTimeFF
    {
      get
      {
        if (_refreshTimeFF <= 0)
          _refreshTimeFF = 10;
        return _refreshTimeFF;
      }
      set { _refreshTimeFF = value; }
    }

    public double RefreshTimeOR
    {
      get
      {
        if (_refreshTimeOR <= 0)
          _refreshTimeOR = 15;
        return _refreshTimeOR;
      }
      set { _refreshTimeOR = value; }
    }

    public double RefreshTimeTS
    {
      get
      {
        if (_refreshTimeTS <= 0)
          _refreshTimeTS = 5;
        return _refreshTimeTS;
      }
      set { _refreshTimeTS = value; }
    }


    public bool ViewRrafIcon { get; set; }


    public string GeoCode
    {
      get
      {
        if (_geoCode == null)
          _geoCode = string.Empty;
        return _geoCode;
      }
      set { _geoCode = value; }
    }

    public bool ViewStateTweets { get; set; }

    public List<TwitterSearchWorkspaceSettings> TwitterSearchWorkspaceSettings
    {
      get
      {
        if (_twitterSearchWorkspaceSettings == null)
          _twitterSearchWorkspaceSettings = new List<TwitterSearchWorkspaceSettings>();
        return _twitterSearchWorkspaceSettings;
      }
      set
      {
        _twitterSearchWorkspaceSettings = value;
        //OnPropertyChanged("TwitterSearchWorkspaceSettings");
      }
    }

    #endregion

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

        reader.ReadStartElement("RefreshTime");
        RefreshTime = double.Parse(reader.ReadContentAsString());
        reader.ReadEndElement();


        reader.ReadStartElement("Rpp");
        NbPostToGet = int.Parse(reader.ReadContentAsString());
        reader.ReadEndElement();

        //reader.ReadStartElement("GeoCode");
        //GeoCode = reader.ReadContentAsString();
        //reader.ReadEndElement();

        reader.ReadStartElement("Language");
        Language = (EnumLanguages)Enum.Parse(typeof(EnumLanguages), reader.ReadContentAsString(), true);
        Language = EnumLanguages.all;
        reader.ReadEndElement();

        reader.ReadStartElement("TwitterSearchWorkspaceSettings");
        TwitterSearchWorkspaceSettings =
          GenericCollectionSerializer.DeserializeObject<List<TwitterSearchWorkspaceSettings>>(reader.ReadContentAsString());
        reader.ReadEndElement();

        reader.ReadStartElement("MaxTweets");
        NbMaxPosts = int.Parse(reader.ReadContentAsString());
        reader.ReadEndElement();

        reader.ReadStartElement("RefreshTimeFF");
        RefreshTimeFF = double.Parse(reader.ReadContentAsString());
        reader.ReadEndElement();

        reader.ReadStartElement("RefreshTimeOR");
        RefreshTimeOR = double.Parse(reader.ReadContentAsString());
        reader.ReadEndElement();

        reader.ReadStartElement("RefreshTimeTS");
        RefreshTimeTS = double.Parse(reader.ReadContentAsString());
        reader.ReadEndElement();
        reader.ReadStartElement("ViewStateTweets");
        ViewStateTweets = bool.Parse(reader.ReadContentAsString());
        reader.ReadEndElement();

        reader.ReadStartElement("ViewRrafIcon");
        ViewRrafIcon = bool.Parse(reader.ReadContentAsString());
        reader.ReadEndElement();

        if (reader.Name.Equals("ShowTwitter"))
        {
          reader.ReadStartElement("ShowTwitter");
          ShowTwitter = bool.Parse(reader.ReadContentAsString());
          reader.ReadEndElement();
        }

        if (reader.Name.Equals("ShowFacebook"))
        {
          reader.ReadStartElement("ShowFacebook");
          ShowFacebook = bool.Parse(reader.ReadContentAsString());
          reader.ReadEndElement();
        }

        reader.ReadEndElement();
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }
    }

    public override void WriteXml(XmlWriter writer)
    {
      writer.WriteElementString("RefreshTime", RefreshTime.ToString());
      writer.WriteElementString("Rpp", NbPostToGet.ToString());

      writer.WriteElementString("Language", Language.ToString());

      string workspaceSearchSettings = GenericCollectionSerializer.SerializeObject(TwitterSearchWorkspaceSettings);
      writer.WriteElementString("TwitterSearchWorkspaceSettings", workspaceSearchSettings);
      writer.WriteElementString("MaxTweets", NbMaxPosts.ToString());
      writer.WriteElementString("RefreshTimeFF", RefreshTimeFF.ToString());
      writer.WriteElementString("RefreshTimeOR", RefreshTimeOR.ToString());
      writer.WriteElementString("RefreshTimeTS", RefreshTimeTS.ToString());
      writer.WriteElementString("ViewStateTweets", ViewStateTweets.ToString());
      writer.WriteElementString("ViewRrafIcon", ViewRrafIcon.ToString());
      writer.WriteElementString("ShowTwitter", ShowTwitter.ToString());
      writer.WriteElementString("ShowFacebook", ShowFacebook.ToString());
    }

  }
}