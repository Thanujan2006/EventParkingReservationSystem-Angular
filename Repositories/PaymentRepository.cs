using EventParkingReservationSystem.API.Data;
using EventParkingReservationSystem.API.IRepositories;
using EventParkingReservationSystem.API.Models;
namespace EventParkingReservationSystem.API.Repositories;
public class PaymentRepository:Repository<Payment>, IPaymentRepository
{
    public PaymentRepository(ApplicationDbContext db):base(db)
    {
        
    }
}
