using AutoMapper;
using CityInfo.Models;
using CityInfo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CityInfo.Controllers
{
    [ApiController]
    [Authorize]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CitiesController:ControllerBase
    {

        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;
        const int maxCitiesPageSize = 20;

        public CitiesController(ICityInfoRepository cityInfoRepository,IMapper mapper)
        {
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities(string? name, string? searchQuery, int pageNumber = 1, int pageSize = 10)
        {   
            if(pageSize>maxCitiesPageSize)
                pageSize= maxCitiesPageSize;

            var (cityEntities,metadata) = await _cityInfoRepository.GetCitiesAsync(name,searchQuery,pageNumber,pageSize);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));
            return Ok(_mapper.Map<List<CityWithoutPointsOfInterestDto>>(cityEntities));         
        }

        /// <summary>
        /// Get a city by id
        /// </summary>
        /// <param name="id">The id of the city to get</param>
        /// <param name="includePointsOfinterest">Whether or not to include the points of interest</param>
        /// <returns>An IActionResult</returns>
        /// <response code="200">Returns the requested city</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CityDto>> GetCity(int id, bool includePointsOfinterest = false) 
        {
           var cityEntity = await _cityInfoRepository.GetCityAsync(id,includePointsOfinterest);
            
            if(cityEntity is null)
                return NotFound();

            return Ok(_mapper.Map<CityDto>(cityEntity));
        }
    }
}
