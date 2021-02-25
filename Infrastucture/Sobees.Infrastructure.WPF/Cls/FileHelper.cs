#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using Sobees.Library.BGenericLib;
using Sobees.Tools.Logging;

#endregion

namespace Sobees.Infrastructure.Cls
{
  public class FileHelper
  {
    /// <summary>
    ///   This function save the list of Twitter friends of a given user
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="friends"></param>
    public static void SaveFriendsList(string userName, List<User> friends)
    {
      var folder = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\{Application.CompanyName}\{Application.ProductName}";

      //Check and create if apply the folder to store friends list
      if (!Directory.Exists(folder))
        Directory.CreateDirectory(folder);

      var writer = XmlWriter.Create($@"{folder}\{userName}.xml");

      if (writer != null)
      {
        writer.WriteStartDocument();
        writer.WriteStartElement("friends");
        foreach (var user in friends)
        {
          writer.WriteStartElement("User");
          writer.WriteElementString("Id", user.Id);
          writer.WriteElementString("NickName", user.NickName);
          writer.WriteElementString("Name", user.Name);
          writer.WriteElementString("ProfileImgUrl", user.ProfileImgUrl);
          writer.WriteEndElement();
        }
        writer.WriteEndElement();
        writer.WriteEndDocument();
        writer.Flush();
        writer.Close();
      }
    }

    /// <summary>
    ///   LoadFriendsList
    /// </summary>
    /// <param name="userName"></param>
    /// <returns></returns>
    public static List<User> LoadFriendsList(string userName)
    {
      try
      {
        var file = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/{Application.CompanyName}/{Application.ProductName}/{userName}.xml";
        if (!File.Exists(file)) return null;

        var xdoc = XDocument.Load(file);

        //Parse the data received
        XNamespace xn = "Friends";
        return (from user in xdoc.Descendants("User")
                select
                  new User
                  {
                    Id = user.Element("Id").Value,
                    NickName = user.Element("NickName").Value,
                    Name = user.Element("Name").Value,
                    ProfileImgUrl = user.Element("ProfileImgUrl").Value,
                  }).ToList();
      }
      catch (Exception e)
      {
        TraceHelper.Trace("LoadFriendsList", e);
      }
      return null;
    }
  }
}