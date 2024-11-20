using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OnlineStoreAPI.Models;
using OnlineStoreAPI.Repositories;
using OnlineStoreAPI.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ProductRepository>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseMiddleware<AuthenticationMiddleware>();

app.MapGet("/products", (ProductRepository repository) =>
{
    var products = repository.GetAll();
    return Results.Json(products);
});

app.MapGet("/products/{id:int}", (int id, ProductRepository repository) =>
{
    var product = repository.GetById(id);
    if (product == null)
    {
        return Results.NotFound(new { error = "Product not found" });
    }
    return Results.Json(product);
});

app.MapPost("/products", async (HttpContext context, ProductRepository repository) =>
{
    var product = await context.Request.ReadFromJsonAsync<Product>();
    if (product == null)
    {
        return Results.BadRequest(new { error = "Invalid product data" });
    }
    var addedProduct = repository.Add(product);
    return Results.Created($"/products/{addedProduct.Id}", addedProduct);
});

app.MapPost("/products/delete", async (HttpContext context, ProductRepository repository) =>
{
    var form = await context.Request.ReadFormAsync();
    if (!int.TryParse(form["id"], out var id))
    {
        return Results.BadRequest(new { error = "Invalid product ID" });
    }

    var success = repository.Delete(id);
    if (!success)
    {
        return Results.NotFound(new { error = "Product not found" });
    }

    return Results.Ok(new { message = "Product deleted successfully" });
});

app.Run();
