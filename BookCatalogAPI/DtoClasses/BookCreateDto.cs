namespace BookCatalogAPI.DtoClasses
{
    // This DTO class represents the data transferred from the client for creating a book object
    public class BookCreateDto
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string PdfFilePath { get; set; }
        public int PageCount { get; set; }

        public IFormFile PdfFile { get; set; }
    }
}
