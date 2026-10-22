using System.ComponentModel.DataAnnotations;

namespace API.Requests;

public class CreateGameReq
{
    [Range(1, int.MaxValue)]
    public int IdUser { get; set; }
    public int WavesCount { get; set; }
    public bool Status { get; set; }
}
