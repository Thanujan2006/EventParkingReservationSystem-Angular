using System.Data;
using EventParkingReservationSystem.API.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace EventParkingReservationSystem.API.IRepositories;

public interface IUnitOfWork
{
    ICustomerRepository Customers { get; }
    IAdminUserRepository AdminUsers { get; }
    IVenueRepository Venues { get; }
    IEventCategoryRepository EventCategories { get; }
    IEventRepository Events { get; }
    ISeatRepository Seats { get; }
    IParkingSlotRepository ParkingSlots { get; }
    IBookingRepository Bookings { get; }
    IBookingSeatRepository BookingSeats { get; }
    IParkingReservationRepository ParkingReservations { get; }
    IPaymentRepository Payments { get; }
    INotificationRepository Notifications { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<IDbContextTransaction> BeginTransactionAsync(
        IsolationLevel isolationLevel = IsolationLevel.Serializable,
        CancellationToken cancellationToken = default);
}
