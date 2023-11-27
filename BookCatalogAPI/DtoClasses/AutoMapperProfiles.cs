using AutoMapper;
using BookInfoLibrary;

namespace BookCatalogAPI.DtoClasses

{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Book, BookDto>();
            CreateMap<BookDto, Book>();

            CreateMap<BookCreateDto, Book>();
            CreateMap<Book, BookCreateDto>();

            CreateMap<Book, BookUpdateDto>();
            CreateMap<BookUpdateDto, Book>();

        }
    }
}
