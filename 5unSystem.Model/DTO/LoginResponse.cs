using System;

namespace _5unSystem.Model.DTO;

public class LoginResponse
{
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public string? UserName { get; set; }
        public string? Role { get; set; }
}
