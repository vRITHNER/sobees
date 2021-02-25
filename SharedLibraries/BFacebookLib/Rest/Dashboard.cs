using System;
using System.Collections.Generic;
using Sobees.Library.BFacebookLibV1.Schema;
using Sobees.Library.BFacebookLibV1.Session;
using Sobees.Library.BFacebookLibV1.Utility;

namespace Sobees.Library.BFacebookLibV1.Rest
{
    /// <summary>
    /// Facebook Admin API methods.
    /// </summary>
    public class Dashboard : RestBase
    {
        #region Enumerations


        #endregion Enumerations
        
        #region Methods

        #region Constructor

        /// <summary>
        /// Public constructor for Facebook.ExampleObject
        /// </summary>
        /// <param name="session">Needs a connected Facebook Session object for making requests</param>
        public Dashboard(FacebookSession session)
            : base(session)
        {
        }

        #endregion Constructor

        #region Public Methods

#if !SILVERLIGHT

#region Synchronous Methods

        /// <summary>
        /// This method creates one or more global news items, which appear in the dashboards of all your users. You can use this to announce new features to your users, for example. 
        /// Calling this method appends the news items included in this call to the news stream. Facebook displays up to two news items (whether personal or global, or both) at a time for a given application, starting from the two most recently added. 
        /// If a user authorizes your application and you have previously added any global news items, then the most recently set global news items appear on the new user's dashboard. 
        /// This method does not take a session key. You must sign this call with your application secret. Applications that should not include their application secret in their code (such as desktop applications), should not make this call. 
        /// </summary>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <param name="news">An array containing between 1 and 8 news item objects. Each news item object is an object that contains the following: message (required): This is a text-only string containing the content of the activity. You can include one {*actor*} token, which is the same token used in stream attachments. When the news is published, Facebook replaces this token with the full name of the current session user. Note: The message gets truncated after 50 characters, including any user name referenced. action_link: An associative array containing an action link text and URL. The action link appears after the message and follows the same guidelines as action links in stream stories. text: The text of the action link, which gets truncated after 25 characters. href: The URL of the action link. image:The absolute URL to the image associated with the array of news items. Facebook formats the image as a 64x64 pixel square. This image cannot be a Facebook.com URL. If you don't specify an image, Facebook displays your application logo instead. </param>
        /// <returns>This method returns an int containing a news_id if the call succeeds. It returns false if it doesn't succeed and, if the call is malformed, may return an error code.  </returns>
        public long AddGlobalNews(dashboard_news news)
        {
            return AddGlobalNews(news, false, null, null);
        }
        /// <summary>
        /// This method adds personal news from your application for the specified user. 
        /// Calling this method appends the news items included in this call to the news stream. Facebook displays up to two news items (either personal, global, or both) at a time for a given application, starting from the two most recently added. 
        /// When calling this method, you must do one of the following: 
        /// Sign your call with a session secret and include a valid session key for the logged-in user. 
        /// Specify a user ID and sign the call with your application secret. Applications that should not include their application secret in their code (such as desktop applications), should sign the call with a session secret and include a valid session key. 
        /// </summary>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <param name="news">An array containing between 1 and 8 news item objects. Each news item object is an object that contains the following: message (required): This is a text-only string containing the content of the activity. You can include one {*actor*} token, which is the same token used in stream attachments. When the news is published, Facebook replaces this token with the full name of the current session user. Note: The message gets truncated after 50 characters, including any user name referenced. action_link: An associative array containing an action link text and URL. The action link appears after the message and follows the same guidelines as action links in stream stories. text: The text of the action link, which gets truncated after 25 characters. href: The URL of the action link. image:The absolute URL to the image associated with the array of news items. Facebook formats the image as a 64x64 pixel square. This image cannot be a Facebook.com URL. If you don't specify an image, Facebook displays your application logo instead. </param>
        /// <param name="uid">The ID of the user whose dashboard you are updating. If you include the uid, you must sign your call with your application secret. If you do not include the uid, you must include a session key and sign your call with a session secret. </param>
        /// <returns>This method returns an int containing a news_id if the call succeeds</returns>
        public long AddNews(dashboard_news news, long uid)
        {
            return AddNews(news, uid, false, null, null);
        }
        /// <summary>
        /// This method adds personal news from your application for an array of users. 
        /// Calling this method appends the news items included in this call to the news stream. Facebook displays up to two news items (either personal or global) at a time for a given application, starting from the two most recently added. 
        /// This method does not take a session key. You must sign this call with your application secret. Applications that should not include their application secret in their code (such as desktop applications), should not make this call. 
        /// </summary>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <param name="news">An array containing between 1 and 8 news item objects. Each news item object is an object that contains the following: message (required): This is a text-only string containing the content of the activity. You can include one {*actor*} token, which is the same token used in stream attachments. When the news is published, Facebook replaces this token with the full name of the current session user. Note: The message gets truncated after 50 characters, including any user name referenced. action_link: An associative array containing an action link text and URL. The action link appears after the message and follows the same guidelines as action links in stream stories. text: The text of the action link, which gets truncated after 25 characters. href: The URL of the action link. image:The absolute URL to the image associated with the array of news items. Facebook formats the image as a 64x64 pixel square. This image cannot be a Facebook.com URL. If you don't specify an image, Facebook displays your application logo instead. </param>
        /// <param name="uids">An array of IDs of the users whose dashboards you are updating. </param>
        /// <returns>This method returns an associative array of key/value pairs, where each key is the UID of the user whose news was set, and the value is either the ID of the news item, or false if the new wasn't set.  </returns>
        public Dictionary<string, object> MultiAddNews(dashboard_news news, List<long> uids)
        {
            return MultiAddNews(news, uids, false, null, null);
        }
        /// <summary>
        /// This method decreases your application's dashboard counter by 1 for an array of users. 
        /// This method does not take a session key. You must sign this call with your application secret. Applications that should not include their application secret in their code (such as desktop applications), should not make this call. 
        /// </summary>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <param name="uids">An array of IDs of the users whose counters you are decrementing. </param>
        /// <returns>This method returns an array of key/value pairs, where the key is the user ID and the value is a boolean indicating whether or not the counter was decremented successfully. 
        /// Note: Decrementing a counter currently set to 0 will return false. 
        /// </returns>
        public Dictionary<string, bool> MultiDecrementCount(List<long> uids)
        {
            return MultiDecrementCount(uids, false, null, null);
        }
        /// <summary>
        /// This method increases your application's dashboard counter by 1 for an array of users. 
        /// As per the Platform Principles and Policy, you must use the counter only to inform users about legitimate actions that they should take within your application, and must not use the counter for promotional or marketing purposes. You should increment the counter only once for each item or action about which you need to notify a user. 
        /// This method does not take a session key. You must sign this call with your application secret. Applications that should not include their application secret in their code (such as desktop applications), should not make this call. 
        /// </summary>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <param name="uids">An array of IDs of the users whose counters you are incrementing.</param>
        /// <returns>This method returns an array of key/value pairs, where the key is user ID and the value is a boolean indicating whether or not the counter was incremented successfully. </returns>
        public Dictionary<string, bool> MultiIncrementCount(List<long> uids)
        {
            return MultiIncrementCount(uids, false, null, null);
        }
        /// <summary>
        /// This method returns the current value for your application's dashboard counter for an array of users. 
        /// This method does not take a session key. You must sign this call with your application secret. Applications that should not include their application secret in their code (such as desktop applications), should not make this call. 
        /// </summary>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <param name="uids">An array of IDs of the users whose counters you are retrieving.  </param>
        /// <returns>This method returns an associative array containing key/value pairs, where the key is the user ID and the value is an int containing the current value for the counter. </returns>
        public Dictionary<string, int> MultiGetCount(List<long> uids)
        {
            return MultiGetCount(uids, false, null, null);
        }
        /// <summary>
        /// This method sets your application's dashboard counter for an array of users. You can use this method to set a user's counter to 0, effectively clearing it. 
        /// As per the Platform Principles and Policy, you must use the counter only to inform users about legitimate actions that they should take within your application, and must not use the counter for promotional or marketing purposes. You should increment the counter only once for each item or action about which you need to notify a user. 
        /// This method does not take a session key. You must sign this call with your application secret. Applications that should not include their application secret in their code (such as desktop applications), should not make this call. 
        /// </summary>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <param name="ids">An array containing between 1 and 8 news item objects. Each news item object is an object that contains the following: message (required): This is a text-only string containing the content of the activity. You can include one {*actor*} token, which is the same token used in stream attachments. When the news is published, Facebook replaces this token with the full name of the current session user. Note: The message gets truncated after 50 characters, including any user name referenced. action_link: An associative array containing an action link text and URL. The action link appears after the message and follows the same guidelines as action links in stream stories. text: The text of the action link, which gets truncated after 25 characters. href: The URL of the action link. image:The absolute URL to the image associated with the array of news items. Facebook formats the image as a 64x64 pixel square. This image cannot be a Facebook.com URL. If you don't specify an image, Facebook displays your application logo instead. </param>
        /// <returns>This method returns an array of key/value pairs, where the key is user ID and the value is a boolean indicating whether or not the counter was set successfully. </returns>
        public Dictionary<string, bool> MultiSetCount(Dictionary<long, long> ids)
        {
            return MultiSetCount(ids, false, null, null);
        }
        /// <summary>
        /// This method removes some or all of your application's personal news items for an array of users. Calling this method does not remove any existing global news items, so existing global news item(s) set with dashboard.addGlobalNews will appear again. To remove all news items for a user, call this method and dashboard.clearGlobalNews for each user. 
        /// This method does not take a session key. You must sign this call with your application secret. Applications that should not include their application secret in their code (such as desktop applications), should not make this call.
        /// </summary>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <param name="ids">An associative array containing key/value pairs, where the key is the ID of the user whose news you are clearing, and the value is the value is an array of news_ids you want to clear for that user. If you leave the news_ids value empty, then all news items get cleared for that user. </param>
        /// <returns>This method returns one of the following: 
        /// If you specified any news_ids, then this method returns an array of key/value pairs, where the key is the user ID and the value is a nested array of key/value pairs. In this second array, the key is a news_id and the value is a boolean indicating whether or not the news_id was cleared successfully. 
        /// If you didn't specify any news_ids for the user, then this method returns a single array containing the user ID and a boolean indicating whether or not the news was cleared successfully.
        /// </returns>
        public Dictionary<string, Dictionary<string, object>> MultiClearNews(Dictionary<long, List<string>> ids)
        {
            return MultiClearNews(ids, false, null, null);
        }
        /// <summary>
        /// This method returns your application’s current set of personal news items for an array of users. 
        /// This method does not take a session key. You must sign this call with your application secret. Applications that should not include their application secret in their code (such as desktop applications), should not make this call. 
        /// </summary>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <param name="ids">An associative array containing key/value pairs, where the key is the ID of the user whose news items you are retrieving, and the value is the value is an array of news_ids. If you leave the news_ids value empty, then all news items get returned for that user. </param>
        /// <returns>This method returns an associative array containing key/value pairs, where the key is a user ID and the value is an object containing two keys: 
        /// image: A URL to the image for this news item, or an empty string if the image doesn't exist. 
        /// news: An array of one or more news item objects. For information about the contents of the news object, see dashboard.addNews.  
        /// </returns>
        public Dictionary<string, List<dashboard_news>> MultiGetNews(Dictionary<long, List<string>> ids)
        {
            return MultiGetNews(ids, false, null, null);
        }
        /// <summary>
        /// This method removes all global news items previously set for your application or any global news items associated with a given news_id. Calling this method does not remove any existing personal news items you set for a specific user. To remove all news items for a given user, call this method and dashboard.clearNews. 
        /// This method does not take a session key. You must sign this call with your application secret. Applications that should not include their application secret in their code (such as desktop applications), should not make this call. 
        /// </summary>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <param name="news_ids">An array of news_ids to clear. If you do not specify any news_ids, then all global news gets cleared for your application</param>
        /// <returns>This method returns an array of key/value pairs, where the key is a news_id and the value is a boolean indicating whether or not the news item was cleared successfully</returns>
        public Dictionary<string, bool> ClearGlobalNews(List<long> news_ids)
        {
            return ClearGlobalNews(news_ids, false, null, null);
        }
        /// <summary>
        /// This method removes some or all of your application's personal news items for a given user. Calling this method does not remove any existing global news items. To remove all news items for a given user, call this method and dashboard.clearGlobalNews. 
        /// </summary>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <param name="news_ids">An array of news_ids to clear. If you do not pass this parameter or specify any news_ids, then all personal news gets cleared for the specified user. </param>
        /// <param name="uid">The ID of the user whose dashboard you are updating.</param>
        /// <returns>This method returns an array of key/value pairs, where the key is a news_id and the value is a boolean indicating whether or not the news item was cleared successfully.</returns>
        public Dictionary<string, bool> ClearNews(List<long> news_ids, long uid)
        {
            return ClearNews(news_ids, uid, false, null, null);
        }
        /// <summary>
        /// This method decreases your application's dashboard counter by 1 for a given user, unless it's already set to 0.
        /// </summary>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <param name="uid">The ID of the user whose counter you are decrementing.</param>
        /// <returns>This method returns true if the counter is set successfully. Otherwise, it returns false or an error code. </returns>
        public bool DecrementCount(long uid)
        {
            return DecrementCount(uid, false, null, null);
        }
        /// <summary>
        /// This method sets your application's dashboard counter for a given user. You can use this method to set a user's counter to 0, effectively clearing it. 
        /// </summary>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <param name="uid">The ID of the user whose counter you are incrementing.</param>
        /// <param name="count">The new value of the counter. The value must be an integer 0 or greater. </param>
        /// <returns>This method returns true if the counter is set successfully. Otherwise, it returns false or an error code. </returns>
        public bool SetCount(long uid, long count)
        {
            return SetCount(uid, count, false, null, null);
        }
        /// <summary>
        /// This method increases your application's dashboard counter by 1 for a given user. 
        /// As per the Platform Principles and Policy, you must use the counter only to inform users about legitimate actions that they should take within your application, and must not use the counter for promotional or marketing purposes. You should increment the counter only once for each item or action about which you need to notify a user. 
        /// </summary>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <param name="uid">The ID of the user whose counter you are incrementing.</param>
        /// <returns>This method returns true if the counter is set successfully. Otherwise, it returns false or an error code. </returns>
        public bool IncrementCount(long uid)
        {
            return IncrementCount(uid, false, null, null);
        }
        /// <summary>
        /// This method returns the current value for your application's dashboard counter for a given user. 
        /// </summary>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <param name="uid">The ID of the user whose counter you are retrieving </param>
        /// <returns>This method returns an int containing the current value for the counter if the call succeeds. Otherwise, it returns an error code. </returns>
        public int GetCount(long uid)
        {
            return GetCount(uid, false, null, null);
        }
        /// <summary>
        /// This method returns up to the 100 most recent application activities for the current user.
        /// </summary>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <param name="activity_ids">An array of activity_ids to return. If you do not specify any activity_ids, then the 100 most recent activities get returned for the current user.</param>
        /// <param name="uid">The ID of the user whose dashboard you are updating.</param>
        /// <returns>TThis method returns an array of activities, where each activity is an object containing a key/value pair, where the key is an activity_id and the value is a structure containing a message and action_link </returns>
        public List<dashboard_activity> GetActivity(List<long> activity_ids, long uid)
        {
            return GetActivity(activity_ids, uid, false, null, null);
        }
        /// <summary>
        /// This method removes one or more of your application's activities for the current user. 
        /// </summary>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <param name="activity_ids">An array of activity_ids to return. If you do not specify any activity_ids, then the 100 most recent activities get returned for the current user.</param>
        /// <returns>This method returns an array of key/value pairs, where the key is an activity_id and the value is a boolean indicating whether or not the activity was cleared successfully.  </returns>
        public Dictionary<string, bool> RemoveActivity(List<long> activity_ids)
        {
            return RemoveActivity(activity_ids, false, null, null);
        }
        /// <summary>
        /// This method returns some or all of your application's personal news items for a given user, up to the limit. Facebook returns the 8 most recent arrays of news items, and each news item array can contain 8 individual news items, for a maximum of 64 news items in total. 
        /// </summary>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <param name="news_ids">An array of news_ids to return. If you do not specify any news_ids, then all personal news (up to the limits specified above) gets returned for the specified user. </param>
        /// <param name="uid">The ID of the user whose dashboard you are updating </param>
        /// <returns>This method returns an array of news items, where each news item is an object containing two keys: 
        /// image: A URL to the image for this news item, or an empty string if the image doesn't exist. 
        /// news: An array of one or more news item objects. For information about the contents of the news object, see dashboard.addNews.  
        /// </returns>
        public List<dashboard_news> GetNews(List<long> news_ids, long uid)
        {
            return GetNews(news_ids, uid, false, null, null);
        }
        /// <summary>
        /// This method returns some or all of your application's global news items, up the limit. Facebook returns the 8 most recent arrays of news items, and each news item array can contain 8 individual news items, for a maximum of 64 news items in total. 
        /// </summary>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <param name="news_ids">An array of news_ids to return. If you do not specify any news_ids, then all global news gets returned. </param>
        /// <returns>This method returns an array of news item objects, where each news item is a JSON-encoded object containing two keys: 
        /// image: A URL to the image for this news item, or an empty string if the image doesn't exist. 
        /// news: An array of one or more news item objects. For information about the contents of the news object, see dashboard.addGlobalNews.  
        /// </returns>
        public List<dashboard_news> GetGlobalNews(List<long> news_ids)
        {
            return GetGlobalNews(news_ids, false, null, null);
        }
        /// <summary>
        /// This method posts one activity the current session user did in your application. Activities appear in the activity streams of the user's friends. 
        /// You don't specify an image for activities; Facebook displays your application logo instead. 
        /// </summary>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <param name="activity">An object containing the following activity components: 
        /// message (required): A text-only string containing the content of the activity. The message must also contain the {*actor*} token, which is the same token used in stream attachments. When the activity is published, Facebook replaces this token with the full name of the user whose session key you're using to made the call. 
        /// You can also mention other users in the message. Use this syntax: @:USER_ID. When the activity is published, Facebook replaces the user ID with that user's full name. 
        /// Note: The message gets truncated after 50 characters, including any user names referenced. 
        /// action_link: An object containing one action link's text and URL. The action link appears after the message and follows the same guidelines as action links in stream stories. 
        /// text: The text of the action link, which gets truncated after 25 characters. 
        /// href: The URL of the action link. 
        /// </param>
        /// <returns>This method returns an int containing an activity_id if the call succeeds. Otherwise, it returns an error code. </returns>
        public long PublishActivity(dashboard_bundle activity)
        {
            return PublishActivity(activity, false, null, null);
        }


#endregion Synchronous Methods

#endif

#region Asynchronous Methods

