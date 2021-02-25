#region

using System;
using System.Collections.Generic;
using System.Windows.Forms;

#endregion

namespace Sobees.Configuration.BGlobals
{
    public static class BGlobals
    {
#if SILVERLIGHT
    public const double OOB_WINDOW_WIDTH = 1000;
    public const double OOB_WINDOW_HEIGHT = 700;
#endif

        public const string CIPHER_KEY = "#sobeesReloaded@Rocks#";
        public const string TOKEN_VALIDATED = "{0}#sobees@token#validator#{1}";
        public const string CLIENTNAME = "sobees";
        public const double DEFAULT_FONTSIZE = 12;
        public const int DEFAULT_NB_POST_TO_GET = 50;
        public const int DEFAULT_NB_POST_TO_GET_SEARCH = 20;
        public const int DEFAULT_NB_POST_TO_KEEP = 100;
        public const int DEFAULT_NB_POST_TO_KEEP_SEARCH = 100;
        public const int DEFAULT_NB_POST_TO_GET_Twitter = 100;
        public static double CACHE_KEEP_TIME = 2.0; //days

        public const double DEFAULT_REFRESH_TIME = 60; // in minutes
        public const double DEFAULT_REFRESH_TIME_SERVICE = 5; // in minutes
#if !SILVERLIGHT
        public const string DEFAULT_THEME = "blue";
#else
    public const string DEFAULT_THEME = "classical";
#endif
        public const string THEMES_CLIENT_ID = "sobees";
        public const string THEMESLIST_XML_PATH = "Themes/ThemesList.xml";
        public const int DEFAULT_TIMER_REFRESH_PROFILE_MYSPACE_MAIL = 6; // in second
        public const int DEFAULT_TIMER_REFRESH_PROFILE_MYSPACE_MSG = 3; // in second
        public const double DEFAULT_TIMER_UPDATE = 15; // in minutes
        public const int DEFAULT_TIMER_UPDATE_FRIENDSLIST = 2; // in minutes
        public const EnumUrlShorteners DEFAULT_URLSHORTENER = EnumUrlShorteners.BitLy;
        public static string DEFAULT_URL_MAP = "http://maps.yahoo.com/#mvt=m&lat={0}&lon={1}&zoom=14";
        public const bool DEFAULT_USESEARCH_TWITTERSEARCH = true;
        public const bool ENABLE_MINIMIZEWINDOWINTRAY = false;
        public const bool ENABLE_ALERTS = true;
        public const bool ENABLE_GLOBALFILTER = false;
        public const bool ENABLE_DISABLEADS = false;
        public const bool ENABLE_RUNASSTARTUP = false;
        public const bool ENABLE_TWITTERFROM = false;

        public const string BING_APPID = "C300AC36FF82B3A21620B93144A82F3760AB6F75";

        public const string TWEETPHOTO_OAUTH_KEY = "d9074fa3-e163-465f-8626-7db63f066826";

        public const string TWITTER_OAUTH_KEY = "TFuWcVHyUGtAFUV2oTVQdQ";
        public const string TWITTER_OAUTH_SECRET = "gLa1LNpbfiyg1z0eRrWzzoFyX042kwTxfhYwA9JPJ0";

        public const double TWITTER_NUMBER_API_TO_KEEP = 15;
        public const int TWITTER_TREND_NUMBER = 25;

        public const string FACEBOOK_SILVERLIGHT_API = "98493d412d65f66ae8b74405cc19877e";
        public const string FACEBOOK_NAPA_SECRET = "8a34107e9f2628449abb68ecb9f6e06f";
        public const double FACEBOOK_NB_DAYS_BIRTHDAY = 7;

        public const string FACEBOOK_URLCALLBACK = "http://www.sobees.com/Silverlight/sobeesUtil/Web/Facebook.html";
        public const string LINKEDIN_URLCALLBACK = "http://www.sobees.com/Silverlight/sobeesUtil/Web/Linkedin.html";
        public const string TWITTER_URLCALLBACK = "http://www.sobees.com/Silverlight/sobeesUtil/Web/Twitter.html";

        public const string FACEBOOK_URLCALLBACKPerm =
            "http://www.sobees.com/Silverlight/sobeesUtil/Web/FacebookPerm.html";

        public const string MYSQPACE_URLCALLBACK = "http://www.sobees.com/Silverlight/sobeesUtil/Web/MySpace.html";
        public const string MYSQPACE_DEV_OAUTH_KEY = "affd08a6621347928e9a3db69109317b";

