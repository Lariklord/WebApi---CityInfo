using AutoMapper;
using CityInfo.Entities;
using CityInfo.Models;
using CityInfo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.Controllers
{
    [ApiController]
    [Authorize(Policy = "MustBeFromAntwerp")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/cities/{cityId}/[controller]")]
    public class PointsOfInterestController : ControllerBase
    {
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, ICityInfoRepository cityInfoRepository,IMapper mapper) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
        {
            if(!await _cityInfoRepository.CityExistAsync(cityId))
            {
                _logger.LogInformation($"City with id {cityId} was not found.");
                return NotFound();
            }
           
            var pointsOfInterests = await _cityInfoRepository.GetPointsOfInterestsAsync(cityId);      

            return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterests));
        }

        [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointOfInterest(int cityId, int pointOfInterestId)
        {
            if (!await _cityInfoRepository.CityExistAsync(cityId))
            {
                _logger.LogInformation($"City with id {cityId} was not found.");
                return NotFound();
            }

            var pointOfInterest = await _cityInfoRepository.GetPointOfInterestAsync(cityId, pointOfInterestId);

            if(pointOfInterest is null)
                return NotFound();

            return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));
        }

        [HttpPost]
        public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(int cityId,[FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            if (!await _cityInfoRepository.CityExistAsync(cityId))
            {
                _logger.LogInformation($"City with id {cityId} was not found.");
                return NotFound();
            };

            var finalPointOfInterest = _mapper.Map<PointOfInterest>(pointOfInterest);

            await _cityInfoRepository.AddPointOfInterestForCityAsync(cityId, finalPointOfInterest);
            await _cityInfoRepository.SaveChangesAsync();

            var createdPointToReturn = _mapper.Map<PointOfInterestDto>(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest", new
            {
                cityId = cityId,
                pointOfInterestId = createdPointToReturn.Id
            },
            createdPointToReturn);
        }

        [HttpPut("{pointOfInterestId}")]
        public async Task<ActionResult> UpdatePointOfInterest(int cityId, int pointOfInterestId,
            PointOfInterestForUpdateDto pointOfInterest)
        {
            if (!await _cityInfoRepository.CityExistAsync(cityId))
            {
                _logger.LogInformation($"City with id {cityId} was not found.");
                return NotFound();
            };

            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestAsync(cityId,pointOfInterestId);

            if(pointOfInterestEntity == null)
                return NotFound();

            _mapper.Map(pointOfInterest, pointOfInterestEntity);

            await _cityInfoRepository.SaveChangesAsync();
            

            return NoContent();
        }

        [HttpPatch("{pointOfInterestId}")]
        public async Task<ActionResult> PartiallyUpdatePointOfInterest(
            int cityId, int pointOfInterestId,
            JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        {
            if (!await _cityInfoRepository.CityExistAsync(cityId))
            {
                _logger.LogInformation($"City with id {cityId} was not found.");
                return NotFound();
            };

            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestAsync(cityId, pointOfInterestId);

            if (pointOfInterestEntity == null)
                return NotFound();

            var pointToPatch =_mapper.Map<PointOfInterestForUpdateDto>(pointOfInterestEntity);

            patchDocument.ApplyTo(pointToPatch,ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if(!TryValidateModel(pointToPatch))
                return BadRequest(ModelState);

            _mapper.Map(pointToPatch,pointOfInterestEntity);

            await _cityInfoRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{pointOfInterestId}")]
        public async Task<ActionResult> DeletePointOfInterest(
           int cityId, int pointOfInterestId, [FromServices] IMailService mailService)
        {
            if (!await _cityInfoRepository.CityExistAsync(cityId))
            {
                _logger.LogInformation($"City with id {cityId} was not found.");
                return NotFound();
            };

            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestAsync(cityId, pointOfInterestId);

            if (pointOfInterestEntity == null)
                return NotFound();

            _cityInfoRepository.DeletePointOfInterestAsync(pointOfInterestEntity);
            await _cityInfoRepository.SaveChangesAsync();

            mailService.Send("Deleting point", $"The point with id {pointOfInterestEntity.Id} was deleted.");

            return NoContent();
        }
    }
}
