using System;
using Sobees.Library.BGenericLib;

namespace Sobees.Library.BTwitterLib
{
  public class TwitterEntry : Entry
  {
    public TwitterUser RetweeterUser { get; set; }
    public string InReplyTo { get; set; }
    public string InReplyToUserId { get; set; }
    public string InReplyToUserName { get; set; }
    //public override User User { get; set; }
    public new TwitterUser User { get; set; }
    public new TwitterUser ToUser { get; set; }
    public int CanPost { get; set; }
    public string ErrorMsg { get; set; }
  }
}