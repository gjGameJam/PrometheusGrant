using System.Threading.Tasks;
using Microsoft.Playwright;

namespace PlaywrightTests.Pages
{
    public class GooglePage
    {
        private readonly IPage _page;

        private readonly string _searchInput = "input[name='q']";  // search box
        private readonly string _searchButton = "input[name='btnK']";  // search button
        private readonly string _resultsContainer = "#search";  // results div
        private readonly string _googleDomain = "https://www.google.com";
        private readonly string _enterButton = "Enter";

        public GooglePage(IPage page)
        {
            _page = page;
        }

        // navigate to google home
        public async Task GotoAsync()
        {
            await _page.GotoAsync(_googleDomain);
        }

        // enter a query in the search box
        public async Task EnterSearchQueryAsync(string query)
        {
            await _page.FillAsync(_searchInput, query);
        }

        // click search button
        public async Task ClickSearchAsync()
        {
            await _page.PressAsync(_searchInput, _enterButton); // pressing Enter is more reliable than button click
        }

        // check if a specific text appears in results
        public async Task<bool> IsResultDisplayedAsync(string text)
        {
            var locator = _page.Locator(_resultsContainer);
            return await locator.InnerTextAsync().ContinueWith(t => t.Result.Contains(text));
        }
    }
}
