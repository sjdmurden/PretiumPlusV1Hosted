using CSV_reader.Models;

namespace CSV_reader.Services
{
    public interface IUserService
    {
        Task RegisterUser(string userLogin, string password);
        Task<User?> Authenticate(string userLogin, string password);
        bool VerifyPassword(string password, string storedHash);
    }
}
