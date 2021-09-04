using ReCode.Cocoon.Proxy.Session;
using Xunit;

namespace ReCode.Cocoon.Proxy.Tests.Session
{
    public class CookieParserTests
    {

        [Theory]
        [InlineData("ASP.NET_SessionId=exg0mgkt1jsk2qonnfehnydv; path=/; HttpOnly; SameSite=Lax;", 1)]
        [InlineData("_ga=GA1.1.1351368910.1589721987;ASP.NET_SessionId=exg0mgkt1jsk2qonnfehnydv; path=/; HttpOnly; SameSite=Lax;", 2)]
        [InlineData("ASP.NET_SessionId=exg0mgkt1jsk2qonnfehnydv; path=/; HttpOnly; SameSite=Lax;_ga=GA1.1.1351368910.1589721987; csrftoken=rLcfYUyPRDYDK884fO0t1WHuqOuHjFn3CZVJ78EHY3ml1PaBvVT4iQ5o5EMI0bxw;",3)]
        [InlineData("ASP.NET_SessionId=exg0mgkt1jsk2qonnfehnydv; path=/; HttpOnly; SameSite=Lax;_ga=GA1.1.1351368910.1589721987; csrftoken=rLcfYUyPRDYDK884fO0t1WHuqOuHjFn3CZVJ78EHY3ml1PaBvVT4iQ5o5EMI0bxw;ASP.NET1_SessionId=exg0mgkt1jsk2qonnfehnydv; path=/; HttpOnly; SameSite=Lax;",4)]
        public void Extracts_The_Session_Cookie(string cookie, int count)
        {
            // Todo : Split these tests and check the result.
            
            // Arrange
            // Act
            // "ASP.NET_SessionId=exg0mgkt1jsk2qonnfehnydv; path=/; HttpOnly; SameSite=Lax"
            var result = CookieParser.Parse(cookie);

            // Assert
            Assert.True(result.Length == count);
        }
    }
}