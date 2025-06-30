using AutoMapper;
using Movies.Options;
using Movies.Mappings;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
using Movies.EntityModels;
using System.Globalization;
using Movies.SignalR.Service.Hubs;
using Microsoft.AspNetCore.DataProtection;
using Serilog;
using Serilog.Events;
using Movies.Services;
using Movies.Repositories;
using AspNetCoreRateLimit;
using Microsoft.Extensions.DependencyInjection;

var defaultCulture = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<OmdbApiOptions>(
    builder.Configuration.GetSection(OmdbApiOptions.SectionName));

builder.Services.Configure<DatabaseOptions>(
    builder.Configuration.GetSection(DatabaseOptions.SectionName));


builder.Services.AddDbContext<MoviesDataContext>(options =>
                            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<MoviesDataContext>();

builder.Services.AddHttpClient();
builder.Services.AddHttpClient("OMDB", client =>
{
    client.BaseAddress = new Uri("https://www.omdbapi.com/");
});

builder.Services.AddAutoMapper(typeof(MovieMappingProfile));



builder.Services.AddServices();
builder.Services.AddRepositories();

builder.Services.AddControllersWithViews();

builder.Host.UseSerilog((context, services, configuration) => configuration
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("../Movies.Logs/Serilog.txt", rollingInterval: RollingInterval.Day));

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});
builder.Services.AddDistributedMemoryCache(); //Needed for sessions
builder.Services.AddSession();

builder.Services.AddMemoryCache();

builder.Services.Configure<IpRateLimitOptions>(options =>
{

    builder.Configuration.GetSection("IpRateLimiting").Bind(options);

    options.QuotaExceededResponse = new QuotaExceededResponse
    {
        Content = "Too many requests. Try again later.",
        ContentType = "text/plain"
    };
});
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();


builder.Services.Configure<IdentityOptions>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.User.RequireUniqueEmail = false;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 429 && !context.Response.HasStarted)
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("{\"error\": \"Too many requests. Try again later.\"}");
    }
});

app.UseIpRateLimiting();
//app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapStaticAssets();


app.Use(async (context, next) =>
{

    if (context.Request.Path.StartsWithSegments("/assets") ||
    context.Request.Path.StartsWithSegments("/covers"))
    {
        context.Response.Headers["Cache-Control"] = "public, max-age=604800, immutable";
    }
    await next();
});

app.MapHub<ChatHub>("/chat");
app.MapHub<VoteDateHub>("/voteDateHub");
app.MapHub<VoteMovieHub>("/voteMovieHub");
app.MapHub<ProposeDateHub>("/proposeDateHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Movies}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();
app.Run();
