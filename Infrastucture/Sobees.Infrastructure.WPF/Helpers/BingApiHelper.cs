#region

using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using Sobees.Tools.Logging;

#endregion

namespace Sobees.Infrastructure.Helpers
{
  internal class BingApiHelper
  {
    public const string BING_APPID = "C300AC36FF82B3A21620B93144A82F3760AB6F75";

    private const string APPNAME = "BingApiHelper";
    private const string AppId = BING_APPID;
    //private const string TranslateApiBaseUrl = "http://api.microsofttranslator.com/V2/Ajax.svc/Translate?appId={0}&from={1}&to={2}&text={3}";
    private const string TranslateApiBaseUrl = "http://api.microsofttranslator.com/V2/Ajax.svc/Translate?appId={0}&to={1}&text={2}";
    private const string DetectApiBaseUrl = "http://api.microsofttranslator.com/V2/Ajax.svc/Detect?appId={0}&text={1}";
    
    /// <summary>
    /// TranslateText
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string TranslateText(string text)
    {
      var result = GetTranslateText(text);
      return result;
    }

    /// <summary>
    /// TranslateText
    /// </summary>
    /// <param name="text"></param>
    /// <param name="targetLanguage"></param>
    /// <returns></returns>
    public static string TranslateText(string text, string targetLanguage)
    {
      var targetlang = GetLanguageCodeFromLanguageName(targetLanguage);
      var result = GetTranslateText(text, targetlang);
      return result;
    }

    /// <summary>
    /// GetResult
    /// </summary>
    /// <param name="stringResult"></param>
    /// <returns></returns>
    public static string GetResult(string stringResult)
    {
      var result = stringResult.Replace("\"", "");
      return result;
    }

    /// <summary>
    /// GetTranslateText
    /// </summary>
    /// <param name="text"></param>
    /// <param name="targetLanguage"></param>
    /// <returns></returns>
    internal static string GetTranslateText(string text, string targetLanguage = "en")
    {
      //var sourcelang = GetDetectedLang(text);
      var response = BuildRequest(text, null, targetLanguage);
      var result = GetResult(response);
      return result;
    }

    /// <summary>
    /// GetDetectedLang
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    internal static string GetDetectedLang(string text)
    {
      var response = BuildDetectRequest(text);
      var result = GetResult(response);
      return result;
    }

    /// <summary>
    /// BuildRequest
    /// </summary>
    /// <param name="textToTranslate"></param>
    /// <param name="sourceLanguage"></param>
    /// <param name="targetLanguage"></param>
    /// <returns></returns>
    public static string BuildRequest(string textToTranslate, string sourceLanguage, string targetLanguage)
    {
      //var requestString = string.Format(TranslateApiBaseUrl, AppId, sourceLanguage, targetLanguage, textToTranslate);
      var requestString = string.Format(TranslateApiBaseUrl, AppId, targetLanguage, textToTranslate);
      // Create and initialize the request.
      var client = new WebClient();
      var buffer = client.DownloadData(requestString);
      var result = Encoding.UTF8.GetString(buffer);
      return result;
    }

    /// <summary>
    /// BuildDetectRequest
    /// </summary>
    /// <param name="textToTranslate"></param>
    /// <returns></returns>
    public static string BuildDetectRequest(string textToTranslate)
    {
      var requestString = string.Format(DetectApiBaseUrl, AppId, textToTranslate);
      var client = new WebClient();
      var buffer = client.DownloadData(requestString);
      var result = Encoding.UTF8.GetString(buffer);
      return result;
    }

    /// <summary>
    /// GetLanguageCodeFromLanguageName
    /// </summary>
    /// <param name="languageName"></param>
    /// <returns></returns>
    public static string GetLanguageCodeFromLanguageName(string languageName)
    {
      
      if (languageName.Length == 2)
        return languageName;

      var lang = "en"; //default value
      try
      {
        
        var codelang = "";
        var codelangs = CultureInfo.GetCultures(CultureTypes.AllCultures).Where(ci => ci.DisplayName.ToLower() == languageName.ToLower()).ToList();
        if (codelangs.Count > 0)
        {
          var firstOrDefault = codelangs.FirstOrDefault();
          if (firstOrDefault != null)
            codelang = firstOrDefault.TwoLetterISOLanguageName;
        }
        //TwoLetterISOLanguageName;
        if (!string.IsNullOrEmpty(codelang))
          lang = codelang;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(APPNAME + "::GetLanguageCodeFromLanguageName", ex);
      }
      return lang;
    }
  }
}