using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

public class Games
{
    [Key]
    public int IdGame { get; set; }
    public bool Status { get; set; }
    public int WavesCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? EndAt { get; set; }
    public int IdUser { get; set; }
    [ForeignKey(nameof(IdUser))]
    public Users Users { get; set; } = null!;
}
