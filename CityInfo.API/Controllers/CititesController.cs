using AutoMapper;
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
    }
}
