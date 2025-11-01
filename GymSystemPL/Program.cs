using GymSystemBLL.MappingProfiles;
using GymSystemBLL.Services.Classes;
using GymSystemBLL.Services.Interfaces;
using GymSystemDAL.Data.Contexts;
using GymSystemDAL.Data.DataSeed;
using GymSystemDAL.Repositories.Classes;
using GymSystemDAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymSystemPL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            // Add services to the container.
            builder.Services.AddControllersWithViews(); // MVC

            #region Dependency Injection

            // 1. Make DbContext class 'public'

            builder.Services.AddDbContext<GymSystemDbContext>(options =>
            {
                //options.UseSqlServer(builder.Configuration.GetSection("ConnectionStrings")["DefaultConnection"]);
                //options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]);
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            #endregion

            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IPlanRepository, PlanRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<ISessionRepository, SessionRepository>();
            builder.Services.AddAutoMapper(X => X.AddProfile(new MappingProfiles()));
            builder.Services.AddScoped<IMemberService, MemberService>();
            builder.Services.AddScoped<ITrainerService, TrainerService>();
            builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
            builder.Services.AddScoped<IPlanService, PlanService>();
            builder.Services.AddScoped<ISessionService, SessionService>();

            var app = builder.Build();

            #region Data Seed

            // Create Session With Database Manually
            /*using*/ var Scope = app.Services.CreateScope();
            var dbContext = Scope.ServiceProvider.GetRequiredService<GymSystemDbContext>();

            // Check If There I Migration Pending or Not
            var PendingMigration = dbContext.Database.GetPendingMigrations();
            if (PendingMigration?.Any() ?? false)
                dbContext.Database.Migrate();

            GymDbContextSeeding.SeedData(dbContext);

            #endregion

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
