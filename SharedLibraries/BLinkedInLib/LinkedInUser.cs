using System;
using System.Collections.Generic;
using Sobees.Library.BGenericLib;

namespace Sobees.Library.BLinkedInLib
{
  public class LinkedInUser : User
  {
    public int NbConnections { get; set; }
    public int NbRecommendations { get; set; }
    public int Distance { get; set; }
    public LinkedInIndustryCode IndustryCode { get; set; }
    public List<LinkedInPosition> Positions { get; set; }
    public string LastStatusString { get; set; }
    public DateTime LastStatusDate { get; set; }
    public string Summary { get; set; }
    public string Specialties { get; set; }
    public string Associations { get; set; }
    public List<LinkedInUrlType> Urls { get; set; }
  }
}