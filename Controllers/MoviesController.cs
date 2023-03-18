using Microsoft.AspNetCore.Mvc;
using WebApi.Services;

namespace WebApi.Controllers
{
    public class MoviesController : ControllerBase
    {
        private IMoviesService moviesService;

        public MoviesController(IMoviesService moviesService)
        {
            this.moviesService = moviesService;
        }

        [HttpGet("most-popular")]
        public async Task<IActionResult> GetMostPopular([FromQuery] int pageNumber, CancellationToken cancellationToken)
        {
            var mostPopular = await moviesService.GetMostPopular(pageNumber, cancellationToken);
            return Ok(mostPopular);
        }

        [HttpGet("top-rated")]
        public async Task<IActionResult> GetMostRated([FromQuery] int pageNumber, [FromQuery] SortDirection sort, CancellationToken cancellationToken)
        {
            var mostPopular = await moviesService.GetTopRated(pageNumber, sort, cancellationToken);
            return Ok(mostPopular);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovieById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var topRated = await moviesService.GetMovieById(id, cancellationToken);
            return Ok(topRated);
        }

        [HttpPost("favorite/{id}")]
        public async Task<IActionResult> AddToFavorites([FromRoute] int id)
        {
            await moviesService.AddToFavorites(id);
            return Ok(new { message = "Added to Favorites" });
        }

        [HttpDelete("favorite/{id}")]
        public async Task<IActionResult> DeleteFromFavoritse([FromRoute] int id)
        {
            await moviesService.DeleteFromFavorites(id);
            return Ok(new { message = "Added to Favorites" });
        }

        [HttpGet("favorites")]
        public async Task<IActionResult> GetFavorites(CancellationToken ct)
        {
            var answer = await moviesService.GetFavorites(ct);
            return Ok(answer);
        }

        [HttpGet("genres")]
        public async Task<IActionResult> GetGenres(CancellationToken ct)
        {
            var answer = await moviesService.GetGenres(ct);
            return Ok(answer);
        }

        [HttpGet("top-rated-grouped")]
        public async Task<IActionResult> GetTopRatedGrouped([FromQuery] int pageNumber, CancellationToken cancellationToken)
        {
            var topRated = await moviesService.GetTopRatedGrouped(pageNumber, cancellationToken);
            return Ok(topRated);
        }
    }
}
