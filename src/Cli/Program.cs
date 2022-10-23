using Cli.Models;
using MongoDB.Driver;

// Connecting to MongoDB server
// Can be registered as a signleton in your DI container
var client = new MongoClient("mongodb://root:testdbpass@127.0.0.1:27017/admin");
var database = client.GetDatabase("cip-mongodb");
var collection = database.GetCollection<Event>("Events");

// Create
var @event = new Event("Ted Heeran concert", DateTime.UtcNow.AddDays(7));
@event.Tickets.AddRange(new[]
{
    new Ticket("Dora the Explorer"),
    new Ticket("Dora the Destroyer"),
    new Ticket("Dora the Destroyer"),
});
await collection.InsertOneAsync(@event);

// Read
var readEvent = await collection.Find(it => it.Id == @event.Id).FirstOrDefaultAsync();
Console.WriteLine($"Read event: {readEvent}");

// Update
var filter = Builders<Event>.Filter;
var eventAndClientFilter = filter.And(
    filter.Eq(it => it.Id, @event.Id),
    filter.ElemMatch(it => it.Tickets, t => t.Client == "Dora the Destroyer")
);
var update = Builders<Event>.Update.Set(it => it.Tickets[-1].Client, "Dora the Pacifist");
var updateResult = await collection.UpdateOneAsync(eventAndClientFilter, update);
Console.WriteLine($"Replace result: {updateResult.MatchedCount}");

// Delete
var deleteResult = await collection.DeleteManyAsync(it => it.StartsAt <= DateTime.UtcNow.AddMonths(1));
Console.WriteLine($"Deleted count: {deleteResult.DeletedCount}");

Console.WriteLine("Done!");