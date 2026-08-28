
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservationSystem.API.Controllers;

[ApiController]
[Authorize(Roles = "Customer,Administrator")]
[Route("api/events/{eventId:int}/parking-slots")]
public class ParkingSlotsController : ControllerBase
{
    
}
