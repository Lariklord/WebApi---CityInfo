using AutoMapper;
using CityInfo.Entities;
using CityInfo.Models;

namespace CityInfo
{

	public class MappingConfig
	{
		public static MapperConfiguration RegisterMaps()
		{
			var mappingConfig = new MapperConfiguration(config =>
			{
				config.CreateMap<City, CityWithoutPointsOfInterestDto>();
                config.CreateMap<CityWithoutPointsOfInterestDto,City>();
				config.CreateMap<City,CityDto>();
				config.CreateMap<CityDto, City>();
				config.CreateMap<PointOfInterestDto, PointOfInterest>();
				config.CreateMap<PointOfInterest, PointOfInterestDto>();
				config.CreateMap<PointOfInterestForCreationDto, PointOfInterest>();
				config.CreateMap<PointOfInterest,PointOfInterestForCreationDto>();
				config.CreateMap<PointOfInterestForUpdateDto, PointOfInterest>();
				config.CreateMap<PointOfInterest,PointOfInterestForUpdateDto>();
            });

			return mappingConfig;
		}
	}
}
