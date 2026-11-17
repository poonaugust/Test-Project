namespace TestProject.Application.Common.Mediator;

/// <summary>
/// Marker interface for CQRS commands that mutate state.
/// </summary>
public interface ICommand<out TResponse> : IRequest<TResponse>;
