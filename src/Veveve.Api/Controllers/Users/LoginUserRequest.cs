using System.ComponentModel.DataAnnotations;

namespace Veveve.Api.Controllers.Users;

public class LoginUserRequest
{
    [Required]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}