using API.Interfaces;
using API.Requests;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
public class UserController : Controller
{
    private readonly IUser _userI;
    public UserController(IUser userI)
    {
        _userI = userI;
    }

    [HttpGet]
    [Route("api/[controller]/[action]")]
    public async Task<IActionResult> Auth(AuthReq  request)
    {
        return await _userI.Auth(request);
    }

    [HttpPost]
    [Route("api/[controller]/[action]")]
    public async Task<IActionResult> Reg(RegReq request)
    {
        return await _userI.Reg(request);
    }
}