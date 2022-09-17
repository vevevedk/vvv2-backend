namespace Veveve.Domain.Models.Jobs;

public class CreateKeywordsItem
{
    public CreateKeywordsItem(string keywordText, string adGroupId)
    {
        KeywordText = keywordText;
        AdGroupId = adGroupId;
    }

    public string KeywordText { get; set; } = null!;
    public string AdGroupId { get; set; }
}