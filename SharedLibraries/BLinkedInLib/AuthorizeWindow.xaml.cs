using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sobees.Library.BLinkedInLib
{
    /// <summary>
    /// Interaction logic for AuthorizeWindow.xaml
    /// </summary>
    public partial class AuthorizeWindow : UserControl
    {
        public AuthorizeWindow()
        {
            InitializeComponent();
        }
        private OAuthLinkedInV2 _oauth;

      public String Token { get; private set; }

      public String Verifier { get; private set; }

      public String TokenSecret { get; }

      public AuthorizeWindow(OAuthLinkedInV2 o)
        {
            _oauth = o;
            Token = null;
            InitializeComponent();
            this.addressTextBox.Text = o.AuthorizationLinkGet();
            Token = _oauth.Token;
            TokenSecret = _oauth.TokenSecret;
            browser.Navigate(new Uri(_oauth.AuthorizationLinkGet()));
        }

        private void browser_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            this.addressTextBox.Text = e.Uri.ToString();
            if (e.Uri.Scheme == "liconnect")
            {
                string queryParams = e.Uri.Query;
                if (queryParams.Length > 0)
                {
                    //Store the Token and Token Secret
                    NameValueCollection qs = HttpUtility.ParseQueryString(queryParams);
                    if (qs["oauth_token"] != null)
                    {
                        Token = qs["oauth_token"];
                    }
                    if (qs["oauth_verifier"] != null)
                    {
                        Verifier = qs["oauth_verifier"];
                    }
                }
                this.Visibility = Visibility.Hidden;
            }            
        }
    }
}
