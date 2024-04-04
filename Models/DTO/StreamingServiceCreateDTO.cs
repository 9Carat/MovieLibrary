namespace MovieLibrary.Models.DTO
{
    public class StreamingServiceCreateDTO
    {
        public string ServiceName { get; set; }
        public string Type { get; set; }
        public string Link { get; set; }
        public string Price { get; set; }
        public Guid Fk_MovieId { get; set; }
    }
}
