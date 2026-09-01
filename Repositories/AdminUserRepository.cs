
using EventParkingReservationSystem.API.Data;
using EventParkingReservationSystem.API.IRepositories;
using EventParkingReservationSystem.API.Repositories;
using EventParkingReservationSystem.API.Models;
namespace EventParkingReservationSystem.API.Repositories;

public class AdminUserRepository : Repository<AdminUser>, IAdminUserRepository
{
    public AdminUserRepository(ApplicationDbContext db) : base(db) { }
}
