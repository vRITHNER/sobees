namespace Sobees.Library.BFacebookLibV2.PostData
{
  #region

  using System.IO;
  using Sobees.Library.BFacebookLibV2.Interfaces;

  #endregion

  public class SocialPostFileValue : ISocialPostValue
  {
    public SocialPostFileValue(string name, string path) : this(name, path, null, null)
    {
    }

    public SocialPostFileValue(string name, string path, string contentType, string filename)
    {
      Name = name;
      Data = File.ReadAllBytes(path);
      ContentType = contentType;
      FileName = filename ?? Path.GetFileName(path);

      if (ContentType != null) return;

      switch ((Path.GetExtension(path) ?? "").ToLower())
      {
        case ".jpg":
        case ".jpeg":
          ContentType = "image/jpeg";
          break;
        case ".png":
          ContentType = "image/png";
          break;
        case ".gif":
          ContentType = "image/gif";
          break;
        case ".tiff":
          ContentType = "image/tiff";
          break;
      }
    }

    public string ContentType { get; }
    public string FileName { get; }
    public byte[] Data { get; }
    public string Name { get; }

    public void WriteToMultipartStream(Stream stream, string boundary, string newLine, bool isLast)
    {
      SocialPostData.Write(stream, "--" + boundary + newLine);
      SocialPostData.Write(stream, "Content-Disposition: form-data; name=\"" + Name + "\"; filename=\"" + FileName + "\"" + newLine);
      SocialPostData.Write(stream, "Content-Type: " + ContentType + newLine);
      SocialPostData.Write(stream, newLine);

      stream.Write(Data, 0, Data.Length);

      SocialPostData.Write(stream, newLine);
      SocialPostData.Write(stream, newLine);
      SocialPostData.Write(stream, "--" + boundary + (isLast ? "--" : "") + newLine);
    }

    public override string ToString()
    {
      return FileName;
    }
  }
}