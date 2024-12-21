using MediatR;
using redis_example.Events;
using redis_example.Abstractions.Caching;

namespace redis_example.EventHandler;

internal class CacheInvalidationTodoHandler 
    : INotificationHandler<DeleteTodoEvent>, INotificationHandler<CreatedTodoEvent>
{
    private const string Key = "todos";
    private readonly ICacheService _cache;

    public CacheInvalidationTodoHandler(ICacheService cache)
    {
        _cache = cache;
    }

    public Task Handle(DeleteTodoEvent notification, CancellationToken cancellationToken)
    {
       return HandleInternal(cancellationToken);
    }
    
    public Task Handle(CreatedTodoEvent notification, CancellationToken cancellationToken)
    {
        return HandleInternal(cancellationToken);
    }

    private async Task HandleInternal(CancellationToken cancellationToken)
    {
        await _cache.RemoveAsync(Key, cancellationToken);
    }
}