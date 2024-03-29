﻿#region

using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

#endregion

namespace Sobees.Library.BFacebookLibV1.Utility
{
  /// <summary>
  ///   Help to serialize the facebook object.
  /// </summary>
  /// <typeparam name="T"> </typeparam>
  public class FacebookObject<T>
  {
    /// <summary>
    ///   Deserializes facebook response to object
    /// </summary>
    /// <param name="xml"> </param>
    /// <returns> This method returns an instance of the specified object (of type T). </returns>
    public static T Deserialize(string xml)
    {
      var deserializer = new XmlSerializer(typeof (T));

      using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
      {
        try
        {
          return (T) deserializer.Deserialize(ms);
        }
        catch (InvalidOperationException e)
        {
          throw new FacebookException("Could not deserialize data returned from server", e);
        }
      }
    }
  }
}