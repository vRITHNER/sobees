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
  public class FacteryMetaData
  {
#if !SILVERLIGHT 
   [DataMember] 
#endif
    public string Url { get; set; }
#if !SILVERLIGHT 
   [DataMember] 
#endif
    public string ExtractCount { get; set; }
#if !SILVERLIGHT 
  [DataMember] 
#endif
    public string ShortUrl { get; set; }
#if !SILVERLIGHT 
  [DataMember] 
#endif
    public string Author { get; set; }
#if !SILVERLIGHT 
  [DataMember] 
#endif
    public string Site { get; set; }
#if !SILVERLIGHT 
  [DataMember] 
#endif
    public int Factrank { get; set; }
  }
}