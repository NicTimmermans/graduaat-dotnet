using System;
using System.Net.NetworkInformation;
using AutoMapper;
using DotNetEnv;
using Hangfire;
using Hangfire.PostgreSql;
using Restaurant.Hubs;
using Restaurant.Services.AdminDashBoardService;
using Restaurant.Services.LoggingService;
using Restaurant.Services.MailService;

var builder = WebApplication.CreateBuilder(args);

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IMailSender, MailSender>();
builder.Services.AddScoped<Restaurant.Services.LoggingService.ICustomLogger, CustomLogger>();
builder.Services.AddScoped<IAdminDash, AdminDash>();


builder.Services.AddSingleton<TafelLayoutState>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSignalR();

string connectionString;
if (!builder.Environment.IsEnvironment("Test"))
{
    try
    {
        var supabaseConn = builder.Configuration.GetConnectionString("DefaultConnection");
        using var conn = new Npgsql.NpgsqlConnection(supabaseConn);
        conn.Open();

        connectionString = supabaseConn;
    }
    catch
    {
        connectionString = builder.Configuration.GetConnectionString("DefaultConnection1");
    }
}
else
{
    // In tests, always use local/fallback DB (or in-memory later)
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection1");
}

builder.Services.AddDbContext<RestaurantContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddIdentity<CustomUser, IdentityRole>()
    .AddEntityFrameworkStores<RestaurantContext>();


builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Pad naar de inlogpagina
    options.AccessDeniedPath = "/Home/Index"; // Pad naar de homepagina
});

Env.Load();

//Opslaan van de tasks naar de DB.
if (!builder.Environment.IsEnvironment("Test"))
{
    builder.Services.AddHangfire(config =>
    {
        config.UsePostgreSqlStorage(connectionString);
    });
    builder.Services.AddHangfireServer();
}

builder.Services.AddOptions();
builder.Services.AddHttpClient<ResendClient>();
builder.Services.Configure<ResendClientOptions>(o =>
{
    o.ApiToken = Environment.GetEnvironmentVariable("RESEND_APITOKEN")!;
});
builder.Services.AddTransient<IResend, ResendClient>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (!app.Environment.IsEnvironment("Test"))
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles();
app.UseDefaultFiles();


app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<BestellingHub>("/bestellingHub"); // route voor SignalR

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();



