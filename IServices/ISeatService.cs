using EventParkingReservationSystem.API.DTOs.Seats;

namespace EventParkingReservationSystem.API.IServices;

public interface ISeatService
{
    Task<IReadOnlyList<SeatResponseDto>> GetMapAsync(int eventId);
    Task<IReadOnlyList<SeatResponseDto>> CreateMapAsync(int eventId, CreateSeatMapDto dto);
    Task<SeatResponseDto> UpdateAsync(int eventId, int seatId, UpdateSeatDto dto);
    Task DeleteAsync(int eventId, int seatId);
}
 
