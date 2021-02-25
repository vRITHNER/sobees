#region

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LinqToTwitter;
using Sobees.Library.BTwitterLib.Cls;

#endregion

namespace Sobees.Library.BTwitterLib.Extensions
{
  public static class ReportSpamExtensions
  {
    internal static ITwitterExecute TwitterExecutor { get; set; }

    public static async Task<User> ReportSpam(this TwitterContext ctx, string userID, string screenName)
    {
      if (string.IsNullOrEmpty(userID) &&
          string.IsNullOrEmpty(screenName))
      {
        throw new ArgumentException("Either userID or screenName is a required parameter.");
      }

      var reportSpamUrl = string.Format("{0}users/report_spam.json", ctx.BaseUrl);

      var createParams = new Dictionary<string, string>
      {
        {"user_id", userID},
        {"screen_name", screenName}
      };

      var reqProc = new UserRequestProcessor<User>();

      var auth = new SingleUserAuthorizer
      {
        CredentialStore = new InMemoryCredentialStore()
        {
          ConsumerKey = BTwitterLibGlobal.SOBEES_DESKTOP_CONSUMER_KEY,
          ConsumerSecret = BTwitterLibGlobal.SOBEES_DESKTOP_CONSUMER_SECRET,
          OAuthToken = BTwitterLibGlobal.MY_ACCESS_TOKEN,
          //".....-....."	= accessToken_TUC
          OAuthTokenSecret = BTwitterLibGlobal.MY_ACCESS_TOKEN_SECRET
          // "........"	= tokenSecret_TUC
        }
      };


      var exec = new TwitterExecute((IAuthorizer) auth);
      
      var resultsJson =
       await exec.PostToTwitterAsync<User>(reportSpamUrl, createParams);
      var user = reqProc.ProcessActionResult(resultsJson, UserAction.SingleUser);
      return user;
    }
  }

  enum UserAction
  {
    SingleUser
  }
}