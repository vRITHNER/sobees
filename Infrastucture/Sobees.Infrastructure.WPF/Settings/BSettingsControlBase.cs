using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Sobees.Configuration.BGlobals;

namespace Sobees.Infrastructure.Settings
{
  public abstract class BSettingsControlBase : IXmlSerializable
  {
    #region Fields

    private int _nbMaxPosts;
    protected int _nbPostToGet;
    private double _refreshTime;

    #endregion

    #region Properties

    public DateTime DateLastUpdate { get; set; }
    public string UserName { get; set; }

    public double RefreshTime
    {
      get
      {
        if (_refreshTime <= 0)
          _refreshTime = BGlobals.DEFAULT_REFRESH_TIME_SERVICE;
        return _refreshTime;
      }
      set { _refreshTime = value; }
    }

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
          _nbMaxPosts = BGlobals.DEFAULT_NB_POST_TO_KEEP;
        }
        return _nbMaxPosts;
      }
      set { _nbMaxPosts = value; }
    }

    public bool IsSpellCheckActivated { get; set; }

		public bool IsWebApp { get; set; }

    #endregion

    #region IXmlSerializable Members

    public abstract XmlSchema GetSchema();

    public abstract void ReadXml(XmlReader reader);

    public abstract void WriteXml(XmlWriter writer);

    #endregion

    protected void ReadBaseXML(XmlReader reader)
    {
      if (reader.Name.Equals("UserName"))
      {
        if (!reader.IsEmptyElement)
        {
          reader.ReadStartElement("UserName");
          UserName = reader.ReadContentAsString();
          reader.ReadEndElement();
        }
        else
        {
          reader.ReadStartElement("UserName");
        }
      }

      if (reader.Name.Equals("DateLastUpdate"))
      {
        if (!reader.IsEmptyElement)
        {
          reader.ReadStartElement("DateLastUpdate");
          DateLastUpdate = DateTime.Parse(reader.ReadContentAsString());
          reader.ReadEndElement();
        }
        else
        {
          reader.ReadStartElement("DateLastUpdate");
        }
      }

      if (reader.Name.Equals("NbPostToGet"))
      {
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
      }
      if (reader.Name.Equals("RefreshTime"))
      {
        if (!reader.IsEmptyElement)
        {
          reader.ReadStartElement("RefreshTime");
          RefreshTime = double.Parse(reader.ReadContentAsString());
          reader.ReadEndElement();
        }
        else
        {
          reader.ReadStartElement("RefreshTime");
        }
      }
      if (reader.Name.Equals("NbMaxPosts"))
      {
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
      }
			if (reader.Name.Equals("IsWebApp"))
			{
				if (!reader.IsEmptyElement)
				{
					reader.ReadStartElement("IsWebApp");
					IsWebApp = bool.Parse(reader.ReadContentAsString());
					reader.ReadEndElement();
				}
				else
				{
					reader.ReadStartElement("IsWebApp");
				}
			}
    }

    protected void WriteBaseXML(XmlWriter writer)
    {
      writer.WriteElementString("UserName", UserName);
      writer.WriteElementString("DateLastUpdate", DateLastUpdate.ToString());
      writer.WriteElementString("NbPostToGet", NbPostToGet.ToString());
      writer.WriteElementString("RefreshTime", RefreshTime.ToString());
      writer.WriteElementString("NbMaxPosts", NbMaxPosts.ToString());
			writer.WriteElementString("IsWebApp", IsWebApp.ToString());
    }
  }
}