using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Auth
{
    public class JwtService
    {
        private readonly IConfiguration configuration;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly RSA rsa;
        public JwtService(IConfiguration configuration,IHttpContextAccessor contextAccessor)
        {
            this.configuration = configuration;
            this.contextAccessor = contextAccessor;
            rsa = RSA.Create();
            rsa.ImportFromPem(File.ReadAllText("private.pem"));
        }
        public string GenerateToken(string username, string role)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");

            var creds = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256);
            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims:
                [
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, role),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                ],
                expires: DateTime.Now.AddMinutes(int.Parse(jwtSettings["ExpirationMinutes"]??"30")),
                signingCredentials: creds
            );
            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(tokenString);
            if (contextAccessor.HttpContext != null)
            {
                var identity = new ClaimsIdentity(jwt.Claims, JwtBearerDefaults.AuthenticationScheme);
                contextAccessor.HttpContext.User = new ClaimsPrincipal(identity);
            }
            return tokenString;
        }
    }
}