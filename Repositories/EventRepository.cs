using EventParkingReservationSystem.API.Data;
using EventParkingReservationSystem.API.IRepositories;
using EventParkingReservationSystem.API.Models;
using EventParkingReservationSystem.API.Repositories;

namespace WebApplication1.Repositories;
public class EventRepository : Repository<Event>, IEventRepository
{
    public EventRepository(ApplicationDbContext db) : base(db) { }
}