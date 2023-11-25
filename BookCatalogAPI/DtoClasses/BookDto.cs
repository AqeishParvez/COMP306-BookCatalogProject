namespace BookCatalogAPI.DtoClasses
{
    // This DTO class represents data transferred to the client
    public class BookDto
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Isbn { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
    }
}
