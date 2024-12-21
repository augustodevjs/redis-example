﻿using MediatR;

namespace redis_example.Abstractions.Messaging.Command;

public interface ICommandHandler<TCommand> : IRequestHandler<TCommand> 
    where TCommand : ICommand;

public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, TResponse> 
    where TCommand : ICommand<TResponse>;