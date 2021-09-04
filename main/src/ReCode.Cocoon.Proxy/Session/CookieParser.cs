using System;
using System.Collections.Generic;

namespace ReCode.Cocoon.Proxy.Session
{
    internal static class CookieParser
    {
        public static CookieDefinition[] Parse(string input)
        {
            int inputLength = input.Length;
            var cookieDefinitions = new List<CookieDefinition>();
            
            // Parse the first
            var cookie = ParseLine(input);
            cookieDefinitions.Add(cookie.cookieDefinition);

            if (cookie.index > 0)
            {
                while (cookie.index < inputLength)
                {
                    var newInput = input.Substring(cookie.index);
                    cookie = ParseLine(newInput);
                    cookieDefinitions.Add(cookie.cookieDefinition);

                    // it found the last item, exit
                    if (cookie.index == 0)
                    {
                        break;
                    }

                    input = newInput;
                }
            }
            
            return cookieDefinitions.ToArray();
        }
        
        private static (CookieDefinition cookieDefinition, int index) ParseLine(string input)
        {
            // The substring before the first ';' is cookie-pair, with format of cookiename[=key1=val2&key2=val2&...]
            int dividerIndex = input.IndexOf(';');
            string cookiePair = dividerIndex >= 0 ? input.Substring(0, dividerIndex) : input;

            // Grab the name and value
            CookieDefinition cookie = new()
            {
                Name = cookiePair.Substring(0, cookiePair.IndexOf("=", StringComparison.Ordinal)),
                Value = cookiePair.Substring(cookiePair.IndexOf("=", StringComparison.Ordinal) + 1)
            };

            // Parse the collections of cookie-av 
            // cookie-av = expires-av/max-age-av/domain-av/path-av/secure-av/httponly-av/extension-av
            // https://tools.ietf.org/html/rfc6265 

            while (dividerIndex >= 0 && dividerIndex < input.Length - 1)
            {
                int cookieAvStartIndex = dividerIndex + 1;
                dividerIndex = input.IndexOf(';', cookieAvStartIndex);
                string cookieAv = dividerIndex >= 0
                    ? input.Substring(cookieAvStartIndex, dividerIndex - cookieAvStartIndex).Trim()
                    : input.Substring(cookieAvStartIndex).Trim();

                int assignmentIndex = cookieAv.IndexOf('=');
                string attributeName = assignmentIndex >= 0 ? cookieAv.Substring(0, assignmentIndex).Trim() : cookieAv;
                string attributeValue = assignmentIndex >= 0 && assignmentIndex < cookieAv.Length - 1
                    ? cookieAv.Substring(assignmentIndex + 1).Trim()
                    : null;

                //
                // Parse supported cookie-av Attribute

                //
                // Expires
                if (string.Equals(attributeName, "Expires", StringComparison.InvariantCultureIgnoreCase))
                {
                    DateTime dt;
                    if (DateTime.TryParse(attributeValue, out dt))
                    {
                        cookie.Expires = dt;
                    }
                }
                //
                // Domain
                else if (attributeValue != null &&
                         string.Equals(attributeName, "Domain", StringComparison.InvariantCultureIgnoreCase))
                {
                    cookie.Domain = attributeValue;
                }
                //
                // Path
                else if (attributeValue != null &&
                         string.Equals(attributeName, "Path", StringComparison.InvariantCultureIgnoreCase))
                {
                    cookie.Path = attributeValue;
                }
                //
                // Secure
                else if (string.Equals(attributeName, "Secure", StringComparison.InvariantCultureIgnoreCase))
                {
                    cookie.Secure = true;
                }
                //
                // HttpOnly
                else if (string.Equals(attributeName, "HttpOnly", StringComparison.InvariantCultureIgnoreCase))
                {
                    cookie.HttpOnly = true;
                }
                // SameSite
                else if (string.Equals(attributeName, "SameSite", StringComparison.InvariantCultureIgnoreCase))
                {
                    // SameSiteMode sameSite = (SameSiteMode)(-1);
                    // if(Enum.TryParse<SameSiteMode>(attributeValue, true, out sameSite)) {
                    //     cookie.SameSite = sameSite;
                    // }
                }
                else
                {
                    // Just bumped into a new cookie.
                    return (cookie, cookieAvStartIndex);
                }
            }

            return (cookie,0);
        }
    }
}