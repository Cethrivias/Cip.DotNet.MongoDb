using MongoDB.Bson;
using MongoDB.Driver;

// Connecting to MongoDB server
// Can be registered as a signleton in your DI container
var client = new MongoClient("mongodb://root:testdbpass@127.0.0.1:27017/admin");
var database = client.GetDatabase("cip-mongodb");
var collection = database.GetCollection<BsonDocument>("Events");

await collection.InsertOneAsync(new BsonDocument()
{
    { "_id", ObjectId.GenerateNewId() },
    { "Title", "Ted Heeran concert" }
});

Console.WriteLine("Done!");