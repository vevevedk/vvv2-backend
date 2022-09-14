namespace Veveve.Domain.Services;

public class PasswordDto
{
    public PasswordDto(string hash, byte[] salt)
    {
        Hash = hash;
        Salt = salt;
    }

    public string Hash { get; set; }
    public byte[] Salt { get; set; }
}