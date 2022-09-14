using System;
using System.ComponentModel.DataAnnotations;

namespace Veveve.Domain.Database.Entities;

public class UserClaimEntity : BaseEntity
{
    public UserClaimEntity(){}

    public UserClaimEntity(ClaimTypeEnum claimType) : this()
    {
        ClaimType = claimType;
    }

    public ClaimTypeEnum ClaimType { get; set; }

    [Required]
    public int UserId { get; set; }
    [Required]
    public UserEntity User { get; set; } = null!;
}

public enum ClaimTypeEnum
{
    User,
    Admin
}
