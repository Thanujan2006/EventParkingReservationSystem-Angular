using EventParkingReservationSystem.API.DTOs.Notifications;
using EventParkingReservationSystem.API.Enums;
using EventParkingReservationSystem.API.Exceptions;
using EventParkingReservationSystem.API.IRepositories;
using EventParkingReservationSystem.API.IServices;
using EventParkingReservationSystem.API.Models;
using EventParkingReservationSystem.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservationSystem.API.Services;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _uow;
    public NotificationService(IUnitOfWork uow) => _uow = uow;

    public async Task CreateAsync(int customerId, NotificationType type, string message)
    {
        await _uow.Notifications.AddAsync(new Notification
        {
            CustomerId = customerId,
            Type = type,
            Message = message,
            IsRead = false
        });
        await _uow.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<NotificationResponseDto>> GetForCustomerAsync(int customerId)
        => await _uow.Notifications.Query().AsNoTracking()
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new NotificationResponseDto(
                x.NotificationId, x.Type.ToString(), x.Message, x.IsRead, x.CreatedAtUtc))
            .ToListAsync();

    public async Task MarkReadAsync(int customerId, int notificationId)
    {
        var item = await _uow.Notifications.Query()
            .FirstOrDefaultAsync(x => x.NotificationId == notificationId && x.CustomerId == customerId)
            ?? throw new ApiException(404, "Notification not found.");

        item.IsRead = true;
        await _uow.SaveChangesAsync();
    }
}
