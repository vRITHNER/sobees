using Sobees.Library.BGenericLib;

namespace Sobees.Library.BFacebookLibV1.Cls
{
  /// <summary>
  /// Class containing Event information
  /// </summary>
  public class FacebookEvent : Event
  {
    /// <summary>
    /// Event tagline
    /// </summary>
    public string TagLine { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public long Nid { get; set; }


    /// <summary>
    /// Large size event picture
    /// </summary>
    public string PictureBig { get; set; }

    /// <summary>
    /// Smaller size event picture
    /// </summary>
    public string PictureSmall { get; set; }

    /// <summary>
    /// Host name
    /// </summary>
    public string Host { get; set; }


    /// <summary>
    /// Event type
    /// </summary>
    public string EventType { get; set; }

    /// <summary>
    /// Event subtype
    /// </summary>
    public string EventSubType { get; set; }


    /// <summary>
    /// Userid of event creator
    /// </summary>
    public long Creator { get; set; }

    /// <summary>
    /// Event location
    /// </summary>
    public FacebookEventMembers Members { get; set; }

    /// <summary>
    /// Event location information
    /// </summary>
    public FacebookLocation Venue { get; set; }

    /// <summary>
    /// Privacy information
    /// </summary>
    public string Privacy { get; set; }

    /// <summary>
    /// Indicates if gues list needs to be hidden
    /// </summary>
    public bool HideGuestList { get; set; }
  }
}