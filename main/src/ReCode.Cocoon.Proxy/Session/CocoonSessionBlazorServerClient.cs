using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ReCode.Cocoon.Proxy.Session
{
    public class CocoonSessionBlazorServerClient : ICocoonSessionClient
    {
        private readonly HttpClient _client;
        private readonly IOptionsMonitor<CocoonSessionOptions> _options;

        public CocoonSessionBlazorServerClient(HttpClient client,ILogger<CocoonSessionBlazorServerClient> logger, IOptionsMonitor<CocoonSessionOptions> options)
        {
            _client = client;
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
            // var setCookieHeaders = response.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value;
        }

        private HttpRequestMessage CreateMessage(string key, HttpRequest request, HttpMethod httpMethod, string? requestUri)
        {
            var message = new HttpRequestMessage(httpMethod, requestUri);
            message.Headers.Add("Cookie", $"ASP.NET_SessionId=ellnlnxnnagshbb1ul0wmcor; Path=/; Domain=localhost; HttpOnly;");
            // Look in the browser state store and append it.
            // If not add a new one.
            return message;
        }
    }
}