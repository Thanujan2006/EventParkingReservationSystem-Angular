using EventParkingReservationSystem.API.Enums;
using EventParkingReservationSystem.API.Helpers;
using EventParkingReservationSystem.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservationSystem.API.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        var passwords = services.GetRequiredService<IPasswordService>();

        if (!await db.AdminUsers.AnyAsync())
        {
            db.AdminUsers.Add(new AdminUser
            {
                Name = "System Administrator",
                Email = "admin@eventparking.local",
                PasswordHash = passwords.Hash("Admin@123"),
                IsActive = true
            });
        }

        if (!await db.Customers.AnyAsync())
        {
            db.Customers.Add(new Customer
            {
                Name = "Demo Customer",
                Email = "customer@eventparking.local",
                Phone = "0771234567",
                PasswordHash = passwords.Hash("Customer@123"),
                Status = CustomerStatus.Active,
                EmailVerified = true
            });
        }

        if (!await db.EventCategories.AnyAsync())
        {
            db.EventCategories.AddRange(
                new EventCategory { Name = "Concert" },
                new EventCategory { Name = "Sports" },
                new EventCategory { Name = "Conference" },
                new EventCategory { Name = "Workshop" });
        }

        if (!await db.Venues.AnyAsync())
        {
            db.Venues.Add(new Venue
            {
                Name = "Demo Convention Hall",
                Address = "Colombo, Sri Lanka",
                TotalCapacity = 100
            });
        }

        await db.SaveChangesAsync();

        if (!await db.Events.AnyAsync())
        {
            var venue = await db.Venues.FirstAsync();
            var category = await db.EventCategories.FirstAsync();

            var evt = new Event
            {
                Name = "Demo Tech Event",
                VenueId = venue.VenueId,
                EventCategoryId = category.EventCategoryId,
                EventDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)),
                StartTime = new TimeOnly(10, 0),
                EndTime = new TimeOnly(13, 0),
                TicketPrice = 1500m,
                Capacity = 20,
                ParkingFee = 300m
            };

            db.Events.Add(evt);
            await db.SaveChangesAsync();

            var seats = new List<Seat>();
            for (var r = 0; r < 4; r++)
            {
                var row = ((char)('A' + r)).ToString();
                for (var c = 1; c <= 5; c++)
                {
                    seats.Add(new Seat
                    {
                        EventId = evt.EventId,
                        SeatNumber = $"{row}{c}",
                        SeatType = "Standard",
                        Status = SeatStatus.Available
                    });
                }
            }
            db.Seats.AddRange(seats);

            db.ParkingSlots.AddRange(
                Enumerable.Range(1, 10).Select(i => new ParkingSlot
                {
                    EventId = evt.EventId,
                    SlotNumber = $"P{i}",
                    Zone = "A",
                    Status = ParkingSlotStatus.Available
                }));

            await db.SaveChangesAsync();
        }
    }
}
