using System;
using System.Runtime.Serialization;
#if !SILVERLIGHT
using Newtonsoft.Json;
#endif
namespace Sobees.Library.BGenericLib
{
#if !SILVERLIGHT
  [Serializable]
  [DataContract]
  [JsonObject(MemberSerialization = MemberSerialization.OptIn, IsReference = true)]
#endif
  public class Thumbnail
  {
    public Thumbnail()
    {
    }

    public Thumbnail(string url, double width, double height)
    {
      Url = url;
      Width = width;
      Height = height;
    }

#if !SILVERLIGHT
    [DataMember]
#endif
    public string Url { get; set; }
#if !SILVERLIGHT
    [DataMember]
#endif
public double Width { get; set; }
#if !SILVERLIGHT
    [DataMember]
#endif
    public double Height { get; set; }
  }
}