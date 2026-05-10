using Microsoft.AspNetCore.Mvc;

namespace eTerminiAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok(new
        {
            message = "eTermini API është aktive!",
            timestamp = DateTime.UtcNow,
            version = "1.0.0"
        });
    }

    [HttpGet("institutions")]
    public IActionResult GetInstitutions()
    {
        var institutions = new[]
        {
            new { id = 1, name = "QKUK", city = "Prishtinë", description = "Qendra Klinike Universitare e Kosovës" },
            new { id = 2, name = "Policia e Kosovës", city = "Prishtinë", description = "Dokumente, patentë shoferi, pasaporta" },
            new { id = 3, name = "Komuna e Prishtinës", city = "Prishtinë", description = "Certifikata dhe dokumente civile" },
            new { id = 4, name = "Komuna e Gjakovës", city = "Gjakovë", description = "Certifikata dhe dokumente civile" },
            new { id = 5, name = "Komuna e Prizrenit", city = "Prizren", description = "Certifikata dhe dokumente civile" },
            new { id = 6, name = "Ministria e Shëndetësisë", city = "Prishtinë", description = "Shërbime shëndetësore qeveritare" },
        };

        return Ok(institutions);
    }
}
