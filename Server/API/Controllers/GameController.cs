using API.Context;
using API.Interfaces;
using API.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly IGame _game;

    public GameController(IGame game, DataBaseContext db)
    {
        _game = game;
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> StartGame([FromBody] CreateGameReq request)
    {
        return await _game.StartGame(request);
    }

    [HttpPut]
    [Route("[action]")]
    public async Task<IActionResult> Endgame([FromBody] int idGame)
    {
        return await _game.Endgame(idGame);
    }
}
