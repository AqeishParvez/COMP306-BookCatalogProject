using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookCatalogAPI.Models;

namespace BookCatalogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;

        public BooksController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        // GET: api/Books
        [HttpGet]
        public ActionResult<IEnumerable<Book>> GetBooks()
        {
            var books = _bookRepository.GetAllBooks();
            return Ok(books);
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public ActionResult<Book> GetBook(int id)
        {
            var book = _bookRepository.GetBookById(id);

            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
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
        public ActionResult<Book> PostBook(Book book)
        {
            _bookRepository.AddBook(book);
            return CreatedAtAction("GetBook", new { id = book.BookId }, book);
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
