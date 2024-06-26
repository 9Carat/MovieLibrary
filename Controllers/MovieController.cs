﻿using AutoMapper;
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
        private readonly UserManager<IdentityUser> _user;
        private readonly IMovieService _movieService;
        private readonly IMapper _mapper;
        IWebHostEnvironment _hostingEnvironment;
        public MovieController(UserManager<IdentityUser> user, IMovieService movieService, IMapper mapper, IWebHostEnvironment hostingEnvironment)
        {
            _user = user;
            _movieService = movieService;
            _mapper = mapper;
            _hostingEnvironment = hostingEnvironment;
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

                // Download image from Eden API response
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
                    var movieDto = _mapper.Map<MovieUpdateDTO>(movie);
                    await _movieService.UpdateAsync<ApiResponse>(movieDto);
                }
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