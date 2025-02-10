namespace Movies.Options;

public class OmdbApiOptions
{
    public const string SectionName = "OmdbApi";
    public string ApiKey { get; set; } = string.Empty;
}
