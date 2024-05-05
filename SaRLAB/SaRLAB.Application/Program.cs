using SaRLAB.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SaRLAB.DataAccess;
using Microsoft.Identity.Client;
using Microsoft.OpenApi.Models;
using SaRLAB.DataAccess.ProjectDto.LoginDto;
using SaRLAB.DataAccess.ProjectDto.SubjectDto;

namespace SaRLAB.Application
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
      

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            builder.Services.AddDbContext<ApplicationDbContext>(options => {
                options.UseSqlServer(builder.Configuration.GetConnectionString("MyDbConnection"));
            });
            builder.Services.AddControllers();


            //add service
            builder.Services.AddScoped<ILoginDto, LoginDto>();
            builder.Services.AddScoped<ISubjectDto, SubjectDto>();

            var app = builder.Build();


            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.RoutePrefix = string.Empty;
                options.DocumentTitle = "My Swagger";
            });



            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.MapControllers();


            app.Run();
        }

       
    }
}
