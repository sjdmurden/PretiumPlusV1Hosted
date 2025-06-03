using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using CSV_reader.Models;

namespace CSV_reader.database
{
    public class ApplicationContext : DbContext
    {
        public DbSet<ClientDetail> ClientDetails { get; set; }
        public DbSet<User> Users { get; set; }
        //public DbSet<IndivClaimDB> IndivClaimsNew { get; set; }
        public DbSet<IndivClaimDB> ClaimsTable { get; set; }

        public DbSet<IndivClaimDataDB> IndivClaimData { get; set; }

        public DbSet<StaticClientDataDB> StaticClientDataDB { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }
    }
}

