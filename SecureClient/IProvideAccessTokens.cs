using System.Threading.Tasks;
using SecureClient.Model;

namespace SecureClient
{
    public interface IProvideAccessTokens
    {
        Task<AccessToken> RequestAccessTokenAsync();
    }
}
