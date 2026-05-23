using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CoursePlanner.Services
{
    public static class PasswordHasher
    {
        // Create Salt & Hash
        private static string GenerateSalt()
        {
            byte[] saltBytes = RandomNumberGenerator.GetBytes(8);
            return Convert.ToBase64String(saltBytes);
        }

        private static string CreateHash(string password, string salt)
        {
            using var hasher = SHA256.Create();

            byte[] bytes = Encoding.UTF8.GetBytes(password + salt);
            byte[] hashBytes = hasher.ComputeHash(bytes);

            return Convert.ToBase64String(hashBytes);

        }

        // Password Hasher
        public static string HashPassword(string password)
        {
            var salt = GenerateSalt();
            var hash = CreateHash(password, salt);
            return $"{salt}:{hash}";
        }

        // Password Verification
        public static bool VerifyPassword(string enteredPassword, string storedPassword)
        {
            var parts = storedPassword.Split(':');

            if (parts.Length != 2)
            {
                return false;
            }

            string salt = parts[0];
            string storedHash = parts[1];

            string enteredHash = CreateHash(enteredPassword, salt);

            return enteredHash == storedHash;
        }
    }
}
