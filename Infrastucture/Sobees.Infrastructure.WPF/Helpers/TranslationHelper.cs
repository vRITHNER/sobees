#region

using System;
using System.Diagnostics;
using System.Globalization;
#endregion

namespace Sobees.Infrastructure.Helpers
{
  public class TranslationHelper
  {
    public static string TranslateYourText(string text, string langFrom, string langTo)
    {
      return TranslateYourText(text, langFrom, langFrom, false);
    }

    public static string TranslateYourText(string text, string langFrom, string langTo, bool autoDetect)
    {
      var translated = "";
      try
      {
        translated = BingApiHelper.TranslateText(text);
        return translated;
         
        //var client = new TranslateClient("");
        //var from = "";
        //string lang1 = "";
        //if (autoDetect)
        //{
        //  translated = client.TranslateAndDetect(text, langTo, out from);
        //  foreach (string options in Language.GetEnums())
        //  {
        //    if (options == from)
        //    {
        //      var ci = new CultureInfo(from);
        //      lang1 = ci.ThreeLetterISOLanguageName;
        //      break;
        //    }
        //  }
        //}
        //translated = client.Translate(text, lang1, langTo, TranslateFormat.Text);
        //Console.WriteLine(translated);

        return translated;
      }
      catch (Exception ex)
      {
        Debug.WriteLine("error:", ex.Message);
        return translated;
      }
    }
  }
}