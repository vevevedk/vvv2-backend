using System.ComponentModel.DataAnnotations;

namespace Veveve.Api.Infrastructure.Database.Entities;

public class AccountEntity : BaseEntity
{
    public AccountEntity()
    {        
    }

    [Required]
    public int GoogleAdsAccountId { get; set; }
    [Required]
    public string GoogleAdsAccountName { get; set; } = null!;

    [Required]
    public int ClientId { get; set; }
    [Required]
    public ClientEntity Client { get; set; } = null!;
}
