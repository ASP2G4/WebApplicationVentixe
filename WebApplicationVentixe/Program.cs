using Authentication.Contexts;
using Authentication.Entities;
using Authentication.Services;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApplicationVentixe.Protos;
using WebApplicationVentixe.Protos.Invoice;
using WebApplicationVentixe.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddMemoryCache();

builder.Services.AddSingleton(new ServiceBusClient(builder.Configuration["AzureServiceBusSettings:ConnectionString"]));
builder.Services.AddScoped<IVerificationService, VerificationService>();
builder.Services.AddScoped<IJwtManager, JwtManager>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<InvoiceGrpcClientService>();

builder.Services.AddHttpClient<BookingsService>();

builder.Services.AddGrpcClient<ProfileHandler.ProfileHandlerClient>(options =>
{
    var grpcUrl = builder.Configuration["GrpcSettings:ProfileHandlerUrl"];
    options.Address = new Uri(grpcUrl!);
});

builder.Services.AddGrpcClient<InvoiceService.InvoiceServiceClient>(options =>
{
    var grpcUrl = builder.Configuration["GrpcSettings:InvoiceServiceUrl"];
    options.Address = new Uri(grpcUrl!);
});

builder.Services.AddGrpcClient<GrpcJwtService.Protos.JwtTokenService.JwtTokenServiceClient>(options =>
{
    var grpcUrl = builder.Configuration["GrpcSettings:JwtTokenServiceUrl"];
    options.Address = new Uri(grpcUrl!);
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
    x.LoginPath = "/Login";
    x.AccessDeniedPath = "/Accessdenied";
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

.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))
    };
})

.AddGitHub(options =>
{
    options.ClientId = builder.Configuration["Authentication:GitHub:ClientId"]!;
    options.ClientSecret = builder.Configuration["Authentication:GitHub:ClientSecret"]!;
    options.CallbackPath = "/signin-github";
    options.Scope.Add("user:email");
});
builder.Services.AddSession();

var app = builder.Build();
await SeedRoleData.SetRolesAsync(app);

app.UseHsts();
app.UseHttpsRedirection();
app.UseStatusCodePagesWithReExecute("/Error/{0}");
app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
