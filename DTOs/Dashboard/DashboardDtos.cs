namespace EventParkingReservationSystem.API.DTOs.Dashboard;
public record CustomerDashboardDto(int UpcomingBookings, int ReservedParking, int RecentPayments, int UnreadNotifications);
public record AdminDashboardDto(int TotalEvents, int TotalBookings, int AvailableSeats, int OccupiedParkingSlots, decimal TotalRevenue, int TotalCustomers);
