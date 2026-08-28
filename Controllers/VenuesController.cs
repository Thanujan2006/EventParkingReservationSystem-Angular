
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservationSystem.API.Controllers;

[ApiController]
[Authorize(Roles = "Customer,Administrator")]
[Route("api/venues")]
public class VenuesController : ControllerBase
{
   
}
