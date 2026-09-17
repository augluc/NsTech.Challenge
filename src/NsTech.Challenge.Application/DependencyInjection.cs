namespace NsTech.Challenge.Application;

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using NsTech.Challenge.Application.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IOrderApplicationService, OrderApplicationService>();
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}