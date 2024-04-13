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

namespace MovieLibrary.Controllers
{
    public class MovieController : Controller
    {
        private readonly UserManager<IdentityUser> _user;
        private readonly IMovieService _movieService;
        private readonly IMapper _mapper;
        public MovieController(UserManager<IdentityUser> user, IMovieService movieService, IMapper mapper)
        {
            _user = user;
            _movieService = movieService;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _user.GetUserAsync(User);
            
            if (user != null)
            {
                var response = await _movieService.GetByUserIdAsync<ApiResponse>(user.Id);
                if (response.IsSuccess)
                {
                    List<Movie> movieList = JsonConvert.DeserializeObject<List<Movie>>(Convert.ToString(response.Result));
                    return View(movieList);
                }
            }
            return View(); 
        }

        public async Task<IActionResult> Details(Guid movieId)
        {
            var response = await _movieService.GetByMovieIdAsync<ApiResponse>(movieId);
            if (response.IsSuccess)
            {
                var movie = JsonConvert.DeserializeObject<Movie>(Convert.ToString(response.Result));
                return View(movie);
            }
            else return View("Error");
        }

        public async Task<IActionResult> ImageAI(Guid movieId)
        {
            var response = await _movieService.GetByMovieIdAsync<ApiResponse>(movieId);
            var movie = JsonConvert.DeserializeObject<Movie>(Convert.ToString(response.Result));

            if (movie != null && response.IsSuccess)
            {
                string title = movie.Title;
                var edenAI = new EdenAI();
                var edenResult = await edenAI.Get(title);
                var edenResponse = JsonConvert.DeserializeObject<dynamic>(edenResult);
                string imgUrl = edenResponse[0].items[0].image_resource_url;

                // Download image
                byte[] imageBytes;
                using (var httpClient = new HttpClient())
                {
                    imageBytes = await httpClient.GetByteArrayAsync(imgUrl);
                }

                // Store image locally
                string fileName = $"{movieId.ToString() + DateTime.Now.ToString("yymmssfff")}.jpg";
                string imagePath = Path.Combine("wwwroot", "images", "posters", fileName);
                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    await fileStream.WriteAsync(imageBytes);
                }

                movie.Poster = $"/images/posters/{fileName}";
                var movieDto = _mapper.Map<MovieUpdateDTO>(movie);
                await _movieService.UpdateAsync<ApiResponse>(movieDto);
                return View("Details", movie);
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

            var viewModel = new MovieViewModel() { Movie = movieData, Ratings = ratingData};

            // Call Streaming Availability API
            var streamingResponse = new StreamingAPI();
            var streamingResult = await streamingResponse.Get(title);

            if (streamingResult != null)
            {
                // Create a StreamingService object from the JSON response
                var streamingService = StreamingServices.DeserializeJSON(streamingResult);

                List<StreamingService> streamingData = streamingService;
                viewModel.StreamingServices = streamingData;
            }
            return View(viewModel);
        }


        public async Task<IActionResult> SaveMovie(MovieViewModel viewModel)
        {
            var user = await _user.GetUserAsync(User);
            MovieCreateDTO movieDto = _mapper.Map<MovieCreateDTO>(viewModel.Movie);
            movieDto.Id = Guid.NewGuid();
            movieDto.Fk_UserId = user.Id;

            if (viewModel.Ratings !=  null)
            {
                var ratings = _mapper.Map<List<RatingCreateDTO>>(viewModel.Ratings);
                foreach (var rating in ratings)
                {
                    rating.Id = Guid.NewGuid();
                    rating.Fk_MovieId = movieDto.Id;
                }
                movieDto.Ratings = ratings;
            }

            if (viewModel.StreamingServices != null)
            {
                var streamingServices = _mapper.Map<List<StreamingServiceCreateDTO>>(viewModel.StreamingServices);

                foreach (var streamingService in streamingServices)
                {
                    streamingService.Id = Guid.NewGuid();
                    streamingService.Fk_MovieId = movieDto.Id;
                }

                movieDto.StreamingServices = streamingServices;
            }

            await _movieService.CreateMovieAsync<ApiResponse>(movieDto);
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Remove(Guid movieId)
        {
            await _movieService.DeleteAsync<ApiResponse>(movieId);
            return RedirectToAction("Index");
        }
    }
}