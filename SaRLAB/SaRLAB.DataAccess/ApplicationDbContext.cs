using Microsoft.EntityFrameworkCore;
using SaRLAB.Models;

namespace SaRLAB.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<User> User { get; set; }
        public DbSet<Subject> Subject { get; set; }
    }
}
