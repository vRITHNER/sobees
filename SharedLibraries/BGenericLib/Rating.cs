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
  public class Rating
  {
    [DataMember]
    public int Min { get; set; }

    [DataMember]
    public int Max { get; set; }

    [DataMember]
    public int NbRaters { get; set; }

    [DataMember]
    public double Average { get; set; }

    public override string ToString()
    {
      if (Max > Average)
        return Average + " / " + Max;

      return Average.ToString();
    }
  }
}