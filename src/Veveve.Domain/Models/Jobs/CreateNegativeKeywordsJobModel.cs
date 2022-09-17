namespace Veveve.Domain.Models.Jobs;

public class CreateNegativeKeywordsJobModel
{
    public string GoogleAdsAccountId { get; set; } = null!;
    public IEnumerable<CreateKeywordsItem> Keywords { get; set; } = Array.Empty<CreateKeywordsItem>();
}