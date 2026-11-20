using Serilog;
using Serilog.Events;

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
        app.UseSerilogRequestLogging(options =>
        {
            options.GetLevel = (context, elapsed, ex) =>
            {
                if (context.Request.Path.StartsWithSegments("/health"))
                {
                    return LogEventLevel.Verbose;
                }

                if (ex is not null || context.Response.StatusCode >= 500)
                {
                    return LogEventLevel.Error;
                }

                return LogEventLevel.Information;
            };
        });


        return app;
    }
}