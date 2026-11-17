using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace TestProject.Application.Common.Mediator;

/// <summary>
/// Lightweight DI-backed Mediator supporting CQRS request handlers and pipeline behaviors.
/// </summary>
public sealed class Mediator(IServiceProvider serviceProvider) : IMediator
{
    private static readonly ConcurrentDictionary<Type, RequestHandlerWrapper> HandlerWrappers = new();

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var wrapper = (RequestHandlerWrapper<TResponse>)HandlerWrappers.GetOrAdd(
            request.GetType(),
            static requestType =>
            {
                var wrapperType = typeof(RequestHandlerWrapperImpl<,>).MakeGenericType(requestType, typeof(TResponse));
                return (RequestHandlerWrapper)Activator.CreateInstance(wrapperType)!;
            });

        return wrapper.Handle(request, serviceProvider, cancellationToken);
    }

    private abstract class RequestHandlerWrapper;

    private abstract class RequestHandlerWrapper<TResponse> : RequestHandlerWrapper
    {
        public abstract Task<TResponse> Handle(
            IRequest<TResponse> request,
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken);
    }

    private sealed class RequestHandlerWrapperImpl<TRequest, TResponse> : RequestHandlerWrapper<TResponse>
        where TRequest : IRequest<TResponse>
    {
        public override Task<TResponse> Handle(
            IRequest<TResponse> request,
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken)
        {
            var typedRequest = (TRequest)request;
            var handler = serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
            var behaviors = serviceProvider.GetServices<IPipelineBehavior<TRequest, TResponse>>().Reverse();

            RequestHandlerDelegate<TResponse> pipeline = () => handler.Handle(typedRequest, cancellationToken);

            foreach (var behavior in behaviors)
            {
                var next = pipeline;
                pipeline = () => behavior.Handle(typedRequest, next, cancellationToken);
            }

            return pipeline();
        }
    }
}
