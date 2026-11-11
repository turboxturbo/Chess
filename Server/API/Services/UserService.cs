using API.Context;
using API.Interfaces;
using API.Models;
using API.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class UserService : IUser
{
    private readonly DataBaseContext _db;
    public UserService(DataBaseContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Auth(AuthReq request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u =>
            u.Username == request.Username && u.Password == request.Password);

        if (user == null)
            return new OkObjectResult(new { status = false });
        return new OkObjectResult(new { status = true });
    }

    public async Task<IActionResult> Reg(RegReq request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
        if (user != null)
            return new OkObjectResult(new { status = false, message = "Пользователь уже существует" });

        try
        {
            var newuser = new Users
            {
                Username = request.Username,
                Password = request.Password,
            };

            await _db.Users.AddAsync(newuser);
            await _db.SaveChangesAsync();
            return new OkObjectResult(new { status = true });
        }
        catch (DbUpdateException)
        {
            return new BadRequestObjectResult(new { status = false, message = "Smth went wrong" });
        }
    }
}