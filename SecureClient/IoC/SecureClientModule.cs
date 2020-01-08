using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace SecureClient.IoC
{
    public class SecureClientModule : Module
    {
        private readonly SecureClientConfiguration _config;

        public SecureClientModule(SecureClientConfiguration config)
        {
            _config = config;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var services = new ServiceCollection();

            services.AddMemoryCache();

            builder.Populate(services);

            builder
                .RegisterType<IdentityServerAccessTokenProvider>()
                .As<IProvideAccessTokens>()
                .UsingConstructor(typeof(SecureClientConfiguration))
                .WithParameters(new List<Parameter>
                {
                    new NamedParameter("config", _config)
                })
                .InstancePerLifetimeScope();

            builder.RegisterType<RestClientFactory>().As<IRestClientFactory>();
        }
    }
}
