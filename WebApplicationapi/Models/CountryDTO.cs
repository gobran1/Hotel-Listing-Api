using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebApplicationapi.Data;

namespace WebApplicationapi.Models
{
  public class CreateCountryDTO
  {
    [Required]
    [StringLength(maximumLength: 50, ErrorMessage = "Country name is too long")]
    public string Name { get; set; }

    [Required]
    [StringLength(maximumLength: 2, MinimumLength = 2,
      ErrorMessage = "country short name should exactly from 2 characters")]
    public string ShortName { get; set; }
  }

  public class CountryDTO : CreateCountryDTO
  {
    public int Id { get; set; }

    public IList<HotelDTO> Hotels { get; set; }
  }

  public class UpdateCountryDTO : CreateCountryDTO
  {
    public IList<CreateHotelDTO> Hotels { get; set; }
  }
}
