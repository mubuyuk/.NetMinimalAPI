using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace JokeApiWebApp.Models
{
    public class Jokes
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string Category { get; set; }
    }
}
