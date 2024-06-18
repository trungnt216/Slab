using SaRLAB.Models.Entity;

namespace SaRLAB.AdminWeb
{
    public class Program
    {
        public static string jwtToken = null;

#if true
        public static string api = "http://localhost:5200/api/";
        public static string FilePath = "https://localhost:7135//";
#endif

#if false
        public static string api = "http://api.sarlabeducation.com/api/";
        public static string FilePath = "https://admin.sarlabeducation.com//";
#endif
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            User loginAccout = new User();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Login}/{action=Index}");

            app.Run();
        }
    }
}
