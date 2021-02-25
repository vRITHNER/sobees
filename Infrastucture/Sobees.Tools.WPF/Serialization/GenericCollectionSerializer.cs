#region

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using BUtility;

#endregion

namespace Sobees.Tools.Serialization
{
  public class GenericCollectionSerializer
  {
    //public static Encoding encoding = Encoding.GetEncoding("ISO-8859-1");
    public static Encoding Encoding = Encoding.UTF8;

    /// <summary>
    ///   To convert a Byte Array of Unicode values (UTF-8 encoded) to a complete String.
    /// </summary>
    /// <param name = "characters">
    ///   Unicode Byte Array to be converted to String
    /// </param>
    /// <returns>
    ///   String converted from Unicode Byte Array
    /// </returns>
    private static string UTF8ByteArrayToString(byte[] characters)
    {
      //UTF8Encoding encoding = new UTF8Encoding();
      var constructedString = Encoding.GetString(characters, 0, characters.Length);
      return (constructedString);
    }

    /// <summary>
    ///   Converts the String to UTF8 Byte array and is used in De serialization
    /// </summary>
    /// <param name = "pXmlString"></param>
    /// <returns></returns>
    private static Byte[] StringToUTF8ByteArray(string pXmlString)
    {
      //UTF8Encoding encoding = new UTF8Encoding();
      var byteArray = Encoding.GetBytes(pXmlString);
      return byteArray;
    }

    /// <summary>
    ///   Serialize an object into an XML string
    /// </summary>
    /// <typeparam name = "T"></typeparam>
    /// <param name = "obj"></param>
    /// <returns></returns>
    public static string SerializeObject<T>(T obj)
    {
      try
      {
        string xmlString = null;
        var memoryStream = new MemoryStream();
        var xs = new XmlSerializer(typeof (T));
        var settings = new XmlWriterSettings {Encoding = Encoding};
        var xmlWriter = XmlWriter.Create(memoryStream, settings);

        xs.Serialize(xmlWriter, obj);
        xmlString = UTF8ByteArrayToString(memoryStream.ToArray());
        return xmlString;
      }
      catch (Exception ex)
      {
        BLogManager.LogEntry("GenericCollectionSerializer::SerializeObject<T>:", ex);
      }
      return null;
    }

    /// <summary>
    ///   Reconstruct an object from an XML string
    /// </summary>
    /// <param name = "xml"></param>
    /// <returns></returns>
    public static T DeserializeObject<T>(string xml)
    {
      try
      {
        var xs = new XmlSerializer(typeof (T));
        var memoryStream = new MemoryStream(StringToUTF8ByteArray(xml));
        var settings = new XmlWriterSettings {Encoding = Encoding};
        var xmlWriter = XmlWriter.Create(memoryStream, settings);
        var res = (T) xs.Deserialize(memoryStream);
        return res;
      }
      catch (Exception ex)
      {
        BLogManager.LogEntry("GenericCollectionSerializer::DeserializeObject<T>:", ex);
      }
      return default(T);
    }
  }
}