
using System.ComponentModel.DataAnnotations;

namespace Veveve.Api.Controllers.Keywords;

public class KeywordRequest
{
    [Required]
    public string AdGroupId { get; set; } = null!;

    [Required]
    public string KeywordText { get; set; } = null!;
}