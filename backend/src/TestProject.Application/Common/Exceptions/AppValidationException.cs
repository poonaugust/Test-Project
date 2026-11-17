using FluentValidation;

namespace TestProject.Application.Common.Exceptions;

public sealed class AppValidationException : Exception
{
    public AppValidationException(IEnumerable<FluentValidation.Results.ValidationFailure> failures)
        : base("One or more validation errors occurred.")
    {
        Errors = failures
            .GroupBy(f => f.PropertyName, f => f.ErrorMessage)
            .ToDictionary(g => g.Key, g => g.Distinct().ToArray());
    }

    public IReadOnlyDictionary<string, string[]> Errors { get; }
}
