using Auth;
using Auth.Types;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var rsa = RSA.Create();
rsa.ImportFromPem(File.ReadAllText("private.pem"));

var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? string.Empty);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
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
            Console.WriteLine($"📩 Token received: {context.Token}");
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
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/login", (AuthRequest request,IHttpContextAccessor contextAccessor) =>
{
    if (request.UserName == "admin" && request.Password == "adminpass") // Replace with your authentication logic
    {
        var jwtService = new JwtService(builder.Configuration,contextAccessor);
        var token = jwtService.GenerateToken(request.UserName, "Admin"); // Replace with your role logic
        return Results.Ok(new AuthResponse { Token = token });
    }
    else if (request.UserName == "user" && request.Password == "userpass") // Another user example
    {
        var jwtService = new JwtService(builder.Configuration,contextAccessor);
        var token = jwtService.GenerateToken(request.UserName, "User"); // Replace with your role logic
        return Results.Ok(new AuthResponse { Token = token });
    }
    return Results.Unauthorized();
})
.WithName("Auth")
.WithOpenApi();

app.Run();