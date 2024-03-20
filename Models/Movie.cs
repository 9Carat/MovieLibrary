using System.ComponentModel.DataAnnotations;

namespace MovieLibrary.Models
{
    public class Movie
    {
        public Guid Id { get; set; }
        [StringLength(100)]
        public string Title { get; set; }
        [StringLength(25)]
        public string Runtime { get; set; }
        [StringLength(25)]
        public string Genre { get; set; }
        [StringLength(50)]
        public string Director { get; set; }

        [StringLength(50)]
        public string Writer { get; set; }
        [StringLength(75)]
        public string Actors { get; set; }
        public string Description { get; set; }
        [StringLength(25)]
        public string ReleaseDate { get; set; }
        [StringLength(100)]
        public string Awards { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }
        public virtual ICollection<StreamingService> StreamingServices { get; set; }

    }
}
