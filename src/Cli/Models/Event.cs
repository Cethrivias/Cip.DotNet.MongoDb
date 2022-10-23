using MongoDB.Bson;

namespace Cli.Models;

// Id, id, _id
public record Event(ObjectId Id, string Title, DateTime StartsAt)
{
    public Event(string title, DateTime startsAt) : this(ObjectId.GenerateNewId(), title, startsAt)
    {
    }
};