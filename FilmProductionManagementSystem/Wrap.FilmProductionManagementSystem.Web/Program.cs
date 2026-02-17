namespace FilmProductionManagementSystem.Web;

using Wrap.Data;
using Wrap.Data.Models.Infrastructure;

using Wrap.Services.Core;
using Wrap.Services.Core.Interface;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http.Features;

using static Wrap.GCommon.ApplicationConstants;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Load user secrets.
        builder.Configuration.AddUserSecrets<Program>();
        
        // Add services to the container.
        string? secretConnection = builder.Configuration[SecretConnectionString];
        string? defaultConnection = builder.Configuration.GetConnectionString(DefaultConnection);
        
        string connectionString = !string.IsNullOrWhiteSpace(secretConnection)
            ? secretConnection
            : defaultConnection
            ?? throw new InvalidOperationException(MissingConnectionStringMessage);

        builder.Services.AddDbContext<FilmProductionDbContext>(options =>
        {
            options
                .UseSqlServer(connectionString);
        });
        
        builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                ConfigureIdentity(options, builder.Configuration);
            })
            .AddEntityFrameworkStores<FilmProductionDbContext>();
        
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = LoginPath;
            options.LogoutPath = LogoutPath;
        });
        
        builder.Services.AddScoped<IHomeService, HomeService>();
        builder.Services.AddScoped<ILoginRegisterService, LoginRegisterService>();
        builder.Services.AddScoped<INavBarService, NavBarService>();
        builder.Services.AddScoped<IProfileService, ProfileService>();
        builder.Services.AddScoped<IProductionService, ProductionService>();

        // Using session + "draft" JSON for the two-step registration form for crew members.
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession(ConfigureSession);
        builder.Services.AddHttpContextAccessor();
        
        // Configure file upload size.
        builder.Services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = MaxFileSize;
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
        
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        
        app.UseSession();

        app.UseAuthentication();
        app.UseAuthorization();
        
        app.UseStatusCodePagesWithReExecute("/Error/{0}");
        
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        
        app.MapControllerRoute(
            name: "area",
            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
        
        app.MapRazorPages();
        
        app.Run();
    }

    private static void ConfigureIdentity(IdentityOptions options, ConfigurationManager configuration)
    {
        /* SignIn settings - Account should not be confirmed fo dev purposes. */
        options.SignIn.RequireConfirmedAccount = configuration.GetValue<bool>(SignInRequireConfirmedAccount);
        options.SignIn.RequireConfirmedEmail = configuration.GetValue<bool>(SignInRequireConfirmedEmail);
        options.SignIn.RequireConfirmedPhoneNumber = configuration.GetValue<bool>(SignInRequireConfirmedPhoneNumber);

        /* User settings - Email must be unique. */
        options.User.RequireUniqueEmail = configuration.GetValue<bool>(UserRequireUniqueEmail);
        
        /* Lockout settings - After 255 failed attempts to login => Account is locked for 1 min. */
        options.Lockout.MaxFailedAccessAttempts = configuration.GetValue<int>(LockoutMaxFailedAccessAttempts);
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(configuration.GetValue<int>(LockoutDefaultLockoutTimeSpan));
                
        /* Password settings - Security is not required for development. */
        options.Password.RequireDigit = configuration.GetValue<bool>(PasswordRequireDigit);
        options.Password.RequireLowercase = configuration.GetValue<bool>(PasswordRequireLowercase);
        options.Password.RequireUppercase = configuration.GetValue<bool>(PasswordRequireUppercase);
        options.Password.RequireNonAlphanumeric = configuration.GetValue<bool>(PasswordRequireNonAlphanumeric);
        options.Password.RequiredUniqueChars = configuration.GetValue<int>(PasswordRequiredUniqueChars);
        options.Password.RequiredLength = configuration.GetValue<int>(PasswordRequiredLength);
             
        /* Stores settings - Protecting personal data is not required for dev. */
        options.Stores.ProtectPersonalData = configuration.GetValue<bool>(StoresProtectPersonalData);
    }

    private static void ConfigureSession(SessionOptions options)
    {
        options.IdleTimeout = TimeSpan.FromMinutes(IdleTimeoutMinutes);
        options.Cookie.HttpOnly = CookieHttpOnly;
        options.Cookie.IsEssential = CookieIsEssential;
    }
}