using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using Users.Interfaces;

namespace Users.Types
{
    public class TokenProvider : ITokenProvider
    {
        private string? token;
        private DateTime expires;
        public Task<string?> GetTokenAsync()
        {
            Console.WriteLine("Reading token: " + token);
            if (DateTime.UtcNow >= expires) { token = null; }

            return Task.FromResult(token);
        }
        

        public void SetToken(string token)
        {
            Console.WriteLine("Setting token: " + token);
            this.token = token;
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var exp = jwt.Payload.Expiration;
            
            expires =(exp!=null)? DateTimeOffset.FromUnixTimeMilliseconds((long)exp).UtcDateTime : DateTime.UtcNow.AddMinutes(15);
        }
    }
}
