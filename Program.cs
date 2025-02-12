
using JokeApiWebApp.Data;
using JokeApiWebApp.Models;
using MongoDB.Bson;

namespace JokeApiWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            JokeCRUD db = new JokeCRUD("JokesApi");

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            //POST
            app.MapPost("/joke", async (Jokes joke) =>
            {
                if (joke == null)
                {
                    return Results.BadRequest("Invalid joke data.");
                }

                if (string.IsNullOrWhiteSpace(joke.Question) || string.IsNullOrWhiteSpace(joke.Answer))
                {
                    return Results.BadRequest("Both question and answer are required.");
                }

                var addJoke = await db.AddJoke("Jokes", joke);
                return Results.Ok(addJoke);
            });


            //GET
            app.MapGet("/jokes", async () =>
            {
                try
                {
                    var getAllJokes = await db.GetAllJokes("Jokes");
                    if (getAllJokes == null || !getAllJokes.Any())
                    {
                        return Results.NotFound("No jokes found.");
                    }

                    return Results.Ok(getAllJokes);
                }
                catch (Exception ex)
                {
                    return Results.Problem($"An error occurred while fetching jokes: {ex.Message}");
                }
            });


            //GET By Id
            app.MapGet("/joke/{id}", async (string id) =>
            {
                if (!ObjectId.TryParse(id, out _))
                {
                    return Results.BadRequest("Invalid ID format. Must be a valid MongoDB ObjectId.");
                }

                var getJokeById = await db.GetJokeById("Jokes", id);

                if (getJokeById == null)
                {
                    return Results.NotFound("No joke was found with the given ID.");
                }

                return Results.Ok(getJokeById);
            });


            //PUT
            app.MapPut("/joke/{id}", async (string id, Jokes updatedJoke) =>
            {
                if (!ObjectId.TryParse(id, out _))
                {
                    return Results.BadRequest("Invalid ID format. Must be a valid MongoDB ObjectId.");
                }

                if (updatedJoke == null || string.IsNullOrWhiteSpace(updatedJoke.Question) || string.IsNullOrWhiteSpace(updatedJoke.Answer))
                {
                    return Results.BadRequest("Both question and answer are required for update.");
                }

                var success = await db.UpdateJoke("Jokes", id, updatedJoke);

                if (!success)
                {
                    return Results.NotFound($"No joke found with ID: {id}");
                }

                return Results.Ok(updatedJoke);
            });


            //DELETE
            app.MapDelete("/joke/{id}", async (string id) =>
            {
                if (!ObjectId.TryParse(id, out _))
                {
                    return Results.BadRequest("Invalid ID format. Must be a valid MongoDB ObjectId.");
                }

                var success = await db.DeleteJoke("Jokes", id);

                if (!success)
                {
                    return Results.NotFound($"No joke found with ID: {id}");
                }

                return Results.Ok("Succesfully deleted!");
            });

            //dotnet run --urls "http://localhost:5252"


            app.Run();
        }
    }
}
