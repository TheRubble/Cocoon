using System;
using FluentAssertions;
using ReCode.Cocoon.Proxy.Session;
using Xunit;

namespace ReCode.Cocoon.Proxy.Tests.Session
{
    public class CookieDefinitionTests
    {
        [Theory]
        [InlineData("ASP.NET_SessionId", "exg0mgkt1jsk2qonnfehnydv", "/", true,false,"local","Lax", "ASP.NET_SessionId=exg0mgkt1jsk2qonnfehnydv;path=/;HttpOnly;SameSite=Lax;")]
        [InlineData("ASP.NET_SessionId", "exg0mgkt1jsk2qonnfehnydv", "/", false,false,"local", "Lax", "ASP.NET_SessionId=exg0mgkt1jsk2qonnfehnydv;path=/;SameSite=Lax;")]
        [InlineData("ASP.NET_SessionId", "exg0mgkt1jsk2qonnfehnydv", "/", false,false,"local", "", "ASP.NET_SessionId=exg0mgkt1jsk2qonnfehnydv;path=/;")]
        public void Returns_The_Correct_Cookie_String(
            string name, 
            string value, 
            string path, 
            bool httpOnly, 
            bool secure, 
            string domain,
            string sameSite,
            string expected)
        {
            // Arrange
            var sut = new CookieDefinition
            {
                Domain = domain,
                Name = name,
                Path = path,
                Secure = secure,
                HttpOnly = httpOnly,
                Value = value,
                SameSite = sameSite
            };
            
            // Act
            var result = sut.ToCookieString();

            // Assert
            result.Should().Be(expected);
        }
    }
}