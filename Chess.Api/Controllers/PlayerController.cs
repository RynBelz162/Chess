using Chess.Api.Grains;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace Chess.Api.Controllers;

[Route("Player")]
public class PlayerController : ControllerBase
{
    private readonly IGrainFactory _grainFactory;

    public PlayerController(IGrainFactory grainFactory)
    {
        _grainFactory = grainFactory;
    }

    [HttpGet]
    public async Task<ActionResult> CreateSession()
    {
        var newPlayerId = Guid.NewGuid();
        await _grainFactory.GetGrain<IUserGrain>(newPlayerId).Create();
        return Ok(newPlayerId);
    }
}