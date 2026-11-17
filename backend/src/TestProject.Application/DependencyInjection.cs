using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TestProject.Application.Common.Behaviors;
using TestProject.Application.Common.Mediator;

namespace TestProject.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddScoped<IMediator, Mediator>();

        RegisterRequestHandlers(services, assembly);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddValidatorsFromAssembly(assembly);

        return services;
    }

    private static void RegisterRequestHandlers(IServiceCollection services, Assembly assembly)
    {
        var handlerInterfaceType = typeof(IRequestHandler<,>);

        var handlerTypes = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterfaceType)
                .Select(i => new { Implementation = t, Service = i }));

        foreach (var handler in handlerTypes)
        {
            services.AddScoped(handler.Service, handler.Implementation);
        }
    }
}
