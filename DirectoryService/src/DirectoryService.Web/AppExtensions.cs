using Serilog;

namespace DirectoryService.Presentation;

public static class AppExtensions
{
    public static IApplicationBuilder Configure(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        return app;
    }
}