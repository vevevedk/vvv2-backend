namespace Veveve.Domain.Models.Jobs;

public class CreateNegativeKeywordsItem
{
    public string KeywordText { get; set; } = null!;
    public int AdGroupId { get; set; }
}