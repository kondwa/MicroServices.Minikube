using Microsoft.AspNetCore.Authentication.Cookies;
using Refit;
using Web.Interfaces;
using Web.Types;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<AuthHeaderHandler>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => { 
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
    });
builder.Services.AddAuthorization();

builder.Services.AddRefitClient<IAuthAPI>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://auth-api"))
    .AddHttpMessageHandler<AuthHeaderHandler>();
builder.Services.AddRefitClient<IUsersAPI>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://users-api"))
    .AddHttpMessageHandler<AuthHeaderHandler>();
builder.Services.AddRefitClient<IOrdersAPI>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://orders-api"))
    .AddHttpMessageHandler<AuthHeaderHandler>();
builder.Services.AddRefitClient<IProductsAPI>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://products-api"))
    .AddHttpMessageHandler<AuthHeaderHandler>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
