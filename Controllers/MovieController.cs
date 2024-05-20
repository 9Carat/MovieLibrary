using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using MovieLibrary.API;
using MovieLibrary.APIComponents;
using MovieLibrary.Data;
using MovieLibrary.Models;
using MovieLibrary.Models.DTO;
using MovieLibrary.Services.IServices;
using Newtonsoft.Json;
using System.Security.Principal;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace MovieLibrary.Controllers
{
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _user;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public MovieController(ApplicationDbContext db, UserManager<IdentityUser> user, IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _user = user;
            _hostingEnvironment = hostingEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _user.GetUserAsync(User);
            if (user != null)
            {
                IQueryable<Movie> movies = _db.Movies.Where(m => m.Fk_UserId == user.Id).Include(m => m.Ratings).Include(m => m.StreamingServices);
                List<Movie> movieList = await movies.ToListAsync();
                return View(movieList);
            }
            else return View();
        }

        public IActionResult Details(Guid MovieId)
        {
            var movie = _db.Movies.Where(m => m.Id == MovieId).Include(m => m.Ratings).Include(m => m.StreamingServices).FirstOrDefault();
            return View(movie);
        }

        public async Task<IActionResult> ImageAI(Guid movieId)
        {
            var movie = _db.Movies.Where(m => m.Id == movieId).Include(m => m.Ratings).Include(m => m.StreamingServices).FirstOrDefault();

            if (movie != null)
            {
                string title = movie.Title;
                var EdenResponse = new EdenAI();
                var EdenResult = await EdenResponse.Get(title);
                var response = JsonConvert.DeserializeObject<dynamic>(EdenResult);
                string imgUrl = response[0].items[0].image_resource_url;

                // Download image
                byte[] imageBytes;
                using (var httpClient = new HttpClient())
                {
                    imageBytes = await httpClient.GetByteArrayAsync(imgUrl);
                }

                // Resize image to match other posters
                using (var image = Image.Load(imageBytes))
                {
                    image.Mutate(x => x
                        .Resize(300, 444));

                    // Store the resized image locally
                    string fileName = $"{movieId.ToString() + DateTime.Now.ToString("yymmssfff")}.jpg";
                    string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images", "posters");

                    // Check if the directory exists, if not create it
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Save the image
                    string imagePath = Path.Combine(uploadsFolder, fileName);
                    using (var fileStream = new FileStream(imagePath, FileMode.Create))
                    {
                        image.Save(fileStream, new JpegEncoder());
                    }

                    movie.Poster = $"/images/posters/{fileName}";
                    _db.Movies.Update(movie);
                    _db.SaveChanges();
                    return View("Details", movie);
                }
            }
            else return View("Error");
        }

        public async Task<IActionResult> SearchResult(string title)
        {

            //Call the OMDb API for general information
            var OMDbResponse = new OMDbAPI();
            var (OMDbResult, isSuccessful) = await OMDbResponse.Get(title);
            var movieData = JsonConvert.DeserializeObject<Movie>(OMDbResult);
            List<Rating> ratingData;

            if (isSuccessful)
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
            else return View("Error");

            // Call Streaming Availability API
            var streamingResponse = new StreamingAPI();
            var streamingResult = await streamingResponse.Get(title);

            if (streamingResult != null)
            {
                // Create a StreamingService object from the JSON response
                var streamingService = StreamingServices.DeserializeJSON(streamingResult);

                List<StreamingService> streamingData = streamingService;
                var viewModel = new MovieViewModel()
                {
                    Movie = movieData,
                    Ratings = ratingData,
                    StreamingServices = streamingData
                };

                return View(viewModel);
            }
            else return RedirectToAction("Index");
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
            if (viewModel.StreamingServices != null)
            {
                foreach (var streamingService in viewModel.StreamingServices)
                {
                    streamingService.Fk_MovieId = viewModel.Movie.Id;
                    _db.StreamingServices.Add(streamingService);
                }
            }

            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Remove(Guid movieId)
        {
            var movie = _db.Movies.Where(m => m.Id == movieId).Include(m => m.Ratings).Include(m => m.StreamingServices).FirstOrDefault();
            _db.Movies.Remove(movie);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}