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
using MongoDB.Bson;
using Amazon.S3;

namespace BookCatalogAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookInfoRepository _bookInfoRepository;
        private readonly IMapper _mapper;
        private readonly IAmazonS3 _s3Client;

        public BooksController(IBookInfoRepository bookInfoRepository, IMapper mapper, IAmazonS3 s3Client)
        {
            _bookInfoRepository = bookInfoRepository;
            _mapper = mapper;
            _s3Client = s3Client;
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
        [HttpGet("{Id}")]
        public async Task<ActionResult<BookDto>> GetBookById(string Id)
        {
            var book = await _bookInfoRepository.GetBookByIdAsync(Id);

            if (book == null)
            {
                return NotFound();
            }

            var bookDto = _mapper.Map<BookDto>(book);

            return Ok(bookDto);
        }

        // PUT: api/Books/5
        [HttpPut("{Id}")]
        public async Task<ActionResult<BookDto>> UpdateBook(string Id, [FromBody] BookUpdateDto bookUpdateDto, IFormFile pdfFile)
        {
            var existingBook = await _bookInfoRepository.GetBookByIdAsync(Id);

            if (existingBook == null)
            {
                return NotFound();
            }

            _mapper.Map(bookUpdateDto, existingBook);

            if (pdfFile != null && pdfFile.Length > 0)
            {
                // Upload the new file to S3
                var s3Key = $"books/{ObjectId.GenerateNewId()}.pdf"; // Adjust the key as needed
                using (var fileStream = pdfFile.OpenReadStream())
                {
                    var putObjectRequest = new Amazon.S3.Model.PutObjectRequest
                    {
                        BucketName = "bookreadingbucket",
                        Key = s3Key,
                        InputStream = fileStream
                    };

                    await _s3Client.PutObjectAsync(putObjectRequest);
                }

                existingBook.PdfFilePath = s3Key; // Update the S3 key in the database
            }

            await _bookInfoRepository.UpdateBookAsync(Id, existingBook);

            var updatedBookDto = _mapper.Map<BookDto>(existingBook);

            return Ok(updatedBookDto);
        }



        [HttpPatch("{Id}")]
        public async Task<ActionResult<BookDto>> PatchBook(string Id, [FromBody] BookUpdateDto bookUpdateDto)
        {
            if (bookUpdateDto == null)
            {
                return BadRequest("Invalid request body");
            }

            var existingBook = await _bookInfoRepository.GetBookByIdAsync(Id);

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
            await _bookInfoRepository.UpdateBookAsync(Id, existingBook);

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

            // Return true if patch operations were successfully applied
            return isPatched;
        }




        // POST: api/Books
        [HttpPost]
        public async Task<ActionResult<BookDto>> AddBook([FromBody] BookCreateDto bookCreateDto, IFormFile pdfFile)
        {
            if (pdfFile == null || pdfFile.Length == 0)
            {
                return BadRequest("Pdf file is required.");
            }

            var book = _mapper.Map<Book>(bookCreateDto);

            // Upload the file to S3
            var s3Key = $"book/{ObjectId.GenerateNewId()}.pdf";
            using (var fileStream = pdfFile.OpenReadStream())
            {
                var putObjectRequest = new Amazon.S3.Model.PutObjectRequest
                {
                    BucketName = "bookreadingbucket",
                    Key = s3Key,
                    InputStream = fileStream
                };

                await _s3Client.PutObjectAsync(putObjectRequest);
            }

            book.PdfFilePath = s3Key; // Save the S3 key in the database

            await _bookInfoRepository.AddBookAsync(book);

            var bookDto = _mapper.Map<BookDto>(book);

            return CreatedAtAction(nameof(GetBookById), new { id = bookDto.Id }, bookDto);
        }

        // DELETE: api/Books/5
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteBook(string Id)
        {
            var existingBook = await _bookInfoRepository.GetBookByIdAsync(Id);

            if (existingBook == null)
            {
                return NotFound();
            }

            // Delete the file from S3
            if (!string.IsNullOrEmpty(existingBook.PdfFilePath))
            {
                var deleteObjectRequest = new Amazon.S3.Model.DeleteObjectRequest
                {
                    BucketName = "bookreadingbucket",
                    Key = existingBook.PdfFilePath
                };

                await _s3Client.DeleteObjectAsync(deleteObjectRequest);
            }

            await _bookInfoRepository.DeleteBookAsync(Id);

            return NoContent();
        }
    }
}
