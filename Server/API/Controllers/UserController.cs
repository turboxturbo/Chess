using API.Interfaces;
using API.Requests;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : Controller
{
    private readonly IUser _userI;
    public UserController(IUser userI)
    {
        _userI = userI;
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> Auth([FromBody] AuthReq request)
    {
        return await _userI.Auth(request);
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> Reg(RegReq request)
    {
        return await _userI.Reg(request);
    }
}