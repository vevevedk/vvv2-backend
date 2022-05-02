using System.ComponentModel.DataAnnotations;

namespace Veveve.Api.Infrastructure.Database.Entities;

public class AccountEntity : BaseEntity
{
    public AccountEntity()
    {        
    }

    [Required]
    public string GoogleAdsAccountId { get; set; } = null!;
    [Required]
    public string GoogleAdsAccountName { get; set; } = null!;

    [Required]
    public int ClientId { get; set; }
    [Required]
    public ClientEntity Client { get; set; } = null!;
}
