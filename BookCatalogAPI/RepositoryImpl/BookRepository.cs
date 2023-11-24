namespace BookCatalogAPI.RepositoryImpl
{
    using BookCatalogAPI.Models;
    // BookRepository.cs
    using System.Linq;

    public class BookRepository : IBookRepository
    {
        private readonly APIDbContext _context;

        public BookRepository(APIDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Book> GetAllBooks()
        {
            return _context.Books.ToList();
        }

        public Book GetBookById(int id)
        {
            return _context.Books.FirstOrDefault(b => b.BookId == id);
        }

        public void AddBook(Book book)
        {
            _context.Books.Add(book);
            _context.SaveChanges();
        }

        public void UpdateBook(Book book)
        {
            var existingBook = _context.Books.FirstOrDefault(b => b.BookId == book.BookId);
            if (existingBook != null)
            {
                existingBook.Title = book.Title;
                existingBook.Author = book.Author;

                _context.SaveChanges();
            }
        }

        public void DeleteBook(int id)
        {
            var bookToRemove = _context.Books.FirstOrDefault(b => b.BookId == id);
            if (bookToRemove != null)
            {
                _context.Books.Remove(bookToRemove);
                _context.SaveChanges();
            }
        }
    }
}
