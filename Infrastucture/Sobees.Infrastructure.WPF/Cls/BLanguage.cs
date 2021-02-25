namespace Sobees.Infrastructure.Cls
{
  public class BLanguage
  {
    public BLanguage(string shortName, string name)
    {
      ShortName = shortName;
      Name = name;
    }

    public string ShortName { get; set; }
    public string Name { get; set; }
  }
}