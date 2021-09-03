using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Playwright;
using Xunit;

namespace ReCode.Cocoon.Integration.Tests
{
    public class CocoonFunctionalityWasm : CocoonFunctionalityBase
    {
        protected override string BaseUrl => "http://localhost:5000";
        
        public CocoonFunctionalityWasm() : base(true)
        {
        }
        
        public override async Task Pages_Available_In_Modern_App_Should_Serve_Before_Cocoon()
        {
            // Arrange
            using var playwright = await Playwright.CreateAsync();

            await using var browser = await playwright.Chromium.LaunchAsync(BrowserTypeLaunchOptions);

            // Act
            var page = await browser.NewPageAsync();
            await page.GotoAsync(BaseUrl);
            var result = await page.TextContentAsync("#app > div > div.navbar.navbar-inverse.navbar-fixed-top > div > div.navbar-header > a");
            
            // Assert
            result.Should().Be("Wingtip Toys - WASM");
        }

        [Fact]
        public override async Task Session_Set_In_Modern_App_Should_Be_Available_From_Cocoon()
        {
            // Arrange
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(BrowserTypeLaunchOptions);
            var sessionValue = Guid.NewGuid();

            // Act

            await using var context = await browser.NewContextAsync();
            var page = await context.NewPageAsync();
            
            await page.GotoAsync($"{BaseUrl}/?id={sessionValue.ToString()}");
            // Because of the nature of blazor and dom updates, you'll need to wait for a span to become visible
            // which indicates that the session item has been set.
            await page.WaitForSelectorAsync("#cocoonSessionSet", new PageWaitForSelectorOptions()
            {
                State = WaitForSelectorState.Attached
            });            
            
            await page.GotoAsync($"{BaseUrl}/session");
            var result = await page.TextContentAsync("//*[@id=\"MainContent_pullFromSession\"]");

            // Assert
            result.Should().Be(sessionValue.ToString());
        }
        
    }
}