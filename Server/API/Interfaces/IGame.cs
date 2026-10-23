using API.Requests;
using Microsoft.AspNetCore.Mvc;

namespace API.Interfaces;

public interface IGame
{
    Task<IActionResult> StartGame(CreateGameReq request);
    Task<IActionResult> Endgame([FromBody] int idGame);
}
