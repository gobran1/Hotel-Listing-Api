using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplicationapi.Data;

namespace WebApplicationapi.Configurations.Entities
{
  public class CountryConfiguration : IEntityTypeConfiguration<Country>
  {
    public void Configure(EntityTypeBuilder<Country> builder)
    {
      builder.HasData(
        new Country
        {
          Id = 1,
          Name = "Virginia",
          ShortName = "VA"
        },
        new Country
        {
          Id = 2,
          Name = "newYork",
          ShortName = "NY"
        },
        new Country{
         Id = 3,
          Name = "Los Angeles",
          ShortName = "LA"
        }
      );
    }
  }
}
