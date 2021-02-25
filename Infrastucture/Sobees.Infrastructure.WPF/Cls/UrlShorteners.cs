namespace Sobees.Infrastructure.Cls
{
  public enum UrlShorteners
  {
    BitLy,
    Digg,
    IsGd,
    TinyUrl,
#if !SILVERLIGHT
    TrIm,
    Twurl,
    MigreMe
#endif
  }
}