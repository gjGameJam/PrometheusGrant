using System.Threading.Tasks;
using Microsoft.Playwright;

namespace PlaywrightTests.Pages
{
    public class ContactUsPage
    {
        private readonly IPage _page;

        // selectors (replace with actual ones from the Prometheus site)
        private readonly string _firstNameInput = "input[name=firstname]";
        private readonly string _lastNameInput = "input[name=lastname]"; // similarly for last name
        private readonly string _submitButton = "input[type=submit]";
        private readonly string _errorMessage = ".error";  // generic error message placeholder
        private readonly string _declineCookiesButton = "#hs-eu-decline-button"; // selector for cookie decline button
        private readonly string _locatorOptions = "input[required], textarea[required], select[required]";

        public ContactUsPage(IPage page)
        {
            _page = page;
        }

        // // navigate directly to the contact us page
        // public async Task GotoAsync()
        // {
        //     await _page.GotoAsync("https://www.prometheusgroup.com/contact-us");
        // }

        // fill out first name
        public async Task EnterFirstNameAsync(string firstName)
        {
            await _page.FillAsync(_firstNameInput, firstName, new PageFillOptions { Force = true });//click through any overlays (the video popup is annoying)
        }

        // fill out last name
        public async Task EnterLastNameAsync(string lastName)
        {
            await _page.FillAsync(_lastNameInput, lastName, new PageFillOptions { Force = true });//click through any overlays (the video popup is annoying)
        }

        // submit the form
        public async Task SubmitFormAsync()
        {
            await _page.ClickAsync(_submitButton, new PageClickOptions { Force = true }); //click through any overlays (the video popup is annoying)
        }

        // check if error message is displayed
        public async Task<bool> IsErrorDisplayedAsync()
        {
            return await _page.Locator(_errorMessage).IsVisibleAsync();
        }

        // count required fields on the form
        public async Task<int> CountRequiredFieldsAsync()
        {
            // locate all required inputs, textareas, and selects
            var allRequired = _page.Locator(_locatorOptions);

            int count = 0;
            int total = await allRequired.CountAsync();

            for (int i = 0; i < total; i++)
            {
                var element = allRequired.Nth(i);

                // check if element is visible and empty
                if (await element.IsVisibleAsync() && string.IsNullOrEmpty(await element.InputValueAsync()))
                {
                    count++;
                }
            }

            return count;
        }

        //function to dismiss popups if they appear to allow for fields to be interacted with
        public async Task DismissPopupsAsync()
        {
            // video popup — update selector when known
            //upon further review, the video popup seems to be instantiated via javascript
            var videoClose = _page.Locator("button, a", new PageLocatorOptions { HasTextString = "×" });
            if (await videoClose.IsVisibleAsync(new LocatorIsVisibleOptions { Timeout = 5000 }))
                await videoClose.ClickAsync();


            // cookie decline
            var declineCookies = _page.Locator(_declineCookiesButton);
            if (await declineCookies.IsVisibleAsync(new LocatorIsVisibleOptions { Timeout = 5000 }))
                await declineCookies.ClickAsync();
        }

    }
}
