using Identity.Contract.Model;
using Identity.Contract.Services;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using SecureClient;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Identity.Client.Services
{
    public class UserServiceV1 : ServiceBase, IUserService
    {
        public const string ClientKey = "IdentityClient";

        public UserServiceV1(IHttpClientFactory httpClientFactory, IProvideAccessTokens accessTokenProvider, IMemoryCache cache) 
            : base(httpClientFactory, accessTokenProvider, cache, ClientKey)
        { }

        public async Task<UserDto> GetByIdentity(Guid id)
        {
            await EnsureAccessTokenAsync().ConfigureAwait(false);

            var response = await HttpClient.GetAsync($"v1/users/{id}").ConfigureAwait(false);

            var content = (response.Content == null)
                ? default
                : await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var dto = JsonConvert.DeserializeObject<UserDto>(content);
                return dto;
            }

            return default;
        }
    }
}
