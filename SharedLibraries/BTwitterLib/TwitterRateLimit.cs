using System;

namespace Sobees.Library.BTwitterLib
{
  public class TwitterRateLimit
  {
    public int RemainingHits { get; set; }
    public int HourlyLimit { get; set; }
    public DateTime ResetTime { get; set; }
  }
}