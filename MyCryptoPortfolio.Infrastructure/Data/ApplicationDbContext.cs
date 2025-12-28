using Microsoft.EntityFrameworkCore;
using MyCryptoPortfolio.Domain.Entities;

namespace MyCryptoPortfolio.Infrastructure.Data 
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Transaction> Transactions { get; set; }
    }
}