        /// <summary>
        /// This method creates one or more global news items, which appear in the dashboards of all your users. You can use this to announce new features to your users, for example. 
        /// Calling this method appends the news items included in this call to the news stream. Facebook displays up to two news items (whether personal or global, or both) at a time for a given application, starting from the two most recently added. 
        /// If a user authorizes your application and you have previously added any global news items, then the most recently set global news items appear on the new user's dashboard. 
        /// This method does not take a session key. You must sign this call with your application secret. Applications that should not include their application secret in their code (such as desktop applications), should not make this call. 
        /// </summary>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <param name="news">An array containing between 1 and 8 news item objects. Each news item object is an object that contains the following: message (required): This is a text-only string containing the content of the activity. You can include one {*actor*} token, which is the same token used in stream attachments. When the news is published, Facebook replaces this token with the full name of the current session user. Note: The message gets truncated after 50 characters, including any user name referenced. action_link: An associative array containing an action link text and URL. The action link appears after the message and follows the same guidelines as action links in stream stories. text: The text of the action link, which gets truncated after 25 characters. href: The URL of the action link. image:The absolute URL to the image associated with the array of news items. Facebook formats the image as a 64x64 pixel square. This image cannot be a Facebook.com URL. If you don't specify an image, Facebook displays your application logo instead. </param>
        /// <returns>This method returns an int containing a news_id if the call succeeds. It returns false if it doesn't succeed and, if the call is malformed, may return an error code.  </returns>
        public void AddGlobalNewsAsync(dashboard_news news, AddGlobalNewsCallback callback, Object state)
        {
            AddGlobalNews(news, true, callback, state);
        }
        /// <summary>
        /// This method adds personal news from your application for the specified user. 
        /// Calling this method appends the news items included in this call to the news stream. Facebook displays up to two news items (either personal, global, or both) at a time for a given application, starting from the two most recently added. 
        /// When calling this method, you must do one of the following: 
        /// Sign your call with a session secret and include a valid session key for the logged-in user. 
        /// Specify a user ID and sign the call with your application secret. Applications that should not include their application secret in their code (such as desktop applications), should sign the call with a session secret and include a valid session key. 
        /// </summary>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <param name="news">An array containing between 1 and 8 news item objects. Each news item object is an object that contains the following: message (required): This is a text-only string containing the content of the activity. You can include one {*actor*} token, which is the same token used in stream attachments. When the news is published, Facebook replaces this token with the full name of the current session user. Note: The message gets truncated after 50 characters, including any user name referenced. action_link: An associative array containing an action link text and URL. The action link appears after the message and follows the same guidelines as action links in stream stories. text: The text of the action link, which gets truncated after 25 characters. href: The URL of the action link. image:The absolute URL to the image associated with the array of news items. Facebook formats the image as a 64x64 pixel square. This image cannot be a Facebook.com URL. If you don't specify an image, Facebook displays your application logo instead. </param>
        /// <param name="uid">The ID of the user whose dashboard you are updating. If you include the uid, you must sign your call with your application secret. If you do not include the uid, you must include a session key and sign your call with a session secret. </param>
        /// <returns>This method returns an int containing a news_id if the call succeeds</returns>
        public void AddNewsAsync(dashboard_news news, long uid, AddNewsCallback callback, Object state)
        {
            AddNews(news, uid, true, callback, state);
        }
        /// <summary>
        /// This method adds personal news from your application for an array of users. 
        /// Calling this method appends the news items included in this call to the news stream. Facebook displays up to two news items (either personal or global) at a time for a given application, starting from the two most recently added. 
        /// This method does not take a session key. You must sign this call with your application secret. Applications that should not include their application secret in their code (such as desktop applications), should not make this call. 
        /// </summary>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <param name="news">An array containing between 1 and 8 news item objects. Each news item object is an object that contains the following: message (required): This is a text-only string containing the content of the activity. You can include one {*actor*} token, which is the same token used in stream attachments. When the news is published, Facebook replaces this token with the full name of the current session user. Note: The message gets truncated after 50 characters, including any user name referenced. action_link: An associative array containing an action link text and URL. The action link appears after the message and follows the same guidelines as action links in stream stories. text: The text of the action link, which gets truncated after 25 characters. href: The URL of the action link. image:The absolute URL to the image associated with the array of news items. Facebook formats the image as a 64x64 pixel square. This image cannot be a Facebook.com URL. If you don't specify an image, Facebook displays your application logo instead. </param>
        /// <param name="uids">An array of IDs of the users whose dashboards you are updating. </param>
        /// <returns>This method returns an associative array of key/value pairs, where each key is the UID of the user whose news was set, and the value is either the ID of the news item, or false if the new wasn't set.  </returns>
        public void MultiAddNewsAsync(dashboard_news news, List<long> uids, MultiAddNewsCallback callback, Object state)
        {
            MultiAddNews(news, uids, true, callback, state);
        }
        /// <summary>
        /// This method decreases your application's dashboard counter by 1 for an array of users. 
        /// This method does not take a session key. You must sign this call with your application secret. Applications that should not include their application secret in their code (such as desktop applications), should not make this call. 
        /// </summary>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <param name="uids">An array of IDs of the users whose counters you are decrementing. </param>
        /// <returns>This method returns an array of key/value pairs, where the key is the user ID and the value is a boolean indicating whether or not the counter was decremented successfully. 
        /// Note: Decrementing a counter currently set to 0 will return false. 
        /// </returns>
        public void MultiDecrementCountAsync(List<long> uids, MultiDecrementCountCallback callback, Object state)
        {
            MultiDecrementCount(uids, true, callback, state);
        }
        /// <summary>
        /// This method increases your application's dashboard counter by 1 for an array of users. 
        /// As per the Platform Principles and Policy, you must use the counter only to inform users about legitimate actions that they should take within your application, and must not use the counter for promotional or marketing purposes. You should increment the counter only once for each item or action about which you need to notify a user. 
        /// This method does not take a session key. You must sign this call with your application secret. Applications that should not include their application secret in their code (such as desktop applications), should not make this call. 
        /// </summary>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <param name="uids">An array of IDs of the users whose counters you are incrementing.</param>
        /// <returns>This method returns an array of key/value pairs, where the key is user ID and the value is a boolean indicating whether or not the counter was incremented successfully. </returns>
        public void MultiIncrementCountAsync(List<long> uids, MultiIncrementCountCallback callback, Object state)
        {
            MultiIncrementCount(uids, true, callback, state);
        }
        /// <summary>
        /// This method returns the current value for your application's dashboard counter for an array of users. 
        /// This method does not take a session key. You must sign this call with your application secret. Applications that should not include their application secret in their code (such as desktop applications), should not make this call. 
        /// </summary>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <param name="uids">An array of IDs of the users whose counters you are retrieving.  </param>
        /// <returns>This method returns an associative array containing key/value pairs, where the key is the user ID and the value is an int containing the current value for the counter. </returns>
        public void MultiGetCountAsync(List<long> uids, MultiGetCountCallback callback, Object state)
        {
            MultiGetCount(uids, true, callback, state);
        }
        /// <summary>
        /// This method sets your application's dashboard counter for an array of users. You can use this method to set a user's counter to 0, effectively clearing it. 
        /// As per the Platform Principles and Policy, you must use the counter only to inform users about legitimate actions that they should take within your application, and must not use the counter for promotional or marketing purposes. You should increment the counter only once for each item or action about which you need to notify a user. 
        /// This method does not take a session key. You must sign this call with your application secret. Applications that should not include their application secret in their code (such as desktop applications), should not make this call. 
        /// </summary>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <param name="ids">An array containing between 1 and 8 news item objects. Each news item object is an object that contains the following: message (required): This is a text-only string containing the content of the activity. You can include one {*actor*} token, which is the same token used in stream attachments. When the news is published, Facebook replaces this token with the full name of the current session user. Note: The message gets truncated after 50 characters, including any user name referenced. action_link: An associative array containing an action link text and URL. The action link appears after the message and follows the same guidelines as action links in stream stories. text: The text of the action link, which gets truncated after 25 characters. href: The URL of the action link. image:The absolute URL to the image associated with the array of news items. Facebook formats the image as a 64x64 pixel square. This image cannot be a Facebook.com URL. If you don't specify an image, Facebook displays your application logo instead. </param>
        /// <returns>This method returns an array of key/value pairs, where the key is user ID and the value is a boolean indicating whether or not the counter was set successfully. </returns>
        public void MultiSetCountAsync(Dictionary<long, long> ids, MultiSetCountCallback callback, Object state)
        {
            MultiSetCount(ids, true, callback, state);
        }
        /// <summary>
        /// This method removes some or all of your application's personal news items for an array of users. Calling this method does not remove any existing global news items, so existing global news item(s) set with dashboard.addGlobalNews will appear again. To remove all news items for a user, call this method and dashboard.clearGlobalNews for each user. 
        /// This method does not take a session key. You must sign this call with your application secret. Applications that should not include their application secret in their code (such as desktop applications), should not make this call.
        /// </summary>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <param name="ids">An associative array containing key/value pairs, where the key is the ID of the user whose news you are clearing, and the value is the value is an array of news_ids you want to clear for that user. If you leave the news_ids value empty, then all news items get cleared for that user. </param>
        /// <returns>This method returns one of the following: 
        /// If you specified any news_ids, then this method returns an array of key/value pairs, where the key is the user ID and the value is a nested array of key/value pairs. In this second array, the key is a news_id and the value is a boolean indicating whether or not the news_id was cleared successfully. 
        /// If you didn't specify any news_ids for the user, then this method returns a single array containing the user ID and a boolean indicating whether or not the news was cleared successfully.
        /// </returns>
        public void MultiClearNewsAsync(Dictionary<long, List<string>> ids, MultiClearNewsCallback callback, Object state)
        {
            MultiClearNews(ids, true, callback, state);
        }
        /// <summary>
        /// This method returns your application’s current set of personal news items for an array of users. 
        /// This method does not take a session key. You must sign this call with your application secret. Applications that should not include their application secret in their code (such as desktop applications), should not make this call. 
        /// </summary>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <param name="ids">An associative array containing key/value pairs, where the key is the ID of the user whose news items you are retrieving, and the value is the value is an array of news_ids. If you leave the news_ids value empty, then all news items get returned for that user. </param>
        /// <returns>This method returns an associative array containing key/value pairs, where the key is a user ID and the value is an object containing two keys: 
        /// image: A URL to the image for this news item, or an empty string if the image doesn't exist. 
        /// news: An array of one or more news item objects. For information about the contents of the news object, see dashboard.addNews.  
        /// </returns>
        public void MultiGetNewsAsync(Dictionary<long, List<string>> ids, MultiGetNewsCallback callback, Object state)
        {
            MultiGetNews(ids, true, callback, state);
        }
        /// <summary>
        /// This method removes all global news items previously set for your application or any global news items associated with a given news_id. Calling this method does not remove any existing personal news items you set for a specific user. To remove all news items for a given user, call this method and dashboard.clearNews. 
        /// This method does not take a session key. You must sign this call with your application secret. Applications that should not include their application secret in their code (such as desktop applications), should not make this call. 
        /// </summary>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <param name="news_ids">An array of news_ids to clear. If you do not specify any news_ids, then all global news gets cleared for your application</param>
        /// <returns>This method returns an array of key/value pairs, where the key is a news_id and the value is a boolean indicating whether or not the news item was cleared successfully</returns>
        public void ClearGlobalNewsAsync(List<long> news_ids, ClearGlobalNewsCallback callback, Object state)
        {
            ClearGlobalNews(news_ids, true, callback, state);
        }
        /// <summary>
        /// This method removes some or all of your application's personal news items for a given user. Calling this method does not remove any existing global news items. To remove all news items for a given user, call this method and dashboard.clearGlobalNews. 
        /// </summary>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <param name="news_ids">An array of news_ids to clear. If you do not pass this parameter or specify any news_ids, then all personal news gets cleared for the specified user. </param>
        /// <param name="uid">The ID of the user whose dashboard you are updating.</param>
        /// <returns>This method returns an array of key/value pairs, where the key is a news_id and the value is a boolean indicating whether or not the news item was cleared successfully.</returns>
        public void ClearNewsAsync(List<long> news_ids, long uid, ClearNewsCallback callback, Object state)
        {
            ClearNews(news_ids, uid, true, callback, state);
        }
        /// <summary>
        /// This method decreases your application's dashboard counter by 1 for a given user, unless it's already set to 0.
        /// </summary>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <param name="uid">The ID of the user whose counter you are decrementing.</param>
        /// <returns>This method returns true if the counter is set successfully. Otherwise, it returns false or an error code. </returns>
        public void DecrementCountAsync(long uid, DecrementCountCallback callback, Object state)
        {
            DecrementCount(uid, true, callback, state);
        }
        /// <summary>
        /// This method sets your application's dashboard counter for a given user. You can use this method to set a user's counter to 0, effectively clearing it. 
        /// </summary>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <param name="uid">The ID of the user whose counter you are incrementing.</param>
        /// <param name="count">The new value of the counter. The value must be an integer 0 or greater. </param>
        /// <returns>This method returns true if the counter is set successfully. Otherwise, it returns false or an error code. </returns>
        public void SetCountAsync(long uid, long count, SetCountCallback callback, Object state)
        {
            SetCount(uid, count, true, callback, state);
        }
        /// <summary>
        /// This method increases your application's dashboard counter by 1 for a given user. 
        /// As per the Platform Principles and Policy, you must use the counter only to inform users about legitimate actions that they should take within your application, and must not use the counter for promotional or marketing purposes. You should increment the counter only once for each item or action about which you need to notify a user. 
        /// </summary>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <param name="uid">The ID of the user whose counter you are incrementing.</param>
        /// <returns>This method returns true if the counter is set successfully. Otherwise, it returns false or an error code. </returns>
        public void IncrementCountAsync(long uid, IncrementCountCallback callback, Object state)
        {
            IncrementCount(uid, true, callback, state);
        }
        /// <summary>
        /// This method returns the current value for your application's dashboard counter for a given user. 
        /// </summary>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <param name="uid">The ID of the user whose counter you are retrieving </param>
        /// <returns>This method returns an int containing the current value for the counter if the call succeeds. Otherwise, it returns an error code. </returns>
        public void GetCountAsync(long uid, GetCountCallback callback, Object state)
        {
            GetCount(uid, true, callback, state);
        }
        /// <summary>
        /// This method returns up to the 100 most recent application activities for the current user.
        /// </summary>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <param name="activity_ids">An array of activity_ids to return. If you do not specify any activity_ids, then the 100 most recent activities get returned for the current user.</param>
        /// <param name="uid">The ID of the user whose dashboard you are updating.</param>
        /// <returns>TThis method returns an array of activities, where each activity is an object containing a key/value pair, where the key is an activity_id and the value is a structure containing a message and action_link </returns>
        public void GetActivityAsync(List<long> activity_ids, long uid, GetActivityCallback callback, Object state)
        {
            GetActivity(activity_ids, uid, true, callback, state);
        }
        /// <summary>
        /// This method removes one or more of your application's activities for the current user. 
        /// </summary>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <param name="activity_ids">An array of activity_ids to return. If you do not specify any activity_ids, then the 100 most recent activities get returned for the current user.</param>
        /// <returns>This method returns an array of key/value pairs, where the key is an activity_id and the value is a boolean indicating whether or not the activity was cleared successfully.  </returns>
        public void RemoveActivityAsync(List<long> activity_ids, RemoveActivityCallback callback, Object state)
        {
            RemoveActivity(activity_ids, true, callback, state);
        }
        /// <summary>
        /// This method returns some or all of your application's personal news items for a given user, up to the limit. Facebook returns the 8 most recent arrays of news items, and each news item array can contain 8 individual news items, for a maximum of 64 news items in total. 
        /// </summary>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <param name="news_ids">An array of news_ids to return. If you do not specify any news_ids, then all personal news (up to the limits specified above) gets returned for the specified user. </param>
        /// <param name="uid">The ID of the user whose dashboard you are updating </param>
        /// <returns>This method returns an array of news items, where each news item is an object containing two keys: 
        /// image: A URL to the image for this news item, or an empty string if the image doesn't exist. 
        /// news: An array of one or more news item objects. For information about the contents of the news object, see dashboard.addNews.  
        /// </returns>
        public void GetNewsAsync(List<long> news_ids, long uid, GetNewsCallback callback, Object state)
        {
            GetNews(news_ids, uid, true, callback, state);
        }
        /// <summary>
        /// This method returns some or all of your application's global news items, up the limit. Facebook returns the 8 most recent arrays of news items, and each news item array can contain 8 individual news items, for a maximum of 64 news items in total. 
        /// </summary>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <param name="news_ids">An array of news_ids to return. If you do not specify any news_ids, then all global news gets returned. </param>
        /// <returns>This method returns an array of news item objects, where each news item is a JSON-encoded object containing two keys: 
        /// image: A URL to the image for this news item, or an empty string if the image doesn't exist. 
        /// news: An array of one or more news item objects. For information about the contents of the news object, see dashboard.addGlobalNews.  
        /// </returns>
        public void GetGlobalNewsAsync(List<long> news_ids, GetGlobalNewsCallback callback, Object state)
        {
            GetGlobalNews(news_ids, true, callback, state);
        }
        /// <summary>
        /// This method posts one activity the current session user did in your application. Activities appear in the activity streams of the user's friends. 
        /// You don't specify an image for activities; Facebook displays your application logo instead. 
        /// </summary>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <param name="activity">An object containing the following activity components: 
        /// message (required): A text-only string containing the content of the activity. The message must also contain the {*actor*} token, which is the same token used in stream attachments. When the activity is published, Facebook replaces this token with the full name of the user whose session key you're using to made the call. 
        /// You can also mention other users in the message. Use this syntax: @:USER_ID. When the activity is published, Facebook replaces the user ID with that user's full name. 
        /// Note: The message gets truncated after 50 characters, including any user names referenced. 
        /// action_link: An object containing one action link's text and URL. The action link appears after the message and follows the same guidelines as action links in stream stories. 
        /// text: The text of the action link, which gets truncated after 25 characters. 
        /// href: The URL of the action link. 
        /// </param>
        /// <returns>This method returns an int containing an activity_id if the call succeeds. Otherwise, it returns an error code. </returns>
        public void PublishActivityAsync(dashboard_bundle activity, PublishActivityCallback callback, Object state)
        {
            PublishActivity(activity, true, callback, state);
        }

#endregion Asynchronous Methods

