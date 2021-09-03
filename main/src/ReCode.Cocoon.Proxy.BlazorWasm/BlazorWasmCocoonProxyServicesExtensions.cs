using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ReCode.Cocoon.Proxy.Proxy;
using Yarp.ReverseProxy.Service.Proxy;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class BlazorWasmCocoonProxyServicesExtensions
    {
        public static IReverseProxyBuilder AddBlazorWasmCocoonProxy(this IServiceCollection services, IConfiguration configuration)
        {
            return AddBlazorWasmCocoonProxy(services, configuration, null);
        }

        public static IReverseProxyBuilder AddBlazorWasmCocoonProxy(this IServiceCollection services, IConfiguration configuration, CocoonProxyOptions? cocoonProxyOptions)
        {
            if (!Uri.TryCreate(configuration.GetValue<string>("Cocoon:Proxy:DestinationPrefix"), UriKind.Absolute, out var destinationPrefixUri))
            {
                throw new InvalidOperationException("Invalid DestinationPrefix");
            }
            
            services.AddSingleton<CocoonProxy>(provider => new CocoonProxy(
                configuration, 
                provider.GetService<ILogger<CocoonProxy>>(), 
                provider.GetService<IHttpProxy>(), 
                new BlazorRedirectTransformer(destinationPrefixUri),
                cocoonProxyOptions)
            );

            return ReverseProxyBuilder(services, configuration);
        }

        private static IReverseProxyBuilder ReverseProxyBuilder(IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddReverseProxy()
                .LoadFromConfig(configuration.GetSection("ReverseProxy"));
        }
    }
}