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
  public class Geoloc
  {
#if !SILVERLIGHT
    [DataMember]
#endif
    public double Latitude { get; set; }
#if !SILVERLIGHT
    [DataMember]
#endif
    public double Longitude { get; set; }
  }
}