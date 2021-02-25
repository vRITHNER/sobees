using System;
using System.Collections.Generic;
using Sobees.Library.BFacebookLibV1.Session;
using Sobees.Library.BFacebookLibV1.Utility;

namespace Sobees.Library.BFacebookLibV1.Rest
{
    /// <summary>
    /// Facebook Admin API methods.
    /// </summary>
    public class Graph : RestBase
    {
        
        #region Enumerations


        #endregion Enumerations
        
        #region Methods

        #region Constructor

        /// <summary>
        /// Public constructor for Facebook.ExampleObject
        /// </summary>
        /// <param name="session">Needs a connected Facebook Session object for making requests</param>
        public Graph(FacebookSession session)
            : base(session)
        {
        }

        #endregion Constructor

		#region Public Methods

#if !SILVERLIGHT

		#region Synchronous Methods

        public string Post(string path, Dictionary<string, string> parameterDictionary)
        {
            return Post(path, parameterDictionary, false, null, null);
        }
        public T Post<T>(string path, Dictionary<string, string> parameterDictionary)
        {
            return Post<T>(path,parameterDictionary,false,null,null);
        }
        #endregion Synchronous Methods

#endif

        #region Asynchronous Methods

        public void PostAsync(string path, Dictionary<string, string> parameterDictionary, PostCallback callback, Object state)
		{
            Post(path, parameterDictionary, true, callback, state);
		}
        public void PostAsync<T>(string path, Dictionary<string, string> parameterDictionary, PostCallback<T> callback, Object state)
        {
            Post<T>(path, parameterDictionary, true, callback, state);
        }




        #endregion Asynchronous Methods

        #endregion Public Methods

        #region Private Methods
        private string Post(string path, Dictionary<string, string> parameterDictionary, bool isAsync, PostCallback callback, Object state)
        {
            if (parameterDictionary == null)
            {
                parameterDictionary = new Dictionary<string, string>();
            }
            var requestUrl = string.Format(Constants.FacebookGraphUrl, path);
            string parameters = CreateHttpParameterList(parameterDictionary);
            if (isAsync)
            {
                SendRequestAsync(parameterDictionary, !string.IsNullOrEmpty(Session.SessionKey), false, new AsyncResult(XmlResultOperationCompleted, new FacebookCallCompleted<string>(callback), state));
                return null;
            }
            else
            {
                var response = postRequest(requestUrl, parameters, false, true);
                return ProcessResponse(response, true);
            }

        }
        private T Post<T>(string path, Dictionary<string, string> parameterDictionary, bool isAsync, PostCallback<T> callback, Object state)
        {
            if (parameterDictionary == null)
            {
                parameterDictionary = new Dictionary<string, string>();
            }
            var requestUrl = string.Format(Constants.FacebookGraphUrl, path);
            string parameters = CreateHttpParameterList(parameterDictionary);
            var response = postRequest(requestUrl, parameters, false, true);
            var result = Utilities.DeserializeJSONDataContractObject<T>(ProcessResponse(response, true));
            if (result is T)
                return (T)result;
            else
                return default(T);
        }

        #endregion Private Methods
        #endregion Methods
		#region Delegates
        
        /// <summary>
        /// Delegate called when Query call completed
        /// </summary>
        /// <param name="result">result of operation</param>
        /// <param name="state">An object containing state information for this asynchronous request</param>
        /// <param name="e">Exception object, if the call resulted in exception.</param>
        public delegate void PostCallback(string result, Object state, FacebookException e);

		// TODO: fix the code for the related Query<T>() method.
		/// <summary>
		/// Delegate called when Query call completed
		/// </summary>
		/// <param name="result">result of operation</param>
		/// <param name="state">An object containing state information for this asynchronous request</param>
		/// <param name="e">Exception object, if the call resulted in exception.</param>
		public delegate void PostCallback<T>(T result, Object state, FacebookException e);


		#endregion Delegates

    }
}








