using Cli.Models;
using MongoDB.Driver;

// Connecting to MongoDB server
// Can be registered as a signleton in your DI container
var client = new MongoClient("mongodb://root:testdbpass@127.0.0.1:27017/admin");
var database = client.GetDatabase("cip-mongodb");
var collection = database.GetCollection<Event>("Events");

// Create
var @event = new Event("Ted Heeran concert", DateTime.UtcNow.AddDays(7));
await collection.InsertOneAsync(@event);

// Read
var readEvent = await collection.Find(it => it.Id == @event.Id).FirstOrDefaultAsync();
Console.WriteLine($"Read event: {readEvent}");

// Update
var updatedEvent = readEvent with { Title = "Taylor Slow concert" };
var replaceResult = await collection.ReplaceOneAsync(it => it.Id == @event.Id, updatedEvent);
Console.WriteLine($"Replace result: {replaceResult.MatchedCount}");

// Delete
var deleteResult = await collection.DeleteManyAsync(it => it.StartsAt <= DateTime.UtcNow.AddMonths(1));
Console.WriteLine($"Deleted count: {deleteResult.DeletedCount}");

Console.WriteLine("Done!");