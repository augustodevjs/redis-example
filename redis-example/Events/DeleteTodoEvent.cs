using MediatR;

namespace redis_example.Events;

public record DeleteTodoEvent : INotification
{
    public int Id { get; init; }
}