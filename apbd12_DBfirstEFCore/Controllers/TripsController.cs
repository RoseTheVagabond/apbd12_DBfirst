using apbd12_DBfirstEFCore.Models;
using apbd12_DBfirstEFCore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace apbd12_DBfirstEFCore.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    private readonly ITripsService _tripsService;

    public TripsController(ITripsService tripsService)
    {
        _tripsService = tripsService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllTripsAsync(CancellationToken cancellationToken = default)
    {
        // var trips = await _context.Trips.ToListAsync(cancellationToken);
        // return Ok(trips);
        var trips = await _tripsService.GetAllTripsAsync(cancellationToken);
        return Ok(trips);
    }
}