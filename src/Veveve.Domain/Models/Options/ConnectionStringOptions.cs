namespace Veveve.Domain.Models.Options;

public class ConnectionStringOptions
{
    public static string SectionName = "ConnectionStrings"; // Name of object in appsettings

    public string DbConnection { get; set; } = string.Empty;
}
