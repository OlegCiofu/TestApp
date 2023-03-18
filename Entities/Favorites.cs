namespace WebApi.Entities
{
    public class Favorites
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public DateTime? Added { get; set; }
        public DateTime? Deleted { get; set; }
        public bool IsDeleted { get; set; }
    }
}
