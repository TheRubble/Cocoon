using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ReCode.Cocoon.Proxy.Session;

namespace ReCode.Cocoon.Proxy.Proxy
{
    public class BlazorRedirectTransformer : RedirectTransformer
    {
        public BlazorRedirectTransformer(Uri destinationPrefix) : base(destinationPrefix)
        {
        }

        public override Task TransformRequestAsync(HttpContext httpContext, HttpRequestMessage proxyRequest,
            string destinationPrefix)
        {
            var cookieHeaderBuilder = new StringBuilder();
            CookieDefinition? sessionCookie = null;
            
            // Todo: For I need to generate a token and attach it as a cookie, all other cookies need to be passed
            // Todo: Without the other cookies you can't post to forms etc.
            if (httpContext.Request.Headers.TryGetValue("Cookie", out var cookieHeader))
            {
                var cookieDefinitions = CookieParser.Parse(cookieHeader);
                foreach (var cookieDefinition in cookieDefinitions)
                {
                    if (cookieDefinition.Name == "ASP.NET_SessionId")
                    {
                        sessionCookie = cookieDefinition;
                        continue;
                    }
                    cookieHeaderBuilder.Append(cookieDefinition.ToCookieString());
                }
                
                httpContext.Request.Headers.Remove("Cookie");
            }

            // Was a cookie sent back for aspnet
            if (sessionCookie != null)
            {
                sessionCookie.Value = "ellnlnxnnagshbb1ul0wmcor";
            }
            else
            {
                sessionCookie = new CookieDefinition
                {
                    Name = "ASP.NET_SessionId",
                    Value = "ellnlnxnnagshbb1ul0wmcor",
                    Domain = "localhost",
                    HttpOnly = true
                };
                
            }
            
            cookieHeaderBuilder.Append(sessionCookie.ToCookieString());
            httpContext.Request.Headers.Add("Cookie", cookieHeaderBuilder.ToString());
            return base.TransformRequestAsync(httpContext, proxyRequest, destinationPrefix);
        }
    }
}