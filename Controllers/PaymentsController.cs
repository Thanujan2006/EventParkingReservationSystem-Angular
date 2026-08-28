using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservationSystem.API.Controllers;

[ApiController]
[Authorize]
[Route("api")]
public class PaymentsController : ControllerBase
{
  
}
