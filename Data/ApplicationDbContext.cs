
//using Microsoft.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore;
using EventParkingReservationSystem.API.Models;

namespace EventParkingReservationSystem.API.Data;

public class ApplicationDbContext :DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();
    public DbSet<Venue> Venues => Set<Venue>();
    public DbSet<EventCategory> EventCategories => Set<EventCategory>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Seat> Seats => Set<Seat>();
    public DbSet<ParkingSlot> ParkingSlots => Set<ParkingSlot>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingSeat> BookingSeats => Set<BookingSeat>();
    public DbSet<ParkingReservation> ParkingReservations => Set<ParkingReservation>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Notification> Notifications => Set<Notification>();

    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    base.OnModelCreating(modelBuilder);

    //    modelBuilder.Entity<Customer>()
    //        .HasIndex(x => x.Email)
    //        .IsUnique();

    //    modelBuilder.Entity<AdminUser>()
    //        .HasIndex(x => x.Email)
    //        .IsUnique();

    //    modelBuilder.Entity<EventCategory>()
    //        .HasIndex(x => x.Name)
    //        .IsUnique();

        //modelBuilder.Entity<Seat>()
        //    .HasIndex(x => new { x.EventId, x.SeatNumber })
        //    .IsUnique();

        //modelBuilder.Entity<ParkingSlot>()
        //    .HasIndex(x => new { x.EventId, x.SlotNumber })
        //    .IsUnique();

        //modelBuilder.Entity<Booking>()
        //    .HasIndex(x => x.BookingNumber)
        //    .IsUnique();

        //modelBuilder.Entity<BookingSeat>()
        //    .HasIndex(x => new { x.BookingId, x.SeatId })
        //    .IsUnique();

        //modelBuilder.Entity<Payment>()
        //    .HasIndex(x => x.BookingId)
        //    .IsUnique();

        //modelBuilder.Entity<Event>().Property(x => x.TicketPrice).HasPrecision(10, 2);
        //modelBuilder.Entity<Event>().Property(x => x.ParkingFee).HasPrecision(10, 2);
        //modelBuilder.Entity<Seat>().Property(x => x.Price).HasPrecision(10, 2);
        //modelBuilder.Entity<Booking>().Property(x => x.TotalAmount).HasPrecision(10, 2);
        //modelBuilder.Entity<ParkingReservation>().Property(x => x.FeeAtReservation).HasPrecision(10, 2);
        //modelBuilder.Entity<Payment>().Property(x => x.Amount).HasPrecision(10, 2);

        //modelBuilder.Entity<Booking>()
        //    .HasOne(x => x.Payment)
        //    .WithOne(x => x.Booking)
        //    .HasForeignKey<Payment>(x => x.BookingId)
        //    .OnDelete(DeleteBehavior.Cascade);

        //modelBuilder.Entity<BookingSeat>()
        //    .HasOne(x => x.Seat)
        //    .WithMany(x => x.BookingSeats)
        //    .HasForeignKey(x => x.SeatId)
        //    .OnDelete(DeleteBehavior.Restrict);

        //modelBuilder.Entity<ParkingReservation>()
        //    .HasOne(x => x.ParkingSlot)
        //    .WithMany(x => x.ParkingReservations)
        //    .HasForeignKey(x => x.ParkingSlotId)
        //    .OnDelete(DeleteBehavior.Restrict);

        //modelBuilder.Entity<Event>()
        //    .HasOne(x => x.Venue)
        //    .WithMany(x => x.Events)
        //    .HasForeignKey(x => x.VenueId)
        //    .OnDelete(DeleteBehavior.Restrict);

        //modelBuilder.Entity<Event>()
        //    .HasOne(x => x.EventCategory)
        //    .WithMany(x => x.Events)
        //    .HasForeignKey(x => x.EventCategoryId)
        //    .OnDelete(DeleteBehavior.Restrict);
    }



