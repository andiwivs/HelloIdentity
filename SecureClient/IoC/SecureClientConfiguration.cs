using SecureClient.Model;
using System;

namespace SecureClient.IoC
{
    public class SecureClientConfiguration
    {
        public string AuthorityBaseUri { get; }
        public GrantType GrantType { get; }
        public string ClientId { get; }
        public string ClientSecret { get; }
        public string Scope { get; }

        public SecureClientConfiguration(string authorityBaseUri, string clientId, string clientSecret, string scope)
        {
            if (string.IsNullOrWhiteSpace(authorityBaseUri))
                throw new ArgumentException("Authority Base Uri is required", nameof(authorityBaseUri));

            if (string.IsNullOrWhiteSpace(clientId))
                throw new ArgumentException("Client Id is required", nameof(clientId));

            if (string.IsNullOrWhiteSpace(clientSecret))
                throw new ArgumentException("Client Secret is required", nameof(clientSecret));

            if (string.IsNullOrWhiteSpace(scope))
                throw new ArgumentException("Scope is required", nameof(scope));

            AuthorityBaseUri = authorityBaseUri;
            ClientId = clientId;
            ClientSecret = clientSecret;
            Scope = scope;
            GrantType = GrantType.client_credentials; // we only need this grant type for now
        }
    }
}
