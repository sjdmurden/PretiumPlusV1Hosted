using CSV_reader.database;
using CSV_reader.Models;
using CSV_reader.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace CSV_reader.Services
{
    public class DeletionService : IDeletionService
    {
        private readonly ApplicationContext _appContext;

        public DeletionService(ApplicationContext appContext)
        {
            _appContext = appContext;
        }

        public async Task<bool> DeleteUserByUsernameAsync(string userEmail)
        {
            if (string.IsNullOrWhiteSpace(userEmail))
                return false;

            if (userEmail.Equals("admin@gmail.com", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Admin user cannot be deleted.");

            var user = await _appContext.Users
                .FirstOrDefaultAsync(u => u.UserEmail == userEmail);

            if (user == null)
                return false;

            _appContext.Users.Remove(user);
            await _appContext.SaveChangesAsync();

            return true;
        }


        public async Task<bool> DeleteQuoteByQuoteIdAsync(string quoteId)
        {
            using var transaction = await _appContext.Database.BeginTransactionAsync();

            try
            {
                var batchIds = await _appContext.StaticClientDataDB
                    .Where(x => x.BatchId.StartsWith(quoteId))
                    .Select(x => x.BatchId)
                    .Distinct()
                    .ToListAsync();

                if (!batchIds.Any())
                    return false;

                var batchIdSet = batchIds.ToHashSet();

                var claimsRows = _appContext.ClaimsTable
                    .Where(x => batchIdSet.Contains(x.BatchId));

                var indivClaimDataRows = _appContext.IndivClaimData
                    .Where(x => batchIdSet.Contains(x.BatchId));

                var staticClientDataDBRows = _appContext.StaticClientDataDB
                    .Where(x => batchIdSet.Contains(x.BatchId));

                _appContext.ClaimsTable.RemoveRange(claimsRows);
                _appContext.IndivClaimData.RemoveRange(indivClaimDataRows);
                _appContext.StaticClientDataDB.RemoveRange(staticClientDataDBRows);

                await _appContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
    }
}
