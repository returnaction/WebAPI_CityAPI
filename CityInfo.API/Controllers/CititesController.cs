using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection.Metadata.Ecma335;

namespace CityInfo.API.Controllers
{
   
    [ApiController]
    [Route("api/cities")]
    public class CititesController : ControllerBase
    {

        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public CititesController(ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities()
        {
            var cityEntities = await _cityInfoRepository.GetCitiesAsync();
            return Ok(_mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities));

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CityDto>> GetCity(int id, bool includePointsOfInterest = false)
        {
            var cityEntity = await _cityInfoRepository.GetCityAsync(id, includePointsOfInterest);

            if (cityEntity is null)
                return NotFound();
            
            if (includePointsOfInterest)
            {
                return Ok(_mapper.Map<CityDto>(cityEntity));
            }

            return Ok(_mapper.Map<CityWithoutPointsOfInterestDto>(cityEntity));
            
        }

        [HttpPost]
        public async Task<ActionResult> AddCity(CityForCreatingDto newCity)
        {
            try
            {
                if (newCity is null)
                    return BadRequest();

                var city = _mapper.Map<City>(newCity);

                await _cityInfoRepository.AddCityAsync(city);
                await _cityInfoRepository.SaveChangesAsync();

                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{cityid}")]
        public async Task<ActionResult> DeleteCity(int cityId)
        {
            try
            {
                if(!await _cityInfoRepository.CityExistsAsync(cityId))
                    return NotFound();


                var city = await _cityInfoRepository.GetCityAsync(cityId, false);

                _cityInfoRepository.DeleteCityAsync(city);

                await _cityInfoRepository.SaveChangesAsync();

                return StatusCode(201);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
                throw;
            }
        }
    }
}
