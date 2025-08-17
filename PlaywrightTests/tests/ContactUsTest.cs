// using Microsoft.Playwright;
// using PlaywrightTests.Fixtures;
// using PlaywrightTests.Pages;
// using Xunit;
// using FluentAssertions;
// using System.Threading.Tasks;

// namespace PlaywrightTests.Tests
// {
//     public class ContactUsTest : IClassFixture<PlaywrightFixture>
//     {
//         private readonly PlaywrightFixture _fixture;

//         public ContactUsTest(PlaywrightFixture fixture)
//         {
//             _fixture = fixture;
//         }

//         [Fact]
//         public async Task SubmitContactForm_ShouldHave4RequiredFields()
//         {
//             // create new browser context + page
//             var (context, page) = await _fixture.CreateNewContextAndPageAsync();

//             // create page object
//             var contactUsPage = new ContactUsPage(page);

//             // navigate directly (or assume Google clicked you here)
//             await contactUsPage.GotoAsync();
//             await contactUsPage.EnterFirstNameAsync("John");
//             await contactUsPage.EnterLastNameAsync("Doe");
//             await contactUsPage.SubmitFormAsync();
//             var requiredFields = await contactUsPage.CountRequiredFieldsAsync();

//             requiredFields.Should().Be(4);

//             // cleanup
//             await context.CloseAsync();
//         }
//     }
// }
