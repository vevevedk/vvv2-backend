using System.ComponentModel.DataAnnotations;

namespace Veveve.Api.Controllers.Accounts;

public class BaseAccountRequest
{
    [Required]
    public int GoogleAdsAccountId { get; set; }
    [Required]
    public string GoogleAdsAccountName { get; set; } = null!;
}