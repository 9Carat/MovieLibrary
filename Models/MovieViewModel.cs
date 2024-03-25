using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MovieLibrary.Models
{
    public class MovieViewModel
    {
        public Movie Movie { get; set; }
        public List<StreamingService> Streaming { get; set; }
    }
}
