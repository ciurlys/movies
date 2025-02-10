namespace Movies.Options;

public class DatabaseOptions
{
    public const string SectionName = "DatabaseOptions";
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
