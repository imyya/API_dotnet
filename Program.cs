using PizzaStore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Pizzas") ?? "Data Source=Pizzas.db";


builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSqlite<PizzaDb>(connectionString);

builder.Services.AddSwaggerGen(c =>
{
c.SwaggerDoc("v1", new OpenApiInfo {
Title = " API PizzaStore ",
Description = " Faire les pizzas que vous aimez ",
Version = "v1" });
});
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
c.SwaggerEndpoint("/swagger/v1/swagger.json", "PizzaStore API V1");
});
app.MapGet("/", () => "Bonjour Sénégal!");
app.MapGet("/pizzas", async (PizzaDb db) => await db.Pizzas.ToListAsync());

app.MapPost("/pizza", async (PizzaDb db, PizzaEhod pizza) =>
{
await db.Pizzas.AddAsync(pizza);
await db.SaveChangesAsync();
return Results.Created($"/pizza/{pizza.Id}", pizza);
});

app.MapGet("/pizza/{id}", async (PizzaDb db, int id) => await db.Pizzas.FindAsync(id));

app.MapPut("/pizza/{id}", async (PizzaDb db, PizzaEhod updatepizza, int id) =>
{
var pizza = await db.Pizzas.FindAsync(id);
if (pizza is null) return Results.NotFound();
pizza.NomEhod = updatepizza.NomEhod;
pizza.DescriptionEhod = updatepizza.DescriptionEhod;
await db.SaveChangesAsync();
return Results.NoContent();
});

app.MapDelete("/pizza/{id}", async (PizzaDb db, int id) =>
{
var pizza = await db.Pizzas.FindAsync(id);
if (pizza is null)
{
return Results.NotFound();
}
db.Pizzas.Remove(pizza);
await db.SaveChangesAsync();
return Results.Ok();
});
app.Run();