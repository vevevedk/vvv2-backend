namespace Veveve.Api.Controllers.Clients;

public class ClientAssumeResponse : BaseResponse
{
    public ClientAssumeResponse() { }
    public ClientAssumeResponse(string jwt)
    {
        Jwt = jwt;
    }

    public string Jwt { get; set; } = null!;
}