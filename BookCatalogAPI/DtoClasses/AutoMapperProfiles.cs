using AutoMapper;
using BookCatalogAPI.Models;

namespace BookCatalogAPI.DtoClasses

{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Book, BookDto>();
            CreateMap<BookDto, Book>();

            CreateMap<CreateBookDto, Book>();
            CreateMap<Book, CreateBookDto>();

            CreateMap<Book, UpdateBookDto>();
            CreateMap<UpdateBookDto, Book>();

        }
    }
}
