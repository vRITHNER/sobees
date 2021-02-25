namespace Sobees.Library.BFacebookLibV1.Utility
{
  #region

  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.IO;
  using System.Linq;
  using System.Xml.Linq;
  using Sobees.Library.BFacebookLibV1.Cls;
  using Sobees.Library.BGenericLib;
  using Sobees.Tools.Logging;

  #endregion

  public class FacebookHelper
  {
    private static readonly XNamespace xn = "http://api.facebook.com/2.0/";

    public static List<FacebookEntry> ConvertFQLToStream(string fqlResult, string currentUid)
    {
      List<FacebookEntry> query = null;
      try
      {
        TextReader tr = new StringReader(fqlResult);
        var xdoc = XDocument.Load(tr);
        //Parse the data received
        query = (from stream_post in xdoc.Descendants(xn + "stream_post")
          let comments = stream_post.Element(xn + "comments")
          where stream_post.Element(xn + "source_id") != null
          select new FacebookEntry
          {
            Id = stream_post.Element(xn + "post_id").Value,
            Title =
              stream_post.Element(xn + "message") != null
                ? stream_post.Element(xn + "message").Value
                : string.Empty,
            AppId =
              stream_post.Element(xn + "app_id") != null
                ? stream_post.Element(xn + "app_id").Value
                : string.Empty,
            PostType =
              string.IsNullOrEmpty(stream_post.Element(xn + "type").Value)
                ? 0
                : int.Parse(stream_post.Element(xn + "type").Value),
            /*AppAttribution = stream_post.Element(xn + "attribution") != null ?stream_post.Element(xn + "attribution").Value : "",*/
            HasBeenViewed = true,
            Content =
              stream_post.Element(xn + "message") != null
                ? stream_post.Element(xn + "message").Value
                : string.Empty,
            PubDate =
              ConvertUniversalTimeToDate(
                double.Parse(stream_post.Element(xn + "created_time").Value)),
            UpdateDate =
              ConvertUniversalTimeToDate(
                double.Parse(stream_post.Element(xn + "updated_time").Value)),
            Type = EnumType.Facebook,
            NbComments =
              int.Parse(stream_post.Element(xn + "comments").Element(xn + "count").Value),
            CanPost =
              int.Parse(stream_post.Element(xn + "comments").Element(xn + "can_post").Value),
            Comments = new ObservableCollection<Comment>(GetComments(comments, stream_post, currentUid).OrderByDescending(c => c.Date)),
            LikeFacebook =
              stream_post.Element(xn + "likes") != null &&
              stream_post.Element(xn + "likes").Element(xn + "can_like") != null &&
              stream_post.Element(xn + "likes").Element(xn + "can_like").Value == "1"
                ? new Like
                {
                  Count = stream_post.Element(xn + "likes").Element(xn + "count") != null
                    ? int.Parse(
                      stream_post.Element(xn + "likes").Element(xn + "count").Value)
                    : 0,
                  LikeIt = stream_post.Element(xn + "likes").Element(xn + "user_likes") != null
                    ? int.Parse(
                      stream_post.Element(xn + "likes").Element(xn + "user_likes").Value)
                    : 0,
                  SampleUsersLike = GetSampleLike(stream_post),
                  FriendsLike = GetFriendsLike(stream_post),
                  Href = stream_post.Element(xn + "likes").Element(xn + "href").Value,
                  CanLike = 1
                }
                : stream_post.Element(xn + "likes") != null
                  ? new Like
                  {
                    Count = stream_post.Element(xn + "likes").Element(xn + "count") != null
                      ? int.Parse(
                        stream_post.Element(xn + "likes").Element(xn + "count").Value)
                      : 0,
                    CanLike = 0,
                    SampleUsersLike = GetSampleLike(stream_post),
                    FriendsLike = GetFriendsLike(stream_post),
                    LikeIt =
                      stream_post.Element(xn + "likes").Element(xn + "user_likes") != null
                        ? int.Parse(
                          stream_post.Element(xn + "likes").Element(xn + "user_likes").Value)
                        : 0,
                    Href =
                      stream_post.Element(xn + "likes").Element(xn + "href") != null
                        ? stream_post.Element(xn + "likes").Element(xn + "href").Value
                        : string.Empty
                  }
                  : null,
            User = new User
            {
              Id = stream_post.Element(xn + "source_id").Value,
              ProfileUrl =
                "http://www.facebook.com/profile.php?id=" +
                stream_post.Element(xn + "source_id").Value
            },
            ToUser = !stream_post.Element(xn + "actor_id").IsEmpty
              ? new User
              {
                Id = stream_post.Element(xn + "actor_id").Value,
                ProfileUrl =
                  "http://www.facebook.com/profile.php?id=" +
                  stream_post.Element(xn + "actor_id").Value
              }
              : null,
            Attachement = new FacebookAttachement
            {
              Href =
                stream_post.Element(xn + "attachment").Element(xn + "href") !=
                null
                  ? stream_post.Element(xn + "attachment").Element(xn +
                                                                   "href").
                    Value
                  : "",
              Caption =
                stream_post.Element(xn + "attachment").Element(xn +
                                                               "caption") !=
                null
                  ? stream_post.Element(xn + "attachment").Element(xn +
                                                                   "caption")
                    .
                    Value
                  : "",
              Name =
                stream_post.Element(xn + "attachment").Element(xn + "name") !=
                null
                  ? stream_post.Element(xn + "attachment").Element(xn +
                                                                   "name").
                    Value
                  : "",
              Description =
                stream_post.Element(xn + "attachment").Element(xn +
                                                               "description") !=
                null
                  ? stream_post.Element(xn + "attachment").Element(xn +
                                                                   "description")
                    .
                    Value
                  : "",
              Properties = GetMediaProperties(stream_post),
              Medias = GetMedias(stream_post)
            }
          }).ToList();

        //Add the data to the list of entry
        InvertUserToUser(query);
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("Convert FQL to Stream", ex);
      }
      return query;
    }

    private static void InvertUserToUser(List<FacebookEntry> entries)
    {
      foreach (var entry in entries)
      {
        if (entry.ToUser == null) continue;
        var tmp = entry.ToUser;

        entry.ToUser = entry.User;
        entry.User = tmp;
      }
    }

    private static List<Cls.FacebookStreamMedia> GetMedias(XContainer stream_post)
    {
      try
      {
        var lst = stream_post.Element(xn + "attachment").HasElements
          ? (from sm in stream_post.Descendants(xn + "stream_media")
            select new Cls.FacebookStreamMedia
            {
              //Alt =
              //  sm.Element(xn + "alt") != null
              //    ? sm.Element(xn + "alt").Value
              //    : "",
              Href =
                sm.Element(xn + "href") != null
                  ? sm.Element(xn + "href").Value
                  : "",
              Src =
                sm.Element(xn + "src") != null
                  ? sm.Element(xn + "src").Value
                  : "",
              Type =
                sm.Element(xn + "type") != null
                  ? sm.Element(xn + "type").Value
                  : "",
              Photos = GetPhotos(sm),
              Videos = GetVideos(sm)
            }).ToList()
          : null;


        return lst;
      }
      catch (Exception e)
      {
        TraceHelper.Trace("GetMedias", e);
        return null;
      }
    }

    private static List<Cls.FacebookStreamVideo> GetVideos(XContainer sm)
    {
      try
      {
        var lst = sm.Element(xn + "videos") != null
          ? (
            from video in
              sm.Descendants(xn +
                             "videos")
            select
              new Cls.FacebookStreamVideo
              {
                DisplayUrl =
                  video.Element(xn +
                                "display_url")
                    .Value,
                SourceUrl =
                  video.Element(xn +
                                "source_url")
                    .Value,
                OwnerId =
                  video.Element(xn +
                                "owner")
                    .
                    Value,
                Permalink =
                  video.Element(xn +
                                "permalink")
                    .Value
              }).ToList()
          : null;
        if (lst == null)
        {
          lst = sm.Element(xn + "video") != null
            ? (
              from video in
                sm.Descendants(xn +
                               "video")
              select
                new Cls.FacebookStreamVideo
                {
                  DisplayUrl =
                    video.Element(xn +
                                  "display_url")
                      .Value,
                  SourceUrl =
                    video.Element(xn +
                                  "source_url")
                      .Value,
                  OwnerId =
                    video.Element(xn +
                                  "source_type")
                      .
                      Value,
                  Permalink = video.Element(xn +
                                            "preview_img") != null
                    ? video.Element(xn +
                                    "preview_img").Value
                    : ""
                }).ToList()
            : null;
        }

        return lst;
      }
      catch (Exception e)
      {
        TraceHelper.Trace("GetVideos", e);
        return null;
      }
    }

    private static List<Cls.FacebookStreamPhoto> GetPhotos(XElement sm)
    {
      try
      {
        var lst = sm.Element(xn + "photo") != null
          ? (
            from photo in
              sm.Descendants(xn + "photo")
            select
              new Cls.FacebookStreamPhoto
              {
                AlbumId =
                  photo.Element(xn +
                                "aid")
                    .Value,
                PictureId =
                  photo.Element(xn +
                                "pid")
                    .Value,
                OwnerId =
                  photo.Element(xn +
                                "owner")
                    .
                    Value,
                Index =
                  photo.Element(xn +
                                "index")
                    .
                    Value
              }).ToList()
          : null;
        return lst;
      }
      catch (Exception e)
      {
        TraceHelper.Trace("GetPhotos", e);
        return null;
      }
    }

    private static List<FacebookMediaProperty> GetMediaProperties(XElement stream_post)
    {
      try
      {
        var lst = stream_post.Element(xn + "attachment").HasElements
          ? (from sp in
            stream_post.Descendants(xn + "stream_property")
            select new FacebookMediaProperty
            {
              Name =
                sp.Element(xn + "name") != null
                  ? sp.Element(xn + "name").Value
                  : "",
              Text =
                sp.Element(xn + "text") != null
                  ? sp.Element(xn + "text").Value
                  : ""
            }).ToList()
          : null;
        return lst;
      }
      catch (Exception e)
      {
        TraceHelper.Trace("GetMediaProperties", e);
        return null;
      }
    }

    private static List<User> GetFriendsLike(XElement stream_post)
    {
      try
      {
        var lst = (from uid in stream_post.Descendants(xn + "friends")
          where uid.HasElements
          select new User
          {
            Id = uid.Element(xn + "uid").Value,
            ProfileUrl =
              "http://www.facebook.com/profile.php?id=" +
              uid.Element(xn + "uid").Value
          }).ToList();
        return lst;
      }
      catch (Exception e)
      {
        TraceHelper.Trace("GetFriendsLike", e);
        return null;
      }
    }

    private static List<User> GetSampleLike(XElement stream_post)
    {
      try
      {
        var lst = (from uid in stream_post.Descendants(xn + "sample")
          where uid.HasElements
          select new User
          {
            Id = uid.Element(xn + "uid").Value,
            ProfileUrl =
              "http://www.facebook.com/profile.php?id=" +
              uid.Element(xn + "uid").Value
          }).ToList();

        return lst;
      }
      catch (Exception e)
      {
        TraceHelper.Trace("GetSampleLike", e);
        return null;
      }
    }

    private static ObservableCollection<Comment> GetComments(XContainer comments, XContainer stream_post,
      string currentUid)
    {
      try
      {
        var lst = (from comment in comments.Descendants(xn + "comment")
          select new Comment
          {
            CanRemoveComment =
              int.Parse(
                stream_post.Element(xn + "comments").Element(xn + "can_remove")
                  .Value) ==
              1
              ||
              comment.Element(xn + "fromid").Value ==
              currentUid
                ? 1
                : 0,
            Body = comment.Element(xn + "text").Value,
            Date = ConvertUniversalTimeToDate(
              double.Parse(comment.Element(xn + "time").Value)),
            Id = comment.Element(xn + "id").Value,
            User = new User
            {
              Id = comment.Element(xn + "fromid").Value,
              ProfileUrl =
                "http://www.facebook.com/profile.php?id=" +
                comment.Element(xn + "fromid").Value
            }
          }).ToList();
        var observable = new ObservableCollection<Comment>();
        foreach (var comment in lst)
        {
          observable.Add(comment);
        }
        return observable;
      }
      catch (Exception e)
      {
        TraceHelper.Trace("GetComments", e);
        return null;
      }
    }

    /// <summary>
    /// </summary>
    /// <param name="timestamp"></param>
    /// <returns></returns>
    public static DateTime ConvertUniversalTimeToDate(double timestamp)
    {
      // First make a System.DateTime equivalent to the UNIX Epoch.
      var dateTime = new DateTime(1970, 1, 1);

      // Add the number of seconds in UNIX timestamp to be converted.
      return dateTime.AddSeconds(timestamp);
    }

    public static List<User> ConvertSTREAMToUser(string result)
    {
      List<User> query = null;
      try
      {
        TextReader tr = new StringReader(result);
        var xdoc = XDocument.Load(tr);
        //Parse the data received
        query = (from stream_post in xdoc.Descendants(xn + "profile")
          select new User
          {
            Id = stream_post.Element(xn + "id").Value,
            Url = stream_post.Element(xn + "url").Value,
            Name = stream_post.Element(xn + "name").Value,
            ProfileImgUrl = stream_post.Element(xn + "pic_square").Value
          }).ToList();

        //Add the data to the list of entry
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("Convert FQL to Stream", ex);
      }
      return query;
    }
  }
}