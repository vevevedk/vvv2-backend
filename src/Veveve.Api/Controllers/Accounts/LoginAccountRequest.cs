using System.ComponentModel.DataAnnotations;

namespace Veveve.Api.Controllers.Accounts;

public class LoginAccountRequest
{
    [Required]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}