using Sobees.Library.BGenericLib;

namespace Sobees.Library.BLinkedInLib
{
  public class LinkedInRecommendation
  {
    public string Id { get; set; }
    public string Type { get; set; }
    public string Snippet { get; set; }
    public User Recommendee { get; set; }
    public string Url { get; set; }
  }
}