using System;
using Veveve.Api.Infrastructure.Database.Entities;

namespace Veveve.Api.Controllers.Users;

public class UserResponse : BaseResponse
{
    public UserResponse() { }
    public UserResponse(UserEntity model)
    {
        Id = model.Id;
        Email = model.Email;
        FullName = model.FullName;
        CreatedDate = model.CreatedDate;
        LastModifiedDate = model.LastModifiedDate;
        IsAdmin = model.HasAdminClaim();
    }

    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string? FullName { get; set; }
    public bool IsAdmin { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime LastModifiedDate { get; set; }
}