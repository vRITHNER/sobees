#region



#endregion

using Sobees.Infrastructure.Mail;

namespace Sobees.Mail
{
  public class MailHelper
  {
    //private static UtilServiceClient _mailproxy;

    //private static UtilServiceClient Mailproxy
    //{
    //  get
    //  {
    //    if (_mailproxy == null)
    //    {
    //      _mailproxy = new UtilServiceClient("BasicHttpBinding_IUtilService");
    //      //_mailproxy.Endpoint.Address = new EndpointAddress(SobeesSettings.);
    //    }

    //    return _mailproxy;
    //  }
    //}



    public static string SendEmail(string to, string subject, string body)
    {
      //BMailHelper.Init();

      return SendEmail(to, subject, body, null);
    }


    public static string SendEmail(string to, string subject, string body, string attachment)
    {
      var result = string.Empty;


      return result;
    }
  }
}