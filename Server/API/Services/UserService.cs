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
            return new NotFoundObjectResult(new { status = false, message = "User is not found" });
        return new OkObjectResult(new  { status = true, data = user });
    }

    public async Task<IActionResult> Reg(RegReq request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
        if (user != null)
            return new BadRequestObjectResult(new { status = false, message = "This username is already taken" });

        try
        {
            var newuser = new Users
            {
                Username = request.Username,
                Password = request.Password,
            };

            await _db.Users.AddAsync(newuser);
            await _db.SaveChangesAsync();
            return new OkObjectResult(newuser);
        }
        catch (DbUpdateException)
        {
            return new BadRequestObjectResult(new { status = false, message = "Smth went wrong" });
        }
    }
}