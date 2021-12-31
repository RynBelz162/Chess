using Microsoft.AspNetCore.Mvc;
using Orleans;
using Chess.Api.Grains;

namespace Chess.Api.Controllers;

[Route("Player")]
public class PlayerController : ControllerBase
{
    private readonly IGrainFactory _grainFactory;

    public PlayerController(IGrainFactory grainFactory)
    {
        _grainFactory = grainFactory;
    }

    [HttpGet("Test")]
    public async Task<ActionResult> Test()
    {
        var player = _grainFactory.GetGrain<IPlayerGrain>("123");
        var test = await player.Test();

        return Ok(new { Test = test });
    }
}