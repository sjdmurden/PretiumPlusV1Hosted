using CSV_reader.Models;

namespace CSV_reader.Services
{
    public interface IDeletionService
    {
        Task<bool> DeleteUserByUsernameAsync(string username);
        Task<bool> DeleteQuoteByQuoteIdAsync(string quoteId);
    }
}
