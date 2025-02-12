using JokeApiWebApp.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace JokeApiWebApp.Data
{
    public class JokeCRUD
    {
        private IMongoDatabase db;

        public JokeCRUD(string database)
        {
            //var connectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");

            //if (string.IsNullOrEmpty(connectionString))
            //{
            //    throw new Exception("MongoDB connection string is missing.");
            //}

            // Skapa MongoDB-klienten och anslut till databasen
            var client = new MongoClient("mongodb+srv://muratbuyuksal:12345admin@muratsapi.rn37f.mongodb.net/?retryWrites=true&w=majority");
            db = client.GetDatabase(database);
        }


        //CREATE Joke
        public async Task<Jokes> AddJoke(string collectionName, Jokes joke)
        {
            if (joke == null || string.IsNullOrWhiteSpace(joke.Question) || string.IsNullOrWhiteSpace(joke.Answer))
            {
                throw new ArgumentException("Joke must have a question and an answer.");
            }

            var collection = db.GetCollection<Jokes>(collectionName);

            await collection.InsertOneAsync(joke);
            return joke;
        }


        //READ All Jokes
        public async Task<List<Jokes>> GetAllJokes(string collectionName)
        {
            var collection = db.GetCollection<Jokes>(collectionName);
            var jokes = await collection.AsQueryable().ToListAsync();

            if (jokes == null || !jokes.Any())
            {
                throw new KeyNotFoundException("No jokes found in the collection.");
            }

            return jokes;
        }


        //READ Joke By Id
        public async Task<Jokes> GetJokeById(string collectionName, string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                throw new ArgumentException("Invalid ID format. Must be a valid MongoDB ObjectId.");
            }

            var collection = db.GetCollection<Jokes>(collectionName);
            var joke = await collection.Find(x => x.Id == id).FirstOrDefaultAsync();

            if (joke == null)
            {
                throw new KeyNotFoundException($"No joke found with ID: {id}");
            }

            return joke;
        }


        //UPDATE joke
        public async Task<bool> UpdateJoke(string collectionName, string id, Jokes updatedJoke)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                throw new ArgumentException("Invalid ID format.");
            }

            var collection = db.GetCollection<Jokes>(collectionName);

            var result = await collection.ReplaceOneAsync(x => x.Id == id, updatedJoke);

            return result.ModifiedCount > 0;
        }


        //DELETE Joke
        public async Task<bool> DeleteJoke(string collectionName, string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                throw new ArgumentException("Invalid ID format.");
            }

            var collection = db.GetCollection<Jokes>(collectionName);
            var result = await collection.DeleteOneAsync(x => x.Id == id);

            return result.DeletedCount > 0;
        }

    }
}