        #region Lookup Methods


        #endregion Lookup Methods

        #endregion Public Methods
        
        #region Private Methods

        private long AddGlobalNews(dashboard_news news, bool isAsync, AddGlobalNewsCallback callback, Object state)
        {
            var parameterList = new Dictionary<string, string> { { "method", "facebook.dashboard.addGlobalNews" } };
            Utilities.AddOptionalParameter(parameterList, "image", news.image);
            List<string> newsList = new List<string>();
            foreach (var item in news.news)
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("message", item.message);
                if (item.action_link != null && item.action_link.href != null)
                {
                    Dictionary<string, string> dict_actionlink = new Dictionary<string, string>();
                    dict_actionlink.Add("text", item.action_link.text);
                    dict_actionlink.Add("href", item.action_link.href);
                    dict.Add("action_link", JSONHelper.ConvertToJSONAssociativeArray(dict_actionlink));
                }
                newsList.Add(JSONHelper.ConvertToJSONAssociativeArray(dict));
            }
            Utilities.AddJSONArray(parameterList, "news", newsList);
            
            if (isAsync)
            {
                SendRequestAsync<dashboard_addGlobalNews_response, long>(parameterList, !string.IsNullOrEmpty(Session.SessionKey), new FacebookCallCompleted<long>(callback), state);
                return 0;
            }

