using Apiaguas.Data;
using Apiaguas.Dtos;
using Apiaguas.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Registra o DbContext
builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseSqlite("Data Source=aguas.db"));

var app = builder.Build();

// Cria o banco se não existir
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
    db.Database.EnsureCreated();
}

// Frontend na mesma origem
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/", () => Results.Redirect("/index.html"));

app.MapGet("/api/aguas", async (ApiDbContext db) =>
    await db.Aguas.ToListAsync());

app.MapGet("/api/aguas/{id:int}", async (int id, ApiDbContext db) =>
{
    var agua = await db.Aguas.FindAsync(id);

    if (agua is null)
        return Results.NotFound();

    return Results.Ok(agua);
});

app.MapPost("/api/aguas", async (AguaInputDto entrada, ApiDbContext db) =>
{
    var agua = new AguaEntity
    {
        Titulo = entrada.Titulo
    };

    db.Aguas.Add(agua);
    await db.SaveChangesAsync();
    return Results.Created($"/api/aguas/{agua.Id}", agua);
});

app.MapPut("/api/aguas/{id:int}", async (int id, AguaInputDto entrada, ApiDbContext db) =>
{
    var agua = await db.Aguas.FindAsync(id);
    if (agua is null) return Results.NotFound();

    agua.Titulo = entrada.Titulo;

    await db.SaveChangesAsync();
    return Results.Ok(agua);
});

app.MapDelete("/api/aguas/{id:int}", async (int id, ApiDbContext db) =>
{
    var agua = await db.Aguas.FindAsync(id);
    if (agua is null) return Results.NotFound();

    db.Aguas.Remove(agua);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
