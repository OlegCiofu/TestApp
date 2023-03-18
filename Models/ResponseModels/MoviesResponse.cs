using WebApi.Entities;

namespace WebApi.Models.ResponseModels;

public class MoviesResponse
{
    public int Page { get; set; }
    public List<Movie> Results { get; set; }
    public int Total_pages { get; set; }
    public int Total_results { get; set; }
}
