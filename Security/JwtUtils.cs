using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MilitaryServicesBackendDotnet.Security
{
    public class JwtUtil : IJwtUtil
    {
        private readonly RSA _privateKey;
        private readonly RSA _publicKey;
        private const int ExpirationMinutes = 300; // 5 hours

        public JwtUtil()
        {
            RSAKeyGenerator.ProduceKeys();
            _privateKey = RSAKeyGenerator.LoadPrivateKey();
            _publicKey = RSAKeyGenerator.LoadPublicKey();
        }

        public bool ValidateRequest(HttpRequest request)
        {
            var token = ExtractToken(request);
            return IsTokenValid(token);
        }

        public string ExtractToken(HttpRequest request)
        {
            var authHeader = request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return string.Empty;
            return authHeader.Substring(7);
        }

        public string ExtractUsername(HttpRequest request)
        {
            var token = ExtractToken(request);
            return ExtractUsername(token);
        }

        public string GenerateToken(string username, List<string> roles = null)
        {
            var handler = new JwtSecurityTokenHandler();

            var key = new RsaSecurityKey(_privateKey);

            var credentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);

            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, username)
            };

            if (roles != null)
            {
                claims.Add(new Claim("roles", System.Text.Json.JsonSerializer.Serialize(roles)));
            }

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(ExpirationMinutes),
                signingCredentials: credentials
            );

            return handler.WriteToken(token);
        }

        public bool IsTokenValid(string token)
        {
            try
            {
                var validationParams = new TokenValidationParameters
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    IssuerSigningKey = new RsaSecurityKey(_publicKey),
                    ValidateIssuerSigningKey = true
                };

                var handler = new JwtSecurityTokenHandler();
                handler.ValidateToken(token, validationParams, out _);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string ExtractUsername(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        }

        public List<string> ExtractRoles(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var rolesClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "roles")?.Value;
            if (string.IsNullOrEmpty(rolesClaim))
                return new List<string>();

            return System.Text.Json.JsonSerializer.Deserialize<List<string>>(rolesClaim);
        }
    }
}
