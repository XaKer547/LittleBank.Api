using LittleBank.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LittleBank.Api.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Card> Cards { get; set; }
        
        public ApplicationDbContext(DbContextOptions options) : base(options)
        { }
    }
}
