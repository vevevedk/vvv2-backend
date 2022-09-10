
using Veveve.Api.Domain.Services;

namespace Veveve.Api.Controllers.GoogleAds.SearchTerms
{
    public class SearchTermResponse
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
            //Cost = model.Cost;
            //CriterionId = model.CriterionId;
        }
        public string CampaignId { get; set; } = null!;
        public string CampaignName { get; set; } = null!;
        public string AdGroupId { get; set; } = null!;
        public string AdGroupName { get; set; } = null!;
        public string CriterionId { get; set; } = null!;
        public string SearchTerm { get; set; } = null!;
        public long Impressions { get; set; }
        public long Clicks { get; set; }
        public float Cost { get; set; }
        public double Conversions { get; set; }
        public double ConversionValue { get; set; }
    }
}
