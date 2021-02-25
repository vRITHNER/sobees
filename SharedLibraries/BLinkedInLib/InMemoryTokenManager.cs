//-----------------------------------------------------------------------
// <copyright file="InMemoryTokenManager.cs" company="Andrew Arnott">
//     Copyright (c) Andrew Arnott. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using DotNetOpenAuth.OAuth.ChannelElements;
using DotNetOpenAuth.OAuth.Messages;

namespace Sobees.Library.BLinkedInLib
{
  public class InMemoryTokenManager : IConsumerTokenManager
  {
    private readonly Dictionary<string, string> _tokensAndSecrets = new Dictionary<string, string>();

    public InMemoryTokenManager(string consumerKey, string consumerSecret)
    {
      if (String.IsNullOrEmpty(consumerKey))
      {
        throw new ArgumentNullException(nameof(consumerKey));
      }

      this.ConsumerKey = consumerKey;
      this.ConsumerSecret = consumerSecret;
    }

    public string ConsumerKey { get; private set; }
    public string ConsumerSecret { get; private set; }

    #region ITokenManager Members

    public string GetTokenSecret(string token)
    {
      return this._tokensAndSecrets[token];
    }

    public void StoreNewRequestToken(string token, string tokenSecret)
    {
      this._tokensAndSecrets[token] = tokenSecret; 
    }

    public void StoreNewRequestToken(UnauthorizedTokenRequest request, ITokenSecretContainingMessage response)
    {
      this._tokensAndSecrets[response.Token] = response.TokenSecret;
    }

    public void ExpireRequestTokenAndStoreNewAccessToken(string consumerKey, string requestToken, string accessToken, string accessTokenSecret)
    {
      this._tokensAndSecrets.Remove(requestToken);
      this._tokensAndSecrets[accessToken] = accessTokenSecret;
    }

    /// <summary>
    /// Classifies a token as a request token or an access token.
    /// </summary>
    /// <param name="token">The token to classify.</param>
    /// <returns>Request or Access token, or invalid if the token is not recognized.</returns>
    public TokenType GetTokenType(string token)
    {
      throw new NotImplementedException();
    }
    #endregion
  }
}
