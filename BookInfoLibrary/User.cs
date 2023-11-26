using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookInfoLibrary
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string UserName { get; set; }

        public List<UserBookData> UserBookDataList { get; set; } = new List<UserBookData>();
        // Other user-related properties to be added later

    }
}