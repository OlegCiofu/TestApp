using WebApi.Entities;

namespace WebApi.Models.ResponseModels
{
    public class GroupedAnswer
    {
        public Genre Genre   { get; set; }
        public List<Movie> Movies { get; set; }
    }
}
