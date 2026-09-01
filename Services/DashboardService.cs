using EventParkingReservationSystem.API.DTOs.Dashboard;
using EventParkingReservationSystem.API.Enums;
using EventParkingReservationSystem.API.IRepositories;
using EventParkingReservationSystem.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservationSystem.API.Services;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _uow;
    public DashboardService(IUnitOfWork uow) => _uow = uow;

    public async Task<CustomerDashboardDto> GetCustomerAsync(int customerId)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var upcoming = await _uow.Bookings.Query()
            .Include(x => x.Event)
            .CountAsync(x => x.CustomerId == customerId &&
                x.Status == BookingStatus.Confirmed &&
                x.Event.EventDate >= today);

        var parking = await _uow.ParkingReservations.Query()
            .CountAsync(x => x.Booking.CustomerId == customerId && x.IsActive &&
                x.Booking.Status == BookingStatus.Confirmed);

        var recentSince = DateTime.UtcNow.AddDays(-30);
        var payments = await _uow.Payments.Query()
            .CountAsync(x => x.Booking.CustomerId == customerId && x.PaidAtUtc >= recentSince);

        var unread = await _uow.Notifications.Query()
            .CountAsync(x => x.CustomerId == customerId && !x.IsRead);

        return new CustomerDashboardDto(upcoming, parking, payments, unread);
    }

    public async Task<AdminDashboardDto> GetAdminAsync()
    {
        var totalEvents = await _uow.Events.Query().CountAsync();
        var totalBookings = await _uow.Bookings.Query().CountAsync();
        var availableSeats = await _uow.Seats.Query().CountAsync(x => x.Status == SeatStatus.Available);
        var occupiedParking = await _uow.ParkingSlots.Query()
            .CountAsync(x => x.Status == ParkingSlotStatus.Held || x.Status == ParkingSlotStatus.Reserved);
        var revenue = await _uow.Payments.Query()
            .Where(x => x.Status == PaymentStatus.Completed)
            .SumAsync(x => (decimal?)x.Amount) ?? 0;
        var customers = await _uow.Customers.Query().CountAsync();

        return new AdminDashboardDto(
            totalEvents, totalBookings, availableSeats, occupiedParking, revenue, customers);
    }
}
