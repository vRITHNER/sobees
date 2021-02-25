using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Linq;
using Sobees.Tools.Crypto;

namespace Sobees.Library.BServicesLib
{
  class TweetPhotoHelper
  {
    //private static byte[] bytes;

    /// <summary>
    /// URL for the TweetPhoto API's upload method
    /// </summary>
    private const string TweetPhotoUpladoApiURL = "http://tweetphotoapi.com/api/tpapi.svc/upload2";

    /// <summary>
    /// URL for the TweetPhoto API's upload and post method
    /// </summary>
    private const string TweetPhotoUploadAndPOSTApiURL = "http://TweetPhoto.com/api/uploadAndPost";

    /// <summary>
    /// Uploads the photo and sends a new Tweet
    /// </summary>
    /// <param name="binaryImageData">The binary image data.</param>
    /// <param name="tweetMessage">The tweet message.</param>
    /// <param name="filename">The filename.</param>
    /// <returns>Return true, if the operation was succeded.</returns>
    public static string UploadPhoto(string login, string passwordHash, byte[] binaryImageData, string tweetMessage,
                                     string filename, string hashKey, string sourceName, out string errorMsg)
    {
      return UploadPhoto(login, passwordHash, binaryImageData, tweetMessage, filename, hashKey, sourceName, out errorMsg,
                         null);
    }

    /// <summary>
    /// Uploads the photo and sends a new Tweet
    /// </summary>
    /// <param name="binaryImageData">The binary image data.</param>
    /// <param name="tweetMessage">The tweet message.</param>
    /// <param name="filename">The filename.</param>
    /// <returns>Return true, if the operation was succeded.</returns>
    public static string UploadPhoto(string login, string passwordHash, byte[] binaryImageData, string tweetMessage,
                                     string filename, string hashKey, string sourceName, out string errorMsg,
                                     WebProxy proxy)
    {
      errorMsg = null;
      try
      {
        // Documentation: http://groups.google.com/group/tweetphoto/web/upload-v2-0-api
        string boundary = Guid.NewGuid().ToString();
        string requestUrl = String.IsNullOrEmpty(tweetMessage)
                              ? TweetPhotoUpladoApiURL
                              : TweetPhotoUploadAndPOSTApiURL;
        var request = (HttpWebRequest) WebRequest.Create(requestUrl);
        string encoding = "iso-8859-1";

        request.PreAuthenticate = true;
        request.AllowWriteStreamBuffering = true;
        if (proxy != null)
        {
          request.Proxy = proxy;
        }
        request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
        request.Method = "POST";

        string header = string.Format("--{0}", boundary);
        string footer = string.Format("--{0}--", boundary);

        var contents = new StringBuilder();
        contents.AppendLine(header);

        string fileContentType = "text/html";
        string fileHeader = String.Format("Content-Disposition: file; name=\"{0}\"; filename=\"{1}\"", "media", filename);

        string fileData = Encoding.GetEncoding(encoding).GetString((binaryImageData));

        contents.AppendLine(fileHeader);
        contents.AppendLine(String.Format("Content-Type: {0}", fileContentType));
        contents.AppendLine();
        contents.AppendLine(fileData);

        contents.AppendLine(header);
        contents.AppendLine(String.Format("Content-Disposition: form-data; name=\"{0}\"", "username"));
        contents.AppendLine();
        contents.AppendLine(login);

        string pwd = string.Empty;
        byte[] key = EncryptionHelper.GetHashKey(hashKey);
        pwd = EncryptionHelper.Decrypt(key, passwordHash);

        contents.AppendLine(header);
        contents.AppendLine(String.Format("Content-Disposition: form-data; name=\"{0}\"", "password"));
        contents.AppendLine();
        contents.AppendLine(pwd);

        contents.AppendLine(header);
        contents.AppendLine(String.Format("Content-Disposition: form-data; name=\"{0}\"", "source"));
        contents.AppendLine();
        contents.AppendLine("sobees");

        if (!String.IsNullOrEmpty(tweetMessage))
        {
          contents.AppendLine(header);
          contents.AppendLine(String.Format("Content-Disposition: form-data; name=\"{0}\"", "message"));
          contents.AppendLine();
          contents.AppendLine(tweetMessage);
        }

        contents.AppendLine(footer);

        byte[] bytes = Encoding.GetEncoding(encoding).GetBytes(contents.ToString());

        request.ContentLength = bytes.Length;

        using (Stream requestStream = request.GetRequestStream())
        {
          requestStream.Write(bytes, 0, bytes.Length);

          using (var response = (HttpWebResponse) request.GetResponse())
          {
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
              string result = reader.ReadToEnd();

              XDocument doc = XDocument.Parse(result);

              XElement rsp = doc.Element("rsp");
              string status = rsp.Element(XName.Get("err")) != null
                                ? rsp.Element(XName.Get("err")).Attribute(XName.Get("code")).Value
                                : rsp.Attribute(XName.Get("stat")).Value;

              if (status.ToUpperInvariant().Equals("OK"))
              {
                return rsp.Element("mediaurl") != null ? rsp.Element("mediaurl").Value : null;
              }

              if (status.Equals("1001"))
                errorMsg = "Invalid twitter username or password.";
              else if (status.Equals("1002"))
                errorMsg = "Image not found.";
              else if (status.Equals("1003"))
                errorMsg = "Invalid image type";
              else if (status.Equals("1004"))
                errorMsg = "Image larger than 5MB";

              return null;
            }
          }
        }
      }
      catch (Exception e)
      {
        errorMsg = e.InnerException != null ? e.Message + "\n" + e.InnerException.Message : e.Message;
      }
      return null;
    }
  }

}
