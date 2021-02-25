#region

using System;
using System.Xml;
using System.Xml.Schema;
using Sobees.Infrastructure.Settings;
using Sobees.Tools.Logging;

#endregion

namespace Sobees.Controls.Facebook.Cls
{
  public class FacebookSettings : BSettingsControlBase
  {
    public FacebookSettings()
    {
    }

    public FacebookSettings(double refreshTime)
    {
      RefreshTime = refreshTime;
    }

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
        WriteBaseXML(writer);
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }
  }
}