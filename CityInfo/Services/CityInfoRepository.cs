using CityInfo.AppDbContext;
using CityInfo.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private readonly CityInfoContext _context;

        public CityInfoRepository(CityInfoContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            return await  _context.Cities.OrderBy(x=>x.Name).ToListAsync();
        }

        public async Task<City?> GetCityAsync(int cityId, bool includePointsOfinterest)
        {
            if(includePointsOfinterest)
            {
                return await _context.Cities.Include(x=>x.PointsOfInterest).FirstOrDefaultAsync(x => x.Id == cityId);
            }
            return await _context.Cities.FirstOrDefaultAsync(x => x.Id == cityId);

        }

        public async Task<PointOfInterest?> GetPointOfInterestAsync(int cityId, int pointOfInterestId)
        {
            return await _context.PointOfInterests.Where(x => x.CityId == cityId && x.Id == pointOfInterestId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestsAsync(int cityId)
        {
            return await _context.PointOfInterests.Where(x=>x.CityId== cityId).ToListAsync();
        }

        public async Task<bool> CityExistAsync(int cityId)
        {
            return await _context.Cities.AnyAsync(x=>x.Id==cityId);
        }

        public async Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest)
        {
            var city = await GetCityAsync(cityId,false);
            if(city is not null)
            {
                city.PointsOfInterest!.Add(pointOfInterest);
                
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync()>=0;
        }
        public void DeletePointOfInterestAsync(PointOfInterest pointOfInterest)
        {
            _context.PointOfInterests.Remove(pointOfInterest);
        }

        public async Task<(IEnumerable<City>,PaginationMetadata)> GetCitiesAsync(string? name, string? searchQuery, int pageNumber, int pageSize)
        {
        
            var collection = _context.Cities as IQueryable<City>;

            if (!string.IsNullOrEmpty(name))
            {
                name = name.Trim();
                collection = collection.Where(x => x.Name == name);
            }

            if (!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(x => x.Name.Contains(searchQuery) || (x.Description!=null && x.Description.Contains(searchQuery)) );
            }

            var totalItemCount = await collection.CountAsync();

            var paginationMetadata = new PaginationMetadata(
                totalItemCount, pageSize, pageNumber);

            var collectionToReturn = await collection.OrderBy(x => x.Name)
                .Skip(pageSize*(pageNumber-1))
                .Take(pageSize)
                .ToListAsync();

            return (collectionToReturn,paginationMetadata);






        }
    }
}
