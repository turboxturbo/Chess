using API.Context;
using API.Interfaces;
using API.Models;
using API.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class GameService : IGame
{
    private readonly DataBaseContext _db;

    public GameService(DataBaseContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> StartGame(CreateGameReq request)
    {
        bool userExists = await _db.Users.AnyAsync(user => user.IdUser == request.IdUser);

        if (!userExists)
        {
            return new NotFoundObjectResult(new
            {
                status = false,
                message = "User is not found"
            });
        }

        var game = new Games
        {
            IdUser = request.IdUser,
            Status = request.Status,
            WavesCount = request.WavesCount,
            CreatedAt = DateTime.UtcNow,
            EndAt = null
        };

        _db.Games.Add(game);
        await _db.SaveChangesAsync();

        return new OkObjectResult(new  { status = true, data = game });
    }

    public async Task<IActionResult> Endgame([FromBody] int idGame)
    {
        var game = await _db.Games.FirstOrDefaultAsync(g => g.IdGame ==  idGame);
        if (game == null)
        {
            return new NotFoundObjectResult(new
            {
                status = false,
                message = "Game is not found"
            });
        }
        
        game.EndAt =  DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return new OkObjectResult(new  { status = true, data = game });
    }
}
