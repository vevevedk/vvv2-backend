using System;
using System.ComponentModel.DataAnnotations;

namespace Veveve.Api.Infrastructure.Database.Entities;

public class AccountClaimEntity : BaseEntity
{
    public AccountClaimEntity(ClaimTypeEnum claimType)
    {
        ClaimType = claimType;
    }

    public ClaimTypeEnum ClaimType { get; set; }

    [Required]
    public int AccountId { get; set; }
    [Required]
    public AccountEntity Account { get; set; } = null!;
}

public enum ClaimTypeEnum
{
    User,
    Admin
}
