using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    // Install EntityFrameworkCore,EntityFrameworkCoreDesign,EntityFrameworkCoreSqlServer from nuget packages
    // Create a class that extends DbContext and add it in program file
    public class DataContext: DbContext
    {

        public DataContext(DbContextOptions options):base(options) { }

        public DbSet<AppUser> Users { get; set; }
    }
}
