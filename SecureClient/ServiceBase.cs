using Microsoft.Extensions.Caching.Memory;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using SecureClient.Model;

namespace SecureClient
{
    public class ServiceBase
    {
        private readonly IProvideAccessTokens _accessTokenProvider;
        private readonly IMemoryCache _cache;
        private readonly string _clientKey;

        protected HttpClient HttpClient { get; }

        public ServiceBase(IHttpClientFactory httpClientFactory, IProvideAccessTokens accessTokenProvider, IMemoryCache cache, string clientKey)
        {
            if (httpClientFactory == null)
                throw new ArgumentNullException(nameof(httpClientFactory));

            _accessTokenProvider = accessTokenProvider ?? throw new ArgumentNullException(nameof(accessTokenProvider));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _clientKey = clientKey;

            if (string.IsNullOrWhiteSpace(clientKey))
                throw new ArgumentException(nameof(clientKey));

            HttpClient = httpClientFactory.CreateClient(clientKey);
        }

        protected async Task EnsureAccessTokenAsync(bool forceRefresh = false)
        {
            var cacheKey = $"identity:accesstoken:{_clientKey}";

            if (forceRefresh || !_cache.TryGetValue<AccessToken>(cacheKey, out var accessToken))
            {
                accessToken = await _accessTokenProvider.RequestAccessTokenAsync().ConfigureAwait(false);
                
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(accessToken.ExpiryDurationSecs - 300)
                };

                _cache.Set(cacheKey, accessToken, cacheEntryOptions);
            }

            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Token);
        }
    }
}
