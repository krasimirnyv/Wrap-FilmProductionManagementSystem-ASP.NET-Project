namespace FilmProductionManagementSystem.Web.Data;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class FilmProductionDbContext : IdentityDbContext
{
    public FilmProductionDbContext(DbContextOptions<FilmProductionDbContext> options)
        : base(options)
    {
    }
}