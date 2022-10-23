using MongoDB.Bson;

namespace Cli.Models;

public record Ticket(ObjectId Id, string Client)
{
    public Ticket(string client) : this(ObjectId.GenerateNewId(), client)
    {
    }
}