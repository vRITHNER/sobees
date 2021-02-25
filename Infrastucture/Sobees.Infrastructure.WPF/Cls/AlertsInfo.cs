#region

using System.Collections.Generic;
using Sobees.Library.BGenericLib;

#endregion

namespace Sobees.Infrastructure.Cls
{
  public class AlertsInfo
  {
    public EnumAccountType type { get; set; }

    public string UserName { get; set; }

    public List<Entry> NewEntries { get; set; }
  }
}