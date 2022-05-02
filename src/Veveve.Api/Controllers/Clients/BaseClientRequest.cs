using System.ComponentModel.DataAnnotations;

namespace Veveve.Api.Controllers.Clients;

public class BaseClientRequest
{
    [Required]
    public string Name { get; set; } = null!;
}