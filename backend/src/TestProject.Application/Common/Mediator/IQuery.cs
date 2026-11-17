namespace TestProject.Application.Common.Mediator;

/// <summary>
/// Marker interface for CQRS queries that read state without side effects.
/// </summary>
public interface IQuery<out TResponse> : IRequest<TResponse>;
