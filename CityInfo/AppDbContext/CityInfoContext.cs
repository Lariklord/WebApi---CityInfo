using CityInfo.Entities;
using CityInfo.Models;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.AppDbContext
{
    public class CityInfoContext : DbContext
    {
        public DbSet<City> Cities { get; set; }
        public DbSet<PointOfInterest> PointOfInterests { get; set; }
        public CityInfoContext(DbContextOptions<CityInfoContext> options) : base(options) { }   
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>().HasData(
                    new City()
                    {
                        Id = 1,
                        Name = "New York City",
                        Description = "The one with that park.",
                    },
                    new City()
                    {
                        Id = 2,
                        Name = "Antwerp",
                        Description = "The one with the cathedral that was never really finished.",
                    },
                    new City()
                    {
                        Id = 3,
                        Name = "Paris",
                        Description = "The one with that big tower.",
                    }
                    );
            modelBuilder.Entity<PointOfInterest>().HasData(
                     new PointOfInterest()
                            {
                                Id = 1,
                                CityId = 1,
                                Name = "Central Park",
                                Description = "The most visited urban park in the United States"
                            },
                     new PointOfInterest()
                            {
                                Id = 2,
                                CityId = 1,
                                Name = "Empire State Building",
                                Description = "A 102-story skyscraper located in Midtown Manhattan"
                            },
                     new PointOfInterest()
                            {
                                Id = 3,
                                CityId = 2,
                                Name = "Cathedral of Our Lady",
                                Description = "A Gothic style cathedral, conceived by architects Jan and Piete."
                            },
                     new PointOfInterest()
                            {
                                Id = 4,
                                CityId = 2,
                                Name = "Antwerp Central Station",
                                Description = "The fines example of railway architecture in Belgium"
                            },
                     new PointOfInterest()
                            {
                                Id = 5,
                                CityId = 3,
                                Name = "Eiffel Tower",
                                Description = "A wrought iron lattice tower on the Champ de Mars."
                            },
                     new PointOfInterest()
                            {
                                Id = 6,
                                CityId = 3,
                                Name = "The Louvre",
                                Description = "The world's largest museum."
                            }
                     );

        }
    }


}
