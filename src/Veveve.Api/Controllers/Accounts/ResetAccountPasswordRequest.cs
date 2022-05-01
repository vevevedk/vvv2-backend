using System.ComponentModel.DataAnnotations;

namespace Veveve.Api.Controllers.Accounts;

public class ResetAccountPasswordRequest
{
    [Required]
    public string Email { get; set; } = null!;
}