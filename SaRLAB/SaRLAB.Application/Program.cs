using SaRLAB.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SaRLAB.DataAccess;
using Microsoft.Identity.Client;
using Microsoft.OpenApi.Models;
using SaRLAB.DataAccess.Service.SubjectDto;
using SaRLAB.DataAccess.Dto.LoginService;
using SaRLAB.DataAccess.Service.UserService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SaRLAB.DataAccess.Service.BannerService;
using SaRLAB.DataAccess.Service.ScientificResearchService;
using SaRLAB.DataAccess.Service.ScientificResearchFileService;
using SaRLAB.DataAccess.Service.EquipmentService;
using SaRLAB.DataAccess.Service.DocumentService;
using SaRLAB.DataAccess.Service.PlanDetailService;
using SaRLAB.DataAccess.Service.PracticePlanService;
using SaRLAB.DataAccess.Service.SchoolService;
using SaRLAB.DataAccess.Service.QuizService;
using SaRLAB.DataAccess.Service.SubjectFlagService;

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
            builder.Services.AddScoped<ILoginService, LoginService>();
            builder.Services.AddScoped<ISubjectDto, SubjectDto>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IBannerService, BannerService>();
            builder.Services.AddScoped<IScientificResearchService, ScientificResearchService>();
            builder.Services.AddScoped<IScientificResearchFileService, ScientificResearchFileService>();
            builder.Services.AddScoped<IEquipmentService, EquipmentService>();
            builder.Services.AddScoped<IDocumentService, DocumentService>();
            builder.Services.AddScoped<IPlanDetailService, PlanDetailService>();
            builder.Services.AddScoped<IPracticePlanService, PracticePlanService>();
            builder.Services.AddScoped<ISchoolService, SchoolService>();
            builder.Services.AddScoped<IQuizService, QuizService>();
            builder.Services.AddScoped<ISubjectFlagService, SubjectFlagService>();


            //Add Authentication and JwtBearer
            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                        ValidAudience = builder.Configuration["JWT:ValidAudience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
                    };
                });
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
            app.UseAuthorization();
            app.MapControllers();


            app.Run();
        }

       
    }
}
