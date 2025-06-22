using Auth.Types;
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
        private readonly JwtSettings? jwtSettings;
        private readonly RSA rsa;
        private readonly ITokenStore tokenStore;
        public JwtService(IConfiguration configuration,ITokenStore tokenStore)
        {
            jwtSettings = configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>();
            this.tokenStore = tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));
            if (rsa == null)
            {
                rsa = RSA.Create();
                rsa.ImportFromPem(File.ReadAllText("private.pem"));
            }
        }
        public string GenerateToken(string username, string role)
        {
            string jti = Guid.NewGuid().ToString();
            var creds = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti,jti),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };
            var token = new JwtSecurityToken(
                issuer: jwtSettings?.Issuer,
                audience: jwtSettings?.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwtSettings?.ExpirationMinutes ?? 30),
                signingCredentials: creds
            );
            // Store the token in the token store
            tokenStore.Store(jti, token.ValidTo);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}