using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using ReCode.Cocoon.Proxy.Session;
using Yarp.ReverseProxy.Service.Proxy;

namespace ReCode.Cocoon.Proxy.Proxy
{
    public class BlazorRedirectTransformer : RedirectTransformer
    {
        public BlazorRedirectTransformer(Uri destinationPrefix) : base(destinationPrefix)
        {
        }

        public override Task TransformRequestAsync(HttpContext httpContext, HttpRequestMessage proxyRequest, string destinationPrefix)
        {
            // Todo: For I need to generate a token and attach it as a cookie, all other cookies need to be passed
            // Todo: Without the other cookies you can't post to forms etc.
            if(httpContext.Request.Headers.TryGetValue("Cookie", out var cookieHeader))
            {
                var definition = CookieParser.Parse(cookieHeader);
                httpContext.Request.Headers.Remove("Cookie");
            }
            
            httpContext.Request.Headers.Add("Cookie",$"ASP.NET_SessionId=ellnlnxnnagshbb1ul0wmcor; Path=/; Domain=localhost; HttpOnly;");
            // proxyRequest.Headers.Add("Cookie",$"ASP.NET_SessionId=ellnlnxnnagshbb1ul0wmcor; Path=/; Domain=localhost; HttpOnly;");
            return base.TransformRequestAsync(httpContext, proxyRequest, destinationPrefix);
        }
    }
}