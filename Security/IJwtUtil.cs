using Microsoft.AspNetCore.Http;

namespace MilitaryServices.App.Security
{
    public interface IJwtUtil
    {
        bool ValidateRequest(HttpRequest request);
        string ExtractToken(HttpRequest request);
        string ExtractUsername(HttpRequest request);
        string GenerateToken(string username, List<string> roles = null);
        bool IsTokenValid(string token);
        string ExtractUsername(string token);
        List<string> ExtractRoles(string token);
    }
}
