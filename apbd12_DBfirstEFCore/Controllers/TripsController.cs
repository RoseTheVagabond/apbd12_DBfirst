using System.Data;
using apbd12_DBfirstEFCore.DTOs;
using apbd12_DBfirstEFCore.Services;
using Microsoft.AspNetCore.Mvc;

namespace apbd12_DBfirstEFCore.Controllers;

[ApiController]
[Route("api")]
public class TripsController : ControllerBase
{
    private readonly ITripsService _tripsService;

    public TripsController(ITripsService tripsService)
    {
        _tripsService = tripsService;
    }
    
    [HttpGet("/trips")]
    public async Task<IActionResult> GetAllTripsAsync(CancellationToken cancellationToken = default)
    {
        var trips = await _tripsService.GetAllTripsAsync(cancellationToken);
        return Ok(trips);
    }

    [HttpDelete("/clients/{id}")]
    public async Task<IActionResult> DeleteClientAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            await _tripsService.DeleteClient(id, cancellationToken);
            return NoContent();
        }
        catch (ArgumentException)
        {
            return BadRequest($"Client with id {id} does not exist");
        }
        catch (DataException)
        {
            return BadRequest($"Client with id {id} is assigned to at least one trip and cannot be deleted");
        }
    }

    [HttpPost("/trips/{idTrip}/clients")]
    public async Task<IActionResult> AddClientToTripAsync([FromRoute] int idTrip, [FromBody] AssignClientDTO clientDto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _tripsService.AssignClientToTrip(idTrip, clientDto);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
        return Created();
    }
}