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
  public class Comment
  {
    public Comment()
    {}

    public Comment(DateTime date,
                   string body,
                   User user)
    {
      Date = date;
      Body = body;
      User = user;
    }

#if !SILVERLIGHT
    [DataMember]
#endif
    public string Id { get; set; }
#if !SILVERLIGHT
    [DataMember]
#endif
    public DateTime Date { get; set; }
#if !SILVERLIGHT
    [DataMember]
#endif
    public string Body { get; set; }
#if !SILVERLIGHT
    [DataMember]
#endif
    public string Link { get; set; }
#if !SILVERLIGHT
    [DataMember]
#endif
public User User { get; set; }
#if !SILVERLIGHT
    [DataMember]
#endif
    public int CanRemoveComment { get; set; }

    public override bool Equals(object obj)
    {
      if (obj == null) return false;
      if (obj.GetType().Equals(typeof (Comment)))
      {
        var e = obj as Comment;
        if (e != null)
        {
          if (((Comment) obj).Id == null)
          {
            return ((Comment) obj).Body.Equals(Body);
          }
          return ((Comment) obj).Id.Equals(Id);
        }
      }
      return false;
    }

    public bool Equals(Comment other)
    {
      if (ReferenceEquals(null,
                          other)) return false;
      if (ReferenceEquals(this,
                          other)) return true;
      return Equals(other.Id,
                    Id);
    }

    public override int GetHashCode()
    {
      return (Id != null ? Id.GetHashCode() : 0);
    }
  }
}