using System;
using System.Net;
using Sobees.Tools.Logging;

namespace Sobees.Infrastructure.Cls
{
  public class ProxyHelper
  {
    public static WebProxy GetConfiguredWebProxy(SobeesSettings settings)
    {
      WebProxy proxy = null;
      if (settings.IsEnabledProxy)
      {
        try
        {
          proxy = new WebProxy(settings.ProxyServer, settings.ProxyPort);
          if (!string.IsNullOrEmpty(settings.ProxyUserName))
          {
            if (!string.IsNullOrEmpty(settings.ProxyUserDomain))
              proxy.Credentials = new NetworkCredential(settings.ProxyUserName, settings.ProxyPassword,
                                                        settings.ProxyUserDomain);
            else
              proxy.Credentials = new NetworkCredential(settings.ProxyUserName, settings.ProxyPassword);
          }
        }
        catch (UriFormatException ex)
        {
          TraceHelper.Trace("bDule.Infrastructure.Cls.ProxyHelper", ex.ToString());
        }
      }
      return proxy;
    }
  }
}
