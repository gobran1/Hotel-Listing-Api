using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using WebApplicationapi.Data;
using WebApplicationapi.IRepository;
using WebApplicationapi.Models;

namespace WebApplicationapi.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class HotelController : Controller
  {
    private readonly ILogger<HotelController> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public HotelController(ILogger<HotelController> logger, IUnitOfWork unitOfWork, IMapper mapper)
    {
      _logger = logger;
      _unitOfWork = unitOfWork;
      _mapper = mapper;
    }

    [HttpGet]
    [ResponseCache(CacheProfileName = "120SecDuration")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<HotelDTO>))]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetHotels()
    {
      var hotels = await _unitOfWork.Hotels.GetAll();
      IEnumerable<HotelDTO> result = _mapper.Map<IEnumerable<HotelDTO>>(hotels);
      return Ok(result);
    }

    [HttpGet("{id:int}", Name = "GetHotel")]
    [ProducesResponseType(201, Type = typeof(HotelDTO))]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetHotel(int id)
    {
      var hotel = await _unitOfWork.Hotels.Get(
        h => h.Id == id,
        new List<string> {"Country"}
      );
      HotelDTO result = _mapper.Map<HotelDTO>(hotel);
      return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(201, Type = typeof(HotelDTO))]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> StoreHotel([FromBody] CreateHotelDTO createHotelDto)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var hotel = _mapper.Map<Hotel>(createHotelDto);
      await _unitOfWork.Hotels.Insert(hotel);
      await _unitOfWork.Save();
      return CreatedAtRoute(
        "GetHotel",
        new {id = hotel.Id},
        _mapper.Map<HotelDTO>(hotel)
      );
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateHotel(int id, [FromBody] UpdateHotelDTO updateHotelDto)
    {
      if (!ModelState.IsValid || id < 1)
      {
        _logger.LogError($"error in {nameof(UpdateHotel)} controller action");
        return BadRequest(ModelState);
      }

      var hotel = await _unitOfWork.Hotels.Get(q => q.Id == id);
      if (hotel == null)
      {
        return BadRequest();
      }

      var updatedHotel = _mapper.Map(updateHotelDto, hotel);
      _unitOfWork.Hotels.Update(updatedHotel);
      await _unitOfWork.Save();
      return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> DeleteHotel(int id)
    {
      if (id < 1)
      {
        return BadRequest();
      }

      var hotel = await _unitOfWork.Hotels.Get(q => q.Id == id);
      if (hotel == null)
      {
        return NotFound();
      }

      await _unitOfWork.Hotels.Delete(id);
      await _unitOfWork.Save();
      return NoContent();
    }
  }
}
