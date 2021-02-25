
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
  public class Picture
  {
#if !SILVERLIGHT 
  [DataMember] 
#endif
    public EnumType Type { get; set; }
#if !SILVERLIGHT 
  [DataMember] 
#endif
    public string Title { get; set; }
#if !SILVERLIGHT 
  [DataMember] 
#endif
    public string Link { get; set; }
#if !SILVERLIGHT 
  [DataMember] 
#endif
    public string DisplayLink { get; set; }
#if !SILVERLIGHT 
  [DataMember] 
#endif
    public string PictureBig { get; set; }
#if !SILVERLIGHT 
  [DataMember] 
#endif
    public string Content { get; set; }
#if !SILVERLIGHT 
  [DataMember] 
#endif
    public string Thumbnail { get; set; }
#if !SILVERLIGHT 
  [DataMember] 
#endif
    public string Height { get; set; }
#if !SILVERLIGHT 
  [DataMember] 
#endif
    public string Width { get; set; }
#if !SILVERLIGHT 
  [DataMember] 
#endif
    public string HeightBig { get; set; }
#if !SILVERLIGHT 
  [DataMember] 
#endif
    public string Tag { get; set; }
#if !SILVERLIGHT 
  [DataMember] 
#endif
    public string WidthBig { get; set; }
#if !SILVERLIGHT 
  [DataMember] 
#endif
    public DateTime Date
    {
      get;
      set;
    }
  }
}
