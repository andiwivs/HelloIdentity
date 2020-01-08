using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Net;
using System.Threading.Tasks;
using SecureClient.IoC;
using SecureClient.Model;

namespace SecureClient
{
    public class IdentityServerAccessTokenProvider : IProvideAccessTokens
    {
        private readonly SecureClientConfiguration _config;

        public IdentityServerAccessTokenProvider(SecureClientConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task<AccessToken> RequestAccessTokenAsync()
        {
            var client = new RestClient(_config.AuthorityBaseUri);
            var request = new RestRequest("connect/token", Method.POST);

            request.AddParameter("grant_type", _config.GrantType.ToString());
            request.AddParameter("client_id", _config.ClientId);
            request.AddParameter("client_secret", _config.ClientSecret);
            request.AddParameter("scope", _config.Scope);

            var response = await client.ExecuteTaskAsync(request).ConfigureAwait(false);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception($"Failed to get access token from identity service:\n{response.Content}\n{response.ErrorMessage}");

            var tokenData = JObject.Parse(response.Content);
            var token = tokenData["access_token"].ToString();
            var expiresIn = int.Parse(tokenData["expires_in"].ToString());

            return new AccessToken
            {
                Token = token,
                ExpiryDurationSecs = expiresIn
            };
        }
    }
}
