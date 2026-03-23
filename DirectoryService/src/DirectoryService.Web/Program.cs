using System.Globalization;
using DirectoryService.Infrastructure.Postgres;
using DirectoryService.Presentation;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
    .CreateBootstrapLogger(); // Стартовый предлоггер, что бы логировать с запуска приложение, пока нормальный логер не начал работать еще.
try
{
    Log.Information("Starting application");

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddProgramDependencies(builder.Configuration);

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.UseSwaggerUI(a => a.SwaggerEndpoint("/openapi/v1.json", "DirectoryService"));
    }

    app.UseSerilogRequestLogging();
    app.MapControllers();

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush(); // гарантировано завершить логирования без потери данных.
}