using System;
using System.Web;

namespace Sobees.Library.BFacebookLibV1.Session
{
    /// <summary>
    /// Represents session object for Facebook Connect apps
    /// </summary>
    public class ConnectSession : FacebookSession
    {
        #region Public Methods
        
        /// <summary>
        /// Creates new ConnectSession object
        /// </summary>
        /// <param name="appKey">Application Key</param>
        /// <param name="appSecret">Application Secret</param>
        public ConnectSession(string appKey, string appSecret)
        {
            ApplicationKey = appKey;
            ApplicationSecret = appSecret;

            // Check for and populate Facebook Connect Session and User information
            PopulateConnectCookieInformation();
        }

        /// <summary>
        /// Logs in user
        /// </summary>
        public override void Login()
        {
        }

        /// <summary>
        /// Logs out user
        /// </summary>
        public override void Logout()
        {
        }

        ///<summary>
        ///</summary>
        ///<param name="cookieName"></param>
        ///<returns></returns>
        public string GetCookie(string cookieName)
        {
            string fullCookieName = $"{ApplicationKey}_{cookieName}";
            if (HttpContext.Current != null
                && HttpContext.Current.Request != null
                && HttpContext.Current.Request.Cookies != null
                && HttpContext.Current.Request.Cookies[fullCookieName] != null)
            {
                return HttpContext.Current.Request.Cookies[fullCookieName].Value;
            }

            return null;
        }

        ///<summary>
        ///</summary>
        ///<returns></returns>
        public bool IsConnected()
        {
            return (SessionKey != null && UserId != -1);
        }

        #endregion Public Methods

        #region Private Methods

        private void PopulateConnectCookieInformation()
        {
            try
            {
                SessionKey = GetCookie("session_key");
                //SessionSecret = GetCookie("ss");
                UserId = GetUserId();
            }
            catch
            {
                throw new Exception("Unable to populate Facebook Connect Session/User cookie information.");
            }
        }

        private long GetUserId()
        {
            long userId;
            long.TryParse(GetCookie("user"), out userId);
            return userId;
        }

        #endregion Private Methods
    }
}
