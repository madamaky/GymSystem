using GymSystemBLL.MappingProfiles;
using GymSystemBLL.Services.AttachmentService;
using GymSystemBLL.Services.Classes;
using GymSystemBLL.Services.Interfaces;
using GymSystemDAL.Data.Contexts;
using GymSystemDAL.Data.DataSeed;
using GymSystemDAL.Entities;
using GymSystemDAL.Repositories.Classes;
using GymSystemDAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
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
            builder.Services.AddScoped<IAttachmentService, AttachmentService>();
            builder.Services.AddScoped<IAccountService, AccountService>();

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(Config =>
            {
                Config.Password.RequiredLength = 6;
                Config.Password.RequireLowercase = true;
                Config.Password.RequireUppercase = true;
                Config.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<GymSystemDbContext>();

            builder.Services.ConfigureApplicationCookie(options =>
            {                 
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });

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


            var RoleManager = Scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = Scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            IdentityDbContextSeeding.SeedData(RoleManager, UserManager);

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
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
