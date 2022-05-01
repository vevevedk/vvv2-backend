using System;
using System.ComponentModel.DataAnnotations;

namespace Veveve.Api.Controllers.Accounts;

public class UpdateAccountIsAdminRequest
{
    [Required]
    public bool IsAdmin { get; set; }
}