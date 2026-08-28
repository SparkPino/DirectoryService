using Serilog;

namespace DirectoryService.Presentation;

public static class AppExtensions
{
    public static IApplicationBuilder Configure(this WebApplication app)
    {
        app.UseCors(a =>
        {
            a.WithOrigins("http://localhost:3000");
            a.AllowAnyHeader();
            a.AllowAnyMethod();
        });
        app.UseSerilogRequestLogging();

        return app;
    }
}