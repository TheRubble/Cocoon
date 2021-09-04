using System;
using System.Text;

namespace ReCode.Cocoon.Proxy.Session
{
    internal record CookieDefinition
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public DateTime? Expires { get; set; }
        public string Path { get; set; } = "/";
        public string Domain { get; set; }
        public bool HttpOnly { get; set; }
        public bool Secure { get; set; }
        
        public string SameSite { get; set; }

        public string ToCookieString()
        {
            var builder = new StringBuilder();
            builder.Append($"{Name}={Value};");
            builder.Append($"path={Path};");
            
            if (HttpOnly)
            {
                builder.Append($"HttpOnly;");    
            }

            if (Secure)
            {
                builder.Append($"Secure;");
            }
            
            if (!string.IsNullOrEmpty(SameSite))
            {
                builder.Append($"SameSite={SameSite};");    
            }
            
            return builder.ToString();
        }
    };
}