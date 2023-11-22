using Microsoft.EntityFrameworkCore;

namespace BookCatalogAPI.Models
{
    public class APIDbContext:DbContext
    {
        public APIDbContext(DbContextOptions<APIDbContext> options):base(options)
        {
        }
        public DbSet<Book> Books { get; set; }
    }
}
