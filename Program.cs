using Microsoft.Data.SqlClient;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () =>
{
    var html = """
        <!DOCTYPE html>
        <html lang="en">
        <head>
            <meta charset="utf-8" />
            <meta name="viewport" content="width=device-width, initial-scale=1.0" />
            <title>Sample .NET App</title>
            <style>
                body {
                    font-family: Arial, sans-serif;
                    margin: 40px;
                    background: #f4f7fb;
                    color: #1f2937;
                }

                .card {
                    max-width: 700px;
                    background: white;
                    padding: 24px;
                    border-radius: 12px;
                    box-shadow: 0 4px 14px rgba(0, 0, 0, 0.08);
                }

                code {
                    background: #eef2ff;
                    padding: 2px 6px;
                    border-radius: 6px;
                }
            </style>
        </head>
        <body>
            <div class="card">
                <h1>Sample ASP.NET Core App</h1>
                <p>Your application is running successfully.</p>
                <p>Try the JSON endpoint at <code>/api/time</code>.</p>
            </div>
        </body>
        </html>
        """;

    return Results.Content(html, "text/html", Encoding.UTF8);
});

app.MapGet("/api/time", () => Results.Json(new
{
    message = "Hello from ASP.NET Core",
    serverTime = DateTime.UtcNow,
    framework = ".NET 8"
}));

app.MapGet("/api/db-check", async (IConfiguration configuration) =>
{
    var connectionString = configuration.GetConnectionString("DefaultConnection");

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        return Results.Problem(
            detail: "Database connection failed. Connection string is missing.",
            statusCode: StatusCodes.Status500InternalServerError);
    }

    try
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        return Results.Json(new
        {
            message = "Database connection established successfully.",
            database = connection.Database,
            dataSource = connection.DataSource,
            status = "success"
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: $"Database connection failed. {ex.Message}",
            statusCode: StatusCodes.Status500InternalServerError);
    }
});

app.Run();
