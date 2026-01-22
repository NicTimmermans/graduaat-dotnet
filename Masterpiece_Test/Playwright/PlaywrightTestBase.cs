using Microsoft.Playwright.NUnit;

namespace Masterpiece_Test.Playwright;

public abstract class PlaywrightTestBase : PageTest
{
    protected string BaseUrl => TestAppManager.BaseUrl;

    [OneTimeSetUp]
    public async Task GlobalSetup()
    {
        await TestAppManager.StartAsync();
    }

    [OneTimeTearDown]
    public void GlobalTeardown()
    {
        TestAppManager.Stop();
    }
}
