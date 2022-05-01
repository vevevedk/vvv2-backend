using System.ComponentModel.DataAnnotations;

namespace Veveve.Api.Controllers.Accounts;

public class CreateAccountRequest : BaseAccountRequest
{
    [Required]
    public int ClientId { get; set; }
}