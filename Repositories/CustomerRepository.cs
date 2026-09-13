using EventParkingReservationSystem.API.Data;
using EventParkingReservationSystem.API.IRepositories;
using EventParkingReservationSystem.API.Repositories;
using EventParkingReservationSystem.API.Models;
namespace EventParkingReservationSystem.API.Repositories;

public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(ApplicationDbContext db) : base(db) { }
}