        public const string MYSQPACE_DEV_OAUTH_SECRET =
            "c95179cfd8254c0197ddcbe8e817216b0ad7d9c42dcd40c88374c931dd0f608f";

        public const string MYSQPACE_PROD_OAUTH_KEY = "b78b4364b39142dcb664cc2109d17c39";

        public const string MYSQPACE_PROD_OAUTH_SECRET =
            "379899c7656c4285b057407ac0d5ef3ff905eb941a564a4b91291e57b7912c57";

        public const string MYSQPACE_URLCALLBACK_MULTI =
            "http://62cpgccs64dhp.c.om.mail.yahoo.net/om/assets/3iob670pm6p31_1/MySpaceCallbackMulti.htm";

        public const bool MYSQPACE_USE_APPLICATION_PROD_KEY = true;

        public const string YAHOO_BASE_URL = "/om/assets/68p9k6gr3id1m_1/";
        public const string YAHOO_BASE_URL_ClientBin = "/om/assets/68p9k6gr3id1m_1/ClientBin/";

        public const string YAHOO_MAIL_HTML =
            "<div style=\"font-family: Arial, Helvetica, sans-serif;\"><p>{TEXT_TITLE}</p><div id=\"msg\" style=\"background-color: #EAF4FF;padding: 10px;border: 1px solid #AAD2FF;\"><img src=\"{IMG_URL}\" width=\"50\" height=\"50\" border=\"0\" /><br />{TWEET} </div><p>Cheers.<br /><br /><span style=\"color:#666;font-size:12px\">{TEXT_CHEERS}</span><span style=\"color:#666;font-size:12px\">{TEXT_SHARE}</span> <a href=\"http://www.sobees.com\" target=<\"_blank\"><img src=\"http://www.sobees.com/Silverlight/sobeesUtil/logo_small.png\" width=\"80\" height=\"20\" border=\"0\" style=\"margin-bottom:-5px\" /></a><br /></div>";

        public static int DEFAULT_TIMER_CHECKSERVICE = 30;
        public static int DEFAULT_TIMER_CLEANCOMMAND = 5;
        public static double MAX_DELAY_FOR_SERVICE = 5000; //in milliseconds


        public static List<string> MYSPACE_WS_URL_LIST = new List<string>
        {
            //"http://localhost:39337/MySpaceService.svc"
            "http://bmyspaceservicecloud.cloudapp.net/MySpaceService.svc",
            //"http://www.bdule.com/Silverlight/bMySpaceService/MySpaceService.svc",
            //"http://sobeessql002.sobees.com/Silverlight/bMySpaceService/MySpaceService.svc"
        };

        public static List<string> TWITTER_WS_URL_LIST = new List<string>
        {
            "http://btwitterservicecloud.cloudapp.net/TwitterService.svc",
            "http://sobeessql002.sobeesdata.com/Silverlight/BTwitterService/TwitterService.svc"

            //"http://a682757a5ab344b6aee6a4d9b45f58da.cloudapp.net/TwitterService.svc",
            //"http://sobeessqlDEV002.sobeesdata.com/Silverlight/BTwitterService/TwitterService.svc"
            //"http://localhost:1695/TwitterService.svc",
            //"http://btwitterservicecloud.cloudapp.net/TwitterService.svc",
            //"http://www.bdule.com/Silverlight/bTwitterservice/TwitterService.svc",
            //"http://sobeessql002.sobees.com/Silverlight/bTwitterService/TwitterService.svc"
        };

        public static List<string> UTIL_WS_URL_LIST = new List<string>
        {
            "http://butilservicecloud.cloudapp.net/SocialUtilService.svc",
            "http://sobeessql002.sobeesdata.com/Silverlight/BSocialServiceCloud/SocialUtilService.svc"

            //"http://localhost:49226/SocialUtilService.svc",                                                      
            //"http://sobeessql002.sobeesdata.com/Silverlight/BUtilService/UtilService.svc",
            //"http://butilservicecloud.cloudapp.net/SocialUtilService.svc",
            //"http://sobeessql002.sobees.com/Silverlight/bUtilService/SocialUtilService.svc",
            //"http://sobeessql002.sobees.com/Silverlight/BSocialServiceCloud/SocialUtilService.svc"
        };

