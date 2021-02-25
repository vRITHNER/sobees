using Sobees.Library.BTwitterLib;
using System.Windows.Media;

namespace Sobees.Controls.Twitter.Cls
{
  public class TwitterListShow : TwitterList
  {
    public TwitterListShow(TwitterList list)
    {
      CanEdit = false;
      Id=list.Id;
      Name = list.Name;
      FullName = list.FullName;
      Slug = list.Slug;
      SubscriberCount = list.SubscriberCount;
      MemberCount = list.MemberCount;
      Url = list.Url;
      Mode = list.Mode;
      Creator = list.Creator;
    }

    public Brush ColorIcon { get; set; }

    public bool IsShowed { get; set; }

    public bool CanEdit { get; set; }
  }
}