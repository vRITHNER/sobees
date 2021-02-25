#region

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Infrastructure.Cls;
using Sobees.Tools.Logging;

#endregion

namespace Sobees.Infrastructure.Mail
{
  public class BMailHelper
  {

    #region Constants

    private const string DEFAULT_FROM_EMAIL_ADDRESS = "info@sobees.com";
    private const string DEFAULT_REPLY_TO_EMAIL_ADDRESS = "info@sobees.com";
    private const string DEFAULT_SMTP_CLIENT = "mail.sobees.com";
    private const string DEFAULT_SMTP_LOGIN = "user@sobees.com";
    private const int DEFAULT_SMTP_PORT = 587;
    private const string DEFAULT_SMTP_PWD = "Eludb1015";

    #endregion

    public static string SendEmail(string from, string to, string subject, string body)
    {
      return SendEmail(from, to, subject, body, null);
    }

    static DispatcherFrame frame = new DispatcherFrame();

    public static string SendEmail(string from, string to, string subject, string body, string attachment)
    {
      var result = "The application wasn't able to send the mail; please contact sobees support -> help@sobees.com";

      try
      {
        SendMail(to, subject, body, attachment, true, true, out result);
        return result;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("BMailHelper::SendEmail:",
                          (ex));
      }

      return result;
    }

    public static void SendMail(string to,
                                    string subject,
                                    string body,
                                    string attachment, 
                                    bool isAsync, 
                                    bool waitForCompleted, 
                                    out string result)
    {
      var res = string.Empty;

      try
      {

        Debug.WriteLine($"SendMailAsync::Start:SendTo:{to}");

        var message = new MailMessage();
        message.To.Add(to);
        message.BodyEncoding = Encoding.ASCII;
        message.SubjectEncoding = Encoding.ASCII;

        message.From = new MailAddress(DEFAULT_FROM_EMAIL_ADDRESS);
        message.ReplyTo = new MailAddress(DEFAULT_REPLY_TO_EMAIL_ADDRESS);
        message.Subject = subject;
        message.Body = body;
        if (attachment != null)
        {
          var a = new Attachment(attachment);
          message.Attachments.Add(a);
        }

        var smtp = new SmtpClient(DEFAULT_SMTP_CLIENT,
                                  DEFAULT_SMTP_PORT);
        var auth = new NetworkCredential(DEFAULT_SMTP_LOGIN,
                                         DEFAULT_SMTP_PWD);
        smtp.UseDefaultCredentials = false;
        smtp.Credentials = auth;
        if (isAsync)
        {
          smtp.SendCompleted += (o, args) =>
                                  {
                                    if (args.Error != null)
                                    {
                                      res = args.Error.Message;
                                      TraceHelper.Trace("BMailHelper::SmtpSendCompleted:",
                                                        res);
                                    }
                                    else
                                    {
                                      res = "Log Submitted. Thank you";
                                    }
                                    frame.Continue = false;
                                  };
          smtp.SendAsync(message,
                         null);


          if (waitForCompleted)
          {
            //Manage a maximum delay to sent email of 1 minute.
            //Manage to not block the Gui during mail sent in backgound but return result when sent or timeout
            var _timer = new DispatcherTimer(DispatcherPriority.Background);
            _timer.Interval = new TimeSpan(0, 0, 1, 0);
            _timer.Tick += (o, args) =>
            {
              frame.Continue = false;
              res = "Maximum delay to sent Mail reached - please verify your connectivity and retry later...";
            };
            _timer.Start();
            Dispatcher.PushFrame(frame);
          }
        }
        else
        {
          smtp.Send(message);
          res = "Sent";
        }

      }
      catch (Exception ex)
      {
        TraceHelper.Trace("BMailHelper::SendMailAsync:",
                          ex);
        res = ex.Message;
      }

      result = res;
    }

  }
}