namespace BookCatalogAPI.DtoClasses
{
    public class BookUpdateDto
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string PdfFilePath { get; set; }
        public int PageCount { get; set; }
        public IFormFile PdfFile { get; set; }
    }

}
