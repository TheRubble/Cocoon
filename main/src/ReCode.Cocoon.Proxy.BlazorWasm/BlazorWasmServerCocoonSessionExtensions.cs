using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ReCode.Cocoon.Proxy.Session;

namespace ReCode.Cocoon.Proxy.BlazorWasm
{
    public static class BlazorWasmServerCocoonSessionExtensions
    {
        public static IServiceCollection AddBlazorWasmCocoonSession(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddOptions<CocoonSessionOptions>()
                .Configure<IConfiguration>((options, configuration) =>
                {
                    configuration.GetSection("Cocoon:Session").Bind(options);
                })
                .Validate(o => o.Cookies is { Length: > 0 }
                               && Uri.TryCreate(o.BackendApiUrl, UriKind.Absolute, out _),
                    "Invalid BackendApiUrl");
            services.AddHttpClient<ICocoonSessionClient, CocoonSessionBlazorServerClient>((provider, client) =>
            {
                var options = provider.GetRequiredService<IOptionsMonitor<CocoonSessionOptions>>();
                client.BaseAddress = new Uri(options.CurrentValue.BackendApiUrl);
            });
            services.AddScoped<CocoonSession>();
            return services;
        }
    }
}