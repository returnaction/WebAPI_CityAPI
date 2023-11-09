using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;


        public PointsOfInterestController(ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _cityInfoRepository = cityInfoRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
        {
            try
            {

                if(!await _cityInfoRepository.CityExistsAsync(cityId))
                    return NotFound();  

                var pointsOfInterests =  await _cityInfoRepository.GetPointsOfInterestForCityAsync(cityId);

                var pointsOfInterestsDto = _mapper.Map<List<PointOfInterestDto>>(pointsOfInterests);

                return Ok(pointsOfInterestsDto);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{pointofinterestid}", Name = "GetPointOfInterest")]
        public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(int cityId, int pointOfInterestId)
        {
            try
            {
                if (!await _cityInfoRepository.CityExistsAsync(cityId))
                    return NotFound();

                var pointOfInterest = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

                if (pointOfInterest is null)
                    return NotFound();

                var pointOfInterestDto = _mapper.Map<PointOfInterestDto>(pointOfInterest);

                return Ok(pointOfInterestDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

           
        }

        [HttpPost]
        public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(
            int cityId, 
            PointOfInterestForCreationDto pointOfInterest)
        {

            try
            {
                if (!await _cityInfoRepository.CityExistsAsync(cityId))
                    return NotFound();

                var finalPoitOfInterest = _mapper.Map<PointOfInterest>(pointOfInterest);

                await _cityInfoRepository.AddPointOfInterestForCityAsync(cityId, finalPoitOfInterest);

                await _cityInfoRepository.SaveChangesAsync();

                var finalPointOfInterestDto = _mapper.Map<PointOfInterestDto>(finalPoitOfInterest);

                return StatusCode(201, finalPointOfInterestDto);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }

        [HttpPut("{pointofinterestid}")]
        public async Task<ActionResult> UpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterest)
        {
            try
            {
                if (!await _cityInfoRepository.CityExistsAsync(cityId))
                    return NotFound();

                if (pointOfInterest is null)
                    return BadRequest();

                var pointOfInterestExist = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
                if (pointOfInterestExist is null)
                    return NotFound();

                _mapper.Map(pointOfInterest, pointOfInterestExist);

                await _cityInfoRepository.SaveChangesAsync();


                return StatusCode(204);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }

        [HttpPatch("{pointOfInterestId}")]
        public async Task<ActionResult> PartiallyUpdatePointOfInterest(
            int cityId, 
            int pointOfInterestId, 
            JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        {

            try
            {
                if (!await _cityInfoRepository.CityExistsAsync(cityId))
                    return NotFound();

                if (patchDocument is null)
                    return NotFound();

                var pointOfInterestExist = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

                if (pointOfInterestExist is null)
                    return NotFound();

                var pointOfInterestToPatch = _mapper.Map<PointOfInterestForUpdateDto>(pointOfInterestExist);

                patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

                if (!ModelState.IsValid)
                    return BadRequest();

                _mapper.Map(pointOfInterestToPatch, pointOfInterestExist);

                await _cityInfoRepository.SaveChangesAsync();

                return StatusCode(204);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{pointOfInterestId}")]
        public async Task<ActionResult> DeletePointOfInterest(int cityId, int pointOfInterestId)
        {
            try
            {
                if (!await _cityInfoRepository.CityExistsAsync(cityId))
                    return NotFound();

                var pointofInterestToDelete = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
                if (pointofInterestToDelete is null)
                    return NotFound();

                _cityInfoRepository.DeletePointOfInterestForCityAsync(pointofInterestToDelete);

                await _cityInfoRepository.SaveChangesAsync();

                return StatusCode(202);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
                throw;
            }
        }
    }
}
