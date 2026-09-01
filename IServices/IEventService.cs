using EventParkingReservationSystem.API.DTOs.Events;

namespace EventParkingReservationSystem.API.IServices;

public interface IEventService
{
    Task<IReadOnlyList<EventResponseDto>> GetAllAsync(string? name, DateOnly? date, int? venueId, int? categoryId);
    Task<EventResponseDto> GetAsync(int id);
    Task<EventResponseDto> CreateAsync(EventUpsertDto dto);
    Task<EventResponseDto> UpdateAsync(int id, EventUpsertDto dto);
    Task DeleteAsync(int id);

}
