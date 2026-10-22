using API.Requests;
using Microsoft.AspNetCore.Mvc;

namespace API.Interfaces;

public interface IUser
{
    Task<IActionResult> Auth(AuthReq  request);
    Task<IActionResult> Reg(RegReq  request);

}