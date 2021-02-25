namespace Sobees.Library.BFacebookLibV2.PostData
{
  using System.IO;
  using Sobees.Library.BFacebookLibV2.Interfaces;

  public class SocialPostValue : ISocialPostValue
  {

    public string Name { get; }

    public string Value { get; set; }

    public SocialPostValue(string name, string value)
    {
      Name = name;
      Value = value;
    }

    public void WriteToMultipartStream(Stream stream, string boundary, string newLine, bool isLast)
    {

      SocialPostData.Write(stream, "--" + boundary + newLine);
      SocialPostData.Write(stream, "Content-Disposition: form-data; name=\"" + Name + "\"" + newLine);
      SocialPostData.Write(stream, newLine);

      SocialPostData.Write(stream, Value);

      SocialPostData.Write(stream, newLine);
      SocialPostData.Write(stream, "--" + boundary + (isLast ? "--" : "") + newLine);

    }

    public override string ToString()
    {
      return Value;
    }

  }

}