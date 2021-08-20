using AutoMapper;
using WebApplicationapi.Data;
using WebApplicationapi.Models;

namespace WebApplicationapi.Configurations
{
  public class MapperInitializer : Profile
  {
    public MapperInitializer()
    {
      CreateMap<Hotel,HotelDTO>().ReverseMap();
      CreateMap<Hotel,CreateHotelDTO>().ReverseMap();
      CreateMap<Country,CountryDTO>().ReverseMap();
      CreateMap<Country,CreateCountryDTO>().ReverseMap();
      CreateMap<ApiUser, UserDTO>().ReverseMap();
    }
  }
}
