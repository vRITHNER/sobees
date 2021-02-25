﻿using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using LinqToTwitter.Common;
using LitJson;

namespace LinqToTwitter
{
  class TwitterErrorHandler
  {
    public static async Task ThrowIfErrorAsync(HttpResponseMessage msg)
    {
      // TODO: research proper handling of 304

      if ((int)msg.StatusCode < 400) return;

      switch (msg.StatusCode)
      {
        case HttpStatusCode.Unauthorized:
          await HandleUnauthorizedAsync(msg);
          break;
        default:
          await HandleGenericErrorAsync(msg);
          break;
      } 
    }
  
    internal static async Task HandleGenericErrorAsync(HttpResponseMessage msg)
    {
      string responseStr = await msg.Content.ReadAsStringAsync();

      BuildAndThrowTwitterQueryException(responseStr, msg);
    }
  
    internal static void BuildAndThrowTwitterQueryException(string responseStr, HttpResponseMessage msg)
    {
      TwitterErrorDetails error = ParseTwitterErrorMessage(responseStr);

      throw new TwitterQueryException(error.Message)
      {
        ErrorCode = error.Code,
        StatusCode = msg.StatusCode,
        ReasonPhrase = msg.ReasonPhrase
      };
    }
  
    internal async static Task HandleUnauthorizedAsync(HttpResponseMessage msg)
    {
      string responseStr = await msg.Content.ReadAsStringAsync();

      TwitterErrorDetails error = ParseTwitterErrorMessage(responseStr);

      string message = error.Message + " - Please visit the LINQ to Twitter FAQ (at the HelpLink) for help on resolving this error.";

      throw new TwitterQueryException(message)
      {
        HelpLink = "https://linqtotwitter.codeplex.com/wikipage?title=LINQ%20to%20Twitter%20FAQ",
        ErrorCode = error.Code,
        StatusCode = HttpStatusCode.Unauthorized,
        ReasonPhrase = msg.ReasonPhrase
      };
    }

    internal static TwitterErrorDetails ParseTwitterErrorMessage(string responseStr)
    {
      if (responseStr.StartsWith("{"))
      {
        JsonData responseJson = JsonMapper.ToObject(responseStr);

        var errors = responseJson.GetValue<JsonData>("errors");

        if (errors != null)
        {
          if (errors.GetJsonType() == JsonType.String)
            return new TwitterErrorDetails
            {
              Message = responseJson.GetValue<string>("errors"),
              Code = -1
            };

          if (errors.Count > 0)
          {
            var error = errors[0];
            return new TwitterErrorDetails
            {
              Message = error.GetValue<string>("message"),
              Code = error.GetValue<int>("code")
            };
          }
        }
      }

      return new TwitterErrorDetails { Message = responseStr };
    }

    internal class TwitterErrorDetails
    {
      public int Code { get; set; }
      public string Message { get; set; }
    }
  }
}

