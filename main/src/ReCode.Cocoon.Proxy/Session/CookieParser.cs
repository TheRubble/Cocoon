using System;

namespace ReCode.Cocoon.Proxy.Session
{
    internal static class CookieParser
    {
        public static CookieDefinition Parse(string input)
        {
            // var result = null;

            // if (string.IsNullOrEmpty(input)) {
            //     return false;
            // }

            // The substring before the first ';' is cookie-pair, with format of cookiename[=key1=val2&key2=val2&...]
            int dividerIndex = input.IndexOf(';');
            string cookiePair = dividerIndex >= 0 ? input.Substring(0, dividerIndex) : input;

            CookieDefinition cookie = new CookieDefinition();
            cookie.Name = cookiePair.Substring(0, cookiePair.IndexOf("=", StringComparison.Ordinal));
            cookie.Value = cookiePair.Substring(cookiePair.IndexOf("=", StringComparison.Ordinal) + 1);;
            //
            // Parse the collections of cookie-av 
            // cookie-av = expires-av/max-age-av/domain-av/path-av/secure-av/httponly-av/extension-av
            // https://tools.ietf.org/html/rfc6265 

            while (dividerIndex >= 0 && dividerIndex < input.Length - 1) {
                int cookieAvStartIndex = dividerIndex + 1;
                dividerIndex = input.IndexOf(';', cookieAvStartIndex);
                string cookieAv = dividerIndex >= 0 ? input.Substring(cookieAvStartIndex, dividerIndex - cookieAvStartIndex).Trim() : input.Substring(cookieAvStartIndex).Trim();

                int assignmentIndex = cookieAv.IndexOf('=');
                string attributeName = assignmentIndex >= 0 ? cookieAv.Substring(0, assignmentIndex).Trim() : cookieAv;
                string attributeValue = assignmentIndex >= 0 && assignmentIndex < cookieAv.Length - 1 ? cookieAv.Substring(assignmentIndex + 1).Trim() : null;

                //
                // Parse supported cookie-av Attribute

                //
                // Expires
                if (string.Equals(attributeName, "Expires", StringComparison.InvariantCultureIgnoreCase)) {
                    DateTime dt;
                    if (DateTime.TryParse(attributeValue, out dt)) {
                        cookie.Expires = dt;
                    }
                }
                //
                // Domain
                else if (attributeValue != null && string.Equals(attributeName,"Domain", StringComparison.InvariantCultureIgnoreCase)) {
                    cookie.Domain = attributeValue;
                }
                //
                // Path
                else if (attributeValue != null && string.Equals(attributeName, "Path", StringComparison.InvariantCultureIgnoreCase)) {
                    cookie.Path = attributeValue;
                }
                //
                // Secure
                else if (string.Equals(attributeName, "Secure", StringComparison.InvariantCultureIgnoreCase)) {
                    cookie.Secure = true;
                }
                //
                // HttpOnly
                else if (string.Equals(attributeName, "HttpOnly", StringComparison.InvariantCultureIgnoreCase)) {
                    cookie.HttpOnly = true;
                }
                //
                // SameSite
                // else if(StringUtil.EqualsIgnoreCase(attributeName, "SameSite")) {
                //     SameSiteMode sameSite = (SameSiteMode)(-1);
                //     if(Enum.TryParse<SameSiteMode>(attributeValue, true, out sameSite)) {
                //         cookie.SameSite = sameSite;
                //     }
                // }
            }
            return cookie;
        }
    }

    internal record CookieDefinition
    {
        public string Name { get; set; }   
        public string Value { get; set; }
        public DateTime? Expires { get; set; }
        public string Path { get; set; }
        public string Domain { get; set; }
        public bool HttpOnly { get; set; }
        public bool Secure { get; set; }
    };
}