using Microsoft.EntityFrameworkCore;
using SaRLAB.Models.Entity;

namespace SaRLAB.DataAccess
{

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<ManageLogic> ManageLogics { get; set; }

        public DbSet<Subject> Subjects { get; set; }
    }
}
