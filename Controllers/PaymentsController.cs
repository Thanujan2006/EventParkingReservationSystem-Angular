using EventParkingReservationSystem.API.DTOs.Payments;
using EventParkingReservationSystem.API.Exceptions;
using EventParkingReservationSystem.API.Helpers;
using EventParkingReservationSystem.API.IServices;
using EventParkingReservationSystem.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace EventParkingReservationSystem.API.Controllers;

[ApiController]
[Authorize]
[Route("api")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _payments;
    private readonly IBookingService _bookings;

    public PaymentsController(IPaymentService payments, IBookingService bookings)
    {
        _payments = payments;
        _bookings = bookings;
    }

    [Authorize(Roles = "Customer,Administrator")]
    [HttpGet("bookings/{id:int}/payment")]
    public async Task<ActionResult<PaymentSummaryDto>> Summary(int id)
    {
        var booking = await _bookings.GetAsync(id);
        EnsureOwnerOrAdmin(booking.CustomerId);
        return Ok(await _payments.GetSummaryAsync(id));
    }

    [Authorize(Roles = "Customer")]
    [HttpPost("bookings/{id:int}/payment")]
    public async Task<ActionResult<PaymentResponseDto>> Pay(int id)
    {
        var booking = await _bookings.GetAsync(id);
        if (booking.CustomerId != User.UserId())
            throw new ApiException(403, "You can only pay for your own booking.");
        return StatusCode(201, await _payments.PayAsync(id));
    }

    [Authorize(Roles = "Customer")]
    [HttpGet("payments/customer/{customerId:int}")]
    public async Task<ActionResult<IReadOnlyList<PaymentResponseDto>>> History(int customerId)
    {
        if (customerId != User.UserId())
            throw new ApiException(403, "You can only view your own payment history.");
        return Ok(await _payments.GetCustomerHistoryAsync(customerId));
    }

    [Authorize(Roles = "Customer,Administrator")]
    [HttpGet("payments/{id:int}/receipt")]
    public async Task<IActionResult> Receipt(int id)
    {
        var historyOwner = await FindReceiptOwnerAsync(id);
        EnsureOwnerOrAdmin(historyOwner);
        var (fileName, content) = await _payments.GetReceiptAsync(id);
        return File(Encoding.UTF8.GetBytes(content), "text/plain", fileName);
    }

    private async Task<int> FindReceiptOwnerAsync(int paymentId)
    {
        // Resolve through the user's/customer payment history without exposing model internals.
        if (User.Role() == "Administrator")
            return 0;

        var history = await _payments.GetCustomerHistoryAsync(User.UserId());
        if (!history.Any(x => x.PaymentId == paymentId))
            throw new ApiException(403, "You are not authorized to access this receipt.");
        return User.UserId();
    }

    private void EnsureOwnerOrAdmin(int customerId)
    {
        if (User.Role() != "Administrator" && User.UserId() != customerId)
            throw new ApiException(403, "You are not authorized to access this payment.");
    }
}
