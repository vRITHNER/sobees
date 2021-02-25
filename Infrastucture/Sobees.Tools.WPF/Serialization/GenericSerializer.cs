#region

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using BUtility;

#endregion

namespace Sobees.Infrastructure.Tools.Serialization
{
  public class GenericSerializer
  {
    public static string SerializeObject(object obj)
    {
      try
      {
        if (obj == null)
          return null;

        string xmlString = null;
        var ms = new MemoryStream();
        var xs = new XmlSerializer(obj.GetType());
        var xws = new XmlWriterSettings();
        xws.Encoding = Encoding.UTF8;
        var writer = XmlWriter.Create(ms, xws);
        xs.Serialize(writer, obj);
        xmlString = Encoding.UTF8.GetString(ms.ToArray(), 0, ms.ToArray().Length);
        return xmlString;
      }
      catch (Exception ex)
      {
        BLogManager.LogEntry("GenericSerializer::SerializeObject:", ex);
      }
      return null;
    }

#if !SILVERLIGHT
    public static String SerializeObjectXML(Object pObject)
    {
      try
      {
        String XmlizedString = null;
        var memoryStream = new MemoryStream();
        var xs = new XmlSerializer(pObject.GetType());
        var xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);

        xs.Serialize(xmlTextWriter, pObject);
        memoryStream = (MemoryStream) xmlTextWriter.BaseStream;
        XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());
        return XmlizedString;
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        return null;
      }
    }

    /// <summary>
    ///   Method to reconstruct an Object from XML string
    /// </summary>
    /// <param name = "pXmlizedString"></param>
    /// <returns></returns>
    public Object DeserializeObjectXML(String pXmlizedString,
                                       Type type)
    {
      var xs = new XmlSerializer(type);
      var memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString));
      var xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);

      return xs.Deserialize(memoryStream);
    }

    /// <summary>
    ///   To convert a Byte Array of Unicode values (UTF-8 encoded) to a complete String.
    /// </summary>
    /// <param name = "characters">Unicode Byte Array to be converted to String</param>
    /// <returns>String converted from Unicode Byte Array</returns>
    private static String UTF8ByteArrayToString(Byte[] characters)
    {
      var encoding = new UTF8Encoding();
      var constructedString = encoding.GetString(characters);
      return (constructedString);
    }

    /// <summary>
    ///   Converts the String to UTF8 Byte array and is used in De serialization
    /// </summary>
    /// <param name = "pXmlString"></param>
    /// <returns></returns>
    private Byte[] StringToUTF8ByteArray(String pXmlString)
    {
      var encoding = new UTF8Encoding();
      var byteArray = encoding.GetBytes(pXmlString);
      return byteArray;
    }
#endif

    public static object DeserializeObject(String xmlString,
                                           Type type)
    {
      try
      {
        if (xmlString == null)
          return null;
        var xs = new XmlSerializer(type);
        var ms = new MemoryStream(Encoding.UTF8.GetBytes(xmlString));
        var xws = new XmlWriterSettings();
        xws.Encoding = Encoding.UTF8;
        XmlWriter.Create(ms, xws);
        return xs.Deserialize(ms);
      }
      catch (Exception ex)
      {
        BLogManager.LogEntry("GenericSerializer::DeserializeObject:", ex);
      }
      return null;
    }
  }
}