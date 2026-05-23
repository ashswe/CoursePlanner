using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoursePlanner.Models;
using CoursePlanner.Data;

namespace CoursePlanner.Services
{
    public class AuthService
    {
        private readonly AppDatabase _database;

        public AuthService(AppDatabase database)
        {
            _database = database;
        }

        public async Task<bool> RegisterUserAsync(string username, string password)
        {
            await _database.Init();

            var existingUser = await _database.GetUserByUsernameAsync(username);

            if (existingUser != null)
                return false;

            string hashedPassword = PasswordHasher.HashPassword(password);

            var user = new User
            {
                Username = username,
                PasswordHash = hashedPassword
            };

            await _database.SaveUserAsync(user);
            return true;
        }

        public async Task<User?> LoginUserAsync(string username, string password)
        {
            var user = await _database.GetUserByUsernameAsync(username);

            if (user == null)
                return null;

            bool isValid = PasswordHasher.VerifyPassword(password, user.PasswordHash);
            return isValid ? user : null;
        }
    }
}
