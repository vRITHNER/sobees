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
  public class Event
  {

#if !SILVERLIGHT
    [DataMember]
#endif
    public string Picture { get; set; }

#if !SILVERLIGHT    
    [DataMember]
#endif
    public long EventId { get; set; }

    /// <summary>
    /// Event name
    /// </summary>
#if !SILVERLIGHT    
    [DataMember]
#endif
    public string Name { get; set; }

    /// <summary>
    /// Event description
    /// </summary>
#if !SILVERLIGHT    
    [DataMember]
#endif
    public string Description { get; set; }


#if !SILVERLIGHT    
    [DataMember]
#endif
    internal long Start { get; set; }

#if !SILVERLIGHT    
    [DataMember]
#endif
    public DateTime StartTime
    {
      get { return ConvertUnixTimeToDateTime(Start); }
      set { Start = ConvertDateToFacebookDate(value); }
    }


#if !SILVERLIGHT    
  [DataMember]
#endif
    public long Update { get; set; }

#if !SILVERLIGHT    
    [DataMember]
#endif
    public DateTime UpdateTime
    {
      get { return ConvertUnixTimeToDateTime(Update); }
      set { Update = ConvertDateToFacebookDate(value); }
    }

#if !SILVERLIGHT    
    [DataMember]
#endif
    public static DateTime BaseUTCDateTime => new DateTime(1970, 1, 1, 0, 0, 0);

#if !SILVERLIGHT    
    [DataMember]
#endif
    public string Url { get; set; }

#if !SILVERLIGHT    
    [DataMember]
#endif
    public string Location { get; set; }

#if !SILVERLIGHT    
    [DataMember]
#endif
    internal long End { get; set; }

#if !SILVERLIGHT    
    [DataMember]
#endif
    public DateTime EndTime
    {
      get { return ConvertUnixTimeToDateTime(End); }
      set { End = ConvertDateToFacebookDate(value); }
    }

    public static DateTime ConvertUnixTimeToDateTime(long secondsSinceEpoch)
    {
      var utcDateTime = BaseUTCDateTime.AddSeconds(secondsSinceEpoch);
      return utcDateTime;
      //int pacificZoneOffset = utcDateTime.IsDaylightSavingTime() ? -7 : -8;
      //return utcDateTime.AddHours(pacificZoneOffset);
    }

    public static long ConvertDateToFacebookDate(DateTime dateToConvert)
    {
      return (long)((dateToConvert - BaseUTCDateTime).TotalSeconds);
    }
  }
}