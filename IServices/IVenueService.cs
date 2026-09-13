using EventParkingReservationSystem.API.DTOs.Venues;

namespace EventParkingReservationSystem.API.IServices;

public interface IVenueService
{
    Task<IReadOnlyList<VenueResponseDto>> GetAllAsync();
    Task<VenueResponseDto> GetAsync(int id);
    Task<IReadOnlyList<VenueResponseDto>> GetAvailableAsync(DateOnly date, TimeOnly startTime, TimeOnly endTime, int? venueId);
    Task<VenueResponseDto> CreateAsync(VenueDto dto);
    Task<VenueResponseDto> UpdateAsync(int id, VenueDto dto);
    Task DeleteAsync(int id);
}
