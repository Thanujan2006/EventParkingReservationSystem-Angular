using EventParkingReservationSystem.API.IServices;
using EventParkingReservationSystem.API.Services;

namespace EventParkingReservationSystem.API.BackgroundServices;

public class BookingExpiryService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _config;
    private readonly ILogger<BookingExpiryService> _logger;

    public BookingExpiryService(
        IServiceScopeFactory scopeFactory,
        IConfiguration config,
        ILogger<BookingExpiryService> logger)
    {
        _scopeFactory = scopeFactory;
        _config = config;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var seconds = Math.Max(10, _config.GetValue<int?>("BookingSettings:ExpiryScanIntervalSeconds") ?? 30);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
                var count = await bookingService.ExpirePendingBookingsAsync(stoppingToken);
                if (count > 0)
                    _logger.LogInformation("Expired {Count} pending booking(s).", count);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Booking expiry scan failed.");
            }

            await Task.Delay(TimeSpan.FromSeconds(seconds), stoppingToken);
        }
    }
}
