using Veveve.Api.Infrastructure.Database.Entities;

namespace Veveve.Api.Controllers.Users;

public class LoginResponse : UserResponse
{
    public LoginResponse() { }
    
    public LoginResponse(string jwt, UserEntity UserEntity) : base(UserEntity)
    {
        this.Jwt = jwt;
    }

    /// <summary>
    /// JWT token containing user claims
    /// </summary>
    /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJjYXNAdHJpZm9yay5jb20iLCJqdGkiOiIzODVlZjM1ZS1hODM3LTRhNzgtOTY3NC0wYzhiMDFhYmM5MGUiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6ImYzMjk4Njg5LTZkM2ItNGE0Yy1hZjc5LTljNzUxMDZlZTM1MCIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6WyJVc2VyIiwiQWRtaW4iXSwiZXhwIjoxNTUyNTc2ODgyLCJpc3MiOiJodHRwOi8vbG9jYWxob3N0OjUwMDAiLCJhdWQiOiJodHRwOi8vbG9jYWxob3N0OjUwMDAifQ._bjNqZDbpN0qKIxFdtVpVJqc3MDJxZP6sAKfR_kLhyI</example>
    public string Jwt { get; set; } = null!;
}