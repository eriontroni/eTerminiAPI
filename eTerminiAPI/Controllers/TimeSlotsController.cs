using eTerminiAPI.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eTerminiAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TimeSlotsController : ControllerBase
{
    private readonly ITimeSlotService _timeSlotService;

    public TimeSlotsController(ITimeSlotService timeSlotService)
    {
        _timeSlotService = timeSlotService;
    }

    /// <summary>
    /// Merr terminet e lira (slot-et) për një mjek në një datë të caktuar.
    /// Përjashton terminet e rezervuara dhe ato të kaluara.
    /// </summary>
    [HttpGet("available")]
    [Authorize(Policy = "CitizenOrAbove")]
    public async Task<IActionResult> GetAvailable(
        [FromQuery] Guid doctorId,
        [FromQuery] DateTime date,
        [FromQuery] int durationMinutes = 30)
    {
        if (doctorId == Guid.Empty)
            return BadRequest(new { message = "DoctorId është i detyrueshëm." });

        try
        {
            var slots = await _timeSlotService.GetAvailableSlotsAsync(doctorId, date, durationMinutes);
            return Ok(slots);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Verifiko nëse një slot specifik është i lirë për një mjek.
    /// </summary>
    [HttpGet("check")]
    [Authorize(Policy = "CitizenOrAbove")]
    public async Task<IActionResult> CheckSlot(
        [FromQuery] Guid doctorId,
        [FromQuery] DateTime startTime,
        [FromQuery] int durationMinutes = 30)
    {
        if (doctorId == Guid.Empty)
            return BadRequest(new { message = "DoctorId është i detyrueshëm." });

        var isFree = await _timeSlotService.IsSlotFreeAsync(doctorId, startTime, durationMinutes);
        return Ok(new { doctorId, startTime, durationMinutes, isFree });
    }
}
