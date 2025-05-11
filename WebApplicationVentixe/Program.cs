using Authentication.Contexts;
using Authentication.Entities;
using Authentication.Services;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplicationVentixe.Protos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddMemoryCache();

builder.Services.AddSingleton(new ServiceBusClient(builder.Configuration["AzureServiceBusSettings:ConnectionString"]));
builder.Services.AddScoped<IVerificationService, VerificationService>();
builder.Services.AddScoped<IAccountService, AccountService>();


builder.Services.AddGrpcClient<ProfileHandler.ProfileHandlerClient>(options =>
{
    var grpcUrl = builder.Configuration["GrpcSettings:ProfileHandlerUrl"];
    options.Address = new Uri(grpcUrl);
});

builder.Services.AddDbContext<AuthenticationContext>(options =>
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
    x.Cookie.SameSite = SameSiteMode.None;
    x.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGitHub(options =>
{
    options.ClientId = builder.Configuration["Authentication:GitHub:ClientId"]!;
    options.ClientSecret = builder.Configuration["Authentication:GitHub:ClientSecret"]!;
    options.CallbackPath = "/signin-github";
    options.Scope.Add("user:email");
});

var app = builder.Build();
await SeedRoleData.SetRolesAsync(app);

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