            var response = SendRequest<dashboard_addGlobalNews_response>(parameterList, !string.IsNullOrEmpty(Session.SessionKey));
            return response == null ? -1 : response.TypedValue;
        }
        private long AddNews(dashboard_news news, long uid, bool isAsync, AddNewsCallback callback, Object state)
        {
            var parameterList = new Dictionary<string, string> { { "method", "facebook.dashboard.addNews" } };
            Utilities.AddOptionalParameter(parameterList, "image", news.image);
            Utilities.AddOptionalParameter(parameterList, "uid", uid);
            List<string> newsList = new List<string>();
            foreach (var item in news.news)
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("message", item.message);
                if (item.action_link != null && item.action_link.href != null)
                {
                    Dictionary<string, string> dict_actionlink = new Dictionary<string, string>();
                    dict_actionlink.Add("text", item.action_link.text);
                    dict_actionlink.Add("href", item.action_link.href);
                    dict.Add("action_link", JSONHelper.ConvertToJSONAssociativeArray(dict_actionlink));
                }
                newsList.Add(JSONHelper.ConvertToJSONAssociativeArray(dict));
            }
            Utilities.AddJSONArray(parameterList, "news", newsList);

            if (isAsync)
            {
                SendRequestAsync<dashboard_addNews_response, long>(parameterList, !string.IsNullOrEmpty(Session.SessionKey), new FacebookCallCompleted<long>(callback), state);
                return 0;
            }

