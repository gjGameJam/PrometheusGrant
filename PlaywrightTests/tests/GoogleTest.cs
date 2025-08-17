using System.Threading.Tasks;
using PlaywrightTests.Fixtures;
using Microsoft.Playwright;
using PlaywrightTests.Pages;
using FluentAssertions;
using Xunit;

namespace PlaywrightTests.Tests
{
    public class GoogleSearchTests : IClassFixture<PlaywrightFixture>
    {
        private readonly PlaywrightFixture _fixture;

        public GoogleSearchTests(PlaywrightFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task CanSearchGoogle()
        {
            var (context, page) = await _fixture.CreateNewContextAndPageAsync();

            await page.GotoAsync("https://www.google.com");
            //await Task.Delay(502); // simulate user pause
            await page.FillAsync("[name=q]", "Prometheus Group");
            //await Task.Delay(479); // simulate user pause
            await page.PressAsync("[name=q]", "Enter");
            //await Task.Delay(1479); // simulate user pause
            //await page.ScreenshotAsync(new() { Path = "google_search.png" });
            

            // wait for search results to load
            await page.WaitForSelectorAsync("#search");  // Google wraps results in a div#search

            // get the text content of the search results container
            var resultsText = await page.InnerTextAsync("#search");

            // assert that it contains "Prometheus Group"
            resultsText.Should().Contain("Prometheus Group");

            var contactUsLink = page.Locator("text=Contact Us");
            await contactUsLink.ClickAsync();
            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

            // ContactUsPage interactions
            var contactUsPage = new ContactUsPage(page);
            await contactUsPage.GotoAsync();
            await contactUsPage.DismissPopupsAsync(); //close the hecking popups if they appear
            await contactUsPage.EnterFirstNameAsync("John");//enter first name
            await contactUsPage.EnterLastNameAsync("Doe");//enter last name
            await contactUsPage.SubmitFormAsync();
            var requiredFields = await contactUsPage.CountRequiredFieldsAsync();
            requiredFields.Should().Be(4, "four required fields remain after filling first and last name");
            await Task.Delay(22000); // wait so I can see the result in non-headless mode
            // cleanup: dispose context (closes page too)
            await context.CloseAsync();
        }


    }
}
