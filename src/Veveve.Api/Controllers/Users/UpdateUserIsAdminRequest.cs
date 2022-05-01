using System;
using System.ComponentModel.DataAnnotations;

namespace Veveve.Api.Controllers.Users;

public class UpdateUserIsAdminRequest
{
    [Required]
    public bool IsAdmin { get; set; }
}