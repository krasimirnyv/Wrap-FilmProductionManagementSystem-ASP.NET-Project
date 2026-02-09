namespace FilmProductionManagementSystem.Web;

using Wrap.Data;
using Wrap.Data.Models.Infrastructure;

using Wrap.Services.Core;
using Wrap.Services.Core.Interface;

using static Wrap.GCommon.ApplicationConstants;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Features;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Load user secrets.
        builder.Configuration.AddUserSecrets<Program>();
        
        // Add services to the container.
        string? secretConnection = builder.Configuration["ConnectionStrings:MyDevConnection"];
        string? defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");
        
        string connectionString = !string.IsNullOrWhiteSpace(secretConnection)
            ? secretConnection
            : defaultConnection
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        builder.Services.AddDbContext<FilmProductionDbContext>(options =>
        {
            options
                .UseSqlServer(connectionString);
        });
        
        builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<FilmProductionDbContext>();

        builder.Services.AddScoped<IWrapService, WrapService>();
        builder.Services.AddScoped<IWrapAccountService, WrapAccountService>();

        // Using session + "draft" JSON for the two-step registration form for crew members
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(10);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        builder.Services.AddHttpContextAccessor();
        
        // Configure file upload size (if needed)
        builder.Services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = MaxFileSize; // 10MB
        });
        
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        
        builder.Services.AddControllersWithViews();
        
        builder.Services.AddRazorPages();

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseStatusCodePagesWithReExecute("Home/StatusCodeError", "?statusCode={0}");

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseSession();
        
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        
        app.MapControllerRoute(
            name: "area",
            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
        
        app.MapRazorPages();
        
        app.Run();
    }
}