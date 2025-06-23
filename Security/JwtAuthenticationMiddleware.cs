using Microsoft.AspNetCore.Http;
using MilitaryServices.App.Services;
using MilitaryServices.App.Security;
using System.Security.Claims;
using System.Text.Json;

namespace MilitaryServices.App.Middleware
{
    public class JwtAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public void Invoke(HttpContext context, JwtUtil jwtUtil, IUserService userService, IMessageService messageService)
        {
            var path = context.Request.Path.Value;

            if (path == "/performLogin")
            {
                _next(context);
                return;
            }

            if (jwtUtil.ValidateRequest(context.Request))
            {
                var username = jwtUtil.ExtractUsername(context.Request);
                var user = userService.FindUser(username);
                var token = jwtUtil.ExtractToken(context.Request);
                var roles = jwtUtil.ExtractRoles(token);
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, username)
                };
                claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
                var identity = new ClaimsIdentity(claims, "jwt");
                var principal = new ClaimsPrincipal(identity);
                context.User = principal;

                _next(context);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                var message = messageService.GetMessage("TOKEN_TAMPERED", System.Globalization.CultureInfo.InvariantCulture);
                var responseBody = JsonSerializer.Serialize(new { message });
                context.Response.WriteAsync(responseBody);
            }
        }
    }
}
