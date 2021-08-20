using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplicationapi.Configurations.Entities;

namespace WebApplicationapi.Data
{
  public class DatabaseContext : IdentityDbContext<ApiUser>
  {
    public DatabaseContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);
      modelBuilder.ApplyConfiguration(new CountryConfiguration());
      modelBuilder.ApplyConfiguration(new HotelConfiguration());
      modelBuilder.ApplyConfiguration(new RoleConfiguration());
    }

    public DbSet<Country> Countries { get; set; }

    public DbSet<Hotel> Hotels { get; set; }
  }
}
