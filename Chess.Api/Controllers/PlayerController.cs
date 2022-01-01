using Microsoft.AspNetCore.Mvc;

namespace Chess.Api.Controllers;

[Route("Player")]
public class PlayerController : ControllerBase
{
    [HttpGet]
    public ActionResult CreateSession()
    {
        return Ok(new
        {
            playerId = Guid.NewGuid()
        });
    }
}