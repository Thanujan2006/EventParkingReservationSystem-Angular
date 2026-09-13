using EventParkingReservationSystem.API.Data;
using EventParkingReservationSystem.API.IRepositories;
using EventParkingReservationSystem.API.Models;
using EventParkingReservationSystem.API.Repositories;
using EventParkingReservationSystem.API;
namespace EventParkingReservationSystem.API.Repositories;
public class ParkingSlotRepository : Repository<ParkingSlot>, IParkingSlotRepository
{
    public ParkingSlotRepository(ApplicationDbContext db) : base(db) { }
}
