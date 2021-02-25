using System;
using System.Collections.Generic;
using System.Linq;
using Sobees.Library.BGenericLib;
using Sobees.Library.BServicesLib.BingService;
using Sobees.Tools.Logging;

namespace Sobees.Library.BServicesLib
{
  public class BingHelper
  {
    private static string apiKey = "C300AC36FF82B3A21620B93144A82F3760AB6F75";

    public static List<Entry> SearchWeb(string query,
                                             int count,
                                             int offset,
                                             string language,
                                             string market)
    {
      List<Entry> entries = null;
      try
      {
        using (var service = new BingPortTypeClient())
        {
          var request = new SearchRequest
                          {
                            AppId = apiKey,
                            Sources = new[] {SourceType.Web},
                            Query = query,
                            UILanguage = language
                          };


          if (!string.IsNullOrEmpty(market))
            request.Market = market; //"fr-FR"

          var webrequest = new WebRequest
                             {
                               Count = uint.Parse(count.ToString()),
                               CountSpecified = true,
                               Offset = uint.Parse(offset.ToString()),
                               OffsetSpecified = true
                             };

          request.Web = webrequest;

          SearchResponse response = service.Search(request);

          var exception = response.Errors;

          try
          {
            if (exception == null)
            {
              //Get results
              //var response = e.Result;

              //If there are some results
              if (response.Web != null && response.Web.Results.Count() > 0)
              {                
                entries = (from result in response.Web.Results
                           select new Entry
                                    {
                                      Title = result.Title,
                                      Content = result.Description,
                                      DisplayLink = result.DisplayUrl,
                                      Link = result.Url,
                                      Type = EnumType.Bing
                                    }).ToList();
              }
            }
          }
          catch (Exception ex)
          {
            //exception = ex;
            TraceHelper.Trace("BingHelper -> soapClient_SearchWebCompleted() -> ",
                              ex);
          }
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("BingHelper -> soapClient_SearchWebCompleted() -> ",
                          ex);
      }
      return entries;
    }

    public static List<Picture> SearchImage(string query,
                                          int count,
                                          int offset,
                                          string language)
    {
      List<Picture> entries = null;
      try
      {
        using (var service = new BingPortTypeClient())
        {
          var request = new SearchRequest {AppId = apiKey, Sources = new[] {SourceType.Image}, Query = query};


          var imagerequest = new ImageRequest
                               {
                                 Count = uint.Parse(count.ToString()),
                                 CountSpecified = true,
                                 Offset = uint.Parse(offset.ToString()),
                                 OffsetSpecified = true
                               };

          request.Image = imagerequest;

          SearchResponse response = service.Search(request);

          var exception = response.Errors;

          try
          {
            if (exception == null)
            {
              //Get results

              //If there are some results
              if (response.Image != null && response.Image.Results.Count() > 0)
              {
                entries = (from result in response.Image.Results
                           select new Picture
                                    {
                                      Type = EnumType.Bing,
                                      Title = result.Title,
                                      DisplayLink = result.DisplayUrl,
                                      PictureBig = result.MediaUrl,
                                      Thumbnail = result.Thumbnail.Url,
                                      //List size --> picture Big Height || picture Big Width || picture Thumbnail height || picture Thumbnail width
                                      Tag =
                                        string.Format("{0}||{1}||{2}||{3}",
                                                      result.Height,
                                                      result.Width,
                                                      result.Thumbnail.Height,
                                                      result.Thumbnail.Width),
                                      Content = ""

                                      //Type = EnumType.Bing,
                                      //Title = result.Title,
                                      //Medias = new List<Media>()
                                      //       {
                                      //         new Media()
                                      //           {                                      
                                      //             Title = result.Title,   
                                      //             Link = result.DisplayUrl,
                                      //             Contents = new List<Content>()
                                      //                          {
                                      //                            new Content()
                                      //                              {
                                      //                                Height = result.Height,
                                      //                                Width = result.Width,
                                      //                                Url = result.MediaUrl
                                      //                              }
                                      //                          },
                                      //             Thumbnails = new List<GenericLib.Thumbnail>()
                                      //                           {
                                      //                             new GenericLib.Thumbnail()
                                      //                               {
                                      //                                 Height = result.Thumbnail.Height,
                                      //                                 Width = result.Thumbnail.Width,
                                      //                                 Url = result.Thumbnail.Url
                                      //                               }
                                      //                           }
                                      //           }
                                      //       }
                                    }).ToList();
              }
            }
          }
          catch (Exception ex)
          {
            //exception = ex;
            TraceHelper.Trace("BingHelper -> soapClient_SearchImageCompleted() -> ",
                              ex);
          }
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("BingHelper -> soapClient_SearchWebCompleted() -> ",
                          ex);
      }
      return entries;
    }

    public static List<Picture> SearchVideo(string query,
                                          int count,
                                          int offset,
                                          string language,
                                          string market)
    {
      List<Picture> entries = null;
      try
      {
        using (var service = new BingPortTypeClient())
        {
          var request = new SearchRequest
                          {
                            AppId = apiKey,
                            Sources = new[] {SourceType.Video},
                            Query = query,
                            UILanguage = language
                          };

          if (!string.IsNullOrEmpty(market))
            request.Market = market; //"fr-FR"


          var videorequest = new VideoRequest
                               {
                                 Count = uint.Parse(count.ToString()),
                                 CountSpecified = true,
                                 Offset = uint.Parse(offset.ToString()),
                                 OffsetSpecified = true,
                                 SortBy = VideoSortOption.Relevance
                               };

          request.Video = videorequest;

          SearchResponse response = service.Search(request);

          var exception = response.Errors;

          try
          {
            if (exception == null)
            {
              //Get results

              //If there are some results
              if (response.Video != null && response.Video.Results.Count() > 0)
              {
                entries = (from result in response.Video.Results
                           select new Picture
                                    {
                                      Type = EnumType.Bing,
                                      Title = result.Title,
                                      Content = result.SourceTitle,
                                      DisplayLink = result.PlayUrl,
                                      PictureBig = result.PlayUrl,
                                      Thumbnail = result.StaticThumbnail.Url,
                                      //List size --> picture Thumbnail height || picture Thumbnail width
                                      Tag =
                                        string.Format("{0}||{1}",
                                                      result.StaticThumbnail.Height,
                                                      result.StaticThumbnail.Width),
                                      //Date = new DateTime(0, 0, 0, 0, 0, 0, int.Parse(result.RunTime.ToString())),
                                      //Medias = new List<Media>()
                                      //       {
                                      //         new Media()
                                      //           {
                                      //             Title = result.Title,
                                      //             Link = result.PlayUrl,
                                      //             DurationInSeconds = int.Parse(result.RunTime.ToString())/1000,
                                      //             Contents = new List<Content>()
                                      //                          {
                                      //                            new Content()
                                      //                              {
                                      //                                Url = result.PlayUrl
                                      //                              }
                                      //                          },
                                      //             Thumbnails = new List<GenericLib.Thumbnail>()
                                      //                           {
                                      //                             new GenericLib.Thumbnail()
                                      //                               {
                                      //                                 Height = result.StaticThumbnail.Height,
                                      //                                 Width = result.StaticThumbnail.Width,
                                      //                                 Url = result.StaticThumbnail.Url
                                      //                               }
                                      //                           }
                                      //           }
                                      //       }
                                    }).ToList();
              }
            }
          }
          catch (Exception ex)
          {
            //exception = ex;
            TraceHelper.Trace("BingHelper -> soapClient_SearchVideoCompleted() -> ",
                              ex);
          }
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("BingHelper -> soapClient_SearchWebCompleted() -> ",
                          ex);
      }
      return entries;
    }

    public static List<Entry> SearchNews(string query,
                                       int count,
                                       int offset,
                                       string language,
                                       bool orderByDate,
                                       string market)
    {
      List<Entry> entries = null;
      try
      {
        using (var service = new BingPortTypeClient())
        {
          var request = new SearchRequest
                          {
                            AppId = apiKey,
                            Sources = new[] {SourceType.News},
                            Query = query,
                            UILanguage = language
                          };

          if (!string.IsNullOrEmpty(market))
            request.Market = market; //"fr-FR"

          var newsrequest = new NewsRequest {Count = uint.Parse(count.ToString()), CountSpecified = true};

          if (orderByDate)
          {
            newsrequest.SortBy = NewsSortOption.Date;
            newsrequest.SortBySpecified = true;
          }

          newsrequest.Offset = uint.Parse(offset.ToString());
          newsrequest.OffsetSpecified = true;

          request.News = newsrequest;

          SearchResponse response = service.Search(request);

          var exception = response.Errors;

          try
          {
            if (exception == null)
            {
              //Get results
              //If there are some results
              if (response.News != null && response.News.Results.Count() > 0)
              {
                entries = (from result in response.News.Results
                           select new Entry
                                    {
                                      Type = EnumType.Bing,
                                      Title = result.Title,
                                      Content = result.Snippet,
                                      SourceName = result.Source,
                                      PubDate = DateTime.Parse(result.Date),
                                      DisplayLink = result.Url,
                                      Link = result.Url
                                    }).ToList();
              }
            }
          }
          catch (Exception ex)
          {
            //exception = ex;
            TraceHelper.Trace("BingHelper -> soapClient_SearchNewsCompleted() -> ",
                              ex);
          }
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("BingHelper -> soapClient_SearchWebCompleted() -> ",
                          ex);
      }
      return entries;
    }

    public static List<Entry> Translate(string text,
                                      string sourceLanguage,
                                      string targetLanguage)
    {
      List<Entry> entries = null;
      try
      {
        using (var service = new BingPortTypeClient())
        {
          var request = new SearchRequest {AppId = apiKey, Sources = new[] {SourceType.Translation}, Query = text};

          var translationrequest = new TranslationRequest
                                     {
                                       SourceLanguage = sourceLanguage,
                                       TargetLanguage = targetLanguage
                                     };

          request.Translation = translationrequest;

          SearchResponse response = service.Search(request);

          var exception = response.Errors;

          try
          {
            if (exception == null)
            {
              //Get results
              //If there are some results
              if (response.Translation != null && response.Translation.Results.Count() > 0)
              {
                entries = (from result in response.Translation.Results
                           select new Entry
                                    {
                                      Title = result.TranslatedTerm,
                                      Type = EnumType.Bing,
                                      Content = result.TranslatedTerm
                                    }).ToList();
              }
            }
          }
          catch (Exception ex)
          {
            TraceHelper.Trace("BingHelper -> soapClient_SearchTranslationCompleted() -> ",
                              ex);
          }
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("BingHelper -> soapClient_SearchWebCompleted() -> ",
                          ex);
      }
      return entries;
    }
  }
}