using System;
using System.Collections.Generic;
using Sobees.Library.BGenericLib;

namespace Sobees.Library.BFacebookLibV1.Cls
{
  ///<summary>
  ///</summary>
  public class FacebookThread
  {
    /// <summary>
    /// About me information
    /// </summary>
    public long ThreadID { get; set; }

    /// <summary>
    /// Activities
    /// </summary>
    public long FolderID { get; set; }

    /// <summary>
    /// Birthday of user
    /// </summary>
    public string Subject { get; set; }

    /// <summary>
    /// Books information
    /// </summary>
    public DateTime UpdatedTime { get; set; }

    /// <summary>
    /// Books information
    /// </summary>
    public string ParentMessageID { get; set; }

    /// <summary>
    /// Books information
    /// </summary>
    public long ParentThreadID { get; set; }

    /// <summary>
    /// Books information
    /// </summary>
    public long MessageCount { get; set; }

    /// <summary>
    /// Books information
    /// </summary>
    public string Snippet { get; set; }

    /// <summary>
    /// Books information
    /// </summary>
    public long SnippetAuthor { get; set; }

    /// <summary>
    /// Books information
    /// </summary>
    public long ObjectID { get; set; }

    /// <summary>
    /// Books information
    /// </summary>
    public bool Unread { get; set; }

    /// <summary>
    /// Books information
    /// </summary>
    public long ViewerID { get; set; }

    ///<summary>
    ///</summary>
    public User User { get; set; }

    ///<summary>
    ///</summary>
    public List<FacebookMessage> Messages { get; set; }
  }
}