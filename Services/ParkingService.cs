using EventParkingReservationSystem.API.Exceptions;
using EventParkingReservationSystem.API.IRepositories;
using EventParkingReservationSystem.API.IServices;
using EventParkingReservationSystem.API.Models;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DTOs.Parking;
using WebApplication1.Enums;
using WebApplication1.Repositories;

namespace WebApplication1.Services;

//public class ParkingService : IParkingService
//{
//    private readonly IUnitOfWork _uow;
//    public ParkingService(IUnitOfWork uow) => _uow = uow;

//    public async Task<IReadOnlyList<ParkingSlotResponseDto>> GetLayoutAsync(int eventId)
//    {
//        var evt = await _uow.Events.GetByIdAsync(eventId)
//            ?? throw new ApiException(404, "Event not found.");

//        return await _uow.ParkingSlots.Query().AsNoTracking()
//            .Where(x => x.EventId == eventId)
//            .OrderBy(x => x.SlotNumber)
//            .Select(x => new ParkingSlotResponseDto(
//                x.ParkingSlotId, x.SlotNumber, x.Zone, x.Status.ToString(), evt.ParkingFee))
//            .ToListAsync();
//    }

//    public async Task<IReadOnlyList<ParkingSlotResponseDto>> CreateLayoutAsync(int eventId, CreateParkingLayoutDto dto)
//    {
//        var evt = await _uow.Events.GetByIdAsync(eventId)
//            ?? throw new ApiException(404, "Event not found.");

//        if (await _uow.ParkingSlots.Query().AnyAsync(x => x.EventId == eventId))
//            throw new ApiException(409, "A parking layout already exists for this event.");

//        evt.ParkingFee = dto.Fee;

//        var slots = Enumerable.Range(1, dto.SlotCount)
//            .Select(i => new ParkingSlot
//            {
//                EventId = eventId,
//                SlotNumber = $"{dto.Prefix.Trim()}{i}",
//                Zone = dto.Zone?.Trim(),
//                Status = ParkingSlotStatus.Available
//            })
//            .ToList();

//        await _uow.ParkingSlots.AddRangeAsync(slots);
//        await _uow.SaveChangesAsync();
//        return await GetLayoutAsync(eventId);
//    }

//    public async Task<ParkingSlotResponseDto> UpdateAsync(int eventId, int slotId, UpdateParkingSlotDto dto)
//    {
//        var slot = await _uow.ParkingSlots.Query()
//            .Include(x => x.Event)
//            .FirstOrDefaultAsync(x => x.ParkingSlotId == slotId && x.EventId == eventId)
//            ?? throw new ApiException(404, "Parking slot not found for this event.");

//        if (await _uow.ParkingReservations.Query()
//            .AnyAsync(x => x.ParkingSlotId == slotId && x.IsActive))
//            throw new ApiException(409, "Cannot edit a parking slot with an active reservation.");

//        var number = dto.SlotNumber.Trim();
//        if (await _uow.ParkingSlots.Query().AnyAsync(x =>
//            x.EventId == eventId && x.ParkingSlotId != slotId && x.SlotNumber == number))
//            throw new ApiException(409, "Parking slot number already exists for this event.");

//        slot.SlotNumber = number;
//        slot.Zone = dto.Zone?.Trim();
//        if (dto.EventParkingFee.HasValue)
//            slot.Event.ParkingFee = dto.EventParkingFee.Value;

//        await _uow.SaveChangesAsync();
//        return new ParkingSlotResponseDto(
//            slot.ParkingSlotId, slot.SlotNumber, slot.Zone, slot.Status.ToString(), slot.Event.ParkingFee);
//    }

//    public async Task DeleteAsync(int eventId, int slotId)
//    {
//        var slot = await _uow.ParkingSlots.Query()
//            .FirstOrDefaultAsync(x => x.ParkingSlotId == slotId && x.EventId == eventId)
//            ?? throw new ApiException(404, "Parking slot not found for this event.");

//        if (await _uow.ParkingReservations.Query()
//            .AnyAsync(x => x.ParkingSlotId == slotId && x.IsActive))
//            throw new ApiException(409, "Cannot remove a slot with an active reservation.");

//        _uow.ParkingSlots.Remove(slot);
//        await _uow.SaveChangesAsync();
//    }


//}
