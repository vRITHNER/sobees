#region

using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

#endregion

namespace Sobees.Library.BGenericLib
{
  [Serializable]
  [DataContract]
  [JsonObject(MemberSerialization = MemberSerialization.OptIn, IsReference = true)]
  public class FacebookEntryForNews : Entry
  {
    [DataMember]
    public int NbComments { get; set; }

    [DataMember]
    public string FacebookType { get; set; }

    [DataMember]
    public int NbLikes { get; set; }

    [DataMember]
    public string AppId { get; set; }

    [DataMember]
    public int CanPost { get; set; }

    [DataMember]
    //Source tag for video, contain embed video(ecapito)
    public string Source { get; set; }
  }
}