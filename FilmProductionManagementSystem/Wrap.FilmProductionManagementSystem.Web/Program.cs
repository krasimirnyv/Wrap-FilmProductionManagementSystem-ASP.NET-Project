namespace FilmProductionManagementSystem.Web;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Authentication.Cookies;

using Wrap.Data;
using Wrap.Data.Models.Infrastructure;
using Wrap.Infrastructure.Extensions;

using static Wrap.GCommon.ApplicationConstants;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        
        string connectionString = ConnectionString(builder);

        builder.Services.AddDbContext<FilmProductionDbContext>(options => 
            options.UseSqlServer(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.RegisterApplicationServices();
        
        builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                ConfigureIdentity(options, builder.Configuration);
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<FilmProductionDbContext>();
        
        builder.Services.ConfigureApplicationCookie(options =>
        {
            ConfigureCookie(options, builder.Configuration);
        });

        // Using session + "draft" JSON for the two-step registration form for crew members.
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession(options =>
        {
            ConfigureSession(options, builder.Configuration);
        });
        builder.Services.AddHttpContextAccessor();
        
        builder.Services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = builder.Configuration.GetValue<long>(MaxFileSizeOptions);
        });
        
        builder.Services.AddControllersWithViews()
            .AddMvcOptions(options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));
        
        builder.Services.AddRazorPages();

        WebApplication app = builder.Build();

        app.UseStatusCodePagesWithReExecute(StatusCodeErrorPath);

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler(ExceptionHandlerPath);
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        
        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseCookiePolicy();
        app.UseStaticFiles();
        app.UseSession();

        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapControllerRoute(
            name: "slugRouteWithId",
            pattern: "{controller=Home}/{action=Index}/{id:required}/{slug:required}");
        app.MapControllerRoute(
            name: "slugRoute",
            pattern: "{controller=Home}/{action=Index}/{slug:required}");
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        
        app.MapRazorPages();
        
        app.Run();
    }
    
    private static string ConnectionString(WebApplicationBuilder builder)
    {
        string? defaultConnection = builder.Configuration.GetConnectionString(DefaultConnection);
        string? connectionString;
        
        bool isRunningInContainer = string.Equals(Environment.GetEnvironmentVariable(ContainerEnvironment), "true", StringComparison.OrdinalIgnoreCase);
        if (isRunningInContainer)
        {
            string? secretDockerConnection = builder.Configuration.GetConnectionString(DockerConnectionString);
            
            connectionString = !string.IsNullOrWhiteSpace(secretDockerConnection) 
                ? secretDockerConnection
                : defaultConnection 
                  ?? throw new InvalidOperationException(MissingConnectionStringMessage);
            
            return connectionString;
        }

        builder.Configuration.AddUserSecrets<Program>();
        string? secretConnection = builder.Configuration[SecretConnectionString];
        
        connectionString = !string.IsNullOrWhiteSpace(secretConnection) 
            ? secretConnection 
            : defaultConnection 
              ?? throw new InvalidOperationException(MissingConnectionStringMessage);
        
        return connectionString;
    }

    private static void ConfigureIdentity(IdentityOptions options, ConfigurationManager configuration)
    {
        options.SignIn.RequireConfirmedAccount = configuration.GetValue<bool>(SignInRequireConfirmedAccount);
        options.SignIn.RequireConfirmedEmail = configuration.GetValue<bool>(SignInRequireConfirmedEmail);
        options.SignIn.RequireConfirmedPhoneNumber = configuration.GetValue<bool>(SignInRequireConfirmedPhoneNumber);

        options.User.RequireUniqueEmail = configuration.GetValue<bool>(UserRequireUniqueEmail);
        
        options.Lockout.MaxFailedAccessAttempts = configuration.GetValue<int>(LockoutMaxFailedAccessAttempts);
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(configuration.GetValue<int>(LockoutDefaultLockoutTimeSpan));
                
        options.Password.RequireDigit = configuration.GetValue<bool>(PasswordRequireDigit);
        options.Password.RequireLowercase = configuration.GetValue<bool>(PasswordRequireLowercase);
        options.Password.RequireUppercase = configuration.GetValue<bool>(PasswordRequireUppercase);
        options.Password.RequireNonAlphanumeric = configuration.GetValue<bool>(PasswordRequireNonAlphanumeric);
        options.Password.RequiredUniqueChars = configuration.GetValue<int>(PasswordRequiredUniqueChars);
        options.Password.RequiredLength = configuration.GetValue<int>(PasswordRequiredLength);
             
        options.Stores.ProtectPersonalData = configuration.GetValue<bool>(StoresProtectPersonalData);
    }

    private static void ConfigureCookie(CookieAuthenticationOptions options, ConfigurationManager configuration)
    {
        options.Cookie.HttpOnly = configuration.GetValue<bool>(CookieHttpOnly);
        options.ExpireTimeSpan  = TimeSpan.FromMinutes(configuration.GetValue<int>(CookieExpireTimeSpan));
        options.SlidingExpiration = configuration.GetValue<bool>(CookieSlidingExpiration);
        
        options.LoginPath = configuration.GetValue<string>(CookieLoginPath);
        options.LogoutPath = configuration.GetValue<string>(CookieLogoutPath);;
        options.AccessDeniedPath = configuration.GetValue<string>(CookieAccessDeniedPath);;
    }
    
    private static void ConfigureSession(SessionOptions options, ConfigurationManager configuration)
    {
        options.IdleTimeout = TimeSpan.FromMinutes(configuration.GetValue<int>(IdleTimeoutMinutes));
        options.Cookie.HttpOnly = configuration.GetValue<bool>(SessionCookieHttpOnly);
        options.Cookie.IsEssential = configuration.GetValue<bool>(SessionCookieIsEssential);
    }
}