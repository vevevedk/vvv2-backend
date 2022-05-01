using System.ComponentModel.DataAnnotations;

namespace Veveve.Api.Controllers.Accounts;

public class BaseAccountRequest
{
    [Required]
    [RegularExpression(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$")]
    public string Email { get; set; } = null!;
    [Required]
    public string FullName { get; set; } = null!;
}