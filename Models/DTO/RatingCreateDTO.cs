namespace MovieLibrary.Models.DTO
{
    public class RatingCreateDTO
    {
        public Guid Id { get; set; }
        public string Source { get; set; }
        public string Value { get; set; }
        public Guid Fk_MovieId { get; set; }
    }
}
