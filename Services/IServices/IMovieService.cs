using MovieLibrary.Models.DTO;

namespace MovieLibrary.Services.IServices
{
    public interface IMovieService
    {
        Task<T> GetByMovieIdAsync<T>(Guid id);
        Task<T> GetByUserIdAsync<T>(string id);
        Task<T> CreateMovieAsync<T>(MovieCreateDTO dto);
        Task<T> UpdateAsync<T>(MovieUpdateDTO dto);
        Task<T> DeleteAsync<T>(Guid id);
    }
}
