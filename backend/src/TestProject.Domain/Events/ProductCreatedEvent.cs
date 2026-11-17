using TestProject.Domain.Common;

namespace TestProject.Domain.Events;

public sealed record ProductCreatedEvent(Guid ProductId, string Sku, DateTimeOffset OccurredOnUtc) : IDomainEvent;
