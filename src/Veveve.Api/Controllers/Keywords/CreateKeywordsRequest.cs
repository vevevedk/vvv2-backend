
using System.ComponentModel.DataAnnotations;

namespace Veveve.Api.Controllers.Keywords;

public class CreateKeywordsRequest
{
    [Required]
    public string GoogleAdsCustomerId { get; set; } = null!;

    [Required]
    public bool Negative { get; set; }

    [Required]
    public IEnumerable<KeywordRequest> Keywords { get; set; } = Array.Empty<KeywordRequest>();
}