
using EventParkingReservationSystem.API.Data;
using EventParkingReservationSystem.API.IRepositories;
using EventParkingReservationSystem.API.Repositories;
using WebApplication1.Models;
namespace WebApplication1.Repositories;

public class EventCategoryRepository : Repository<EventCategory>, IEventCategoryRepository
{
    public EventCategoryRepository(ApplicationDbContext db) : base(db) { }
}

