using EventParkingReservationSystem.API.Data;
using EventParkingReservationSystem.API.IRepositories;
using EventParkingReservationSystem.API.Models;
using EventParkingReservationSystem.API.Repositories;
namespace EventParkingReservationSystem.API.Repositories;
public class BookingSeatRepository : Repository <BookingSeat>, IBookingSeatRepository
{
    public BookingSeatRepository(ApplicationDbContext db) : base(db) { }

}
