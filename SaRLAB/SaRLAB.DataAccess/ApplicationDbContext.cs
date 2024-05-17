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
        public DbSet<Document> Documents { get; set; }
        public DbSet<ScientificResearch> ScientificResearchs { get; set; }
        public DbSet<ScientificResearchFile> ScientificResearchFiles { get; set; }
        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<PracticePlan>  PracticePlans { get; set; }
        public DbSet<PlanDetail>  PlanDetails { get; set; }
    }
}
