using WebApplication1.DTOs.Parking;

namespace EventParkingReservationSystem.API.IServices;

public interface IParkingService
{
    Task<IReadOnlyList<ParkingSlotResponseDto>> GetLayoutAsync(int eventId);
    Task<IReadOnlyList<ParkingSlotResponseDto>> CreateLayoutAsync(int eventId, CreateParkingLayoutDto dto);
    Task<ParkingSlotResponseDto> UpdateAsync(int eventId, int slotId, UpdateParkingSlotDto dto);
    Task DeleteAsync(int eventId, int slotId);

}
