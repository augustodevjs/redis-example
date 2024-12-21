using MediatR;

namespace redis_example.Events;

public record CreatedTodoEvent : INotification
{
    public string Title { get; init; }
    public string Description { get; init; }
}