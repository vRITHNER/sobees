namespace Sobees.Library.BFacebookLibV2.Login
{
  #region

  using System;
  using System.Collections.Generic;
  using System.Security.Permissions;
  using System.Windows.Forms;
  using Facebook;
  using Sobees.Library.BFacebookLibV2.Tools;

  #endregion

  internal sealed partial class FacebookWinformBrowser : Form
  {
    private readonly FacebookClient _fb = new FacebookClient();

    private FacebookWinformBrowser()
    {
      InitializeComponent();
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

    public Dictionary<string, string> SessionProperties { get; set; }

    public string WindowTitle
    {
      get { return Text; }
      set { Text = value; }
    }

    private void WbFacebookLoginNavigated(object sender, WebBrowserNavigatedEventArgs e)
    {
      FacebookOAuthResult oauthResult;
      if (!_fb.TryParseOAuthCallbackUrl(e.Url, out oauthResult))
        return;

      if (oauthResult.IsSuccess)
      {
        var accessToken = oauthResult.AccessToken;

        DialogResult = DialogResult.OK;

        SessionProperties = JsonHelper.ConvertFromJsonAssoicativeArray("");
        {
          SessionProperties.Add("access_token", accessToken);
        }
      }
    }

    private void FacebookWinformBrowserFormClosed(object sender, FormClosedEventArgs e)
    {
      if (DialogResult != DialogResult.OK)
        DialogResult = DialogResult.Cancel;
    }
  }
}