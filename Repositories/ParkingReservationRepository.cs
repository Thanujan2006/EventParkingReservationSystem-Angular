using EventParkingReservationSystem.API.Data;
using EventParkingReservationSystem.API.IRepositories;
using EventParkingReservationSystem.API.Models;
using EventParkingReservationSystem.API.Repositories;
namespace EventParkingReservationSystem.API.Repositories;
public class ParkingReservationRepository : Repository<ParkingReservation>, IParkingReservationRepository
{
   public ParkingReservationRepository(ApplicationDbContext db) : base(db) { }
}
