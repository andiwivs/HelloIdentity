using Autofac;
using Autofac.Core;
using Identity.Client.Services;
using Identity.Contract.Services;
using SecureClient;
using SecureClient.IoC;
using System;
using System.Collections.Generic;

namespace Identity.Client.IoC
{
    public class IdentityClientModule : Module
    {
        private const string DefaultAccessTokenKey = "IdentityClient";

        private readonly string _authorityRootUrl;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _scope;
        private readonly string _apiRootUrl;
        private readonly string _sharedAccessTokenKey;

        public IdentityClientModule(string authorityRootUrl, string clientId, string clientSecret, string scope, string apiRootUrl)
        : this(authorityRootUrl, clientId, clientSecret, scope, apiRootUrl, DefaultAccessTokenKey)
        { }

        public IdentityClientModule(string authorityRootUrl, string clientId, string clientSecret, string scope, string apiRootUrl, string sharedAccessTokenKey)
        {
            if (string.IsNullOrWhiteSpace(authorityRootUrl))
                throw new ArgumentException("Parameter is invalid.", nameof(authorityRootUrl));

            if (string.IsNullOrWhiteSpace(clientId))
                throw new ArgumentException("Parameter is invalid.", nameof(clientId));

            if (string.IsNullOrWhiteSpace(clientSecret))
                throw new ArgumentException("Parameter is invalid.", nameof(clientSecret));

            if (string.IsNullOrWhiteSpace(apiRootUrl))
                throw new ArgumentException("Parameter is invalid.", nameof(apiRootUrl));

            _authorityRootUrl = authorityRootUrl;
            _clientId = clientId;
            _clientSecret = clientSecret;
            _scope = scope;
            _apiRootUrl = apiRootUrl;
            _sharedAccessTokenKey = sharedAccessTokenKey ?? DefaultAccessTokenKey;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var secureClientConfig = new SecureClientConfiguration(_authorityRootUrl, _clientId, _clientSecret, _scope);

            builder.RegisterModule(new SecureClientModule(secureClientConfig));

            builder
                .RegisterType<UserService>()
                .As<IUserService>()
                .UsingConstructor(typeof(IRestClientFactory), typeof(string), typeof(string))
                .WithParameters(new List<Parameter>
                {
                    new ResolvedParameter(
                        (pi, ctx) => pi.ParameterType == typeof(IRestClientFactory),
                        (pi, ctx) => ctx.Resolve<IRestClientFactory>()),
                    new NamedParameter("baseUrl", _apiRootUrl),
                    new NamedParameter("accessTokenKey", _sharedAccessTokenKey)
                })
                .InstancePerLifetimeScope();
        }
    }
}
