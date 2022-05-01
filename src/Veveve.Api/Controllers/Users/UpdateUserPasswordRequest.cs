using System;
using System.ComponentModel.DataAnnotations;

namespace Veveve.Api.Controllers.Users;

public class UpdateUserPasswordRequest
{
    [Required]
    public Guid ResetPasswordToken { get; set; }
    [Required]
    public string Password { get; set; } = null!;
}