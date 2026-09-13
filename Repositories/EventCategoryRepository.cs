
using EventParkingReservationSystem.API.Data;
using EventParkingReservationSystem.API.IRepositories;
using EventParkingReservationSystem.API.Repositories;
using EventParkingReservationSystem.API.Models;
namespace EventParkingReservationSystem.API.Repositories;

public class EventCategoryRepository : Repository<EventCategory>, IEventCategoryRepository
{
    public EventCategoryRepository(ApplicationDbContext db) : base(db) { }
}

