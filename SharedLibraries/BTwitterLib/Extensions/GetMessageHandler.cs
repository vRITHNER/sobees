using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LinqToTwitter;

namespace Sobees.Library.BTwitterLib.Extensions
{
  class GetMessageHandler : HttpClientHandler
  {
    TwitterExecute exe;
    IDictionary<string, string> parameters;
    string url;

    public GetMessageHandler(TwitterExecute exe, IDictionary<string, string> parameters, string url)
    {
      this.exe = exe;
      this.parameters = parameters;
      this.url = url;
    }

    protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
      exe.SetAuthorizationHeader(HttpMethod.Get, url, parameters, request);
      request.Headers.Add("User-Agent", exe.UserAgent);
      request.Headers.ExpectContinue = false;
      if (SupportsAutomaticDecompression)
        AutomaticDecompression = DecompressionMethods.GZip;
      if (exe.Authorizer.Proxy != null && SupportsProxy)
        Proxy = exe.Authorizer.Proxy;

      //if (exe.ReadWriteTimeout != 0)
      //    ReadWriteTimeout = exe.ReadWriteTimeout;

      return await base.SendAsync(request, cancellationToken);
    }
  }
}
