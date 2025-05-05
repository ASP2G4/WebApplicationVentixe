using Authentication;
using Authentication.Entities;
using Authentication.Models;
using Authentication.Services;
using Azure.Communication.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
//verification service
builder.Services.AddMemoryCache();

builder.Services.AddSingleton(x => new EmailClient(builder.Configuration["AzureCommunicationServices:ConnectionString"]));
builder.Services.Configure<AzureCommunicationsSettings>(builder.Configuration.GetSection("AzureCommunicationServices"));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IVerificationService, VerificationService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAccountProfileService, AccountProfileService>();

// Seperera Accountprofile till egen microservice senare
builder.Services.AddDbContext<AuthenticationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AccountDBConnection")));
builder.Services.AddDbContext<AccountProfileContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AccountDBConnection")));
builder.Services.AddIdentity<AccountUser, IdentityRole>(x =>
{
    x.SignIn.RequireConfirmedAccount = true;
    x.Password.RequiredLength = 8;
    x.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<AuthenticationContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(x =>
{
    x.LoginPath = "/login";
    x.AccessDeniedPath = "/accessdenied";
    x.Cookie.IsEssential = true;
    x.ExpireTimeSpan = TimeSpan.FromDays(30);
    x.SlidingExpiration = true;
    x.Cookie.SameSite = SameSiteMode.Lax;
});

var app = builder.Build();


app.UseHsts();
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
