using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookCatalogAPI.Models;
using BookCatalogAPI.RepositoryInterface;
using AutoMapper;
using BookCatalogAPI.DtoClasses;
using static System.Reflection.Metadata.BlobBuilder;

namespace BookCatalogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;

        public BooksController(IBookRepository bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        // GET: api/Books
        [HttpGet]
        // return type is a collection of BookDto type rather the Book type
        public ActionResult<IEnumerable<BookDto>> GetBooks()
        {
            // get collection of book type, then map them to the BookDto type
            var books = _bookRepository.GetAllBooks();
            var bookDtos = _mapper.Map<IEnumerable<BookDto>>(books);

            // return BookDto type
            return Ok(bookDtos);
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public ActionResult<BookDto> GetBook(int id)
        {
            var book = _bookRepository.GetBookById(id);

            if (book == null)
            {
                return NotFound();
            }
            var bookDto = _mapper.Map<BookDto>(book);
            return Ok(bookDto);
        }

        // PUT: api/Books/5
        [HttpPut("{id}")]
        public IActionResult PutBook(int id, Book book)
        {
            if (id != book.BookId)
            {
                return BadRequest();
            }

            _bookRepository.UpdateBook(book);
            return NoContent();
        }

        // POST: api/Books
        [HttpPost]
        public ActionResult<BookDto> PostBook(CreateBookDto createBookDto)
        {
            var newBook = _mapper.Map<Book>(createBookDto);
            _bookRepository.AddBook(newBook);

            var newBookDto = _mapper.Map<BookDto>(newBook);

            return CreatedAtAction(nameof(GetBook), new { id = newBookDto.BookId }, newBookDto);
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public IActionResult DeleteBook(int id)
        {
            _bookRepository.DeleteBook(id);
            return NoContent();
        }

        private bool BookExists(int id)
        {
            return (_bookRepository.GetBookById(id) != null);
        }
    }
}
