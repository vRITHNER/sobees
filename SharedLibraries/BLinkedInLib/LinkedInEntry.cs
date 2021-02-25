using System.Collections.Generic;
using Sobees.Library.BGenericLib;

namespace Sobees.Library.BLinkedInLib
{
  public class LinkedInEntry : Entry
  {
    public List<LinkedInUser> Users { get; set; }

    public List<LinkedInEducation> Educations { get; set; }

    public LinkedInJob Job { get; set; }

    public List<LinkedInRecommendation> Recommendations { get; set; }

    public List<LinkedInGroup> Groups { get; set; }

    public List<LinkedInActivity> Activities { get; set; }

    public new LinkedInUser User { get; set; }
    public new LinkedInUser ToUser { get; set; }
    public string UpdateType { get; set; }
    public int CanPost { get; set; }
  }
}