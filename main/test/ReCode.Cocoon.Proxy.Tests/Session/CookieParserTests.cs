using ReCode.Cocoon.Proxy.Session;
using Xunit;

namespace ReCode.Cocoon.Proxy.Tests.Session
{
    public class CookieParserTests
    {

        [Fact]
        public void Extracts_The_Session_Cookie()
        {
            // Arrange
            // Act
            var result = CookieParser.Parse("ASP.NET_SessionId=exg0mgkt1jsk2qonnfehnydv; path=/; HttpOnly; SameSite=Lax");
            
            // Assert
            Assert.True(result.Name == "ASP.NET_SessionId");
            Assert.True(result.Value == "exg0mgkt1jsk2qonnfehnydv");
            Assert.True(result.Path == "/");
            Assert.True(result.HttpOnly == true);
        }
    }
}