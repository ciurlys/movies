using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
using Movies.Mvc.Data;
using Movies.EntityModels;
using System.Globalization;
using Movies.SignalR.Service.Hubs;
using Microsoft.AspNetCore.DataProtection;
using Serilog;
using Serilog.Events;
using Movies.Services;
using Movies.Extensions;
using Movies.Repositories;



var defaultCulture = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplicationDbContext();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();  //Database for storing Users
builder.Services.AddMoviesDataContext(); //Database for storing Movies

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddHttpClient();
builder.Services.AddScoped<MovieService>();
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();

builder.Services.AddControllersWithViews();

builder.Host.UseSerilog((context, services, configuration) => configuration
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("../Movies.Logs/Serilog.txt", rollingInterval: RollingInterval.Day));

builder.Services.AddRateLimiting();

builder.Services.AddSignalR(options => 
{
    options.EnableDetailedErrors = true;
});
builder.Services.AddDistributedMemoryCache(); //Needed for sessions
builder.Services.AddSession();

builder.Services.AddAuthorization();

builder.Services.Configure<IdentityOptions>(options =>{
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


app.UseRateLimiter();
//app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapStaticAssets();


app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatHub>("/chat");
    endpoints.MapHub<VoteDateHub>("/voteDateHub");
    endpoints.MapHub<VoteMovieHub>("/voteMovieHub");
    endpoints.MapHub<ProposeDateHub>("/proposeDateHub");
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Movies}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
