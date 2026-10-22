using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Context;

public class DataBaseContext : DbContext
{
    public DataBaseContext(DbContextOptions<DataBaseContext> options)
        : base(options)
    {
    }

    public DbSet<Users> Users { get; set; }
    public DbSet<Games> Games { get; set; }
}
