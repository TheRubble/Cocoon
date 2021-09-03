using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ReCode.Cocoon.Proxy.Session
{
    public interface ICocoonSessionClient
    {
        Task<byte[]?> GetAsync(string key, HttpRequest request);
        Task SetAsync(string key, byte[] bytes, Type type, HttpRequest request);
    }

    public class CocoonSessionClient : ICocoonSessionClient
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CocoonSessionClient> _logger;
        private readonly IOptionsMonitor<CocoonSessionOptions> _options;

        public CocoonSessionClient(HttpClient client, IHttpContextAccessor httpContextAccessor, ILogger<CocoonSessionClient> logger,
            IOptionsMonitor<CocoonSessionOptions> options)
        {
            _client = client;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _options = options;
        }

        public async Task<byte[]?> GetAsync(string key, HttpRequest request)
        {
            var message = CreateMessage(key, request, HttpMethod.Get, $"?key={key}");

            using var response = await _client.SendAsync(message);
            if (!response.IsSuccessStatusCode)
            {
                return default;
            }

            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task SetAsync(string key, byte[] bytes, Type type, HttpRequest request)
        {
            var uri = $"?key={key}&type={type.AssemblyQualifiedName}";
            var message = CreateMessage(key, request, HttpMethod.Put, uri);
            message.Content = new ByteArrayContent(bytes);
            var response = await _client.SendAsync(message);
            var setCookieHeaders = response.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value;

            /*
             * With aspnet core there won't be a session cookie available unless:
             * a) the core app has accessed session.
             * b) the cocoon app has set a cookie.
             * If this hasn't happened the session values won't line up, this is taking the cookie from the server
             * to server call and setting it on the client if the cookie doesn't already exist.
             */

            if (setCookieHeaders != null && _httpContextAccessor.HttpContext != null)
            {
                foreach (var setCookie in setCookieHeaders)
                {
                    var cookieDefinition = CookieParser.Parse(setCookie);
                    _httpContextAccessor.HttpContext.Response.Cookies.Append(
                        cookieDefinition.Name, 
                        cookieDefinition.Value,
                        new CookieOptions
                        {
                            Path = "/",
                            SameSite = SameSiteMode.Lax
                        });
                }
            }
            else
            {
                _logger.LogInformation($"A session cookie wasn't returned from the call to set the following : {key}");
            }
        }

        private HttpRequestMessage CreateMessage(string key, HttpRequest request, HttpMethod httpMethod, string? requestUri)
        {
            var message = new HttpRequestMessage(httpMethod, requestUri);

            foreach (var cookieName in _options.CurrentValue.Cookies)
            {
                if (request.Cookies.TryGetValue(cookieName, out var cookieValue))
                {
                    message.Headers.Add("Cookie", $"{cookieName}={cookieValue}");
                }
            }
            
            /* If a cookie value isn't already in play this will create one for the session */
            if(!request.Cookies.TryGetValue("ASP.NET_SessionId", out _))
            {
                message.Headers.Add("Cookie", $"ASP.NET_SessionId={Guid.NewGuid():N}");
            }
            
            return message;
        }
    }
}