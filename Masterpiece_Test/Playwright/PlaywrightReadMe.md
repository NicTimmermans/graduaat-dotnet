## 🧪 Playwright Test Setup (IMPORTANT)

⚠️ REQUIRED ONCE PER MACHINE

Playwright requires a one-time browser installation.  
If this step is skipped, tests will fail with a missing browser error.

Please follow the instructions below carefully.

----------------------------------------------------------------

✅ FIRST-TIME PROJECT SETUP 

Run the following commands in the NuGet Package Manager Console  
or in a terminal:

    dotnet tool install --global Microsoft.Playwright.CLI
    dotnet add package Microsoft.Playwright
    dotnet build
    playwright install

----------------------------------------------------------------

🔁 IN A FRESH CLONE

⚠️ Make sure the active project is set to:  
    the test project (our case: Masterpiece_Test)

Then run:

    dotnet tool install --global Microsoft.Playwright.CLI
    dotnet build
    playwright install

❗ IMPORTANT:  
    playwright install must ALWAYS be run last

----------------------------------------------------------------

▶️ RUNNING THE TESTS (after setup)

Once Playwright is installed, run the tests using:

    dotnet test --settings Playwright.runsettings

Or run tests via Visual Studio Test Explorer 
(make sure “Build project before running tests” is enabled and solution wide runsettings within the test configure run settings has the playwright.runsettings selected ).

----------------------------------------------------------------

## ⚙️ ADDITIONAL PLAYWRIGHT SETUP (HOW IT WORKS)

The Playwright tests automatically start the ASP.NET Core web application
before running any tests. No manual app startup is required.

The application is started using `dotnet run` on a dynamically selected port
and runs in the `Test` environment.

----------------------------------------------------------------

## 📁 REQUIRED FILES FOR PLAYWRIGHT SETUP

The following files are required for the Playwright setup to work correctly.
Teachers can inspect or modify these files if needed.

----------------------------------------------------------------

1️⃣ Playwright.runsettings

Location: repository root

Purpose:
- Defines where the ASP.NET Core web project is located
- Supplies this path to the test runner via an environment variable

Contents:

<RunSettings>
  <RunConfiguration>
    <EnvironmentVariables>
      <WEB_PROJECT_PATH>Masterpiece/Restaurant.csproj</WEB_PROJECT_PATH>
    </EnvironmentVariables>
  </RunConfiguration>
</RunSettings>

If the web project is renamed or moved, this path must be updated.

----------------------------------------------------------------

2️⃣ TestAppManager.cs

Location:
    Masterpiece_Test/Playwright/TestAppManager.cs

Purpose:
- Starts the web application once per test run
- Chooses a free TCP port automatically
- Waits until the server is ready
- Exposes the BaseUrl used by all tests
- Shuts the application down after tests finish

This is the ONLY file responsible for starting and stopping the web app
for Playwright tests.

----------------------------------------------------------------

3️⃣ PlaywrightTestBase.cs

Location:
    Masterpiece_Test/Playwright/PlaywrightTestBase.cs

Purpose:
- Shared base class for all Playwright tests
- Ensures the application is started before tests run
- Provides the BaseUrl property to test classes

All Playwright test classes must inherit from this base class.

----------------------------------------------------------------

4️⃣ Individual Playwright Test Files

Location:
    Masterpiece_Test/Playwright/

Purpose:
- Contain only test logic
- No application startup or configuration code

Each test class should inherit from PlaywrightTestBase.

----------------------------------------------------------------

## ℹ️ IMPORTANT NOTES FOR GRADING / MODIFICATION

- The web application runs in the `Test` environment during Playwright tests
- Any services that should not run during tests (e.g. Hangfire, HTTPS redirect)
  should be disabled when ASPNETCORE_ENVIRONMENT is `Test`
- Startup behavior should only be modified in TestAppManager.cs
- Test logic should only be modified in individual test files

----------------------------------------------------------------

## ✅ SUMMARY

- Playwright browsers must be installed once per machine
- Tests start the ASP.NET Core app automatically
- Application startup is centralized in TestAppManager.cs
- Test files remain clean and focused on behavior
- This setup works across machines, Visual Studio, CLI, and CI