using System.Diagnostics;

namespace Masterpiece_Test.Playwright;

public static class TestAppManager
{
    private static Process? _app;
    public static string BaseUrl { get; private set; } = default!;
    private static bool _started;

    public static async Task StartAsync()
    {
        if (_started)
            return;

        var projectPath = GetWebProjectPath();
        var port = GetFreeTcpPort();
        BaseUrl = $"http://127.0.0.1:{port}";

        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"run --project \"{projectPath}\" --no-launch-profile",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        startInfo.Environment["ASPNETCORE_ENVIRONMENT"] = "Test";
        startInfo.Environment["ASPNETCORE_URLS"] = BaseUrl;

        _app = Process.Start(startInfo)
            ?? throw new Exception("Failed to start web app");

        _app.OutputDataReceived += (_, e) =>
        {
            if (!string.IsNullOrWhiteSpace(e.Data))
                TestContext.Progress.WriteLine("[app] " + e.Data);
        };

        _app.ErrorDataReceived += (_, e) =>
        {
            if (!string.IsNullOrWhiteSpace(e.Data))
                TestContext.Progress.WriteLine("[app ERR] " + e.Data);
        };

        _app.BeginOutputReadLine();
        _app.BeginErrorReadLine();

        await WaitForServerAsync(BaseUrl);

        _started = true;
    }

    public static void Stop()
    {
        if (_app is { HasExited: false })
        {
            _app.Kill(true);
            _app.WaitForExit();
        }

        _app?.Dispose();
        _app = null;
        _started = false;
    }

    // ---------- helpers ----------

    private static int GetFreeTcpPort()
    {
        var listener = new System.Net.Sockets.TcpListener(
            System.Net.IPAddress.Loopback, 0);
        listener.Start();
        var port = ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    private static string GetWebProjectPath()
    {
        var relativePath = Environment.GetEnvironmentVariable("WEB_PROJECT_PATH")
            ?? throw new InvalidOperationException("WEB_PROJECT_PATH not set");

        var dir = new DirectoryInfo(AppContext.BaseDirectory);

        while (dir != null && !Directory.Exists(Path.Combine(dir.FullName, ".git")))
            dir = dir.Parent;

        if (dir == null)
            throw new InvalidOperationException("Could not locate repo root");

        var fullPath = Path.GetFullPath(Path.Combine(dir.FullName, relativePath));

        if (!File.Exists(fullPath))
            throw new FileNotFoundException("Web project not found", fullPath);

        return fullPath;
    }

    private static async Task WaitForServerAsync(string baseUrl, int timeoutMs = 30_000)
    {
        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(2) };
        var start = DateTime.UtcNow;

        while ((DateTime.UtcNow - start).TotalMilliseconds < timeoutMs)
        {
            try
            {
                await client.GetAsync(baseUrl);
                return;
            }
            catch
            {
                await Task.Delay(500);
            }
        }

        throw new TimeoutException($"Server did not start at {baseUrl}");
    }
}
