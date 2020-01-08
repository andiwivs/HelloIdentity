using RestSharp;
using System.Threading.Tasks;

namespace SecureClient
{

    public interface IRestClientFactory
    {
        Task<RestClient> CreateWithAuthorizationHeader(string baseUrl, string accessTokenKey);
    }
}
