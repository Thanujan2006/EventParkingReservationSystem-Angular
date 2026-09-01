using EventParkingReservationSystem.API.DTOs.Customers;
using EventParkingReservationSystem.API.IRepositories;
using EventParkingReservationSystem.API.IServices;
using Microsoft.EntityFrameworkCore;

using EventParkingReservationSystem.API.Enums;
using EventParkingReservationSystem.API.Exceptions;
using EventParkingReservationSystem.API.Helpers;
using EventParkingReservationSystem.API.Models;
using EventParkingReservationSystem.API.Repositories;

namespace EventParkingReservationSystem.API.Services;

public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _uow;
    private readonly IPasswordService _passwords;
    private readonly ITokenService _tokens;
    private readonly IEmailService _email;
    private readonly IConfiguration _config;

    public CustomerService(
        IUnitOfWork uow,
        IPasswordService passwords,
        ITokenService tokens,
        IEmailService email,
        IConfiguration config)
    {
        _uow = uow;
        _passwords = passwords;
        _tokens = tokens;
        _email = email;
        _config = config;
    }

    public async Task<CustomerResponseDto> RegisterAsync(RegisterCustomerDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        if (await _uow.Customers.Query().AnyAsync(x => x.Email == email))
            throw new ApiException(409, "Email already registered. Please login or reset your password.");

        var rawToken = _tokens.CreateSecureToken();
        var customer = new Customer
        {
            Name = dto.Name.Trim(),
            Email = email,
            Phone = dto.Phone.Trim(),
            PasswordHash = _passwords.Hash(dto.Password),
            Status = CustomerStatus.Active,
            EmailVerified = false,
            EmailVerificationTokenHash = _tokens.HashToken(rawToken),
            EmailVerificationTokenExpiresAtUtc = DateTime.UtcNow.AddHours(
                _config.GetValue<int?>("TokenSettings:EmailVerificationHours") ?? 24)
        };

        await _uow.Customers.AddAsync(customer);
        await _uow.SaveChangesAsync();

        var frontend = _config["Frontend:BaseUrl"] ?? "http://localhost:5500";
        var link = $"{frontend}/verify.html?token={Uri.EscapeDataString(rawToken)}";
        await _email.SendAsync(customer.Email, "Verify your email",
            $"Welcome {customer.Name}!\nVerify your email using this one-time link:\n{link}");

        return Map(customer, 0);
    }

    public async Task<CustomerResponseDto> GetAsync(int id)
    {
        var customer = await _uow.Customers.Query()
            .Include(x => x.Bookings)
            .FirstOrDefaultAsync(x => x.CustomerId == id)
            ?? throw new ApiException(404, "Customer record not found.");

        return Map(customer, customer.Bookings.Count);
    }

    //public async Task<CustomerResponseDto> UpdateAsync(int id, UpdateCustomerDto dto)
    //{
    //    var customer = await _uow.Customers.GetByIdAsync(id)
    //                   ?? throw new ApiException(404, "Customer record not found.");

    //    customer.Name = dto.Name.Trim();
    //    customer.Phone = dto.Phone.Trim();
    //    customer.UpdatedAtUtc = DateTime.UtcNow;

    //    await _uow.SaveChangesAsync();
    //    var bookingCount = await _uow.Bookings.Query().CountAsync(x => x.CustomerId == id);
    //    return Map(customer, bookingCount);
    //}

    public async Task<IReadOnlyList<CustomerResponseDto>> SearchAsync(string? search)
    {
        var query = _uow.Customers.Query().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(x => x.Name.Contains(term) || x.Email.Contains(term));
        }

        return await query
            .OrderBy(x => x.Name)
            .Select(x => new CustomerResponseDto(
                x.CustomerId, x.Name, x.Email, x.Phone, x.Status.ToString(),
                x.EmailVerified, x.Bookings.Count, x.CreatedAtUtc))
            .ToListAsync();
    }

    //public async Task DeactivateAsync(int id)
    //{
    //    var customer = await _uow.Customers.GetByIdAsync(id)
    //        ?? throw new ApiException(404, "Customer record not found.");

    //    var today = DateOnly.FromDateTime(DateTime.UtcNow);
    //    var activeFuture = await _uow.Bookings.Query()
    //        .Include(x => x.Event)
    //        .CountAsync(x => x.CustomerId == id
    //            && (x.Status == BookingStatus.Pending || x.Status == BookingStatus.Confirmed)
    //            && x.Event.EventDate >= today);

    //    if (activeFuture > 0)
    //        throw new ApiException(400, $"Cannot deactivate: {activeFuture} active future booking(s) exist.");

    //    customer.Status = CustomerStatus.Deactivated;
    //    customer.UpdatedAtUtc = DateTime.UtcNow;
    //    await _uow.SaveChangesAsync();
    //}

    public async Task ReactivateAsync(int id)
    {
        var customer = await _uow.Customers.GetByIdAsync(id)
            ?? throw new ApiException(404, "Customer record not found.");

        customer.Status = CustomerStatus.Active;
        customer.UpdatedAtUtc = DateTime.UtcNow;
        await _uow.SaveChangesAsync();
    }

    private static CustomerResponseDto Map(Customer x, int bookingCount)
        => new(x.CustomerId, x.Name, x.Email, x.Phone, x.Status.ToString(),
            x.EmailVerified, bookingCount, x.CreatedAtUtc);
}