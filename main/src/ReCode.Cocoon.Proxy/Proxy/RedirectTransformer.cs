﻿using System;
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
    public class RedirectTransformer : HttpTransformer
    {
        private readonly Uri _destinationPrefix;

        public RedirectTransformer(Uri destinationPrefix)
        {
            _destinationPrefix = destinationPrefix;
        }

        public override async Task TransformResponseAsync(HttpContext context, HttpResponseMessage response)
        {
            var location = response.Headers.Location;
            
            if (location?.IsAbsoluteUri == true && _destinationPrefix.IsBaseOf(location))
            {
                var relative = location.PathAndQuery;
                response.Headers.Location = new Uri(relative, UriKind.Relative);
            }
            
            await base.TransformResponseAsync(context, response);
        }
    }
}