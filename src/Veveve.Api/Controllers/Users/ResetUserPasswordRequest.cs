using System.ComponentModel.DataAnnotations;

namespace Veveve.Api.Controllers.Users;

public class ResetUserPasswordRequest
{
    [Required]
    public string Email { get; set; } = null!;
}