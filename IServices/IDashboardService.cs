using EventParkingReservationSystem.API.DTOs.Dashboard;

namespace EventParkingReservationSystem.API.Services;

public interface IDashboardService
{
    Task<CustomerDashboardDto> GetCustomerAsync(int customerId);
    Task<AdminDashboardDto> GetAdminAsync();
}
