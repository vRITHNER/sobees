namespace Sobees.Library.BFacebookLibV2.Interfaces
{
  using System.IO;

  public interface ISocialPostValue
  {

    string Name { get; }

    void WriteToMultipartStream(Stream stream, string boundary, string newLine, bool isLast);

  }
}
