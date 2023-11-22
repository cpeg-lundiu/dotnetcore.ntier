using Microsoft.EntityFrameworkCore;
using DotNetCore.NTier.Models.Dao;

namespace DotNetCore.NTier.Services
{
    public class AppDataContext : DbContext
    {
        public AppDataContext(DbContextOptions<AppDataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}
