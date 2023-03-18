using WebApi.Entities;
using WebApi.Models;
using WebApi.Models.ResponseModels;

namespace WebApi.Services;

public interface IMoviesService
{
    Task<List<Movie>> GetMostPopular(int page, CancellationToken ct);
    Task<List<Movie>> GetTopRated(int page, SortDirection sort, CancellationToken ct);
    Task<MovieDetails> GetMovieById(int id, CancellationToken ct);
    Task AddToFavorites(int id);
    Task DeleteFromFavorites(int id);
    Task<List<MovieDetails>> GetFavorites(CancellationToken tn);
    Task<List<Genre>> GetGenres(CancellationToken ct);
    Task<List<GroupedAnswer>> GetTopRatedGrouped(int page, CancellationToken ct);
}
