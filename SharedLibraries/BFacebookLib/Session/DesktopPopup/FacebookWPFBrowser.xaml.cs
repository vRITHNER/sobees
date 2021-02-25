using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using Facebook;
using Sobees.Library.BFacebookLibV1.Utility;

namespace Sobees.Library.BFacebookLibV1.Session.DesktopPopup
{
    /// <summary>
    /// Interaction logic for FacebookWPFBrowser.xaml
    /// </summary>
    public partial class FacebookWPFBrowser : Window
    {
        const string _graphTokenUrl = @"https://graph.facebook.com/oauth/access_token?client_id={0}&redirect_uri=http://www.facebook.com/connect/login_success2.html&code={1}";
        Dispatcher dispatcher;
        string _url;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="url">The url to navigate to when loaded.</param>
        public FacebookWPFBrowser(string url)
        {
            dispatcher = Dispatcher.CurrentDispatcher;
            InitializeComponent();
            _url = url;

            dispatcher.BeginInvoke(new Action(() => this.webBrowser.Navigate(new Uri(_url))));
        }
        /// <summary>
        /// Key value pairs of the session information returned by facebook includes session_key, secret, expires and uid
        /// </summary>
        public Dictionary<string, string> SessionProperties
        {
            get;
            set;
        }

        private readonly FacebookClient _fb = new FacebookClient();
        private void WebBrowserNavigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
          FacebookOAuthResult oauthResult;
          if (!_fb.TryParseOAuthCallbackUrl(e.Uri, out oauthResult))
          {
            return;
          }

          if (oauthResult.IsSuccess)
          {
            var accessToken = oauthResult.AccessToken;

            DialogResult = true;
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
            
            //if (e.Uri.PathAndQuery.StartsWith("/connect/login_success.html"))
            //{
            //    //DialogResult = true;
            //    var sessionString = HttpUtility.ParseQueryString(e.Uri.Query).Get("session");
            //    SessionProperties = JSONHelper.ConvertFromJSONAssoicativeArray(sessionString);
            //    if (SessionProperties.Count == 0)
            //    {
            //        var frag = e.Uri.Fragment.Substring(1).Split('=');
            //        SessionProperties.Add(frag[0], HttpUtility.UrlDecode(frag[1]));
            //    }
            //}
        }

        private void WebBrowserNavigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {

        }  

    }
}
