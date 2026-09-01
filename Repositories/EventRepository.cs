using EventParkingReservationSystem.API.Data;
using EventParkingReservationSystem.API.IRepositories;
using EventParkingReservationSystem.API.Models;
using EventParkingReservationSystem.API.Repositories;

namespace EventParkingReservationSystem.API.Repositories;
public class EventRepository : Repository<Event>, IEventRepository
{
    public EventRepository(ApplicationDbContext db) : base(db) { }
}