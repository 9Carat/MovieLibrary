using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieLibrary.API;
using MovieLibrary.APIComponents;
using MovieLibrary.Data;
using MovieLibrary.Models;
using Newtonsoft.Json;
using System.Security.Principal;

namespace MovieLibrary.Controllers
{
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _user;
        public MovieController(ApplicationDbContext db, UserManager<IdentityUser> user)
        {
            _db = db;
            _user = user;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Search()
        {
            var movie = new Movie();
            return View(movie);
        }

        public async Task<IActionResult> SearchResult(string title)
        {

            //Call the OMDb API for general information
            var OMDbResponse = new OMDbAPI();
            var OMDbResult = await OMDbResponse.Get(title);
            var movieData = JsonConvert.DeserializeObject<Movie>(OMDbResult);
            List<Rating> ratingData;

            if (movieData != null ) 
            {
                ratingData = movieData.Ratings.Select(rating =>
                {
                    return new Rating
                    {
                        Id = Guid.NewGuid(),
                        Source = rating.Source,
                        Value = rating.Value,
                    };
                }).ToList();
            }
            else
            {
                return RedirectToAction("Index");
            }
            
            var streamingResponse = new StreamingAPI();
            var streamingResult = await streamingResponse.Get(title);

            if (streamingResult != null ) 
            {
                // Create a StreamingService object from the JSON response
                var streamingService = StreamingServices.FromJson(streamingResult);

                List<StreamingService> streamingData = streamingService;
                var viewModel = new MovieViewModel()
                {
                    Movie = movieData,
                    Ratings = ratingData,
                    StreamingServices = streamingData
                };

                return View(viewModel);
            }
            else 
            { 
                return RedirectToAction("Index"); 
            }
        }
        public async Task<IActionResult> SaveMovie(MovieViewModel viewModel)
        {
            viewModel.Movie.Id = new Guid();
            var user = await _user.GetUserAsync(User);
            viewModel.Movie.Fk_UserId = user.Id;
            _db.Movies.Add(viewModel.Movie);
            foreach (var rating in viewModel.Ratings)
            {
                rating.Fk_MovieId = viewModel.Movie.Id;
                _db.Ratings.Add(rating);
            }
            foreach (var streamingService in viewModel.StreamingServices)
            {
                streamingService.Fk_MovieId = viewModel.Movie.Id;
                _db.StreamingServices.Add(streamingService);
            }
            _db.SaveChanges();

            //return View(viewModel);
            return RedirectToAction("Index");
        }
    }
}