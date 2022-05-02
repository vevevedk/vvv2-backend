using System.ComponentModel.DataAnnotations;

namespace Veveve.Api.Controllers.Accounts;

public class BaseAccountRequest
{
    [Required]
    public string GoogleAdsAccountId { get; set; } = null!;
    [Required]
    public string GoogleAdsAccountName { get; set; } = null!;
}