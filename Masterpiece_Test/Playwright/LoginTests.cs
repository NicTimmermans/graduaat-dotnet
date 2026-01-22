using System.Text.RegularExpressions;

namespace Masterpiece_Test.Playwright;

[TestFixture]
public class LoginTests : PlaywrightTestBase
{
    [Test]
    public async Task Login_Page_Loads()
    {
        await Page.GotoAsync($"{BaseUrl}/Account/Login");

        await Expect(Page).ToHaveURLAsync(new Regex(".*/Account/Login"));
        await Expect(Page.Locator("form")).ToBeVisibleAsync();
        await Expect(Page.Locator("input[name='Email']")).ToBeVisibleAsync();
        await Expect(Page.Locator("input[name='Password']")).ToBeVisibleAsync();
    }
}