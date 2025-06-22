using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Refit;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Users.Interfaces;
using Users.Types;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddScoped<AuthHeaderHandler>();

// Refit clients
builder.Services.AddRefitClient<IOrdersAPI>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://orders-api"))
    .AddHttpMessageHandler<AuthHeaderHandler>();

builder.Services.AddRefitClient<IProductsAPI>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://products-api"))
    .AddHttpMessageHandler<AuthHeaderHandler>();

// Auth API for token generation
builder.Services.AddRefitClient<IAuthAPI>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://auth-api"));

// JWT Auth
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var rsa = RSA.Create();
rsa.ImportFromPem(File.ReadAllText("public.pem"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new RsaSecurityKey(rsa)
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("❌ Authentication failed: " + context.Exception.Message);
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                Console.WriteLine("⚠️ Challenge triggered");
                return Task.CompletedTask;
            },
            OnMessageReceived = context =>
            {
                var tokenFromCookie = context.HttpContext.Request.Cookies["token"];
                if (!string.IsNullOrEmpty(tokenFromCookie))
                {
                    context.Token = tokenFromCookie;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Auth middleware
app.UseAuthentication();
app.UseAuthorization();

// Endpoints
app.MapGet("/", () => Results.Ok("Users API is running")).WithOpenApi();
app.MapGet("/users", UsersAsync).RequireAuthorization().WithName("Users").WithOpenApi();
app.MapGet("/orders", OrdersAsync).RequireAuthorization().WithName("Orders").WithOpenApi();
app.MapGet("/products", ProductsAsync).RequireAuthorization().WithName("Products").WithOpenApi();

app.MapPost("/login", LoginAsync)
    .WithOpenApi()
    .WithName("Login");

app.Run();

static async Task<IResult> OrdersAsync(IOrdersAPI ordersAPI)
{
    try
    {
        var orders = await ordersAPI.GetOrdersAsync();
        return Results.Ok(new { Orders = orders });
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
}

static async Task<IResult> ProductsAsync(IProductsAPI productsAPI)
{
    try
    {
        var products = await productsAPI.GetProductsAsync();
        return Results.Ok(new { Products = products });
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
}

static async Task<IResult> UsersAsync(IOrdersAPI ordersAPI, IProductsAPI productsAPI)
{
    try
    {
        var orders = await ordersAPI.GetOrdersAsync();
        var products = await productsAPI.GetProductsAsync();
        return Results.Ok(new { User = "John Doe", Orders = orders, Products = products });
    }
    catch (Exception e)
    {
        return Results.Problem(e.Message);
    }
}
static async Task<IResult> LoginAsync(IAuthAPI authAPI, AuthRequest request, HttpContext context)
{
    try
    {
        var response = await authAPI.LoginAsync(request);
        if (!string.IsNullOrEmpty(response.Token))
        {
            var token = response.Token;
            context.Response.Cookies.Append("token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });
            return Results.Ok(new { Token = token });
        }
        return Results.Unauthorized();
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
}