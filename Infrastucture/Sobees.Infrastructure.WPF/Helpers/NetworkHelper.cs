using System;
using System.Net;
using System.Net.NetworkInformation;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Tools.Logging;

namespace Sobees.Infrastructure.Helpers
{
  public class NetworkHelper
  {
    private static bool _mNetworkAvailable = true;

    public static void NetworkChangeNetworkAvailabilityChanged(object sender,
                                                               NetworkAvailabilityEventArgs e)
    {
      _mNetworkAvailable = e.IsAvailable;

      if (CheckConnection())
      {
        Messenger.Default.Send("GoOnline");
        TraceHelper.Trace("NetHelper::NetworkChangeNetworkAvailabilityChanged:",
                          ":IsAvailable: True");
      }
      else
      {
        Messenger.Default.Send("GoOffline");
        TraceHelper.Trace("NetHelper::NetworkChangeNetworkAvailabilityChanged:",
                          ":IsAvailable: False");
      }
    }
    public static bool CheckConnection()
    {
      _mNetworkAvailable = NetworkInterface.GetIsNetworkAvailable() && BExistWebConnection();
      return _mNetworkAvailable;
    }
    public static bool BExistWebConnection()
    {
      try
      {
        var reqFP = (HttpWebRequest)WebRequest.Create("http://www.google.com");
        //google tombe pas trop souvent alors fiable !!!!????

        using (var rspFP = (HttpWebResponse)reqFP.GetResponse())
        {
          // HTTP = 200 - Internet connection available, server online
          // other code = no Internet connection
          _mNetworkAvailable = (rspFP.StatusCode == HttpStatusCode.OK);
          rspFP.Close();
          return _mNetworkAvailable;
        }
      }
      catch (WebException wex)
      {
        TraceHelper.Trace("App -> BExistWebConnection() -> ", wex);
        return false;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("App -> BExistWebConnection() -> ", ex);
        return false;
      }
    }
    public static bool GetStatus()
    {
      return _mNetworkAvailable;
    }
  }
}