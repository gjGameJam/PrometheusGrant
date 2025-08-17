using System.Threading.Tasks;
using Microsoft.Playwright;

namespace PlaywrightTests.Pages
{
    public class GooglePage
    {
        private readonly IPage _page;

        // selectors (replace with actual ones after inspecting the page)
        private readonly string _searchInput = "input[name='q']";  // search box
        private readonly string _searchButton = "input[name='btnK']";  // search button
        private readonly string _resultsContainer = "#search";  // results div

        public GooglePage(IPage page)
        {
            _page = page;
        }

        // navigate to google home
        public async Task GotoAsync()
        {
            await _page.GotoAsync("https://www.google.com");
        }

        // enter a query in the search box
        public async Task EnterSearchQueryAsync(string query)
        {
            await _page.FillAsync(_searchInput, query);
        }

        // click search button
        public async Task ClickSearchAsync()
        {
            await _page.PressAsync(_searchInput, "Enter"); // pressing Enter is more reliable than button click
        }

        // check if a specific text appears in results
        public async Task<bool> IsResultDisplayedAsync(string text)
        {
            var locator = _page.Locator(_resultsContainer);
            return await locator.InnerTextAsync().ContinueWith(t => t.Result.Contains(text));
        }
    }
}
