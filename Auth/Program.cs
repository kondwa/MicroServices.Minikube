using Auth;
using Auth.Types;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<JwtService>();
builder.Services.AddSingleton<ITokenStore, InMemoryTokenStore>();
JwtSettings? jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

string privateKey = "private.pem";
if (!File.Exists(privateKey))
{
    throw new FileNotFoundException($"Private key file '{privateKey}' not found.");
}
var rsa = RSA.Create();
rsa.ImportFromPem(File.ReadAllText(privateKey));

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings?.Issuer,
        ValidAudience = jwtSettings?.Audience,
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
            Console.WriteLine($"📩 Token received: {context.Token}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var jti = context.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            var tokenStore = context.HttpContext.RequestServices.GetRequiredService<ITokenStore>();
            if (string.IsNullOrEmpty(jti) || !tokenStore.IsValid(jti))
            {
                context.Fail("Token is invalid or has been revoked.");
            }
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddAuthorization();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Ok("Auth API is running")).WithOpenApi();

app.MapPost("/login", (AuthRequest request, IHttpContextAccessor contextAccessor, JwtService jwtService) =>
{
    var (isValid, role) = Authenticate(request.UserName, request.Password);
    if (isValid)
    {
        var token = jwtService.GenerateToken(request.UserName, role);
        return Results.Ok(new AuthResponse { Token = token });
    }
    return Results.Unauthorized();
})
.WithName("Login")
.WithOpenApi();
app.MapGet("/logout", (IHttpContextAccessor contextAccessor, ITokenStore tokenStore) =>
{
    var jti = contextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
    if (!string.IsNullOrEmpty(jti))
    {
        tokenStore.Revoke(jti);
    }
    contextAccessor.HttpContext?.Response.Cookies.Delete("token");
    return Results.Ok("Logged out successfully.");
})
    .WithName("Logout")
    .WithOpenApi();

app.Run();

static (bool isValid, string role) Authenticate(string userName, string password) => (userName, password) switch
{
    ("admin", "adminpass") => (true, "admin"),
    ("user", "userpass") => (true, "user"),
    _ => (false, string.Empty)
};