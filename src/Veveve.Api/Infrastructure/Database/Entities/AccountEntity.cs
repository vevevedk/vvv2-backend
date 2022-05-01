using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Veveve.Api.Infrastructure.Database.Entities;

public class AccountEntity : BaseEntity
{
    public AccountEntity(string fullName, string email)
    {
        FullName = fullName;
        Email = email;
        Claims = new List<AccountClaimEntity>();
    }

    [Required]
    public string FullName { get; set; }
    [Required]
    public string Email { get; set; }
    public string? Password { get; set; }
    public byte[]? Salt { get; set; }
    public Guid? ResetPasswordToken { get; set; }

    public ICollection<AccountClaimEntity> Claims { get; set; }

    public bool HasAdminClaim() => Claims.Any(x => x.ClaimType == ClaimTypeEnum.Admin);
}
