
namespace Backend;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

    var app = builder.Build();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHttpsRedirection();
        app.UseAuthorization();

        // Create CRUD API using Messages from the below code  
        app.MapGet("/messages", () => MessageStore.Messages);
        app.MapGet("/messages/{id}", (int id) => MessageStore.Messages[id]);
        app.MapPost("/messages", (Message message) => {
            MessageStore.Messages.Add(message);
            return message;
        });
        app.MapPut("/messages/{id}", (int id, Message message) => {
            MessageStore.Messages[id] = message;
            return message;
        });
        app.MapDelete("/messages/{id}", (int id) => {
            MessageStore.Messages.RemoveAt(id);
        });

        app.Run();
    }
}

// Generate a record by the name Message which have properties Name and Body
// Path: Backend/Models/Message.cs  
public record Message(string Name, string Body);

// Create a list of Message records and it shall be static
// Path: Backend/Models/MessageStore.cs
public static class MessageStore
{
    public static List<Message> Messages { get; } = new List<Message>();
}