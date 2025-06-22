using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net.Http.Headers;
using Users.Interfaces;

namespace Users.Types
{
    public class AuthHeaderHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _accessor;

        public AuthHeaderHandler(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if(_accessor.HttpContext != null)
            {
                var token = _accessor.HttpContext.Request.Cookies["token"];
                if (string.IsNullOrEmpty(token))
                {
                    token = _accessor.HttpContext.User?.Claims.FirstOrDefault(c=>c.Type =="access_token")?.Value;
                }
                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                else
                {
                    token = _accessor.HttpContext?.Request.Headers["Authorization"].ToString();
                }

                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Split(" ").Last());
                }
            }
            return base.SendAsync(request, cancellationToken);
        }
    }

}
