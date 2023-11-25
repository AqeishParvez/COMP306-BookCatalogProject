namespace BookCatalogAPI.DtoClasses
{
    // This DTO class represents the data transferred from the client for creating a book object
    public class CreateBookDto
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Isbn { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
    }
}
