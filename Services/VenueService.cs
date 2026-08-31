using EventParkingReservationSystem.API.IRepositories;
using EventParkingReservationSystem.API.IServices;
using Microsoft.EntityFrameworkCore;
using EventParkingReservationSystem.API.DTOs.Venues;
using EventParkingReservationSystem.API.Exceptions;
using EventParkingReservationSystem.API.Models;
using EventParkingReservationSystem.API.Repositories;

namespace EventParkingReservationSystem.API.Services;

public class VenueService : IVenueService
{
    private readonly IUnitOfWork _uow;
    public VenueService(IUnitOfWork uow) => _uow = uow;

    public async Task<IReadOnlyList<VenueResponseDto>> GetAllAsync()
        => await _uow.Venues.Query().AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new VenueResponseDto(x.VenueId, x.Name, x.Address, x.TotalCapacity))
            .ToListAsync();

    public async Task<VenueResponseDto> GetAsync(int id)
    {
        var x = await _uow.Venues.GetByIdAsync(id)
            ?? throw new ApiException(404, "Venue not found.");
        return Map(x);
    }

    //public async Task<IReadOnlyList<VenueResponseDto>> GetAvailableAsync(
    //    DateOnly date, TimeOnly startTime, TimeOnly endTime, int? venueId)
    //{
    //    if (startTime >= endTime)
    //        throw new ApiException(400, "End time must be after start time.");

    //    var venueQuery = _uow.Venues.Query().AsNoTracking();
    //    if (venueId.HasValue)
    //        venueQuery = venueQuery.Where(x => x.VenueId == venueId.Value);

    //    var busyVenueIds = _uow.Events.Query()
    //        .Where(x => x.EventDate == date && x.StartTime < endTime && x.EndTime > startTime)
    //        .Select(x => x.VenueId);

    //    return await venueQuery
    //        .Where(x => !busyVenueIds.Contains(x.VenueId))
    //        .OrderBy(x => x.Name)
    //        .Select(x => new VenueResponseDto(x.VenueId, x.Name, x.Address, x.TotalCapacity))
    //        .ToListAsync();
    //}

    public async Task<VenueResponseDto> CreateAsync(VenueDto dto)
    {
        var venue = new Venue
        {
            Name = dto.Name.Trim(),
            Address = dto.Address.Trim(),
            TotalCapacity = dto.TotalCapacity
        };
        await _uow.Venues.AddAsync(venue);
        await _uow.SaveChangesAsync();
        return Map(venue);
    }

    //public async Task<VenueResponseDto> UpdateAsync(int id, VenueDto dto)
    //{
    //    var venue = await _uow.Venues.GetByIdAsync(id)
    //        ?? throw new ApiException(404, "Venue not found.");

    //    var maxEventCapacity = await _uow.Events.Query()
    //        .Where(x => x.VenueId == id)
    //        .Select(x => (int?)x.Capacity)
    //        .MaxAsync() ?? 0;

    //    if (dto.TotalCapacity < maxEventCapacity)
    //        throw new ApiException(400, $"Venue capacity cannot be below existing event capacity ({maxEventCapacity}).");

    //    venue.Name = dto.Name.Trim();
    //    venue.Address = dto.Address.Trim();
    //    venue.TotalCapacity = dto.TotalCapacity;
    //    await _uow.SaveChangesAsync();
    //    return Map(venue);
    //}

    //public async Task DeleteAsync(int id)
    //{
    //    var venue = await _uow.Venues.GetByIdAsync(id)
    //        ?? throw new ApiException(404, "Venue not found.");

    //    var today = DateOnly.FromDateTime(DateTime.UtcNow);
    //    if (await _uow.Events.Query().AnyAsync(x => x.VenueId == id && x.EventDate >= today))
    //        throw new ApiException(409, "This venue has upcoming events and cannot be deleted.");

    //    _uow.Venues.Remove(venue);
    //    await _uow.SaveChangesAsync();
    //}

    private static VenueResponseDto Map(Venue x)
        => new(x.VenueId, x.Name, x.Address, x.TotalCapacity);
}

