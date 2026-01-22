using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace Masterpiece_Test.Playwright
{
    [TestFixture]
    public class AdminLoginTests : PlaywrightTestBase
    {
        [Test]
        public async Task Admin_Login_Redirects_To_Admin_Index()
        {
            
            await Page.GotoAsync($"{BaseUrl}/Account/Login");

            // Fill in admin credentials
            await Page.FillAsync("input[name='Email']", "Admin@mail.com");
            await Page.FillAsync("input[name='Password']", "Admin123!");

            // Submit login form
            await Page.ClickAsync("#login-submit");

            // Wait for navigation to admin dashboard
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            //Assert URL
            await Expect(Page).ToHaveURLAsync(new Regex(".*/Admin(/Index)?$"));

            // 2Assert key admin dashboard buttons exist
            await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Accounts" }))
                .ToBeVisibleAsync();

            await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Reservaties" }))
                .ToBeVisibleAsync();

            await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Parameters" }))
                .ToBeVisibleAsync();

            await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Admin account" }))
                .ToBeVisibleAsync();
        }
    }
}
