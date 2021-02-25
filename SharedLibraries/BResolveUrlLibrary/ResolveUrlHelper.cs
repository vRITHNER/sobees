#region

using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

#endregion

namespace Sobees.Library.BResolveUrlLibrary
{
  public class ResolveUrlHelper
  {
    public static async Task<string> ResolveFullShortUrl(string shortUrl)
    {
      var tempUrl = shortUrl;
      var lastUrl = "";
      while (true)
      {
        lastUrl = await ResolveShortUrl(tempUrl);

        if (lastUrl.ToLower().Contains("unsupported service"))
        {
          lastUrl = tempUrl;
          break;
        }
        if (lastUrl == tempUrl)
          break;

        tempUrl = lastUrl;

        Debug.WriteLine(string.Format("lastUrl:{0}", lastUrl));
      }
      return lastUrl;
    }

    public async static Task<string> ResolveShortUrl(string shortUrl)
    {
      var longUrl = string.Empty;
      using (var client = new HttpClient())
      {
        // client.BaseAddress = new Uri(string.Format("http://untiny.me/api/1.0/", shortUrl));
        var url = string.Format("http://untiny.me/api/1.0/extract/?url={0}&format=text", shortUrl);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/text"));

        // New code:
        var response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode) return longUrl;
        longUrl = await response.Content.ReadAsStringAsync();
        //Debug.WriteLine("{0}", longUrl);
      }
      return longUrl;
    }
  }
}