using MovieLibrary.Models;

namespace MovieLibrary.Services.IServices
{
    public interface IBaseService
    {
        ApiResponse response { get; set; }
        Task<T> SendAsync<T>(ApiRequest apiRequest);
    }
}
