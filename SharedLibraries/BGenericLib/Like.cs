using System;
using System.Collections.Generic;
using System.ComponentModel;
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
  public class Like : INotifyPropertyChanged
  {
    private int _count;
    private int _likeIt;

    public Like()
    {
    }

    public Like(DateTime date,
                User user)
    {
      Date = date;
      User = user;
    }

#if !SILVERLIGHT 
  [DataMember] 
#endif
    public DateTime Date { get; set; }

    public User User { get; set; }
#if !SILVERLIGHT 
    [JsonProperty("FriendsLike")]
#endif
    public List<User> FriendsLike { get; set; }
#if !SILVERLIGHT 
    [JsonProperty("SampleUsersLike")]
#endif
    public List<User> SampleUsersLike { get; set; }

#if !SILVERLIGHT 
  [DataMember] 
#endif
    public int LikeIt
    {
      get { return _likeIt; }
      set
      {
        if (value == _likeIt)
          return;

        _likeIt = value;
        OnPropertyChanged("LikeIt");
      }
    }

#if !SILVERLIGHT 
  [DataMember] 
#endif
    public int Count
    {
      get { return _count; }
      set
      {
        if (value == _count)
          return;

        _count = value;
        OnPropertyChanged("Count");
      }
    }

#if !SILVERLIGHT 
  [DataMember] 
#endif
    public int CanLike { get; set; }

#if !SILVERLIGHT 
  [DataMember] 
#endif
    public string Href { get; set; }

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    protected void OnPropertyChanged(string propertyName)
    {
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null)
        handler(this,
                new PropertyChangedEventArgs(propertyName));
    }
  }
}