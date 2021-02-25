
using System;

namespace Sobees.Library.BTwitterLib
{
  public class TwitterList
  {
    public string Id { get; set; }
    public string Name { get; set; }
    public string FullName { get; set; }
    public string Slug { get; set; }
    public int SubscriberCount { get; set; }
    public int MemberCount { get; set; }
    public string Url { get; set; }
    public string Mode { get; set; }
    public TwitterUser Creator { get; set; }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != typeof (TwitterList)) return false;
      return Equals((TwitterList) obj);
    }

    public bool Equals(TwitterList other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return String.Equals(other.FullName, FullName, StringComparison.CurrentCulture);
    }

    public override int GetHashCode()
    {
      return (FullName != null ? FullName.GetHashCode() : 0);
    }
  }
}