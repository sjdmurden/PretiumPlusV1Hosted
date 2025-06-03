using CSV_reader.database;
using CSV_reader.Models;
using DocumentFormat.OpenXml.Math;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using Microsoft.AspNetCore.Identity;

namespace CSV_reader.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationContext _appContext;

        public UserService(ApplicationContext appContext)
        {
            _appContext = appContext;
        }

        public async Task RegisterUser(string userEmail, string password)
        {
            if (DoesUsernameAlreadyExist(userEmail))
            {
                throw new InvalidOperationException("Account with this email already exists.");
            }

            // string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            // create a new user without the hashed password at first
            var user = new User
            {
                UserEmail = userEmail,                
                UserType = 2,
            };

            // add hashed password to user
            var hasher = new PasswordHasher<User>();
            user.UserPassword = hasher.HashPassword(user, password);

            _appContext.Users.Add(user);
            await _appContext.SaveChangesAsync();
        }

        public bool DoesUsernameAlreadyExist(string userEmail)
        {
            return _appContext.Users.Any(x => x.UserEmail == userEmail);
        }


        public async Task<User> Authenticate(string userEmail, string password)
        {
            // find user in table where UserLogin matches the inputted userLogin
            var user = await _appContext.Users
                .FirstOrDefaultAsync(u => u.UserEmail == userEmail);

            //  if the username exists, the inputted password is then checked with the 'UserPassword' which is in the database, otherwise gives invalid login error
            if (user == null || !VerifyPassword(password, user.UserPassword))
            {
                return null; // invalid login
            }

            return user;
        }


        public bool VerifyPassword(string password, string storedHash)
        {
            var hasher = new PasswordHasher<object>();

            var result = hasher.VerifyHashedPassword(null, storedHash, password);

            return result == PasswordVerificationResult.Success;
        }

        /*public bool VerifyPassword(User user, string inputPassword)
        {
            var hasher = new PasswordHasher<User>();

            var result = hasher.VerifyHashedPassword(user, user.UserPassword, inputPassword);

            return result == PasswordVerificationResult.Success;
        }*/
    }

}
