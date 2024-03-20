using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieLibrary.Models
{
    public class Rating
    {
        public Guid Id { get; set; }
        [StringLength(50)]
        public string Reviewer { get; set; }
        [StringLength(10)]
        public string Score { get; set; }
        [ForeignKey("Movie")]
        public Guid Fk_MovieId{ get; set; }
        public virtual Movie Movie { get; set; }
    }
}
