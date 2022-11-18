using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Uppgift2.Data
{
    public class Blog
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; }

        public string? AuthorId { get; set; }

        public string Title { get; set; }

        public string AuthorName { get; set; }

        public DateTime DateTimeCreated { get; set; }

        public string HtmlText { get; set; }

        public Blog()
        {
            AuthorName = "";
            Title = "";
            HtmlText = "";
        }
    }
}