using System.ComponentModel.DataAnnotations;

namespace Veveve.Api.Infrastructure.Database.Entities;

public class ClientEntity : BaseEntity
{
    public ClientEntity(){
        Users = new List<UserEntity>();
        Accounts = new List<AccountEntity>();
    }

    [Required]
    public string Name { get; set; } = null!;

    public ICollection<UserEntity> Users { get; set; }
    public ICollection<AccountEntity> Accounts { get; set; }
}
