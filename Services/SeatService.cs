using EventParkingReservationSystem.API.Exceptions;
using EventParkingReservationSystem.API.IRepositories;
using EventParkingReservationSystem.API.IServices;
using EventParkingReservationSystem.API.Models;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DTOs.Seats;
using WebApplication1.Enums;
using WebApplication1.Repositories;

namespace WebApplication1.Services;

//public class SeatService : ISeatService
//{
//    private readonly IUnitOfWork _uow;
//    public SeatService(IUnitOfWork uow) => _uow = uow;
//    public async Task<IReadOnlyList<SeatResponseDto>> GetMapAsync(int eventId)
//    {
//        var evt = await _uow.Events.GetByIdAsync(eventId)
//            ?? throw new ApiException(404, "Event not found.");

//        return await _uow.Seats.Query().AsNoTracking()
//            .Where(x => x.EventId == eventId)
//            .OrderBy(x => x.SeatNumber)
//            .Select(x => new SeatResponseDto(
//                x.SeatId, x.SeatNumber, x.SeatType,
//                x.Price ?? evt.TicketPrice, x.Status.ToString()))
//            .ToListAsync();
//    }
//    public async Task<IReadOnlyList<SeatResponseDto>> CreateMapAsync(int eventId, CreateSeatMapDto dto)
//    {
//        var evt = await _uow.Events.GetByIdAsync(eventId)
//            ?? throw new ApiException(404, "Event not found.");

//        var count = dto.Rows * dto.Columns;
//        if (count != evt.Capacity)
//            throw new ApiException(400, $"Seat count must match event capacity ({evt.Capacity}).");

//        if (await _uow.Seats.Query().AnyAsync(x => x.EventId == eventId))
//            throw new ApiException(409, "A seat map already exists for this event.");

//        var seats = new List<Seat>(count);
//        for (var r = 0; r < dto.Rows; r++)
//        {
//            var rowLabel = r < 26 ? ((char)('A' + r)).ToString() : $"R{r + 1}";
//            for (var c = 1; c <= dto.Columns; c++)
//            {
//                seats.Add(new Seat
//                {
//                    EventId = eventId,
//                    SeatNumber = $"{rowLabel}{c}",
//                    SeatType = dto.SeatType,
//                    Price = dto.SeatPrice,
//                    Status = SeatStatus.Available
//                });
//            }
//        }

//        await _uow.Seats.AddRangeAsync(seats);
//        await _uow.SaveChangesAsync();
//        return await GetMapAsync(eventId);
//    }

//    public async Task<SeatResponseDto> UpdateAsync(int eventId, int seatId, UpdateSeatDto dto)
//    {
//        var seat = await _uow.Seats.Query()
//            .Include(x => x.Event)
//            .FirstOrDefaultAsync(x => x.SeatId == seatId && x.EventId == eventId)
//            ?? throw new ApiException(404, "Seat not found for this event.");

//        if (await HasActiveBookingAsync(seatId))
//            throw new ApiException(409, "Cannot edit a seat with an active booking.");

//        var number = dto.SeatNumber.Trim();
//        if (await _uow.Seats.Query().AnyAsync(x =>
//            x.EventId == eventId && x.SeatId != seatId && x.SeatNumber == number))
//            throw new ApiException(409, "Seat number already exists for this event.");

//        seat.SeatNumber = number;
//        seat.SeatType = dto.SeatType?.Trim();
//        seat.Price = dto.SeatPrice;

//        await _uow.SaveChangesAsync();
//        return new SeatResponseDto(seat.SeatId, seat.SeatNumber, seat.SeatType,
//            seat.Price ?? seat.Event.TicketPrice, seat.Status.ToString());
//    }

//    public async Task DeleteAsync(int eventId, int seatId)
//    {
//        var seat = await _uow.Seats.Query()
//            .FirstOrDefaultAsync(x => x.SeatId == seatId && x.EventId == eventId)
//            ?? throw new ApiException(404, "Seat not found for this event.");

//        if (await HasActiveBookingAsync(seatId))
//            throw new ApiException(409, "Cannot delete a seat with an active booking.");

//        _uow.Seats.Remove(seat);
//        await _uow.SaveChangesAsync();
//    }

//    private Task<bool> HasActiveBookingAsync(int seatId)
//        => _uow.BookingSeats.Query().AnyAsync(bs =>
//            bs.SeatId == seatId &&
//            (bs.Booking.Status == BookingStatus.Pending || bs.Booking.Status == BookingStatus.Confirmed));
//}




