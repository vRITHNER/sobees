using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Windows.Forms;
using Facebook;
using Sobees.Library.BFacebookLibV1.Utility;

namespace Sobees.Library.BFacebookLibV1.Session.DesktopPopup
{
    internal sealed partial class FacebookWinformBrowser : Form
    {
        private FacebookWinformBrowser()
        {
            InitializeComponent();
        }

        public Dictionary<string, string> SessionProperties
        {
            get;
            set;
        }
        public string WindowTitle
        {
            get { return this.Text; }
            set { this.Text = value; }
        }
        [SecurityPermission(SecurityAction.LinkDemand)]
        internal FacebookWinformBrowser(string loginUrl)
            : this(new Uri(loginUrl))
        {
        }

        [SecurityPermission(SecurityAction.LinkDemand)]
        internal FacebookWinformBrowser(Uri loginUrl)
            : this()
        {
            wbFacebookLogin.Navigate(loginUrl);
        }

        private readonly FacebookClient _fb = new FacebookClient();
        private void WbFacebookLoginNavigated(object sender, WebBrowserNavigatedEventArgs e)
        {
          FacebookOAuthResult oauthResult;
          if (!_fb.TryParseOAuthCallbackUrl(e.Url, out oauthResult))
          {
            return;
          }

          if (oauthResult.IsSuccess)
          {
            var accessToken = oauthResult.AccessToken;

            DialogResult = DialogResult.OK;
            //var sessionString = HttpUtility.ParseQueryString(e.Url.Query).Get("session");
            SessionProperties = JSONHelper.ConvertFromJSONAssoicativeArray("");
            //if (SessionProperties.Count == 0)
            {
              //var frag = e.Url.Fragment.Substring(1).Split('=');
              SessionProperties.Add("access_token", accessToken);
            }
          }
          else
          {
            // user cancelled
          }
            //if(e.Url.PathAndQuery.StartsWith("/connect/login_success.html"))
            //{
            //    DialogResult = DialogResult.OK;
            //    var sessionString = HttpUtility.ParseQueryString(e.Url.Query).Get("session");
            //    SessionProperties = JSONHelper.ConvertFromJSONAssoicativeArray(sessionString);
            //    if (SessionProperties.Count == 0)
            //    {
            //        var frag = e.Url.Fragment.Substring(1).Split('=');
            //        SessionProperties.Add(frag[0], HttpUtility.UrlDecode(frag[1]));
            //    }
            //}
        }

        private void FacebookWinformBrowserFormClosed(object sender, FormClosedEventArgs e)
        {
            if (DialogResult != DialogResult.OK)
            {
                DialogResult = DialogResult.Cancel;
            }
        }
    }
}