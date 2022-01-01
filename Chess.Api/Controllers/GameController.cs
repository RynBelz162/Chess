using Microsoft.AspNetCore.Mvc;
using Orleans;
using Chess.Api.Grains;

namespace Chess.Api.Controllers;

[Route("Game")]
public class GameController : ControllerBase
{
    private readonly IGrainFactory _grainFactory;

    public GameController(IGrainFactory grainFactory)
    {
        _grainFactory = grainFactory;
    }

    [HttpGet]
    [Route("{playerId}")]
    public async Task<ActionResult> CreateGame(Guid playerId)
    {
        var gameId = await _grainFactory
            .GetGrain<IPlayerGrain>(playerId)
            .CreateGame();

        return Ok(new
        {
            gameId = gameId,
            createdOn = DateTime.UtcNow
        });
    }
}