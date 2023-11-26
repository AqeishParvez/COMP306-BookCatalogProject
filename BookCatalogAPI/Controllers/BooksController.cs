using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookInfoLibrary;
using BookCatalogAPI.RepositoryInterface;
using AutoMapper;
using BookCatalogAPI.DtoClasses;
using static System.Reflection.Metadata.BlobBuilder;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.AspNetCore.JsonPatch;

namespace BookCatalogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookInfoRepository _bookInfoRepository;
        private readonly IMapper _mapper;

        public BooksController(IBookInfoRepository bookInfoRepository, IMapper mapper)
        {
            _bookInfoRepository = bookInfoRepository;
            _mapper = mapper;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetAllBooks()
        {
            var books = await _bookInfoRepository.GetAllBooksAsync();
            var bookDtos = _mapper.Map<IEnumerable<BookDto>>(books);

            return Ok(bookDtos);
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetBookById(string bookId)
        {
            var book = await _bookInfoRepository.GetBookByIdAsync(bookId);

            if (book == null)
            {
                return NotFound();
            }

            var bookDto = _mapper.Map<BookDto>(book);

            return Ok(bookDto);
        }

        // PUT: api/Books/5
        [HttpPut("{id}")]
        public async Task<ActionResult<BookDto>> UpdateBook(string bookId, [FromBody] BookUpdateDto bookUpdateDto)
        {
            var existingBook = await _bookInfoRepository.GetBookByIdAsync(bookId);

            if (existingBook == null)
            {
                return NotFound();
            }

            _mapper.Map(bookUpdateDto, existingBook);

            await _bookInfoRepository.UpdateBookAsync(bookId, existingBook);

            var updatedBookDto = _mapper.Map<BookDto>(existingBook);

            return Ok(updatedBookDto);
        }


        [HttpPatch("{id}")]
        public async Task<ActionResult<BookDto>> PatchBook(string bookId, [FromBody] BookUpdateDto bookUpdateDto)
        {
            if (bookUpdateDto == null)
            {
                return BadRequest("Invalid request body");
            }

            var existingBook = await _bookInfoRepository.GetBookByIdAsync(bookId);

            if (existingBook == null)
            {
                return NotFound();
            }

            // Apply patch operations manually
            if (!ApplyPatchOperations(bookUpdateDto, existingBook))
            {
                return BadRequest("Invalid patch operations");
            }

            // Validate the updated entity
            if (!TryValidateModel(existingBook))
            {
                return ValidationProblem(ModelState);
            }

            // Update the entity in the repository
            await _bookInfoRepository.UpdateBookAsync(bookId, existingBook);

            // Map the updated book to BookDto for the response
            var updatedBookDto = _mapper.Map<BookDto>(existingBook);

            return Ok(updatedBookDto);
        }

        private bool ApplyPatchOperations(BookUpdateDto updatedBookDto, Book existingBook)
        {
            bool isPatched = false;

            // Apply patch operations manually
            // Example: Only handle "replace" operation for specified properties
            if (updatedBookDto.Title != null)
            {
                existingBook.Title = updatedBookDto.Title;
                isPatched = true;
            }

            if (updatedBookDto.Author != null)
            {
                existingBook.Author = updatedBookDto.Author;
                isPatched = true;
            }

            if (updatedBookDto.PdfFilePath != null)
            {
                existingBook.PdfFilePath = updatedBookDto.PdfFilePath;
                isPatched = true;
            }

            if (updatedBookDto.PageCount > 0)
            {
                existingBook.PageCount = updatedBookDto.PageCount;
                isPatched = true;
            }

            // Add more operations for other properties as needed

            // Return true if patch operations were successfully applied
            return isPatched;
        }




        // POST: api/Books
        [HttpPost]
        public async Task<ActionResult<BookDto>> AddBook([FromBody] BookCreateDto bookCreateDto)
        {
            var book = _mapper.Map<Book>(bookCreateDto);
            await _bookInfoRepository.AddBookAsync(book);

            var bookDto = _mapper.Map<BookDto>(book);

            return CreatedAtAction(nameof(GetBookById), new { bookId = bookDto.Id }, bookDto);
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(string bookId)
        {
            var existingBook = await _bookInfoRepository.GetBookByIdAsync(bookId);

            if (existingBook == null)
            {
                return NotFound();
            }

            await _bookInfoRepository.DeleteBookAsync(bookId);

            return NoContent();
        }
    }
}
