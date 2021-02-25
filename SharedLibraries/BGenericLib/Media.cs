using System;
using System.Collections.Generic;
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
  public class Media
  {
    public Media()
    {
    }

    public Media(string title,
                 string description,
                 List<Thumbnail> thumbnails,
                 List<Content> contents)
      : this(string.Empty, title, description, thumbnails, contents)
    {
    }

    public Media(string link,
                 string title,
                 string description,
                 List<Thumbnail> thumbnails,
                 List<Content> contents)
    {
      Link = link;
      Title = title;
      Description = description;
      Thumbnails = thumbnails;
      Contents = contents;
    }

#if !SILVERLIGHT 
  [DataMember] 
#endif
    public string Link { get; set; }
#if !SILVERLIGHT 
  [DataMember] 
#endif
    public string Id { get; set; }
#if !SILVERLIGHT 
  [DataMember] 
#endif
    public string Title { get; set; }
#if !SILVERLIGHT 
  [DataMember] 
#endif
    public string Player { get; set; }
#if !SILVERLIGHT 
  [DataMember] 
#endif
    public string Description { get; set; }
#if !SILVERLIGHT 
  [DataMember] 
#endif
    public int DurationInSeconds { get; set; }
#if !SILVERLIGHT 
    [JsonProperty("Thumbnails")]
#endif
    public List<Thumbnail> Thumbnails { get; set; }
#if !SILVERLIGHT 
    [JsonProperty("Contents")]
#endif
    public List<Content> Contents { get; set; }
#if !SILVERLIGHT 
    [JsonProperty("Keywords")]
#endif
    public List<string> Keywords { get; set; }
  }
}