namespace Veveve.Domain.Models.Options;

public class DefaultAdminDataOptions
{
    public static string SectionName = "DefaultAdminData"; // Name of object in appsettings

    public string DefaultAdminClientName { get; set; } = string.Empty;
    public DefaultAdminUser[] DefaultAdminUsers { get; set; } = new DefaultAdminUser[0];
}
public class DefaultAdminUser
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
