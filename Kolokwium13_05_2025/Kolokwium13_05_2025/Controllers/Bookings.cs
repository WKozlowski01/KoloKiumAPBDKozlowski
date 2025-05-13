using Kolokwium13_05_2025.DTOs;
using Kolokwium13_05_2025.Exceptions;
using Kolokwium13_05_2025.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kolokwium13_05_2025.Controllers;

[ApiController]
[Route("api/[controller]")]
public class Bookings: ControllerBase
{
    private readonly IDbService _DBService;

    public Bookings(IDbService DBService)
    {
        _DBService = DBService;
    }

    
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRentalsAsync(int id)
    {
        
        try
        {
            var result = await _DBService.GetBookingsAsync(id);
            return Ok(result);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
     

    }

    [HttpPost("{id}/rentals")]
    public async Task<IActionResult> AddNewBookingAsync(InsertData insertData)
    {

        if (!insertData.attractions.Any())
        {
            return BadRequest("At least one item is required.");
        }
        try
        {
            await _DBService.InsertBookingAsync(insertData);
            return NoContent();
        }
        catch (ConflictException e)
        {
            return Conflict(e.Message);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        

    }

}