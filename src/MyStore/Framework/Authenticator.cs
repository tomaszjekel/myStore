using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace MyStore.Framework
{
    public class Authenticator : IAuthenticator
    {
        private readonly IHttpContextAccessor _contextAccessor;
        

        public Authenticator(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        
        public async Task SignInAsync(Guid userId, string email, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.Role, role),
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim("UserId", userId.ToString()));
            var principal = new ClaimsPrincipal(identity);
            await _contextAccessor.HttpContext.SignInAsync(principal);
        }

        public async Task SignOutAsync()
            => await _contextAccessor.HttpContext.SignOutAsync();
    }
}