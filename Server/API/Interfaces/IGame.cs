using API.Requests;
using Microsoft.AspNetCore.Mvc;

namespace API.Interfaces;

public interface IGame
{
    Task<IActionResult> Create(CreateGameReq request);
}
