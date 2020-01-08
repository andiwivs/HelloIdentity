using Microsoft.Extensions.Caching.Memory;
using RestSharp;
using SecureClient.Model;
using System;
using System.Threading.Tasks;

namespace SecureClient
{
    public class RestClientFactory : IRestClientFactory
    {
        private const int ExpiryBufferSecs = 60;
        private readonly IProvideAccessTokens _accessTokenProvider;
        private readonly IMemoryCache _cache;

        public RestClientFactory(IMemoryCache cache, IProvideAccessTokens accessTokenProvider)
        {
            _accessTokenProvider = accessTokenProvider;
            _cache = cache;
        }

        public async Task<RestClient> CreateWithAuthorizationHeader(string baseUrl, string accessTokenKey)
        {
            var client = new RestClient { BaseUrl = new Uri(baseUrl) };
            
            if (!_cache.TryGetValue<AccessToken>(accessTokenKey, out var accessToken))
            {
                accessToken = await _accessTokenProvider.RequestAccessTokenAsync();

                var expirySecs = Math.Max(accessToken.ExpiryDurationSecs - ExpiryBufferSecs, 0);

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expirySecs)
                };

                _cache.Set(accessTokenKey, accessToken, cacheEntryOptions);
            }

            client.AddDefaultHeader("Authorization", $"Bearer {accessToken.Token}");

            return client;
        }
    }
}
