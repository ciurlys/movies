using AutoMapper;
using Movies.EntityModels;
using Movies.Models;

namespace Movies.Mappings;

public class MovieMappingProfile : Profile
{
    public MovieMappingProfile()
    {
        CreateMap<OmdbApiResponse, Movie>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Director, opt => opt.MapFrom(src => src.Director))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Plot))
            .ForMember(dest => dest.ImagePath, opt => opt.MapFrom(src => src.Poster))
            .ForMember(dest => dest.ReleaseDate, opt =>
            opt.MapFrom(src => new DateOnly(int.Parse(src.Year ?? "0"), 1, 1)));
    }


}
