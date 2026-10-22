using System.ComponentModel.DataAnnotations;

namespace API.Models;

public class Users
{
    [Key]
    public int IdUser { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public ICollection<Games> Games { get; set; } = new List<Games>();
}
