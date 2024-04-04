using MovieLibrary.Models;
using MovieLibrary.Models.DTO;
using MovieLibrary.Services.IServices;
using MovieLibrary.Utility;

namespace MovieLibrary.Services
{
    public class MovieService : BaseService, IMovieService
    {
        private string _movieUrl;
        public MovieService(IHttpClientFactory httpClient, IConfiguration config) : base(httpClient)
        {
            this.httpClient = httpClient;
            this._movieUrl = config.GetValue<string>("ServiceUrls:MovieLibraryAPI");
        }

        public Task<T> CreateMovieAsync<T>(MovieCreateDTO dto)
        {
            return SendAsync<T>(apiRequest: new ApiRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = dto,
                ApiUrl = this._movieUrl + "/movie"
            });
        }

        public Task<T> GetByMovieIdAsync<T>(Guid id)
        {
            return SendAsync<T>(apiRequest: new ApiRequest()
            {
                ApiType = SD.ApiType.GET,
                ApiUrl = this._movieUrl + "/movie/movieid/" + id
            });
        }

        public Task<T> GetByUserIdAsync<T>(string id)
        {
            return SendAsync<T>(apiRequest: new ApiRequest()
            {
                ApiType = SD.ApiType.GET,
                ApiUrl = this._movieUrl + "/movie/user/" + id
            });
        }
        public Task<T> UpdateAsync<T>(MovieUpdateDTO dto)
        {
            return SendAsync<T>(apiRequest: new ApiRequest()
            {
                ApiType = SD.ApiType.PUT,
                Data = dto,
                ApiUrl = this._movieUrl + "/movie/" + dto.Id
            });
        }

        public Task<T> DeleteAsync<T>(Guid id)
        {
            return SendAsync<T>(apiRequest: new ApiRequest()
            {
                ApiType = SD.ApiType.DELETE,
                ApiUrl = this._movieUrl + "/movie/" + id
            });
        }
    }
}
