using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Sobees.Tools.Logging;

namespace Sobees.Library.BGenericLib
{
  public enum EnumType
  {
    Facebook,
    Twitter,
    TwitterSearch,
    Flickr,
    NYtimes,
    TwitterBitlynow,
    TweetMeme,
    WhatTheTrend,
    LinkedIn,
    Rss,
    Bing,
    Boss,
    Youtube,
    Google,
    GoogleNews,
    GoogleBlogs,
    Other
  }

  [Serializable]
  [DataContract]
  [JsonObject(MemberSerialization = MemberSerialization.OptIn, IsReference = true)]
  public class Entry : INotifyPropertyChanged
  {
    #region Fields

    private ObservableCollection<Comment> _comments;
   
    private bool _hasBeenViewed;
    private string _origLink;
    private DateTime _pubDate;
    private Rating _rating;
    private const string APPNAME = "Entry";

    #endregion

    #region Properties
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
    public string Section { get; set; }

#if !SILVERLIGHT 
  [DataMember] 
#endif
    public string Link { get; set; }

#if !SILVERLIGHT 
    [JsonProperty("Comments")]
#endif
    public ObservableCollection<Comment> Comments
    {
      get { return _comments; }
      set
      {
        _comments = value;
        OnPropertyChanged("Comments");
      }
    }

#if !SILVERLIGHT 
  [DataMember] 
#endif
    public string OrigLink
    {
      get
      {
        if (!string.IsNullOrEmpty(_origLink))
          return _origLink;

        return Link;
      }
      set { _origLink = value; }
    }

#if !SILVERLIGHT 
  [DataMember] 
#endif
    public string DisplayLink { get; set; }

#if !SILVERLIGHT 
  [DataMember] 
#endif
    public DateTime PubDate
    {
      get { return _pubDate; }
      set
      {
        _pubDate = value;
        OnPropertyChanged("PubDate");
      }
    }

#if !SILVERLIGHT 
  [DataMember] 
#endif
    public DateTime UpdateDate { get; set; }

#if !SILVERLIGHT 
    [JsonProperty("User")]
#endif
    public virtual User User { get; set; }

#if !SILVERLIGHT 
    [JsonProperty("toUser")]
#endif
    public virtual User ToUser { get; set; }

#if !SILVERLIGHT 
    [JsonProperty("Medias")]
#endif
    public List<Media> Medias { get; set; }

#if !SILVERLIGHT 
  [DataMember] 
#endif
    public string Content { get; set; }
#if !SILVERLIGHT 
  [DataMember] 
#endif
    public string XamlContent { get; set; }
#if !SILVERLIGHT 
  [DataMember] 
#endif
    public string SourceName { get; set; }

#if !SILVERLIGHT 
    [JsonProperty("Rating")]
#endif
    public Rating Rating
    {
      get { return _rating; }
      set
      {
        _rating = value;
        OnPropertyChanged("Rating");
      }
    }
#if !SILVERLIGHT 
  [DataMember] 
#endif
    public bool HasBeenViewed
    {
      get { return _hasBeenViewed; }
      set
      {
        _hasBeenViewed = value;
        OnPropertyChanged("HasBeenViewed");
      }
    }
//#if !SILVERLIGHT 
//    [JsonProperty("FacteryData")]
//#endif
//    public FacteryMetaData FacteryData
//    {
//      get { return _facteryData; }
//      set
//      {
//        _facteryData = value;
//        OnPropertyChanged("FacteryData");
//      }
//    }

#if !SILVERLIGHT 
  [DataMember] 
#endif
    public string ImageUrl { get; set; }
#if !SILVERLIGHT 
  [DataMember] 
#endif
    public int PostType { get; set; }
#if !SILVERLIGHT 
  [DataMember] 
#endif
    public EnumType Type { get; set; }

#if !SILVERLIGHT 
    [JsonProperty("Likes")]
#endif
    public List<Like> Likes { get; set; }
    #endregion

    #region Constructors

    #endregion

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
      try
      {
        if (ReferenceEquals(null,
                            obj)) return false;


        var e = obj as Entry;
        if (e != null)
        {
          switch (e.Type)
          {
            case EnumType.Facebook:
              return Id != null && e.Id.Equals(Id);
            case EnumType.TwitterBitlynow:
              return Id != null && e.Id.Equals(Id);
            case EnumType.Twitter:
              return Id != null && e.Id.Equals(Id);
            case EnumType.TwitterSearch:
              return Id != null && e.Id.Equals(Id);
            case EnumType.LinkedIn:
              return Id != null && e.Id.Equals(Id);
            case EnumType.Google:
              return Title != null && e.Title.Equals(Title);
            case EnumType.Bing:
              return Title != null && e.Title.Equals(Title);
            case EnumType.NYtimes:
              return Content != null && e.Content.Equals(Content);
            case EnumType.Boss:
              return Title != null && e.Title.Equals(Title);
            default:
              return Title != null && e.Title.Equals(Title);
          }
        }

        return false;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(APPNAME,ex);
      }
      return false;
    }

    public bool Equals(Entry other)
    {
      try
      {
        if (ReferenceEquals(null,
                            other)) return false;
        if (ReferenceEquals(this,
                            other)) return true;

        ////TODO: to verify - Added by VR - 9-16-09: cause a lot of exceptions
        //if (Id == null) return false;

        return String.Equals(other.Id, Id, StringComparison.CurrentCulture) && Equals(other.Title,Title);
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
      return false;
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return ((Id != null ? Id.GetHashCode() : 0) * 397) ^ (Title != null ? Title.GetHashCode() : 0);
      }
    }
  }

  public class EntryComparer : IEqualityComparer<Entry>
  {
    #region IEqualityComparer<Entry> Members

    public bool Equals(Entry x,
                       Entry y)
    {
      try
      {
        if (x.Title.Equals(y.Title))
          return true;
        return false;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
      return false;
    }

    public int GetHashCode(Entry e)
    {
      try
      {
        return e.Title.ToLower().Trim().GetHashCode();
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
      return 0;
    }

    #endregion
  }
}