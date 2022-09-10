namespace Veveve.Api.Controllers.Users;

public class UpdateUserRequest : BaseUserRequest
{
    /// <summary>
    /// This property requires admin rights to update
    /// </summary>
    public bool? IsAdmin { get; set; }
}