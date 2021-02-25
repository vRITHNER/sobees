using System;
using System.Collections.Generic;
using Sobees.Library.BFacebookLibV1.Schema;
using Sobees.Library.BFacebookLibV1.Session;
using Sobees.Library.BFacebookLibV1.Utility;

namespace Sobees.Library.BFacebookLibV1.Rest
{
    /// <summary>
    /// Facebook Sms API methods.
    /// </summary>
    public class Sms : RestBase
    {
        #region Methods

        #region Constructor

        /// <summary>
        /// Public constructor for facebook.Sms
        /// </summary>
        /// <param name="session">Needs a connected Facebook Session object for making requests</param>
        public Sms(FacebookSession session)
            : base(session)
        {
        }

        #endregion Constructor

        #region Public Methods

#if !SILVERLIGHT

        #region Synchronous Methods


        /// <summary>
        /// Sends an SMS to a user.
        /// See the facebook guide for more information.
        /// </summary>
        /// <param name="uid">The user ID of the user to send the SMS to.</param>
        /// <param name="message">The message to send.</param>
        /// <param name="session_id">The SMS session ID.</param>
        /// <param name="req_session">Whether to request a session.</param>
        /// <returns>If successful, this method returns the session_id of the SMS session that was just created if req_session is true.</returns>
        public long Send(long? uid, string message, long? session_id, bool req_session)
        {
            return Send(uid, message, session_id, req_session, false, null, null);
        }

        /// <summary>
        /// Checks if sending SMS to a user is allowed.
        /// See the facebook guide for more information.
        /// </summary>
        /// <param name="uid">The user ID of the user to query SMS sending abilities.</param>
        /// <returns>If successful, this method returns 0.</returns>
        public long CanSend(long? uid)
        {
            return CanSend(uid, false, null, null);
        }

        #endregion

#endif

        #region Asynchronous Methods

        /// <summary>
        /// Sends an SMS to a user.
        /// See the facebook guide for more information.
        /// </summary>
        /// <param name="uid">The user ID of the user to send the SMS to.</param>
        /// <param name="message">The message to send.</param>
        /// <param name="session_id">The SMS session ID.</param>
        /// <param name="req_session">Whether to request a session.</param>
        /// <param name="callback">The AsyncCallback delegate</param>
        /// <param name="state">An object containing state information for this asynchronous request</param>        
        /// <returns>If successful, this method returns the session_id of the SMS session that was just created if req_session is true.</returns>
        public long SendAsync(long? uid, string message, long? session_id, bool req_session, SendCallback callback, Object state)
        {
            return Send(uid, message, session_id, req_session, true, callback, state);
        }

        /// <summary>
        /// Checks if sending SMS to a user is allowed.
        /// See the facebook guide for more information.
        /// </summary>
        /// <param name="uid">The user ID of the user to query SMS sending abilities.</param>
        /// <param name="callback">The AsyncCallback delegate</param>
        /// <param name="state">An object containing state information for this asynchronous request</param>        
        /// <returns>If successful, this method returns 0.</returns>
        public long CanSendAsync(long? uid, CanSendCallback callback, Object state)
        {
            return CanSend(uid, true, callback, state);
        }

        #endregion

        #endregion Public Methods

        #region Private Methods

        private long Send(long? uid, string message, long? session_id, bool req_session, bool isAsync, SendCallback callback, Object state)
        {
            var parameterList = new Dictionary<string, string> { { "method", "facebook.sms.send" } };
            Utilities.AddOptionalParameter(parameterList, "uid", uid);
            Utilities.AddOptionalParameter(parameterList, "message", message);
            Utilities.AddOptionalParameter(parameterList, "session_id", session_id);
            Utilities.AddOptionalParameter(parameterList, "req_session", req_session ? 1 : 0);

            if (isAsync)
            {
                SendRequestAsync<sms_send_response, long>(parameterList, !string.IsNullOrEmpty(Session.SessionKey), new FacebookCallCompleted<long>(callback), state);
                return 0;
            }

            var response = SendRequest<sms_send_response>(parameterList, !string.IsNullOrEmpty(Session.SessionKey));
            return response == null ? -1 : response.TypedValue;
        }

        private long CanSend(long? uid, bool isAsync, CanSendCallback callback, Object state)
        {
            var parameterList = new Dictionary<string, string> { { "method", "facebook.sms.canSend" } };
            Utilities.AddOptionalParameter(parameterList, "uid", uid);

            if (isAsync)
            {
                SendRequestAsync<sms_canSend_response, long>(parameterList, !string.IsNullOrEmpty(Session.SessionKey), new FacebookCallCompleted<long>(callback), state);
                return 0;
            }

            var response = SendRequest<sms_canSend_response>(parameterList, !string.IsNullOrEmpty(Session.SessionKey));
            return response == null ? -1 : response.TypedValue;
        }


        #endregion Private Methods

        #endregion Methods

        #region Delegates

        /// <summary>
        /// Delegate called when Send call is completed.
        /// </summary>
        /// <param name="session_id">Session identifier</param>
        /// <param name="state">An object containing state information for this asynchronous request</param>
        /// <param name="e">Exception object, if the call resulted in exception.</param>
        public delegate void SendCallback(long session_id, Object state, FacebookException e);

        /// <summary>
        /// Delegate called when CanSend call is completed.
        /// </summary>
        /// <param name="result">Result code from canSend</param>
        /// <param name="state">An object containing state information for this asynchronous request</param>
        /// <param name="e">Exception object, if the call resulted in exception.</param>
        public delegate void CanSendCallback(long result, Object state, FacebookException e);


        #endregion Delegates
    }
}
