namespace BookCatalogClient.Models
{
    public class BookModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string PdfFilePath { get; set; }
        public int PageCount { get; set; }
    }
}
