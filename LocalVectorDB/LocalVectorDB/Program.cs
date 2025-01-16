using LiteDB;
using LocalVectorDB.Services;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LocalVectorDB.Models;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Register the VectorDatabase as a singleton service
builder.Services.AddSingleton<VectorDatabase>();

var app = builder.Build();

// Enable developer-friendly error pages in development environment
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// ===============================
// Insert a Single Vector Endpoint
// ===============================
app.MapPost("/insert", async (HttpContext context, VectorDatabase db) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var content = await reader.ReadToEndAsync();  // Read the incoming data
    var vector = System.Text.Json.JsonSerializer.Deserialize<List<double>>(content);  // Deserialize JSON to List<double>

    if (vector != null)
    {
        db.InsertVector(new VectorItem { Vector = vector });  // Insert vector into the database
        await context.Response.WriteAsync("Vector inserted successfully.");
    }
    else
    {
        await context.Response.WriteAsync("Invalid vector data.");
    }
});

// ===================================
// Batch Insert Multiple Vectors Endpoint
// ===================================
app.MapPost("/insert-batch", async (HttpContext context, VectorDatabase db) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var content = await reader.ReadToEndAsync();  // Read the incoming data
    var vectors = System.Text.Json.JsonSerializer.Deserialize<List<List<double>>>(content);  // Deserialize JSON to List of vectors

    if (vectors != null)
    {
        foreach (var vector in vectors)
        {
            db.InsertVector(new VectorItem { Vector = vector });  // Insert each vector into the database
        }
        await context.Response.WriteAsync("Batch vectors inserted successfully.");
    }
    else
    {
        await context.Response.WriteAsync("Invalid batch vector data.");
    }
});

// =========================================
// K-Nearest Neighbors (KNN) Search Endpoint
// =========================================
app.MapPost("/search", async (HttpContext context, VectorDatabase db) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var content = await reader.ReadToEndAsync();  // Read the incoming data
    var searchRequest = System.Text.Json.JsonSerializer.Deserialize<SearchRequest>(content);  // Deserialize search request

    if (searchRequest != null)
    {
        var queryVector = Vector<double>.Build.DenseOfEnumerable(searchRequest.QueryVector);  // Convert query to vector
        var results = db.SearchSimilar(queryVector, searchRequest.TopK);  // Perform similarity search
        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(results));  // Return search results
    }
    else
    {
        await context.Response.WriteAsync("Invalid search data.");
    }
});

// ============================
// Update an Existing Vector Endpoint
// ============================
app.MapPut("/update/{id}", async (int id, HttpContext context, VectorDatabase db) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var content = await reader.ReadToEndAsync();  // Read the incoming data
    var vector = System.Text.Json.JsonSerializer.Deserialize<List<double>>(content);  // Deserialize JSON to List<double>

    if (vector != null)
    {
        db.UpdateVector(id, vector);  // Update the vector in the database
        await context.Response.WriteAsync("Vector updated successfully.");
    }
    else
    {
        await context.Response.WriteAsync("Invalid vector data.");
    }
});

// =============================
// Delete a Vector by ID Endpoint
// =============================
app.MapDelete("/delete/{id}", (int id, VectorDatabase db) =>
{
    db.DeleteVector(id);  // Delete vector from the database
    return Results.Ok("Vector deleted successfully.");
});

// ===============================
// Cosine Similarity Calculation Endpoint
// ===============================
app.MapPost("/cosine-similarity", async (HttpContext context) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var content = await reader.ReadToEndAsync();  // Read the incoming data
    var vectors = System.Text.Json.JsonSerializer.Deserialize<List<List<double>>>(content);  // Deserialize JSON to two vectors

    if (vectors != null && vectors.Count == 2)
    {
        var vectorA = Vector<double>.Build.DenseOfEnumerable(vectors[0]);  // First vector
        var vectorB = Vector<double>.Build.DenseOfEnumerable(vectors[1]);  // Second vector
        double similarity = vectorA.DotProduct(vectorB) / (vectorA.L2Norm() * vectorB.L2Norm());  // Cosine similarity

        await context.Response.WriteAsync($"Cosine Similarity: {similarity}");
    }
    else
    {
        await context.Response.WriteAsync("Please provide exactly two vectors.");
    }
});

// ===========================
// Retrieve All Stored Vectors Endpoint
// ===========================
app.MapGet("/vectors", (VectorDatabase db) =>
{
    var allVectors = db.GetAllVectors();  // Fetch all vectors from the database
    return Results.Json(allVectors);  // Return the data as JSON
});

// Run the application
app.Run();
