#region

using System;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.ChannelElements;

#endregion

namespace LinkedIn
{
  /// <summary>
  ///   Provides OAuth authorization to LinkedIn client web applications.
  /// </summary>
  [Serializable]
  public class WebOAuthAuthorization : OAuthAuthorization
  {
    /// <summary>
    ///   Private member to hold the access token.
    /// </summary>
    private string _accessToken;

    /// <summary>
    ///   Initializes a new instance of the <see cref="WebOAuthAuthorization" /> class.
    /// </summary>
    /// <param name="tokenManager">The token manager.</param>
    /// <param name="accessToken">The access token, or null if the user doesn't have one yet.</param>
    public WebOAuthAuthorization(IConsumerTokenManager tokenManager, string accessToken)
      : this(tokenManager, accessToken, LinkedInServiceDescription)
    {
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="WebOAuthAuthorization" /> class.
    /// </summary>
    /// <param name="tokenManager">The token manager.</param>
    /// <param name="accessToken">The access token, or null if the user doesn't have one yet.</param>
    /// <param name="serviceProviderDescription">The service provider description.</param>
    public WebOAuthAuthorization(IConsumerTokenManager tokenManager, string accessToken,
      ServiceProviderDescription serviceProviderDescription) :
        base(new WebConsumer(serviceProviderDescription, tokenManager))
    {
      _accessToken = accessToken;
    }

    /// <summary>
    ///   Gets the access token.
    /// </summary>
    /// <value>The access token.</value>
    public override string AccessToken => _accessToken;

    /// <summary>
    ///   Gets the consumer.
    /// </summary>
    /// <value>The consumer.</value>
    protected new WebConsumer Consumer => (WebConsumer) base.Consumer;

    /// <summary>
    ///   Requests LinkedIn to authorize this client.
    /// </summary>
    public void BeginAuthorize()
    {
      Consumer.Channel.Send(Consumer.PrepareRequestUserAuthorization());
    }

    /// <summary>
    ///   Requests LinkedIn to authorize this client.
    /// </summary>
    public void BeginAuthorize(Uri callback)
    {
      Consumer.Channel.Send(Consumer.PrepareRequestUserAuthorization(callback, null, null));
    }

    /// <summary>
    ///   Exchanges an authorized request token for an access token.
    /// </summary>
    /// <returns>The newly acquired access token, or <c>null</c> if no authorization complete message was in the HTTP request.</returns>
    public string CompleteAuthorize()
    {
      var response = Consumer.ProcessUserAuthorization();
      if (response == null) return null;
      _accessToken = response.AccessToken;
      return response.AccessToken;
    }
  }
}