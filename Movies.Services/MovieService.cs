using Movies.EntityModels;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Http;

namespace Movies.Services;

public class MovieService
{
    private readonly ILogger<MovieService> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public MovieService(
        ILogger<MovieService> logger,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
        _apiKey = Environment.GetEnvironmentVariable("API_KEY");
    }

    public async Task<Movie?> GetMovieAsync(string title)
    {
	using(_logger.BeginScope("Fetching movie - {Title}", title))
        {
	    try
	    {
		string apiUrl = $"https://www.omdbapi.com/?t={Uri.EscapeDataString(title)}&apikey={_apiKey}";
		HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
		
		_logger.LogInformation("API response received - Status: {StatusCode}",
				       response.StatusCode);
		if (!response.IsSuccessStatusCode)
		{
		    _logger.LogWarning("API request failed - Status: {StatusCode}",
				       response.StatusCode);
		    return null;
		}
		string jsonResponse = await response.Content.ReadAsStringAsync();
                var movieData = JsonSerializer
		 .Deserialize<OmdbApiResponse>(jsonResponse);

		if (movieData?.Response == "false")
		{
		    _logger.LogWarning("Movie not found in API response - Title: {Title}",
				       title);
		    return null;
		}
		_logger.LogInformation("Successfully retrieved movie - Title: {Title}",
				       title);
		
		return MapToMovie(movieData);
	    }
	    catch (Exception ex)
	    {
		_logger.LogError(ex, "Error retrieving movie - {Title}", title);
		throw;
	    }

	}
    }

	private Movie MapToMovie(OmdbApiResponse movieData)
	{
            var movie = new Movie
            {
                Title = movieData.Title,
                Director = movieData.Director,
                Description = movieData.Plot,
                ReleaseDate = new DateOnly(int.Parse(movieData.Year), 1, 1),
                ImagePath = movieData.Poster
            };
	    return movie;
	}
}
    
public class OmdbApiResponse
{
    public string Title { get; set; }
    public string Director { get; set; }
    public string Plot { get; set; }
    public string Year { get; set; }
    public string Poster { get; set; }
    public string Response { get; set; }
}
