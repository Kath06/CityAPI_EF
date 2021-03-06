﻿using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
	[ApiController]
	[Route("api/cities")]
	public class CitiesController : ControllerBase
	{
		private readonly ICityInfoRepository _cityInfoRepository;
		private readonly IMapper _mapper;

		public CitiesController(ICityInfoRepository cityInfoRepository,
			IMapper mapper)
		{
			_cityInfoRepository = cityInfoRepository ??
				throw new ArgumentNullException(nameof(cityInfoRepository));
			_mapper = mapper ??
				throw new ArgumentNullException(nameof(mapper));
		}

		[HttpGet(Name = "GetCities")]
		public IActionResult GetCities()
		{
			var cityEntities = _cityInfoRepository.GetCities();

			//var results = new List<CityWithoutPointsOfInterestDto>();

			//foreach (var cityEntity in cityEntities)
			//{
			//    results.Add(new CityWithoutPointsOfInterestDto
			//    {
			//        Id = cityEntity.Id,
			//        Description = cityEntity.Description,
			//        Name = cityEntity.Name
			//    });
			//}

			return Ok(_mapper.Map<IEnumerable<CityDto>>(cityEntities));
		}

		[HttpGet("{id}")]
		public IActionResult GetCity(int id, bool includePointsOfInterest = false)
		{
			var city = _cityInfoRepository.GetCity(id, includePointsOfInterest);

			if (city == null)
			{
				return NotFound();
			}

			if (includePointsOfInterest)
			{
				return Ok(_mapper.Map<CityDto>(city));
			}

			return Ok(_mapper.Map<CityDetailsDto>(city));
		}

		[HttpPost]
		public IActionResult CreateCity([FromBody] CityDetailsDto cityDTO)
		{
			try
			{
				var entityCity = _mapper.Map<City>(cityDTO);
				_cityInfoRepository.CreateCity(entityCity);
				_cityInfoRepository.Save();
				return Ok();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}

		}

		[Route("CreateCityMayor")]
		[HttpPost]
		public IActionResult CreateCityMayor([FromBody] CityMayorDTO cityMayorDTO)
		{
			try
			{
				var entityCity = _mapper.Map<City>(cityMayorDTO);
				_cityInfoRepository.CreateCity(entityCity);
				_cityInfoRepository.Save();
				return Ok();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}

		}

		[HttpPut("{cityID}")]
		public IActionResult UpdateCity(int cityID, [FromBody] CityDetailsDto cityDTO)
		{
			try
			{
				var entityCity = _cityInfoRepository.GetCity(cityID, false);
				if (entityCity == null)
				{
					return NotFound();
				}
				_mapper.Map(cityDTO, entityCity);
				_cityInfoRepository.UpdateCity(cityID, entityCity);
				_cityInfoRepository.Save();
				return Ok();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpDelete("{cityID}")]
		public IActionResult DeleteCity(int cityID)
		{
			try
			{
				_cityInfoRepository.DeleteCity(cityID);
				_cityInfoRepository.Save();
				return Ok();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}
