namespace Veveve.Domain.Services
{
    public class SearchTermsDto
    {
        public string CampaignId { get; set; } = null!;
        public string CampaignName { get; set; } = null!;
        public string AdGroupId { get; set; } = null!;
        public string AdGroupName { get; set; } = null!;
        public string SearchTerm { get; set; } = null!;
        public long Impressions { get; set; }
        public long Clicks { get; set; }
        public double Conversions { get; set; }
        public double ConversionValue { get; set; }
        //Cut for now
        //public string CriterionId { get; set; } = null!;
        //Cut for now
        //public float Cost { get; set; }
    }
}