            var response = SendRequest<dashboard_addNews_response>(parameterList, !string.IsNullOrEmpty(Session.SessionKey));
            return response == null ? -1 : response.TypedValue;
        }
        private Dictionary<string,object> MultiAddNews(dashboard_news news, List<long> uid, bool isAsync, MultiAddNewsCallback callback, Object state)
        {
            //if (uid.Count <= 1)
            //    throw new FacebookException("method only supports multiple uids");
            var parameterList = new Dictionary<string, string> { { "method", "facebook.dashboard.multiAddNews" } };
            Utilities.AddOptionalParameter(parameterList, "image", news.image);
            Utilities.AddJSONArray(parameterList, "uids", uid);
            List<string> newsList = new List<string>();
            foreach (var item in news.news)
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("message", item.message);
                if (item.action_link != null && item.action_link.href != null)
                {
                    Dictionary<string, string> dict_actionlink = new Dictionary<string, string>();
                    dict_actionlink.Add("text", item.action_link.text);
                    dict_actionlink.Add("href", item.action_link.href);
                    dict.Add("action_link", JSONHelper.ConvertToJSONAssociativeArray(dict_actionlink));
                }
                newsList.Add(JSONHelper.ConvertToJSONAssociativeArray(dict));
            }
            Utilities.AddJSONArray(parameterList, "news", newsList);

            if (isAsync)
            {
                SendRequestAsync<Dictionary<string, object>, Dictionary<string, object>>(parameterList, !string.IsNullOrEmpty(Session.SessionKey), true, new FacebookCallCompleted<Dictionary<string, object>>(callback), state, null);
                return null;
            }

            var response = SendRequest<dashboard_multiAddNews_response>(parameterList, !string.IsNullOrEmpty(Session.SessionKey));
            return response == null ? null : response;
        }
        private Dictionary<string, bool> MultiDecrementCount(List<long> uids, bool isAsync, MultiDecrementCountCallback callback, Object state)
        {
            var parameterList = new Dictionary<string, string> { { "method", "facebook.dashboard.multiDecrementCount" } };
            Utilities.AddJSONArray(parameterList, "uids", uids);

            if (isAsync)
            {
                SendRequestAsync<Dictionary<string, bool>, Dictionary<string, bool>>(parameterList, !string.IsNullOrEmpty(Session.SessionKey), true, new FacebookCallCompleted<Dictionary<string, bool>>(callback), state, null);
                return null;
            }

            var response = SendRequest<dashboard_multiDecrementCount_response>(parameterList, !string.IsNullOrEmpty(Session.SessionKey), true);
            return response == null ? null : response;
        }
        private Dictionary<string, bool> MultiIncrementCount(List<long> uids, bool isAsync, MultiIncrementCountCallback callback, Object state)
        {
            var parameterList = new Dictionary<string, string> { { "method", "facebook.dashboard.multiIncrementCount" } };
            Utilities.AddJSONArray(parameterList, "uids", uids);

            if (isAsync)
            {
                SendRequestAsync<Dictionary<string, bool>, Dictionary<string, bool>>(parameterList, !string.IsNullOrEmpty(Session.SessionKey), true, new FacebookCallCompleted<Dictionary<string, bool>>(callback), state, null);
                return null;
            }

            var response = SendRequest<dashboard_multiIncrementCount_response>(parameterList, !string.IsNullOrEmpty(Session.SessionKey), true);
            return response == null ? null : response;
        }
        private Dictionary<string, int> MultiGetCount(List<long> uids, bool isAsync, MultiGetCountCallback callback, Object state)
        {
            var parameterList = new Dictionary<string, string> { { "method", "facebook.dashboard.multiGetCount" } };
            Utilities.AddJSONArray(parameterList, "uids", uids);

            if (isAsync)
            {
                SendRequestAsync<Dictionary<string, int>, Dictionary<string, int>>(parameterList, !string.IsNullOrEmpty(Session.SessionKey), true, new FacebookCallCompleted<Dictionary<string, int>>(callback), state, null);
                return null;
            }

            var response = SendRequest<dashboard_multiGetCount_response>(parameterList, !string.IsNullOrEmpty(Session.SessionKey), true);
            return response == null ? null : response;
        }
        private Dictionary<string, bool> MultiSetCount(Dictionary<long,long> ids, bool isAsync, MultiSetCountCallback callback, Object state)
        {

            var parameterList = new Dictionary<string, string> { { "method", "facebook.dashboard.multiSetCount" } };
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var item in ids)
            {
                dict.Add(item.Key.ToString(), item.Value.ToString());
            }
            Utilities.AddJSONAssociativeArray(parameterList, "ids", dict);

            if (isAsync)
            {
                SendRequestAsync<Dictionary<string, bool>, Dictionary<string, bool>>(parameterList, !string.IsNullOrEmpty(Session.SessionKey), true, new FacebookCallCompleted<Dictionary<string, bool>>(callback), state, null);
                return null;
            }

            var response = SendRequest<dashboard_multiSetCount_response>(parameterList, !string.IsNullOrEmpty(Session.SessionKey), true);
            return response == null ? null : response;
        }
        private Dictionary<string, Dictionary<string, object>> MultiClearNews(Dictionary<long, List<string>> ids, bool isAsync, MultiClearNewsCallback callback, Object state)
        {

            var parameterList = new Dictionary<string, string> { { "method", "facebook.dashboard.multiClearNews" } };
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var item in ids)
            {
                dict.Add(item.Key.ToString(), JSONHelper.ConvertToJSONArray(item.Value));
            }
            Utilities.AddJSONAssociativeArray(parameterList, "ids", dict);

            if (isAsync)
            {
                SendRequestAsync<Dictionary<string, Dictionary<string, object>>, Dictionary<string, Dictionary<string, object>>>(parameterList, !string.IsNullOrEmpty(Session.SessionKey), true, new FacebookCallCompleted<Dictionary<string, Dictionary<string, object>>>(callback), state, null);
                return null;
            }

            var response = SendRequest<dashboard_multiClearNews_response>(parameterList, !string.IsNullOrEmpty(Session.SessionKey), true);
            return response == null ? null : response;
        }
        private Dictionary<string, List<dashboard_news>> MultiGetNews(Dictionary<long, List<string>> ids, bool isAsync, MultiGetNewsCallback callback, Object state)
        {

            var parameterList = new Dictionary<string, string> { { "method", "facebook.dashboard.multiGetNews" } };
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var item in ids)
            {
                dict.Add(item.Key.ToString(), JSONHelper.ConvertToJSONArray(item.Value));
            }
            Utilities.AddJSONAssociativeArray(parameterList, "ids", dict);

            if (isAsync)
            {
                SendRequestAsync<Dictionary<string, List<dashboard_news>>, Dictionary<string, List<dashboard_news>>>(parameterList, !string.IsNullOrEmpty(Session.SessionKey), true, new FacebookCallCompleted<Dictionary<string, List<dashboard_news>>>(callback), state, null);
                return null;
            }

            var response = SendRequest<dashboard_multiGetNews_response>(parameterList, !string.IsNullOrEmpty(Session.SessionKey), true);
            if (response != null)
            {
                var news = new Dictionary<string, List<dashboard_news>>();

#if !SILVERLIGHT
                foreach (var item in response)
                {
                    var converter = new Dashboard_GetNewsResponseConverter();
                    news.Add(item.Key, (List<dashboard_news>)converter.Deserialize(item.Value, typeof(dashboard_news), null));
                }
#endif
                return news;
            }
            else
            {
                return null;
            }
        }
        private Dictionary<string, bool> ClearGlobalNews(List<long> news_ids, bool isAsync, ClearGlobalNewsCallback callback, Object state)
        {
            var parameterList = new Dictionary<string, string> { { "method", "facebook.dashboard.clearGlobalNews" } };
            Utilities.AddJSONArray(parameterList, "news_ids", news_ids);

            if (isAsync)
            {
                SendRequestAsync<Dictionary<string, bool>, Dictionary<string, bool>>(parameterList, !string.IsNullOrEmpty(Session.SessionKey), true, new FacebookCallCompleted<Dictionary<string, bool>>(callback), state, null);
                return null;
            }

            var response = SendRequest<dashboard_clearGlobalNews_response>(parameterList, !string.IsNullOrEmpty(Session.SessionKey), true);

            if (response != null)
            {
                return response;
            }
            else
            {
                return null;
            }

        }
        private Dictionary<string, bool> ClearNews(List<long> news_ids, long uid, bool isAsync, ClearNewsCallback callback, Object state)
        {
            var parameterList = new Dictionary<string, string> { { "method", "facebook.dashboard.clearNews" } };
            Utilities.AddOptionalParameter(parameterList, "uid", uid);
            Utilities.AddJSONArray(parameterList, "news_ids", news_ids);

            if (isAsync)
            {
                SendRequestAsync<Dictionary<string, bool>, Dictionary<string, bool>>(parameterList, !string.IsNullOrEmpty(Session.SessionKey), true, new FacebookCallCompleted<Dictionary<string, bool>>(callback), state, null);
                return null;
            }

            var response = SendRequest<dashboard_clearNews_response>(parameterList, !string.IsNullOrEmpty(Session.SessionKey), true);

            if (response != null)
            {
                return response;
            }
            else
            {
                return null;
            }

        }
        private bool DecrementCount(long uid, bool isAsync, DecrementCountCallback callback, Object state)
        {
            var parameterList = new Dictionary<string, string> { { "method", "facebook.dashboard.decrementCount" } };
            Utilities.AddOptionalParameter(parameterList, "uid", uid);

            if (isAsync)
            {
                SendRequestAsync<dashboard_decrementCount_response, bool>(parameterList, !string.IsNullOrEmpty(Session.SessionKey), new FacebookCallCompleted<bool>(callback), state);
                return false;
            }

            var response = SendRequest<dashboard_decrementCount_response>(parameterList, !string.IsNullOrEmpty(Session.SessionKey));
            return response == null ? false : response.TypedValue;
        }
        private bool IncrementCount(long uid, bool isAsync, IncrementCountCallback callback, Object state)
        {
            var parameterList = new Dictionary<string, string> { { "method", "facebook.dashboard.incrementCount" } };
            Utilities.AddOptionalParameter(parameterList, "uid", uid);

            if (isAsync)
            {
                SendRequestAsync<dashboard_incrementCount_response, bool>(parameterList, !string.IsNullOrEmpty(Session.SessionKey), new FacebookCallCompleted<bool>(callback), state);
                return false;
            }

            var response = SendRequest<dashboard_incrementCount_response>(parameterList, !string.IsNullOrEmpty(Session.SessionKey));
            return response == null ? false : response.TypedValue;
        }
        private bool SetCount(long uid, long count, bool isAsync, SetCountCallback callback, Object state)
        {
            var parameterList = new Dictionary<string, string> { { "method", "facebook.dashboard.setCount" } };
            Utilities.AddOptionalParameter(parameterList, "uid", uid);
            Utilities.AddOptionalParameter(parameterList, "count", count);

            if (isAsync)
            {
                SendRequestAsync<dashboard_setCount_response, bool>(parameterList, !string.IsNullOrEmpty(Session.SessionKey), new FacebookCallCompleted<bool>(callback), state);
                return false;
            }

            var response = SendRequest<dashboard_setCount_response>(parameterList, !string.IsNullOrEmpty(Session.SessionKey));
            return response == null ? false : response.TypedValue;
        }
        private List<dashboard_activity> GetActivity(List<long> activity_ids, long uid, bool isAsync, GetActivityCallback callback, Object state)
        {
            var parameterList = new Dictionary<string, string> { { "method", "facebook.dashboard.getActivity" } };
            Utilities.AddOptionalParameter(parameterList, "uid", uid);
            Utilities.AddJSONArray(parameterList, "activity_ids", activity_ids);

            if (isAsync)
            {
                SendRequestAsync<List<dashboard_activity>, List<dashboard_activity>>(parameterList, !string.IsNullOrEmpty(Session.SessionKey), true, new FacebookCallCompleted<List<dashboard_activity>>(callback), state, null);
                return null;
            }

            var response = SendRequest<List<dashboard_activity>>(parameterList, !string.IsNullOrEmpty(Session.SessionKey), true);
            return response == null ? null : response;
        }
        private Dictionary<string,bool> RemoveActivity(List<long> activity_ids, bool isAsync, RemoveActivityCallback callback, Object state)
        {
            var parameterList = new Dictionary<string, string> { { "method", "facebook.dashboard.removeActivity" } };
            Utilities.AddJSONArray(parameterList, "activity_ids", activity_ids);

            if (isAsync)
            {
                SendRequestAsync<Dictionary<string, bool>, Dictionary<string, bool>>(parameterList, !string.IsNullOrEmpty(Session.SessionKey), true, new FacebookCallCompleted<Dictionary<string, bool>>(callback), state, null);
                return null;
            }

            var response = SendRequest<dashboard_removeActivity_response>(parameterList, !string.IsNullOrEmpty(Session.SessionKey), true);
            return response == null ? null : response;
        }
        private int GetCount(long uid, bool isAsync, GetCountCallback callback, Object state)
        {
            var parameterList = new Dictionary<string, string> { { "method", "facebook.dashboard.getCount" } };
            Utilities.AddOptionalParameter(parameterList, "uid", uid);

            if (isAsync)
            {
                SendRequestAsync<dashboard_getCount_response, int>(parameterList, !string.IsNullOrEmpty(Session.SessionKey), new FacebookCallCompleted<int>(callback), state);
                return 0;
            }

            var response = SendRequest<dashboard_getCount_response>(parameterList, !string.IsNullOrEmpty(Session.SessionKey));
            return response == null ? -1 : response.TypedValue;
        }
        private List<dashboard_news> GetNews(List<long> news_ids, long uid, bool isAsync, GetNewsCallback callback, Object state)
        {
            var parameterList = new Dictionary<string, string> { { "method", "facebook.dashboard.getNews" } };
            Utilities.AddOptionalParameter(parameterList, "uid", uid);
            Utilities.AddJSONArray(parameterList, "news_ids", news_ids);

            if (isAsync)
            {
                SendRequestAsync<List<dashboard_news>, List<dashboard_news>>(parameterList, !string.IsNullOrEmpty(Session.SessionKey), true, new FacebookCallCompleted<List<dashboard_news>>(callback), state, null);
                return null;
            }

            var response = SendRequest<List<dashboard_news>>(parameterList, !string.IsNullOrEmpty(Session.SessionKey), true);
            return response == null ? null : response;
        }
        private List<dashboard_news> GetGlobalNews(List<long> news_ids, bool isAsync, GetGlobalNewsCallback callback, Object state)
        {
            var parameterList = new Dictionary<string, string> { { "method", "facebook.dashboard.getGlobalNews" } };
            Utilities.AddJSONArray(parameterList, "news_ids", news_ids);

            if (isAsync)
            {
                SendRequestAsync<List<dashboard_news>, List<dashboard_news>>(parameterList, !string.IsNullOrEmpty(Session.SessionKey), true, new FacebookCallCompleted<List<dashboard_news>>(callback), state, null);
                return null;
            }

            var response = SendRequest<List<dashboard_news>>(parameterList, !string.IsNullOrEmpty(Session.SessionKey), true);
            return response == null ? null : response;
        }
        private long PublishActivity(dashboard_bundle activity, bool isAsync, PublishActivityCallback callback, Object state)
        {
            var parameterList = new Dictionary<string, string> { { "method", "facebook.dashboard.publishActivity" } };
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("message", activity.message);
            if (activity.action_link != null && activity.action_link.href != null)
            {
                Dictionary<string, string> dict_actionlink = new Dictionary<string, string>();
                dict_actionlink.Add("text", activity.action_link.text);
                dict_actionlink.Add("href", activity.action_link.href);
                dict.Add("action_link", JSONHelper.ConvertToJSONAssociativeArray(dict_actionlink));
            }
            Utilities.AddJSONAssociativeArray(parameterList, "activity", dict);

            if (isAsync)
            {
                SendRequestAsync<dashboard_publishActivity_response, long>(parameterList, !string.IsNullOrEmpty(Session.SessionKey), new FacebookCallCompleted<long>(callback), state);
                return 0;
            }

            var response = SendRequest<dashboard_publishActivity_response>(parameterList, !string.IsNullOrEmpty(Session.SessionKey));
            return response == null ? -1 : response.TypedValue;
        }

        
        #endregion Private Methods
   
        #endregion Methods

        #region Delegates

    /// <summary>
    /// Delegate called when AddGlobalNews call is completed.
    /// </summary>
    /// <param name="news_id">news</param>
    /// <param name="state">An object containing state information for this asynchronous request</param>
    /// <param name="e">Exception object, if the call resulted in exception.</param>
    public delegate void AddGlobalNewsCallback(long news_id, Object state, FacebookException e);

    /// <summary>
    /// Delegate called when AddNews call is completed.
    /// </summary>
    /// <param name="news_id">news</param>
    /// <param name="state">An object containing state information for this asynchronous request</param>
    /// <param name="e">Exception object, if the call resulted in exception.</param>
    public delegate void AddNewsCallback(long news_id, Object state, FacebookException e);
    /// <summary>
    /// Delegate called when MultiAddNews call is completed.
    /// </summary>
    /// <param name="result">result</param>
    /// <param name="state">An object containing state information for this asynchronous request</param>
    /// <param name="e">Exception object, if the call resulted in exception.</param>
    public delegate void MultiAddNewsCallback(Dictionary<string, object> result, Object state, FacebookException e);
    /// <summary>
    /// Delegate called when MultiDecrementCount call is completed.
    /// </summary>
    /// <param name="result">result</param>
    /// <param name="state">An object containing state information for this asynchronous request</param>
    /// <param name="e">Exception object, if the call resulted in exception.</param>
    public delegate void MultiDecrementCountCallback(Dictionary<string, bool> result, Object state, FacebookException e);
    /// <summary>
    /// Delegate called when MultiIncrementCount call is completed.
    /// </summary>
    /// <param name="result">result</param>
    /// <param name="state">An object containing state information for this asynchronous request</param>
    /// <param name="e">Exception object, if the call resulted in exception.</param>
    public delegate void MultiIncrementCountCallback(Dictionary<string, bool> result, Object state, FacebookException e);
    /// <summary>
    /// Delegate called when MultiGetCount call is completed.
    /// </summary>
    /// <param name="result">result</param>
    /// <param name="state">An object containing state information for this asynchronous request</param>
    /// <param name="e">Exception object, if the call resulted in exception.</param>
    public delegate void MultiGetCountCallback(Dictionary<string, int> result, Object state, FacebookException e);
    /// <summary>
    /// Delegate called when MultiSetCount call is completed.
    /// </summary>
    /// <param name="result">result</param>
    /// <param name="state">An object containing state information for this asynchronous request</param>
    /// <param name="e">Exception object, if the call resulted in exception.</param>
    public delegate void MultiSetCountCallback(Dictionary<string, bool> result, Object state, FacebookException e);
    /// <summary>
    /// Delegate called when MultiClearNews call is completed.
    /// </summary>
    /// <param name="result">result</param>
    /// <param name="state">An object containing state information for this asynchronous request</param>
    /// <param name="e">Exception object, if the call resulted in exception.</param>
    public delegate void MultiClearNewsCallback(Dictionary<string, Dictionary<string, object>> result, Object state, FacebookException e);
    /// <summary>
    /// Delegate called when MultiGetNews call is completed.
    /// </summary>
    /// <param name="result">result</param>
    /// <param name="state">An object containing state information for this asynchronous request</param>
    /// <param name="e">Exception object, if the call resulted in exception.</param>
    public delegate void MultiGetNewsCallback(Dictionary<string, List<dashboard_news>> result, Object state, FacebookException e);
    /// <summary>
    /// Delegate called when ClearGlobalNews call is completed.
    /// </summary>
    /// <param name="news_ids">news</param>
    /// <param name="state">An object containing state information for this asynchronous request</param>
    /// <param name="e">Exception object, if the call resulted in exception.</param>
    public delegate void ClearGlobalNewsCallback(Dictionary<string,bool> news_ids, Object state, FacebookException e);
    /// <summary>
    /// Delegate called when ClearNews call is completed.
    /// </summary>
    /// <param name="news_ids">news</param>
    /// <param name="state">An object containing state information for this asynchronous request</param>
    /// <param name="e">Exception object, if the call resulted in exception.</param>
    public delegate void ClearNewsCallback(Dictionary<string, bool> news_ids, Object state, FacebookException e);
    /// <summary>
    /// Delegate called when DecrementCount call is completed.
    /// </summary>
    /// <param name="success">success</param>
    /// <param name="state">An object containing state information for this asynchronous request</param>
    /// <param name="e">Exception object, if the call resulted in exception.</param>
    public delegate void DecrementCountCallback(bool success, Object state, FacebookException e);
    /// <summary>
    /// Delegate called when SetCount call is completed.
    /// </summary>
    /// <param name="success">success</param>
    /// <param name="state">An object containing state information for this asynchronous request</param>
    /// <param name="e">Exception object, if the call resulted in exception.</param>
    public delegate void SetCountCallback(bool success, Object state, FacebookException e);
    /// <summary>
    /// Delegate called when IncrementCount call is completed.
    /// </summary>
    /// <param name="success">success</param>
    /// <param name="state">An object containing state information for this asynchronous request</param>
    /// <param name="e">Exception object, if the call resulted in exception.</param>
    public delegate void IncrementCountCallback(bool success, Object state, FacebookException e);
    /// <summary>
    /// Delegate called when GetCount call is completed.
    /// </summary>
    /// <param name="count">count</param>
    /// <param name="state">An object containing state information for this asynchronous request</param>
    /// <param name="e">Exception object, if the call resulted in exception.</param>
    public delegate void GetCountCallback(int count, Object state, FacebookException e);
    /// <summary>
    /// Delegate called when GetActivity call is completed.
    /// </summary>
    /// <param name="activities">news</param>
    /// <param name="state">An object containing state information for this asynchronous request</param>
    /// <param name="e">Exception object, if the call resulted in exception.</param>
    public delegate void GetActivityCallback(List<dashboard_activity> activities, Object state, FacebookException e);
    /// <summary>
    /// Delegate called when GetActivity call is completed.
    /// </summary>
    /// <param name="result">result</param>
    /// <param name="state">An object containing state information for this asynchronous request</param>
    /// <param name="e">Exception object, if the call resulted in exception.</param>
    public delegate void RemoveActivityCallback(Dictionary<string, bool> result, Object state, FacebookException e);
    /// <summary>
    /// Delegate called when PublishActivity call is completed.
    /// </summary>
    /// <param name="activity_id">activity</param>
    /// <param name="state">An object containing state information for this asynchronous request</param>
    /// <param name="e">Exception object, if the call resulted in exception.</param>
    public delegate void PublishActivityCallback(long activity_id, Object state, FacebookException e);
    /// <summary>
    /// Delegate called when GetNews call is completed.
    /// </summary>
    /// <param name="news">news</param>
    /// <param name="state">An object containing state information for this asynchronous request</param>
    /// <param name="e">Exception object, if the call resulted in exception.</param>
    public delegate void GetNewsCallback(List<dashboard_news> news, Object state, FacebookException e);
    /// <summary>
    /// Delegate called when GetGlobalNews call is completed.
    /// </summary>
    /// <param name="news">news</param>
    /// <param name="state">An object containing state information for this asynchronous request</param>
    /// <param name="e">Exception object, if the call resulted in exception.</param>
    public delegate void GetGlobalNewsCallback(List<dashboard_news> news, Object state, FacebookException e);

    
    #endregion Delegates
    }
}








