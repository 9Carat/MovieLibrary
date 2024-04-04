using AutoMapper;
using MovieLibrary.Models.DTO;
using MovieLibrary.Models;

namespace MovieLibrary
{
    public class MappingConfig : Profile
    {
        public MappingConfig() 
        {
            CreateMap<Movie, MovieCreateDTO>().ReverseMap();
            CreateMap<Movie, MovieUpdateDTO>().ReverseMap();
            CreateMap<Rating, RatingCreateDTO>().ReverseMap();
            CreateMap<StreamingService, StreamingServiceCreateDTO>().ReverseMap();
        }
    }
}
