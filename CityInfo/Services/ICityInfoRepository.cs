using CityInfo.Entities;

namespace CityInfo.Services
{
    public interface ICityInfoRepository
    {
        Task<IEnumerable<City>> GetCitiesAsync();
        Task<(IEnumerable<City>,PaginationMetadata)> GetCitiesAsync(string? name,string? searchQuery, int pageNumber, int pageSize);
        Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest);
        Task<IEnumerable<PointOfInterest>> GetPointsOfInterestsAsync(int cityId);
        Task<PointOfInterest?> GetPointOfInterestAsync(int cityId, int pointOfInterestId );
        Task<bool> CityExistAsync(int cityId);
        Task AddPointOfInterestForCityAsync(int cityId,PointOfInterest pointOfInterest);
        Task<bool> SaveChangesAsync();
        void DeletePointOfInterestAsync(PointOfInterest pointOfInterest);
    }
}
