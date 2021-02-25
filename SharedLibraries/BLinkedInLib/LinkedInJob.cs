namespace Sobees.Library.BLinkedInLib
{
  public class LinkedInJob
  {
    public string Id { get; set; }

    public string Title { get; set; }

    public string Compagny { get; set; }

    public LinkedInUser JobPoster { get; set; }

    public string Url { get; set; }
  }
}