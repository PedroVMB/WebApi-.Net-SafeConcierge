using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace SafeConcierge.Application.DependencyInjection;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(ApplicationExtensions).Assembly;

        // MediatR — registra todos os Handlers, Behaviors, etc. do assembly
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(assembly));

        // FluentValidation — registra todos os validators do assembly
        services.AddValidatorsFromAssembly(assembly);

        // Pipeline behavior: valida automaticamente via FluentValidation antes de cada Command/Query
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));

        return services;
    }
}

