namespace Veveve.Domain.Models.Jobs;

public class CreateNegativeKeywordsJobModel
{
    public string GoogleAdsAccountId { get; set; } = null!;
    public IEnumerable<CreateNegativeKeywordsItem> Keywords { get; set; } = Array.Empty<CreateNegativeKeywordsItem>();
}