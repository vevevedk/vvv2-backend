using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Veveve.Domain.Database.Entities;

public class UserEntity : BaseEntity
{
    public UserEntity()
    {
        Claims = new List<UserClaimEntity>();
    }

    [Required]
    public string FullName { get; set; } = null!;
    [Required]
    public string Email { get; set; } = null!;
    public string? Password { get; set; }
    public byte[]? Salt { get; set; }
    public Guid? ResetPasswordToken { get; set; }
    [Required]
    public int ClientId { get; set; }

    public ClientEntity Client { get; set; } = null!;

    public ICollection<UserClaimEntity> Claims { get; set; }

    public bool HasAdminClaim() => Claims.Any(x => x.ClaimType == ClaimTypeEnum.Admin);
}
