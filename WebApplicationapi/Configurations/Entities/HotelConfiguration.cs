using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplicationapi.Data;

namespace WebApplicationapi.Configurations.Entities
{
  public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
  {
    public void Configure(EntityTypeBuilder<Hotel> builder)
    {
      builder.HasData(
        new Hotel
        {
          Id = 1,
          Name = "hotel name1",
          Address = "hotel address1",
          Rate = 4.0,
          CountryId = 1
        },
        new Hotel
        {
          Id = 2,
          Name = "hotel name2",
          Address = "hotel address2",
          Rate = 4.4,
          CountryId = 2
        },
        new Hotel
        {
          Id = 3,
          Name = "hotel name3",
          Address = "hotel address3",
          Rate = 3.5,
          CountryId = 2
        }
      );
    }
  }
}
