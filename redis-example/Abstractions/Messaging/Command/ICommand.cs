using MediatR;

namespace redis_example.Abstractions.Messaging.Command;

public interface IBaseCommand;

public interface ICommand : IRequest, IBaseCommand;

public interface ICommand<TResponse> : IRequest<TResponse>, IBaseCommand;