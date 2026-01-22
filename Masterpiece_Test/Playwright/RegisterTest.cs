using System.Text.RegularExpressions;

namespace Masterpiece_Test.Playwright;

[TestFixture]
public class RegisterTests : PlaywrightTestBase
{
    [Test]
    public async Task Register_Page_Loads()
    {
        // Register page in your project = Account/Create
        await Page.GotoAsync($"{BaseUrl}/Account/Create");

        await Expect(Page).ToHaveURLAsync(new Regex(".*/Account/Create"));

        // Form + required fields from AccountCreateViewModel
        await Expect(Page.Locator("form")).ToBeVisibleAsync();

        await Expect(Page.Locator("input[name='Voornaam']")).ToBeVisibleAsync();
        await Expect(Page.Locator("input[name='Achternaam']")).ToBeVisibleAsync();
        await Expect(Page.Locator("input[name='Adres']")).ToBeVisibleAsync();
        await Expect(Page.Locator("input[name='Huisnummer']")).ToBeVisibleAsync();
        await Expect(Page.Locator("input[name='Postcode']")).ToBeVisibleAsync();
        await Expect(Page.Locator("input[name='Gemeente']")).ToBeVisibleAsync();

        await Expect(Page.Locator("input[name='Email']")).ToBeVisibleAsync();
        await Expect(Page.Locator("input[name='Password']")).ToBeVisibleAsync();

        // LandId is usually a select
        await Expect(Page.Locator("select[name='LandId']")).ToBeVisibleAsync();

        // RolId exists in VM, but for unauthenticated users it might be hidden or absent.
        // So: don't hard-require it in the UI test.
    }
}
