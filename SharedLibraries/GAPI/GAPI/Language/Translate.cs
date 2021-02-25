using System.Web;
using Sobees.Library.BGoogleLib.Core;
using Sobees.Library.BGoogleLib.Json;

namespace Sobees.Library.BGoogleLib.Language
{
  public static class Translator
  {
    private const string LanguageApiVersion = "1.0";
    private const string LanguageDetectUrl = "http://ajax.googleapis.com/ajax/services/language/detect?v={0}&q={1}";

    private const string LanguageTranslateUrl =
      "http://ajax.googleapis.com/ajax/services/language/translate?v={0}&q={1}&langpair={2}|{3}";

    public static string Translate(string phrase, Language sourceLanguage, Language targetLanguage)
    {
      return Translate(phrase, ref sourceLanguage, targetLanguage);
    }

    public static string Translate(string phrase, Language targetLanguage)
    {
      Language sourceLanguage = Language.Unknown;
      return Translate(phrase, ref sourceLanguage, targetLanguage);
    }

    public static string Translate(string phrase, Language targetLanguage, out Language detectedSourceLanguage)
    {
      detectedSourceLanguage = Language.Unknown;
      return Translate(phrase, ref detectedSourceLanguage, targetLanguage);
    }

    private static string Translate(string phrase, ref Language sourceLanguage, Language targetLanguage)
    {
      if (string.IsNullOrEmpty(phrase))
        return "";

      string url = string.Format(LanguageTranslateUrl, LanguageApiVersion,
                                 HttpUtility.UrlEncode(phrase),
                                 LanguageHelper.GetLanguageString(sourceLanguage),
                                 LanguageHelper.GetLanguageString(targetLanguage));

      string responseData = CoreHelper.PerformRequest(url);

      JsonObject jsonObject = CoreHelper.ParseGoogleAjaxAPIResponse(responseData);

      // Translation response validation
      // Get 'responseData'
      if (jsonObject.ContainsKey("responseData") == false)
        throw new GapiException("Invalid response - no responseData: " + responseData);

      if (!(jsonObject["responseData"] is JsonObject))
        throw new GapiException("Invalid response - responseData is not JsonObject: " + responseData);

      // Get 'translatedText'
      var responseContent = (JsonObject) jsonObject["responseData"];
      if (responseContent.ContainsKey("translatedText") == false)
        throw new GapiException("Invalid response - no translatedText: " + responseData);

      if (!(responseContent["translatedText"] is JsonString))
        throw new GapiException("Invalid response - translatedText is not JsonString: " + responseData);

      string translatedPhrase = ((JsonString) responseContent["translatedText"]).Value;

      // If there's a detected language - return it
      if (responseContent.ContainsKey("detectedSourceLanguage") &&
          (responseContent["detectedSourceLanguage"] is JsonString))
      {
        var detectedSourceLanguage = (JsonString) responseContent["detectedSourceLanguage"];
        sourceLanguage = LanguageHelper.GetLanguage(detectedSourceLanguage.Value);
      }

      return HttpUtility.HtmlDecode(translatedPhrase);
    }

    public static Language Detect(string phrase)
    {
      bool isReliable;
      double confidence;

      return Detect(phrase, out isReliable, out confidence);
    }

    public static Language Detect(string phrase, out bool isReliable, out double confidence)
    {
      if (string.IsNullOrEmpty(phrase))
        throw new GapiException("No phrase to detect from");

      string url = string.Format(LanguageDetectUrl,
                                 LanguageApiVersion,
                                 HttpUtility.UrlEncode(phrase));

      string responseData = CoreHelper.PerformRequest(url);

      JsonObject jsonObject = CoreHelper.ParseGoogleAjaxAPIResponse(responseData);

      // Translation response validation
      // Get 'responseData'
      if (jsonObject.ContainsKey("responseData") == false)
        throw new GapiException("Invalid response - no responseData: " + responseData);

      if (!(jsonObject["responseData"] is JsonObject))
        throw new GapiException("Invalid response - responseData is not JsonObject: " + responseData);

      // Get 'translatedText'
      var responseContent = (JsonObject) jsonObject["responseData"];
      if (responseContent.ContainsKey("language") == false)
        throw new GapiException("Invalid response - no language: " + responseData);

      if (!(responseContent["language"] is JsonString))
        throw new GapiException("Invalid response - language is not JsonString: " + responseData);

      string language = ((JsonString) responseContent["language"]).Value;

      isReliable = false;
      confidence = 0;

      if (responseContent.ContainsKey("isReliable") &&
          (responseContent["isReliable"] is JsonBoolean))
      {
        isReliable = ((JsonBoolean) responseContent["isReliable"]).Value;
      }

      if (responseContent.ContainsKey("confidence") &&
          (responseContent["confidence"] is JsonNumber))
      {
        confidence = ((JsonNumber) responseContent["confidence"]).DoubleValue;
      }

      return LanguageHelper.GetLanguage(language);
    }
  }
}