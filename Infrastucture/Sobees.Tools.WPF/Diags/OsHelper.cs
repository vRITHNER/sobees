#region

using System;
using Sobees.Tools.Logging;

#endregion

namespace Sobees.Tools.Diags
{
  public class OsHelper
  {
    public static bool BisRunXP()
    {
      try
      {
        var os = Environment.OSVersion;
        return os.Version.Major == 5;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("OSHelper::bRunXP:", ex);
      }
      return false;
    }

    public static bool BisRunVista()
    {
      try
      {
        var os = Environment.OSVersion;
        return os.Version.Major == 6 && os.Version.Minor == 0;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("OSHelper::bRunVista:", ex);
      }
      return false;
    }

    public static bool BisRunWin7()
    {
      try
      {
        var os = Environment.OSVersion;
        return os.Version.Major == 6 && os.Version.Minor > 0;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("OSHelper::bRunWin7:", ex);
      }
      return false;
    }
  }
}