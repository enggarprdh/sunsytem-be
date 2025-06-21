using System.Security.Claims;
using Microsoft.AspNetCore.Http;
namespace _5unSystem.Utility
{
    public static class UserClaimHelper
    {
        private static IHttpContextAccessor? _httpContextAccessor;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public static string? UserName()
        {
            var user = _httpContextAccessor?.HttpContext?.User;
            return user?.Claims.FirstOrDefault(c => c.Type == "userName")?.Value;
        }
    }
}
