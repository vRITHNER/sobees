#region

using System.Linq;
using LinqToTwitter;
using Sobees.Library.BTwitterLib.Cls;

#endregion

namespace Sobees.Library.BTwitterLib
{
  public class Context
  {
    public static User GetAccountUser(TwitterContext twitterCtx)
    {
      var user = GetAccountCredentials(twitterCtx);
      return user;
    }

    /// <summary>
    ///   verifies that account credentials are correct
    /// </summary>
    /// <param name="twitterCtx">TwitterContext</param>
    public static User GetAccountCredentials(TwitterContext twitterCtx)
    {
      var accounts =
        from acct in twitterCtx.Account
        where acct.Type == AccountType.VerifyCredentials
        select acct;

      var account = accounts.SingleOrDefault();
      if (account != null)
      {
        var user = account.User;
        return user;
      }
      return null;
    }

    public static TwitterContext CreateTwitterContext(string accessTokenTuc, string tokenSecretTuc)
    {
      return new TwitterContext(new SingleUserAuthorizer
      {
        CredentialStore = new InMemoryCredentialStore
        {
          ConsumerKey = BTwitterLibGlobal.SOBEES_DESKTOP_CONSUMER_KEY,
          ConsumerSecret = BTwitterLibGlobal.SOBEES_DESKTOP_CONSUMER_SECRET,
          OAuthToken = accessTokenTuc,
          OAuthTokenSecret = tokenSecretTuc
        }
      });
    }

    public static TwitterContext CreateTwitterContext(string consumerKey, string consumerSecret, string accessTokenTuc,
      string tokenSecretTuc)
    {
      return new TwitterContext(new SingleUserAuthorizer
      {
        CredentialStore = new InMemoryCredentialStore
        {
          ConsumerKey = consumerKey,
          ConsumerSecret = consumerSecret,
          OAuthTokenSecret = tokenSecretTuc,
          OAuthToken = accessTokenTuc
        }
      });
    }


    public static TwitterContext CreateTwitterContext() //For Test Only
    {
      return new TwitterContext(new SingleUserAuthorizer
      {
        CredentialStore = new InMemoryCredentialStore
        {
          ConsumerKey = BTwitterLibGlobal.SOBEES_DESKTOP_CONSUMER_KEY,
          ConsumerSecret = BTwitterLibGlobal.SOBEES_DESKTOP_CONSUMER_SECRET,
          OAuthToken = BTwitterLibGlobal.MY_ACCESS_TOKEN,
          //".....-....."	= accessToken_TUC
          OAuthTokenSecret = BTwitterLibGlobal.MY_ACCESS_TOKEN_SECRET
          // "........"	= tokenSecret_TUC
        }
      });
    }
  }
}