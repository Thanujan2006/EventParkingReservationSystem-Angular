using EventParkingReservationSystem.API.Data;
using EventParkingReservationSystem.API.IRepositories;
using EventParkingReservationSystem.API.Repositories;
using EventParkingReservationSystem.API.Models;
namespace EventParkingReservationSystem.API.Repositories;

public class VenueRepository : Repository<Venue>, IVenueRepository
{
    public VenueRepository(ApplicationDbContext db) : base(db) { }
}

