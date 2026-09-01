using EventParkingReservationSystem.API.Data;
using EventParkingReservationSystem.API.IRepositories;
using EventParkingReservationSystem.API.Models;
using EventParkingReservationSystem.API.Repositories;
using WebApplication1;
namespace WebApplication1.Repositories;
public class ParkingSlotRepository : Repository<ParkingSlot>, IParkingSlotRepository
{
    public ParkingSlotRepository(ApplicationDbContext db) : base(db) { }
}
