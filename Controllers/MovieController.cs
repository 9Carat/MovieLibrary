using Microsoft.AspNetCore.Mvc;
using MovieLibrary.API;
using MovieLibrary.APIComponents;
using MovieLibrary.Models;
using Newtonsoft.Json;
using System.Security.Principal;

namespace MovieLibrary.Controllers
{
    public class MovieController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Search()
        {
            //Call the OMDb API for general information
            string title = "inception";
            var OMDbResponse = new OMDbAPI();
            var OMDbResult = await OMDbResponse.Get(title);
            var movieData = JsonConvert.DeserializeObject<Movie>(OMDbResult);

            if (movieData != null ) 
            {
                movieData.Ratings = movieData.Ratings.Select(rating =>
                {
                    return new Rating
                    {
                        Id = Guid.NewGuid(),
                        Source = rating.Source,
                        Value = rating.Value,
                        Fk_MovieId = movieData.Id
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

                // Pass both the movieData and streamingService to the view
                var viewModel = new MovieViewModel()
                {
                    Movie = movieData,
                    Streaming = streamingService
                };
                return View(viewModel);
            }
            else 
            { 
                return RedirectToAction("Index"); 
            }
        }
    }
}