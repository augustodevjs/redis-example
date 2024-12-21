using MediatR;

namespace redis_example.Abstractions.Messaging.Query;

public interface IQuery<TResponse> : IRequest<TResponse>;