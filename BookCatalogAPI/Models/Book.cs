using System.ComponentModel.DataAnnotations;

namespace BookCatalogAPI.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }

        [ Required]
        public string Title { get; set; } = "Unknown";
        public string Isbn { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }

    }
}
