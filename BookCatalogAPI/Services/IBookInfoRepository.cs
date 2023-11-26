using BookInfoLibrary;

public interface IBookInfoRepository
{
    Task<Book> GetBookByIdAsync(string bookId);
    Task<IEnumerable<Book>> GetAllBooksAsync();
    Task AddBookAsync(Book book);
    Task UpdateBookAsync(string bookId, Book book);
    Task DeleteBookAsync(string bookId);
}

