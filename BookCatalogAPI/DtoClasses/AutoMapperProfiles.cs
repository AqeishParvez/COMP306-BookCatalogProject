using AutoMapper;
using BookCatalogAPI.Models;

namespace BookCatalogAPI.DtoClasses

{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Book, BookDto>();
            CreateMap<CreateBookDto, Book>();
            
        }
    }
}
