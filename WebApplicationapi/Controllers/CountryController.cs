using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog;
using WebApplicationapi.Data;
using WebApplicationapi.IRepository;
using WebApplicationapi.Models;
using ILogger = Serilog.ILogger;

namespace WebApplicationapi.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class CountryController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CountryController> _logger;
    private readonly IMapper _mapper;

    public CountryController(IUnitOfWork unitOfWork, ILogger<CountryController> logger, IMapper mapper)
    {
      _unitOfWork = unitOfWork;
      _logger = logger;
      _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<CountryDTO>))]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetCountries([FromQuery] PagingRequestParams requestParams)
    {
      var countries = await _unitOfWork.Countries.GetPagedList(requestParams);

      IEnumerable<CountryDTO> result = _mapper.Map<IEnumerable<CountryDTO>>(countries);

      return Ok(result);
    }

    [HttpGet("{id:int}", Name = "GetCountry")]
    [ProducesResponseType(200, Type = typeof(CountryDTO))]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetCountry(int id)
    {
      Country country = await _unitOfWork.Countries.Get(
        c => c.Id == id,
        new List<string> {"Hotels"}
      );
      CountryDTO result = _mapper.Map<CountryDTO>(country);
      return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(201, Type = typeof(CountryDTO))]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> StoreCountry([FromBody] CreateCountryDTO createCountryDto)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var country = _mapper.Map<Country>(createCountryDto);
      await _unitOfWork.Countries.Insert(country);
      await _unitOfWork.Save();
      return CreatedAtRoute(
        "GetCountry",
        new {id = country.Id},
        _mapper.Map<CountryDTO>(country)
      );
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateCountry(int id, [FromBody] UpdateCountryDTO updateCountryDto)
    {
      if (!ModelState.IsValid || id < 1)
      {
        return BadRequest(ModelState);
      }

      var country = await _unitOfWork.Countries.Get(q => q.Id == id);

      if (country == null)
      {
        return BadRequest();
      }

      var updatedCountry = _mapper.Map(updateCountryDto, country);
      _unitOfWork.Countries.Update(updatedCountry);
      await _unitOfWork.Save();
      return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> DeleteCountry(int id)
    {
      if (id < 1)
      {
        return BadRequest();
      }

      var country = await _unitOfWork.Countries.Get(q => q.Id == id);
      if (country == null)
      {
        return NotFound();
      }

      await _unitOfWork.Countries.Delete(id);
      await _unitOfWork.Save();
      return NoContent();
    }
  }
}
