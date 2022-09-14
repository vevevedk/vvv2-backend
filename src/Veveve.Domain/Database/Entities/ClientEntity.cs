using System.ComponentModel.DataAnnotations;

namespace Veveve.Domain.Database.Entities;

public class ClientEntity : BaseEntity
{
    public ClientEntity(){
        Users = new List<UserEntity>();
        Accounts = new List<AccountEntity>();
    }

    /// <summary>
    /// Must be unique
    /// </summary>
    [Required]
    public string Name { get; set; } = null!;

    public ICollection<UserEntity> Users { get; set; }
    public ICollection<AccountEntity> Accounts { get; set; }
}
