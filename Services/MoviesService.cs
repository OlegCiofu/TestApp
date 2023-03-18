using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RestSharp;
using System.Linq;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models;
using WebApi.Models.ResponseModels;

namespace WebApi.Services;

public class MoviesService : IMoviesService
{
    private DataContext context;
    private readonly string apiKey;
    private readonly string baseUrl = "https://api.themoviedb.org/3/";

    public MoviesService(IConfiguration configuration, DataContext context)
    {
        apiKey = configuration.GetValue<string>("ApiKey");
        this.context = context;
    }
    public async Task<List<Movie>> GetMostPopular(int page, CancellationToken ct)
    {
        var client = new RestClient(baseUrl);

        var request = BuildQuery(new QueryOptions { Section = "movie/popular", Page = page });

        var response = await client.GetAsync<MoviesResponse>(request, ct);
        return response.Results;
    }

    public async Task<List<Movie>> GetTopRated(int page, SortDirection sort, CancellationToken ct)
    {
        var client = new RestClient(baseUrl);

        var request = BuildQuery(new QueryOptions { Section = "discover/movie", Page = page, SortDirection = sort });

        var response = await client.GetAsync<MoviesResponse>(request, ct);
        return response.Results;
    }

    public async Task<MovieDetails> GetMovieById(int id, CancellationToken ct)
    {
        var client = new RestClient(baseUrl);

        var request = BuildQuery(new QueryOptions { Section = $"movie/{id}" });

        return await client.GetAsync<MovieDetails>(request, ct);
    }

    public async Task<List<Genre>> GetGenres(CancellationToken ct)
    {
        var client = new RestClient(baseUrl);

        var request = BuildQuery(new QueryOptions { Section = "genre/movie/list"});

        var response = await client.GetAsync<GenresResponse>(request, ct);
        return response.Genres;
    }

    public async Task AddToFavorites(int id)
    {
        var favToSave = new Favorites
        {
            MovieId = id,
            Added = DateTime.Now
        };

        await context.Favorites.AddAsync(favToSave);
        await context.SaveChangesAsync();
    }

    public async Task DeleteFromFavorites(int id)
    {
        var favToEdit = context.Favorites.FirstOrDefault(f => f.Id == id);

        if (favToEdit != null)
        {
            favToEdit.IsDeleted = true;
            favToEdit.Deleted = DateTime.Now;
        }

        await context.SaveChangesAsync();
    }

    public async Task<List<MovieDetails>> GetFavorites(CancellationToken tn)
    {
        var favIds = context.Favorites.Where(f => !f.IsDeleted)
            .Select(x=>x.MovieId)
            .ToList();

        var tasks = await Task.WhenAll(favIds.Select(x=> GetMovieById(x, tn)));

        return tasks.ToList();
    }

    public async Task<List<GroupedAnswer>> GetTopRatedGrouped(int page, CancellationToken ct)
    {
        var client = new RestClient(baseUrl);

        var request = BuildQuery(new QueryOptions { Section = "movie/top_rated", Page = page});

        var genres = await GetGenres(ct);

        var moviesAnswer= await client.GetAsync<MoviesResponse>(request, ct);

        var movies = moviesAnswer.Results.SelectMany(x => x.Genre_ids.Select(y => new Movie
        {
            Genre_id = y,
            Adult = x.Adult,
            Backdrop_path = x.Backdrop_path,
            Id = x.Id,
            Original_language = x.Original_language,
            Original_title = x.Original_title,
            Overview = x.Overview,
            Popularity = x.Popularity,
            Poster_path = x.Poster_path,
            Release_date = x.Release_date,
            Title = x.Title,
            Video = x.Video,
            Vote_average = x.Vote_average,
            Vote_count = x.Vote_count
        }));

        return genres.
            Take(5)
            .GroupJoin(movies, genre => genre.Id, movie => movie.Genre_id, (genre, movie) => new GroupedAnswer { Genre = genre, Movies = movie.ToList() })
            .ToList();
    }



    private RestRequest BuildQuery(QueryOptions options)
    {
        var answer = new RestRequest(options.Section);
        
        answer.AddQueryParameter("page", options.Page >= 1 ? options.Page.ToString() : "1");

        if (options.SortDirection.HasValue)
        {
            answer.AddQueryParameter("sort_by", $"popularity.{options.SortDirection.ToString()}");
        }

        answer.AddQueryParameter("api_key", this.apiKey);

        return answer;
    }
}

public class QueryOptions
{
    public string Section { get; set; }
    public int Page { get; set; }
    public SortDirection? SortDirection { get; set; }
    public string? SortField { get; set; }
}

[JsonConverter(typeof(StringEnumConverter))]
public enum SortDirection
{
    asc,
    desc
}
