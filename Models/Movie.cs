using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieLibrary.Models
{
    public class Movie
    {
        public Guid Id { get; set; }
        [StringLength(125)]
        public string Title { get; set; }
        [StringLength(25)]
        public string Runtime { get; set; }
        [StringLength(75)]
        public string Genre { get; set; }
        [StringLength(50)]
        public string Director { get; set; }
        [StringLength(100)]
        public string Writer { get; set; }
        [StringLength(100)]
        public string Actors { get; set; }
        public string Plot { get; set; }
        [StringLength(25)]
        public string Year { get; set; }
        [StringLength(150)]
        public string Awards { get; set; }
        public string Poster { get; set; }
        [ForeignKey("User")]
        public string Fk_UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<Rating>? Ratings { get; set; }
        public virtual ICollection<StreamingService>? StreamingServices { get; set; }
    }
}
