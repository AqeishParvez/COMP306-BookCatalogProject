using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BookInfoLibrary
{
    public class Book
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Title { get; set; }
        public string Author { get; set; }
        public string PdfFilePath { get; set; }
        public int PageCount { get; set; }

        [BsonIgnore] // Ignore this property in MongoDB
        public IFormFile PdfFile { get; set; }

        // Other book-related properties to be added as needed
    }
}
