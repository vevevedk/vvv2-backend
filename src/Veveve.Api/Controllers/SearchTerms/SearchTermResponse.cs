
using Veveve.Domain.Services;

namespace Veveve.Api.Controllers.SearchTerms;

public class SearchTermResponse : BaseResponse
{
    public SearchTermResponse(SearchTermsDto model)
    {
        CampaignName = model.CampaignName;
        AdGroupName = model.AdGroupName;
        SearchTerm = model.SearchTerm;
        Impressions = model.Impressions;
        Clicks = model.Clicks;
        Conversions = model.Conversions;
        ConversionValue = model.ConversionValue;
        CampaignId = model.CampaignId;
        AdGroupId = model.AdGroupId;
        CostMicros = model.CostMicros;
    }
    public string CampaignId { get; set; } = null!;
    public string CampaignName { get; set; } = null!;
    public string AdGroupId { get; set; } = null!;
    public string AdGroupName { get; set; } = null!;
    public string SearchTerm { get; set; } = null!;
    public long Impressions { get; set; }
    public long Clicks { get; set; }
    public double Conversions { get; set; }
    public double ConversionValue { get; set; }
    public long CostMicros { get; set; }
}
