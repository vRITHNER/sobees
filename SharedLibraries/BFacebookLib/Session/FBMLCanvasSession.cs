using System.Collections.Generic;
using System.Web;
using Sobees.Library.BFacebookLibV1.Schema;

namespace Sobees.Library.BFacebookLibV1.Session
{
    /// <summary>
    /// Session object used by fbml canvas applications
    /// </summary>
    public class FBMLCanvasSession : CanvasSession
	{
        /// <summary>
        /// Constructor
        /// </summary>
        public FBMLCanvasSession(string appKey, string appSecret)
			: base(appKey, appSecret)
		{
		}
        /// <summary>
        /// Constructor
        /// </summary>
        public FBMLCanvasSession(string appKey, string appSecret, List<Enums.ExtendedPermissions> permissions)
            : base(appKey, appSecret, permissions)
        {
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public FBMLCanvasSession(string appKey, string appSecret, bool readRequest)
            : base(appKey, appSecret, readRequest)
        {
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public FBMLCanvasSession(string appKey, string appSecret, List<Enums.ExtendedPermissions> permissions, bool readRequest)
            : base(appKey, appSecret, permissions, readRequest)
        {
        }
		
		internal override void RedirectToLogin()
		{
            HttpContext.Current.Response.Write(GetRedirect());
            HttpContext.Current.Response.End();
		}

        /// <summary>
        /// Get string for redirect response
        /// </summary>
        public override string GetRedirect()
        {
            return string.Format("<fb:redirect url=\"{0}\"/>", GetLoginUrl());
        }
        internal override void PromptPermissions(string permissionsUrl)
        {
            HttpContext.Current.Response.Write(string.Format("<fb:redirect url=\"{0}\"/>", permissionsUrl));
            HttpContext.Current.Response.End();
        }

		internal override void CacheSession()
		{
			return;
		}

		internal override CachedSessionInfo LoadCachedSession()
		{
			return null;
		}
	}
}
