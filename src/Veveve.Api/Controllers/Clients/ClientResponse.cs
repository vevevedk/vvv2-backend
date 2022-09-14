using Veveve.Domain.Database.Entities;

namespace Veveve.Api.Controllers.Clients;

public class ClientResponse : BaseResponse
{
    public ClientResponse() { }
    public ClientResponse(ClientEntity model)
    {
        Id = model.Id;
        Name = model.Name;
        CreatedDate = model.CreatedDate;
        LastModifiedDate = model.LastModifiedDate;
    }

    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public DateTime LastModifiedDate { get; set; }
}