        public static List<string> SEARCH_WS_URL_LIST = new List<string>
        {
            "http://bsearchservicecloud.cloudapp.net/BossService.svc",
            "http://www.bdule.com/Silverlight/bSearchService/BossService.svc",
            "http://sobeessql002.sobeesdata.com/Silverlight/bSearchService/BossService.svc"
        };

        public static int YAHOO_MAIL_SUBJECT_LENGTH = 40;
        public static string FACEBOOK_WPF_API = "95695045581";
        //public static string FACEBOOK_WPF_API_OLD = "2450322b04cb03bc257c67839357fad4";
        public static string FACEBOOK_WPF_SECRET = "d0dc7a9c8e64cfef216a08eab22f3df3";

#if !SILVERLIGHT
        public static string FOLDER = string.Format(@"{0}\{1}\{2}\tmp",
            Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData),
            Application.CompanyName, Application.ProductName);

        public static string FOLDERBASE = string.Format(@"{0}\{1}\{2}\",
            Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData),
            Application.CompanyName, Application.ProductName);

        public static string FOLDERBASESOBEES = string.Format(@"{0}\{1}\",
            Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData),
            Application.CompanyName);
#endif
        public static string FILEAUTOBACKUPNAME = "SobeesSettingAutoBackup.xml";

        public static string LINKEDIN_WPF_KEY = "iL9ASsDF23PPJhf1jK3zVdHgmm1n3vJS36Z_Nlix3g6SlLa5-Wgcu-BR6WJOzphG";
        public static string LINKEDIN_WPF_SECRET = "WKCYU-b9Zw5HXrO7OZ7qvbi7nHxWWRQgo35dQeJtoqRFCtsGrYjCpDecm7YTT1JA";
        public static string LINKEDIN_SL_KEY = "7XGBQP9hr1OzEWClyEXwwyxX7YMbJIV8qGAI19MVSujcSJpsMgSLLPrl1QLO5KF9";
        public static string LINKEDIN_SL_SECRET = "bj937JDCM5pJ36yi2HPvrKvSXeCzB07jwWYqQtYnVSzR6masI5NJbPH2e4ipmYIK";
        public static double DEFAULT_ERROR_VISIBILITY = 10; //secs

        public static string YAHOO_IMG_URL_BASE = "http://local.yahooapis.com/MapsService/V1/mapImage?";
        public static string YAHOO_APPID = "IJb2g1DV34GH0Iqj1LlR7QVmNDwe0XTgBpoWLuR8Bg.fACnwhQi9ZmJa1w2jPQSDdLQsgIE-";

        public static string SUPPORTLOG_DEFAULT_SUBJECT = "Sobees - Log Feedbacks";
        public static string SUPPORTLOG_FILENAME = "SupportLog.zip";
        public static string SUPPORTLOG_FILE_PATTERN = "*-logfile.*";
        public static string SUPPORTLOG_DEFAULT_MAIL_TO = "help@sobees.com";
        public static string SUPPORTLOG_CAPTURE_FILENAME = "Capture-logfile.jpg";
        public static string SUPPORTLOG_CRASH_EMAIL_FROM = "LogAfterCrash@sobees.com";

        public static int NUMBERTWEETSBETWEETADS = 50; // Number of tweets between two ads
        public static int RESTORESETTINGSTIMER = 30; //secondes

        public static string RESOURCES_ASSEMBLY = "Sobees.Configuration.BGlobals.Resources.Resources";

        /// <summary>
        ///     this function return the difference of hours from the SilliconValley Date against our actual Timezone
        /// </summary>
        /// <returns></returns>
        public static int GetDateTimeOffsetForFriendStatus()
        {
            try
            {
                const int silliconValleyGap = 7;
                //var dl = DateTime.Now.IsDaylightSavingTime();
                //var dlh = 0;
                //if (dl) dlh = -1;
                return GetDateTimeOffsetForMyActivity() + silliconValleyGap;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Globals::GetDateTimeOffsetForFriendStatus:",
                    ex);
            }

            return 0;
        }

        public static int GetDateTimeOffsetForMyActivity()
        {
            try
            {
                //var dl = DateTime.Now.IsDaylightSavingTime();
                //var dlh = 0;
                //if (dl) dlh = 1;
                //return DateTime.Now.Subtract(DateTime.UtcNow).Add(dlh.HoursAsTimeSpan()).Hours;
                return DateTime.Now.Subtract(DateTime.UtcNow).Hours;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Globals::GetDateTimeOffsetForMyActivity:",
                    ex);
            }

            return 0;
        }
    }
}