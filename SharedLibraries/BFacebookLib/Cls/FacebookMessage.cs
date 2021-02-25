using System;
using Sobees.Library.BGenericLib;

namespace Sobees.Library.BFacebookLibV1.Cls
{
  ///<summary>
  ///</summary>
  public class FacebookMessage
  {
    /// <summary>
    /// About me information
    /// </summary>
    public string MessageID { get; set; }

    /// <summary>
    /// Activities
    /// </summary>
    public long ThreadID { get; set; }

    /// <summary>
    /// Birthday of user
    /// </summary>
    public long AuthorID { get; set; }

    /// <summary>
    /// Books information
    /// </summary>
    public string Body { get; set; }


    /// <summary>
    /// Books information
    /// </summary>
    public DateTime CreatedTime { get; set; }

    /// <summary>
    /// Books information
    /// </summary>
    public long ViewerID { get; set; }

    ///<summary>
    ///</summary>
    public User User { get; set; }

    ///<summary>
    ///</summary>
    public FacebookAttachement Attachement { get; set; }
  }
}