using Movies.EntityModels;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Http;
using Movies.Models;
using AutoMapper;
using Movies.Options;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace Movies.Services;

public class MovieService : IMovieService
{
    private readonly ILogger<MovieService> _logger;
    private readonly HttpClient _httpClient;
    private readonly IMapper _mapper;
    private readonly string _apiKey;

    public MovieService(
        ILogger<MovieService> logger,
        IHttpClientFactory httpClientFactory,
	IOptions<OmdbApiOptions> options,
	IMapper mapper)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("OMDB");
        _apiKey = options.Value.ApiKey;
        _mapper = mapper;
    }

    public async Task<Movie?> GetMovieAsync(string title)
    {
        using (_logger.BeginScope("Fetching movie - {Title}", title))
        {
            try
            {
                string apiUrl = $"?t={Uri.EscapeDataString(title)}&apikey={_apiKey}";
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                _logger.LogInformation("API response received - Status: {StatusCode}",
                               response.StatusCode);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("API request failed - Status: {StatusCode}",
                               response.StatusCode);
                    return null;
                }
                var movieData = await response
                    .Content
                    .ReadFromJsonAsync<OmdbApiResponse>();

                if (movieData?.Response == "false")
                {
                    _logger.LogWarning("Movie not found in API response - Title: {Title}",
                               title);
                    return null;
                }
                _logger.LogInformation("Successfully retrieved movie - Title: {Title}",
                               title);

                return _mapper.Map<Movie>(movieData!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving movie - {Title}", title);
                throw;
            }

        }
    }
}

