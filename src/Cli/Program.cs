using Cli.Models;
using MongoDB.Bson;
using MongoDB.Driver;

// Connecting to MongoDB server
// Can be registered as a signleton in your DI container
var client = new MongoClient("mongodb://root:testdbpass@127.0.0.1:27017/admin");
var database = client.GetDatabase("cip-mongodb");
var collection = database.GetCollection<Event>("Events");

// Creating index
await collection.Indexes.CreateOneAsync(
    new CreateIndexModel<Event>(
        Builders<Event>.IndexKeys.Text(it => it.Title)
    )
);

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

// Bulk operations
var models = new WriteModel<Event>[]
{
    new InsertOneModel<Event>(new Event("50 pesos concert", DateTime.UtcNow.AddDays(7))),
    new ReplaceOneModel<Event>(
        new ExpressionFilterDefinition<Event>(it => it.Id == @event.Id),
        @event with { Title = "Taylor Slow concert" }
    )
};
await collection.BulkWriteAsync(models);

// Aggregate
var eventsAggregate = await collection.Aggregate()
    .Match(it => it.Id == @event.Id)
    .Unwind(it => it.Tickets)
    .Skip(1)
    .Limit(2)
    .Group(BsonDocument.Parse("{ _id: { id: '$_id' }, Tickets: { $push: '$Tickets' } }"))
    .Project(BsonDocument.Parse("{ Tickets: 1 }"))
    .FirstAsync();
var tickets = eventsAggregate[nameof(Event.Tickets)].AsBsonArray
    .Select(it => new Ticket(it["_id"].AsObjectId, it["Client"].AsString))
    .ToList();
Console.WriteLine($"Found tickets: {tickets.Count()}");

// Delete
var deleteResult = await collection.DeleteManyAsync(it => it.StartsAt <= DateTime.UtcNow.AddMonths(1));
Console.WriteLine($"Deleted count: {deleteResult.DeletedCount}");

Console.WriteLine("Done!");