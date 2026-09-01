using System.Security.Claims;
using System.Text.Json;
using EventParkingReservationSystem.API.Data;
using EventParkingReservationSystem.API.Enums;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservationSystem.API.Middleware;

/// <summary>
/// Re-checks customer account state on authenticated requests so a customer who
/// is deactivated after a JWT was issued cannot continue browsing/booking with
/// the old token until it expires.
/// </summary>
public class ActiveCustomerMiddleware
{
    private readonly RequestDelegate _next;

    public ActiveCustomerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ApplicationDbContext db)
    {
        if (context.User.Identity?.IsAuthenticated == true &&
            context.User.IsInRole("Customer"))
        {
            var idText = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(idText, out var customerId))
            {
                await Reject(context, "Invalid customer identity.");
                return;
            }

            var customer = await db.Customers.AsNoTracking()
                .Where(x => x.CustomerId == customerId)
                .Select(x => new { x.Status, x.EmailVerified })
                .FirstOrDefaultAsync();

            if (customer is null || customer.Status != CustomerStatus.Active)
            {
                await Reject(context, "Account is deactivated.");
                return;
            }

            if (!customer.EmailVerified)
            {
                await Reject(context, "Please verify your email.");
                return;
            }
        }

        await _next(context);
    }

    private static async Task Reject(HttpContext context, string message)
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(new { message }));
    }
}
