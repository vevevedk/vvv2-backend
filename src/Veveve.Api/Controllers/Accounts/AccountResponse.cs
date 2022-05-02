using Veveve.Api.Infrastructure.Database.Entities;

namespace Veveve.Api.Controllers.Accounts;

public class AccountResponse : BaseResponse
{
    public AccountResponse() { }
    public AccountResponse(AccountEntity model)
    {
        Id = model.Id;
        GoogleAdsAccountId = model.GoogleAdsAccountId;
        GoogleAdsAccountName = model.GoogleAdsAccountName;
        CreatedDate = model.CreatedDate;
        LastModifiedDate = model.LastModifiedDate;
    }

    public int Id { get; set; }
    public string GoogleAdsAccountId { get; set; } = null!;
    public string GoogleAdsAccountName { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public DateTime LastModifiedDate { get; set; }
}