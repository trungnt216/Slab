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
        public DbSet<Banner> Banners { get; set; }
        public DbSet<ScientificResearch> scientificResearches { get; set; }
        public DbSet<ScientificResearchFile> ScientificResearchFiles { get; set; }
    }
}
