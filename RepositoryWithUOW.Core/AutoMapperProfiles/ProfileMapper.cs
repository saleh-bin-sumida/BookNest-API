using AutoMapper;
using RepositoryWithUOW.Core.DTOs;
using RepositoryWithUOW.Core.Models;


namespace RepositoryWithUOW.Core.AutoMapperProfiles;

public class ProfileMapper : Profile
{
    public ProfileMapper()
    {
        CreateMap<Author, AuthorDTO>().ReverseMap();
        CreateMap<Book, BookDTO>().ReverseMap();
    }
}
