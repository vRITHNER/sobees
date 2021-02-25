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
  public class User : INotifyPropertyChanged
  {
    private bool _isSelected;

    public User()
    {
    }


    public User(string id,
                string name,
                string nickname,
                string profileUrl)
    {
      Id = id;
      Name = name;
      NickName = nickname;
      ProfileUrl = profileUrl;
    }

    #if !SILVERLIGHT 
  [DataMember] 
  #endif
    public string Id { get; set; }
    #if !SILVERLIGHT 
  [DataMember] 
  #endif
    public string Name { get; set; }
    #if !SILVERLIGHT 
  [DataMember] 
  #endif
    public bool Online { get; set; }
    #if !SILVERLIGHT 
  [DataMember] 
  #endif
    public string FirstName { get; set; }
    #if !SILVERLIGHT 
  [DataMember] 
  #endif
    public string NickName { get; set; }
    #if !SILVERLIGHT 
  [DataMember] 
  #endif
    public string Description { get; set; }
    #if !SILVERLIGHT 
  [DataMember] 
  #endif
    public string Location { get; set; }
    #if !SILVERLIGHT 
  [DataMember] 
  #endif
    public string ProfileUrl { get; set; }
    #if !SILVERLIGHT 
  [DataMember] 
  #endif
    public string ProfileImgUrl { get; set; }
    #if !SILVERLIGHT 
  [DataMember] 
  #endif
    public string Url { get; set; }
    #if !SILVERLIGHT 
  [DataMember] 
  #endif
    public string Birthday { get; set; }
    #if !SILVERLIGHT 
  [DataMember] 
  #endif
    public DateTime BirthdayDateTimeActu { get; set; }
    #if !SILVERLIGHT 
  [DataMember] 
  #endif
    public int FollowersCount { get; set; }
    #if !SILVERLIGHT 
  [DataMember] 
  #endif
    public int FriendsCount { get; set; }
    #if !SILVERLIGHT 
  [DataMember] 
  #endif
    public DateTime CreatedAt { get; set; }
    #if !SILVERLIGHT 
  [DataMember] 
  #endif
    public Entry LastStatus { get; set; }
    #if !SILVERLIGHT 
  [DataMember] 
  #endif
    public bool IsSelected
    {
      get { return _isSelected; }
      set
      {
        _isSelected = value;
        OnPropertyChanged("IsSelected");
      }
    }

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

    public override bool Equals(object obj)
    {
      if (obj != null && obj.GetType() == typeof(User))
      {
        return NickName.Equals(((User)obj).NickName);
      }
      return false;
    }

    public bool Equals(User other)
    {
      if (ReferenceEquals(null,
                          other)) return false;
      if (ReferenceEquals(this,
                          other)) return true;
      return Equals(other.NickName,
                    NickName);
    }

    public override int GetHashCode()
    {
      return (NickName != null ? NickName.GetHashCode() : 0);
    }
  }

  public class CustomUserComparer : IEqualityComparer<User>
  {
    #region IEqualityComparer<User> Members

    public bool Equals(User x,
                       User y)
    {
      return x.NickName.Equals(y.NickName);
    }

    public int GetHashCode(User obj)
    {
      return obj.NickName.GetHashCode();
    }

    #endregion
  }
}