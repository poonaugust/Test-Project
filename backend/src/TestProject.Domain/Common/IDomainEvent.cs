namespace TestProject.Domain.Common;

public interface IDomainEvent
{
    DateTimeOffset OccurredOnUtc { get; }
}
