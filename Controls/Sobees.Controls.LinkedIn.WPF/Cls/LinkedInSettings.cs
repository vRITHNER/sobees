using System;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using System.Xml.Schema;
using Sobees.Infrastructure.Settings;
using Sobees.Tools.Logging;

namespace Sobees.Controls.LinkedIn.Cls
{
  public class LinkedInSettings : BSettingsControlBase
  {
    private double _refreshTime;
    public LinkedInSettings()
    {
      ShowAPPS = true;
      ShowCONN = true;
      ShowJGRP = true;
      ShowJOBS = true;
      ShowOTHER = true;
      ShowPRFU = true;
      ShowRECU = true;
      ShowSTAT = true;
    }
    public new double RefreshTime
    {
      get
      {
        if (_refreshTime <= 0)
          _refreshTime = 10;
        return _refreshTime;
      }
      set { _refreshTime = value; }
    }
    public bool ShowCONN { get; set; }

    public bool ShowSTAT { get; set; }

    public bool ShowAPPS { get; set; }

    public bool ShowJOBS { get; set; }

    public bool ShowJGRP { get; set; }

    public bool ShowRECU { get; set; }

    public bool ShowPRFU { get; set; }

    public bool ShowOTHER { get; set; }


    public override XmlSchema GetSchema()
    {
      throw new NotImplementedException();
    }

    public override void ReadXml(XmlReader reader)
    {
      try
      {
        reader.MoveToContent();
        reader.Read();
        ReadBaseXML(reader);
        //Hack for old settings
        if (reader.Name == "Token")
        {
          if (!reader.IsEmptyElement)
          {
            reader.ReadStartElement("Token");
            UserName = reader.ReadContentAsString();
            reader.ReadEndElement();
          }
          else
          {
            reader.ReadStartElement("Token");
          }
        }
        if (reader.Name == "TokenSecret")
        {
          if (!reader.IsEmptyElement)
          {
            reader.ReadStartElement("TokenSecret");
            UserName = reader.ReadContentAsString();
            reader.ReadEndElement();
          }
          else
          {
            reader.ReadStartElement("TokenSecret");
          }
        }
        if (reader.Name == "NbToGet")
        {
          if (!reader.IsEmptyElement)
          {
            reader.ReadStartElement("NbToGet");
            UserName = reader.ReadContentAsString();
            reader.ReadEndElement();
          }
          else
          {
            reader.ReadStartElement("NbToGet");
          }
        }
        //End of hack for old settings
        if (reader.Name == "UserName")
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
        if (reader.Name == "ShowAPPS")
        {
          if (!reader.IsEmptyElement)
          {
            reader.ReadStartElement("ShowAPPS");
            ShowAPPS = bool.Parse(reader.ReadContentAsString());
            reader.ReadEndElement();
          }
          else
          {
            reader.ReadStartElement("ShowAPPS");
          }
        }
        if (reader.Name == "ShowCONN")
        {
          if (!reader.IsEmptyElement)
          {
            reader.ReadStartElement("ShowCONN");
            ShowCONN = bool.Parse(reader.ReadContentAsString());
            reader.ReadEndElement();
          }
          else
          {
            reader.ReadStartElement("ShowCONN");
          }
        }
        if (reader.Name == "ShowJGRP")
        {
          if (!reader.IsEmptyElement)
          {
            reader.ReadStartElement("ShowJGRP");
            ShowJGRP = bool.Parse(reader.ReadContentAsString());
            reader.ReadEndElement();
          }
          else
          {
            reader.ReadStartElement("ShowJGRP");
          }
        }
        if (reader.Name == "ShowJOBS")
        {
          if (!reader.IsEmptyElement)
          {
            reader.ReadStartElement("ShowJOBS");
            ShowJOBS = bool.Parse(reader.ReadContentAsString());
            reader.ReadEndElement();
          }
          else
          {
            reader.ReadStartElement("ShowJOBS");
          }
        }
        if (reader.Name == "ShowOTHER")
        {
          if (!reader.IsEmptyElement)
          {
            reader.ReadStartElement("ShowOTHER");
            ShowOTHER = bool.Parse(reader.ReadContentAsString());
            reader.ReadEndElement();
          }
          else
          {
            reader.ReadStartElement("ShowOTHER");
          }
        }
        if (reader.Name == "ShowPRFU")
        {
          if (!reader.IsEmptyElement)
          {
            reader.ReadStartElement("ShowPRFU");
            ShowPRFU = bool.Parse(reader.ReadContentAsString());
            reader.ReadEndElement();
          }
          else
          {
            reader.ReadStartElement("ShowPRFU");
          }
        }
        if (reader.Name == "ShowRECU")
        {
          if (!reader.IsEmptyElement)
          {
            reader.ReadStartElement("ShowRECU");
            ShowRECU = bool.Parse(reader.ReadContentAsString());
            reader.ReadEndElement();
          }
          else
          {
            reader.ReadStartElement("ShowRECU");
          }
        }
        if (reader.Name == "ShowSTAT")
        {
          if (!reader.IsEmptyElement)
          {
            reader.ReadStartElement("ShowSTAT");
            ShowSTAT = bool.Parse(reader.ReadContentAsString());
            reader.ReadEndElement();
          }
          else
          {
            reader.ReadStartElement("ShowSTAT");
          }
        }
        

        //reader.ReadEndElement();
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
        WriteBaseXML(writer);
        writer.WriteElementString("ShowAPPS", ShowAPPS.ToString());
        writer.WriteElementString("ShowCONN", ShowCONN.ToString());
        writer.WriteElementString("ShowJGRP", ShowJGRP.ToString());
        writer.WriteElementString("ShowJOBS", ShowJOBS.ToString());
        writer.WriteElementString("ShowOTHER", ShowOTHER.ToString());
        writer.WriteElementString("ShowPRFU", ShowPRFU.ToString());
        writer.WriteElementString("ShowRECU", ShowRECU.ToString());
        writer.WriteElementString("ShowSTAT", ShowSTAT.ToString());
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }
  }
}
