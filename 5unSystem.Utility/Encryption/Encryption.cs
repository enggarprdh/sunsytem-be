using System;

namespace _5unSystem.Utility;

public static class Encryption
{
    public static string HashString(this string value)
    {
        // Contoh sederhana dari fungsi hash, sebaiknya gunakan algoritma hash yang lebih aman seperti BCrypt atau Argon2
        using (var sha256 = System.Security.Cryptography.SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(value));
            var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLowerInvariant();
            return hash;
        }
    }

}
