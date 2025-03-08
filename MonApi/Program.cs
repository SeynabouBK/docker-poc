using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;
using MonApi.Models; // 🔥 Import du modèle `User`

var builder = WebApplication.CreateBuilder(args);

// 🔥 Connexion PostgreSQL forcée 🔥
var connectionString = "Host=db-container;Port=5432;Database=monapi;Username=postgres;Password=postgres";
Console.WriteLine($"🔍 Connexion forcée à PostgreSQL avec : {connectionString}");

try
{
    using var conn = new Npgsql.NpgsqlConnection(connectionString);
    conn.Open();
    Console.WriteLine("✅ Connexion PostgreSQL réussie !");
}
catch (Exception ex)
{
    Console.WriteLine("❌ Erreur de connexion à PostgreSQL : " + ex.Message);
}

// Ajout des services pour les contrôleurs
builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

// ✅ Test simple pour voir si l'API fonctionne
app.MapGet("/test", () => "API is running fine!");

// ✅ Endpoint pour récupérer les utilisateurs depuis PostgreSQL
app.MapGet("/api/users", async () =>
{
    var users = new List<User>();

    await using var conn = new NpgsqlConnection(connectionString);
    await conn.OpenAsync();

    await using var cmd = new NpgsqlCommand("SELECT id, name, email FROM users", conn);
    await using var reader = await cmd.ExecuteReaderAsync();

    while (await reader.ReadAsync())
    {
        users.Add(new User
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            Email = reader.GetString(2)
        });
    }

    return Results.Ok(users);
});

// ✅ Endpoint pour ajouter un utilisateur
app.MapPost("/api/users", async ([FromBody] User user) =>
{
    await using var conn = new NpgsqlConnection(connectionString);
    await conn.OpenAsync();

    await using var cmd = new NpgsqlCommand("INSERT INTO users (name, email) VALUES (@name, @email) RETURNING id", conn);
    cmd.Parameters.AddWithValue("@name", user.Name);
    cmd.Parameters.AddWithValue("@email", user.Email);

    user.Id = (int)await cmd.ExecuteScalarAsync();
    return Results.Created($"/api/users/{user.Id}", user);
});

app.Run();
