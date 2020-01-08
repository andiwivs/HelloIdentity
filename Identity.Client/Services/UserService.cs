using Identity.Contract.Model;
using Identity.Contract.Services;
using RestSharp;
using SecureClient;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Identity.Client.Services
{
    public class UserService : IUserService
    {
        private readonly IRestClientFactory _clientFactory;
        private readonly string _baseUrl;
        private readonly string _accessTokenKey;

        public UserService(IRestClientFactory clientFactory, string baseUrl, string accessTokenKey)
        {
            _clientFactory = clientFactory;
            _baseUrl = baseUrl;
            _accessTokenKey = accessTokenKey;
        }

        public async Task<UserDto> GetByIdentity(Guid id)
        {
            var client = await _clientFactory.CreateWithAuthorizationHeader(_baseUrl, _accessTokenKey);

            var request = new RestRequest($"api/v1/users/{id}");

            var response = await client.ExecuteGetTaskAsync<UserDto>(request);

            switch (response.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    return default;

                case HttpStatusCode.OK:
                    return response.Data;

                default:
                    throw new Exception("Api returned unexpected response");
            }
        }
    }
}
