namespace BookCatalogAPI.DtoClasses
{
    // This DTO class represents data transferred to the client
    public class BookDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int PageCount { get; set; }
        
    }

}
