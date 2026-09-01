using EventParkingReservationSystem.API.Data;
using EventParkingReservationSystem.API.IRepositories;
using EventParkingReservationSystem.API.Models;
using EventParkingReservationSystem.API.Repositories;
namespace WebApplication1.Repositories;
public class SeatRepository : Repository<Seat>, ISeatRepository
{
    public SeatRepository(ApplicationDbContext db) : base(db) { }
}
