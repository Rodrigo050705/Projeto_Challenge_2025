using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Bson;
using Henry.AI.Core.Host.Documentation.Models;
using Henry.AI.Core.Host.Documentation.Repositories;
using Doc = Henry.AI.Core.Host.Documentation.Models.Documentation;

var builder = WebApplication.CreateBuilder(args);

// HttpClient -> Agent
var agentUrl = builder.Configuration["URL_AGENT"] ?? "http://localhost:5001";
builder.Services.AddHttpClient("agent", c => c.BaseAddress = new Uri(agentUrl));

// CORS para front local
builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(p => p
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        .SetIsOriginAllowed(h => h == "http://localhost:5173" || h == "http://127.0.0.1:5173"));
});

// OpenAPI (ignorar se não existir no seu projeto)
try { builder.Services.AddOpenApi(); } catch {}

builder.Services.AddEndpointsApiExplorer();

// MongoDB
builder.Services.AddSingleton<IMongoClient>(_ =>
{
    var uri = builder.Configuration["MONGODB_URI"]; // Atlas SRV
    if (string.IsNullOrWhiteSpace(uri))
        throw new InvalidOperationException("MONGODB_URI not set. Configure your Atlas connection string.");
    return new MongoClient(uri);
});

builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IMongoClient>().GetDatabase(builder.Configuration["MONGODB_DB"] ?? "henry_ai"));
builder.Services.AddSingleton(sp =>
{
    var db = sp.GetRequiredService<IMongoDatabase>();
    var col = db.GetCollection<Doc>("documentation");
    try {
        col.Indexes.CreateMany(new[] {
            new CreateIndexModel<Doc>(
                Builders<Doc>.IndexKeys.Text(x => x.Title).Text(x => x.Content),
                new CreateIndexOptions { Name = "text_all", Weights = new BsonDocument { { "title", 5 }, { "content", 1 } } }
            ),
            new CreateIndexModel<Doc>(Builders<Doc>.IndexKeys.Ascending(x => x.Language).Ascending(x => x.Function)),
            new CreateIndexModel<Doc>(Builders<Doc>.IndexKeys.Descending(x => x.CreatedAt))
        });
    } catch {}
    return col;
});
builder.Services.AddSingleton<IDocumentationRepository, DocumentationRepository>();

var app = builder.Build();

app.UseCors();
try { app.MapOpenApi(); } catch {}

app.MapGet("/healthz", () => Results.Ok(new { ok = true }));

// ===== Endpoints =====
app.MapPost("/documentation/rawcode", async ([FromBody] RawCodeInput input, IHttpClientFactory http, IDocumentationRepository repo) =>
{
    var client = http.CreateClient("agent");
    var res = await client.PostAsJsonAsync("/documentation/rawcode", input);
    var json = await res.Content.ReadAsStringAsync();

    var (content, title, func) = DocUtils.ExtractDoc(json);
    var lang = DocUtils.GuessLanguage(input.Code);
    var doc = new Doc {
        CodeHash = DocUtils.Sha256(input.Code),
        Title = title,
        Language = lang,
        Function = func,
        Tags = new() { lang },
        Content = content,
        CreatedAt = DateTime.UtcNow
    };
    await repo.Create(doc);

    return Results.Ok(new {
        id = doc.Id.ToString(),
        documentedCode = doc.Content,
        language = doc.Language,
        title = doc.Title,
        function = doc.Function,
        createdAt = doc.CreatedAt
    });
});

app.MapGet("/documentation/{id}", async (string id, IDocumentationRepository repo) =>
{
    var d = await repo.Get(id);
    if (d is null) return Results.NotFound();
    return Results.Ok(new {
        id = d.Id.ToString(),
        codeHash = d.CodeHash,
        title = d.Title,
        language = d.Language,
        function = d.Function,
        tags = d.Tags,
        content = d.Content,
        createdAt = d.CreatedAt,
        updatedAt = d.UpdatedAt
    });
});

app.MapGet("/documentation", async (string? query, string? language, string? function, int skip, int limit, IDocumentationRepository repo) =>
{
    var (items, total) = await repo.Search(query, language, function, skip, limit);
    var shaped = items.Select(d => new {
        id = d.Id.ToString(),
        codeHash = d.CodeHash,
        title = d.Title,
        language = d.Language,
        function = d.Function,
        tags = d.Tags,
        content = d.Content,
        createdAt = d.CreatedAt,
        updatedAt = d.UpdatedAt
    });
    return Results.Ok(new { total, items = shaped });
});


app.MapPut("/documentation/{id}", async (string id, [FromBody] UpdateDocInput body, IDocumentationRepository repo) =>
{
    var content = string.IsNullOrWhiteSpace(body.Content) ? (body.DocumentedCode ?? "") : body.Content;
    var updated = await repo.Update(id, body.Title ?? "", body.Language ?? "", body.Function ?? "", content ?? "");
    if (updated is null) return Results.NotFound();
    return Results.Ok(new {
        id = updated.Id.ToString(),
        title = updated.Title,
        language = updated.Language,
        function = updated.Function,
        content = updated.Content,
        createdAt = updated.CreatedAt,
        updatedAt = updated.UpdatedAt
    });
});

app.MapDelete("/documentation/{id}", async (string id, IDocumentationRepository repo) =>
{
    var ok = await repo.Delete(id);
    return ok ? Results.NoContent() : Results.NotFound();
});



app.MapGet("/documentation/{id}/pdf", async (string id, IDocumentationRepository repo) =>
{
    var doc = await repo.Get(id);
    if (doc is null) return Results.NotFound();
    var title = string.IsNullOrWhiteSpace(doc.Title) ? "documentacao" : doc.Title;
    var pdf = DocumentationPdf.Render(doc.Title, doc.Language, doc.Function, doc.Content);
    var fileName = $"{title}.pdf";
    return Results.File(pdf, "application/pdf", fileName);
});

app.Run();


// ===== Helpers (fora de namespace) =====
internal static class DocUtils
{
    public static string GuessLanguage(string code)
    {
        var s = code ?? string.Empty;
        if (s.Contains("def ") || s.Contains("print(")) return "python";
        if (s.Contains("Console.WriteLine") || s.Contains("namespace ") || s.Contains("public class")) return "csharp";
        if (s.Contains("function ") || s.Contains("console.log(")) return "javascript";
        if (s.Contains("System.out.")) return "java";
        return "unknown";
    }

    public static string Sha256(string s)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        return Convert.ToHexString(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(s))).ToLowerInvariant();
    }

    public static (string content, string title, string func) ExtractDoc(string rawJson)
    {
        try {
            using var j = JsonDocument.Parse(rawJson);
            string content =
                j.RootElement.TryGetProperty("documentedCode", out var dc) ? (dc.GetString() ?? rawJson) :
                j.RootElement.TryGetProperty("documentation", out var d2) ? (d2.GetString() ?? rawJson) :
                rawJson;

            string title = content.Split('\n').FirstOrDefault()?.Trim() ?? "Documentação";
            string func =
                content.Split('\n').FirstOrDefault(l => l.Contains("Funcionalidade", StringComparison.OrdinalIgnoreCase)
                                                      || l.Contains("Propósito", StringComparison.OrdinalIgnoreCase)
                                                      || l.Contains("Objetivo", StringComparison.OrdinalIgnoreCase))?.Trim() ?? "";
            return (content, title, func);
        } catch { return (rawJson, "Documentação", ""); }
    }
}

internal record RawCodeInput(string Code);
