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
    private readonly DataBaseContext _db;

    public GameController(IGame game, DataBaseContext db)
    {
        _game = game;
        _db = db;
    }

    [HttpPost]
    public Task<IActionResult> Create([FromBody] CreateGameReq request)
    {
        return _game.Create(request);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var game = await _db.Games
            .AsNoTracking()
            .Where(game => game.IdGame == id)
            .Select(game => new
            {
                game.IdGame,
                game.IdUser,
                game.Status,
                game.WavesCount,
                game.CreatedAt,
                game.EndAt
            })
            .FirstOrDefaultAsync();

        return game == null ? NotFound() : Ok(game);
    }
}
