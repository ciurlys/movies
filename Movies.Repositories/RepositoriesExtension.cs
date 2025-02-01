using Microsoft.Extensions.DependencyInjection;

namespace Movies.Repositories;

public static class RepositoriesExtension
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
	services.AddScoped<IMovieRepository, MovieRepository>();
	services.AddScoped<IChatRepository, ChatRepository>();
	services.AddScoped<ICommentRepository, CommentRepository>();
	services.AddScoped<IUserRepository, UserRepository>();
	services.AddScoped<IVoteRepository, VoteRepository>();

	return services;
    }
}
