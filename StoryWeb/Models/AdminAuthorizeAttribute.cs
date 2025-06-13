// File: AdminAuthorizeAttribute.cs
using System;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public class AdminAuthorizeAttribute : AuthorizeAttribute
{
    protected override bool AuthorizeCore(HttpContextBase httpContext)
    {
        var tokenCookie = httpContext.Request.Cookies["AuthToken"];
        if (tokenCookie == null || string.IsNullOrEmpty(tokenCookie.Value))
        {
            return false;
        }

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenCookie.Value;

            var secretKey = "your-super-secret-key-that-is-long-and-secure";
            var key = Encoding.ASCII.GetBytes(secretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false, 
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero 
            };

            SecurityToken validatedToken;
            var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

            var roleClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role || c.Type == "role");

            if (roleClaim != null && roleClaim.Value == "0")
            {
                httpContext.User = principal; 
                return true;
            }

            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
    {
        filterContext.Result = new RedirectToRouteResult(
            new System.Web.Routing.RouteValueDictionary(new
            {
                controller = "User",
                action = "Login"
            })
        );
    }
}