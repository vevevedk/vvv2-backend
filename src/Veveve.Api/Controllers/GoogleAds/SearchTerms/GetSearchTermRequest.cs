using System.ComponentModel.DataAnnotations;

namespace Veveve.Api.Controllers.GoogleAds.SearchTerms
{
    public class GetSearchTermRequest
    {
        [Required]
        public string GoogleAdsCustomerId { get; set; } = null!;

        [Required]
        public int LookbackDays { get; set; }

    }
}
