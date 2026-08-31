using System.Data;
using EventParkingReservationSystem.API.Data;
using EventParkingReservationSystem.API.IRepositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace EventParkingReservationSystem.API.Repositories;

public class UnitOfWork:IUnitOfWork{
    private readonly ApplicationDbContext _db;

    public UnitOfWork(ApplicationDbContext db)
    {
        _db = db;
        Customers = new CustomerRepository(db);
        AdminUsers = new AdminUserRepository(db);
        Venues = new VenueRepository(db);
        EventCategories = new EventCategoryRepository(db);
        //Events = new EventRepository(db);
        //Seats = new SeatRepository(db);
        //ParkingSlots = new ParkingSlotRepository(db);
        //Bookings = new BookingRepository(db);
        //BookingSeats = new BookingSeatRepository(db);
        //ParkingReservations = new ParkingReservationRepository(db);
        //Payments = new PaymentRepository(db);
        //Notifications = new NotificationRepository(db);
    }

    public ICustomerRepository Customers { get; }
    public IAdminUserRepository AdminUsers { get; }
    public IVenueRepository Venues { get; }
    public IEventCategoryRepository EventCategories { get; }
    //public IEventRepository Events { get; }
    //public ISeatRepository Seats { get; }
    //public IParkingSlotRepository ParkingSlots { get; }
    //public IBookingRepository Bookings { get; }
    //public IBookingSeatRepository BookingSeats { get; }
    //public IParkingReservationRepository ParkingReservations { get; }
    //public IPaymentRepository Payments { get; }
    //public INotificationRepository Notifications { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _db.SaveChangesAsync(cancellationToken);

    public Task<IDbContextTransaction> BeginTransactionAsync(
        IsolationLevel isolationLevel = IsolationLevel.Serializable,
        CancellationToken cancellationToken = default)
        => _db.Database.BeginTransactionAsync(cancellationToken);
}
