using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var jobs = new ConcurrentDictionary<string, object>();

app.MapPost("/api/v1/jobs", async (HttpContext context) => {
    using var doc = await JsonDocument.ParseAsync(context.Request.Body);
    var root = doc.RootElement;
    var id = Guid.NewGuid().ToString("N")[..8];
    var job = new {
        id,
        name = root.TryGetProperty("name", out var n) ? n.GetString() : "unnamed",
        schedule = root.TryGetProperty("schedule", out var s) ? s.GetString() : "0 * * * *",
        command = root.TryGetProperty("command", out var c) ? c.GetString() : "",
        created_at = DateTimeOffset.UtcNow
    };
    jobs[id] = job;
    await context.Response.WriteAsJsonAsync(new { status = "created", job });
});

app.MapGet("/api/v1/jobs", () => jobs.Values.ToArray());
app.MapDelete("/api/v1/jobs/{id}", (string id) => {
    jobs.TryRemove(id, out _);
    return Results.Ok(new { status = "deleted", id });
});
app.MapGet("/health", () => new { status = "healthy" });

app.Run("http://0.0.0.0:8080");
