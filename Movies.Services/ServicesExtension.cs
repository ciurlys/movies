using Microsoft.Extensions.DependencyInjection;

namespace Movies.Services;

public static class ServicesExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
	services.AddScoped<IMovieService, MovieService>();
	services.AddScoped<IImageService, ImageService>();
	return services;
    }
}
