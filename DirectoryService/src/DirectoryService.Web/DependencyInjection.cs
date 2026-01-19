namespace DirectoryService.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddProgramDependencies(this IServiceCollection services) =>
    services
        .AddWebDependencies();

    private static IServiceCollection AddWebDependencies(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddOpenApi();
        return services;
    }
}