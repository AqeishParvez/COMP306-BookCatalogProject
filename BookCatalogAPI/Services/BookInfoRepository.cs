using BookInfoLibrary;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

public class BookInfoRepository : IBookInfoRepository
{
    private readonly IMongoCollection<Book> _bookCollection;

    public BookInfoRepository(IMongoDatabase database)
    {
        _bookCollection = database.GetCollection<Book>("books");
    }

    public async Task<Book> GetBookByIdAsync(string bookId)
    {
        var filter = Builders<Book>.Filter.Eq(b => b.Id, bookId);
        return await _bookCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Book>> GetAllBooksAsync()
    {
        return await _bookCollection.Find(_ => true).ToListAsync();
    }

    public async Task AddBookAsync(Book book)
    {
        await _bookCollection.InsertOneAsync(book);
    }

    public async Task UpdateBookAsync(string bookId, Book book)
    {
        var filter = Builders<Book>.Filter.Eq(b => b.Id, bookId);
        await _bookCollection.ReplaceOneAsync(filter, book);
    }

    public async Task DeleteBookAsync(string bookId)
    {
        var filter = Builders<Book>.Filter.Eq(b => b.Id, bookId);
        await _bookCollection.DeleteOneAsync(filter);
    }
}
