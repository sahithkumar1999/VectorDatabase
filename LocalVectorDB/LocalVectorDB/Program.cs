using LiteDB;
using LocalVectorDB.Services;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LocalVectorDB.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Register the VectorDatabase
builder.Services.AddSingleton<VectorDatabase>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Insert a Single Vector
app.MapPost("/insert", async (HttpContext context, VectorDatabase db) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var content = await reader.ReadToEndAsync();
    var vector = System.Text.Json.JsonSerializer.Deserialize<List<double>>(content);

    if (vector != null)
    {
        db.InsertVector(new VectorItem { Vector = vector });
        await context.Response.WriteAsync("Vector inserted successfully.");
    }
    else
    {
        await context.Response.WriteAsync("Invalid vector data.");
    }
});

// Batch Insert Vectors
app.MapPost("/insert-batch", async (HttpContext context, VectorDatabase db) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var content = await reader.ReadToEndAsync();
    var vectors = System.Text.Json.JsonSerializer.Deserialize<List<List<double>>>(content);

    if (vectors != null)
    {
        foreach (var vector in vectors)
        {
            db.InsertVector(new VectorItem { Vector = vector });
        }
        await context.Response.WriteAsync("Batch vectors inserted successfully.");
    }
    else
    {
        await context.Response.WriteAsync("Invalid batch vector data.");
    }
});

// KNN Search
app.MapPost("/search", async (HttpContext context, VectorDatabase db) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var content = await reader.ReadToEndAsync();
    var searchRequest = System.Text.Json.JsonSerializer.Deserialize<SearchRequest>(content);

    if (searchRequest != null)
    {
        var queryVector = Vector<double>.Build.DenseOfEnumerable(searchRequest.QueryVector);
        var results = db.SearchSimilar(queryVector, searchRequest.TopK);
        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(results));
    }
    else
    {
        await context.Response.WriteAsync("Invalid search data.");
    }
});

// Update Vector by ID
app.MapPut("/update/{id}", async (int id, HttpContext context, VectorDatabase db) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var content = await reader.ReadToEndAsync();
    var vector = System.Text.Json.JsonSerializer.Deserialize<List<double>>(content);

    if (vector != null)
    {
        db.UpdateVector(id, vector);
        await context.Response.WriteAsync("Vector updated successfully.");
    }
    else
    {
        await context.Response.WriteAsync("Invalid vector data.");
    }
});

// Delete Vector by ID
app.MapDelete("/delete/{id}", (int id, VectorDatabase db) =>
{
    db.DeleteVector(id);
    return Results.Ok("Vector deleted successfully.");
});

// Cosine Similarity Check
app.MapPost("/cosine-similarity", async (HttpContext context) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var content = await reader.ReadToEndAsync();
    var vectors = System.Text.Json.JsonSerializer.Deserialize<List<List<double>>>(content);

    if (vectors != null && vectors.Count == 2)
    {
        var vectorA = Vector<double>.Build.DenseOfEnumerable(vectors[0]);
        var vectorB = Vector<double>.Build.DenseOfEnumerable(vectors[1]);
        double similarity = vectorA.DotProduct(vectorB) / (vectorA.L2Norm() * vectorB.L2Norm());

        await context.Response.WriteAsync($"Cosine Similarity: {similarity}");
    }
    else
    {
        await context.Response.WriteAsync("Please provide exactly two vectors.");
    }
});

app.MapGet("/vectors", (VectorDatabase db) =>
{
    var allVectors = db.GetAllVectors();
    return Results.Json(allVectors);
});

app.Run();
