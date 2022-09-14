using System.ComponentModel.DataAnnotations;

namespace Veveve.Api.Controllers.Users;

public class CreateUserRequest : BaseUserRequest
{
    [Required]
    public bool IsAdmin { get; set; }
}