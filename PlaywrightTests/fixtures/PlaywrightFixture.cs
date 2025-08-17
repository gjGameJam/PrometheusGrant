using Microsoft.Playwright;
using System.Threading.Tasks;
using Xunit;

namespace PlaywrightTests.Fixtures
{
    // fixture that provides browser, and gives each test an isolated context + page
    public class PlaywrightFixture : IAsyncLifetime
    {
        public IPlaywright Playwright { get; private set; }   // shared Playwright driver
        public IBrowser Browser { get; private set; }         // shared browser instance

        public async Task InitializeAsync()
        {
            // create playwright driver
            Playwright = await Microsoft.Playwright.Playwright.CreateAsync();

            // launch browser once for all tests (fast, efficient)
            Browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,   // set true for CI/CD or speed
                SlowMo = 50         // slows actions down for debugging visibility
            });
        }

        public async Task DisposeAsync()
        {
            if (Browser != null)
                await Browser.CloseAsync();        // close browser at the very end

            Playwright?.Dispose();                 // release driver
        }

        // helper: create a fresh browser context + page for each test
        public async Task<(IBrowserContext context, IPage page)> CreateNewContextAndPageAsync()
        {
            // context = isolated environment (cookies, storage, cache unique)
            var context = await Browser.NewContextAsync();

            // page = a new tab inside that context
            var page = await context.NewPageAsync();

            return (context, page);
        }
    }
}
