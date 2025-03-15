using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;
using MonApi.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = "Host=db-container;Port=5432;Database=monapi;Username=postgres;Password=postgres";
Console.WriteLine($" Connexion forcée à PostgreSQL avec : {connectionString}");

try
{
    using var conn = new NpgsqlConnection(connectionString);
    conn.Open();
    Console.WriteLine(" Connexion PostgreSQL réussie !");

    //  Créer automatiquement la table `users` si elle n'existe pas
    using var createTableCmd = new NpgsqlCommand(@"
        CREATE TABLE IF NOT EXISTS users (
            id SERIAL PRIMARY KEY,
            name VARCHAR(100) NOT NULL,
            email VARCHAR(100) NOT NULL UNIQUE
        )", conn);
    
    createTableCmd.ExecuteNonQuery(); //  Exécute le script de création de table
    Console.WriteLine(" Table `users` vérifiée/créée avec succès !");

    //  Insérer automatiquement des utilisateurs s'ils n'existent pas
    using var checkCmd = new NpgsqlCommand("SELECT COUNT(*) FROM users", conn);
    var userCount = (long)checkCmd.ExecuteScalar();

    if (userCount == 0) //  Si aucun utilisateur n'est présent → Insérer automatiquement des valeurs
    {
        using var insertCmd = new NpgsqlCommand(@"
            INSERT INTO users (name, email) VALUES 
            ('Seynabou', 'seynabou@example.com'),
            ('Chris', 'chris@example.com'),
            ('Alice', 'alice@example.com')
            ON CONFLICT (email) DO NOTHING;", conn);

        insertCmd.ExecuteNonQuery(); //  Insère les utilisateurs par défaut
        Console.WriteLine(" Utilisateurs par défaut insérés !");
    }
}
catch (Exception ex)
{
    Console.WriteLine(" Erreur de connexion à PostgreSQL : " + ex.Message);
}

//  Ajout du service des contrôleurs
builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

//  Test simple pour vérifier que l'API fonctionne
app.MapGet("/test", () => "API is running fine!");

//  Endpoint pour récupérer les utilisateurs depuis PostgreSQL
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

//  Endpoint pour ajouter un nouvel utilisateur
app.MapPost("/api/users", async ([FromBody] User user) =>
{
    await using var conn = new NpgsqlConnection(connectionString);
    await conn.OpenAsync();

    await using var cmd = new NpgsqlCommand("INSERT INTO users (name, email) VALUES (@name, @email) RETURNING id", conn);
    cmd.Parameters.AddWithValue("@name", user.Name);
    cmd.Parameters.AddWithValue("@email", user.Email);

    try
    {
        user.Id = (int)await cmd.ExecuteScalarAsync();
        return Results.Created($"/api/users/{user.Id}", user);
    }
    catch (PostgresException ex) when (ex.SqlState == "23505") // Gestion des doublons
    {
        return Results.Conflict("L'utilisateur avec cet email existe déjà.");
    }
});

app.Run();









































// using Microsoft.AspNetCore.Mvc;
// using Npgsql;
// using System.Data;
// using MonApi.Models;

// var builder = WebApplication.CreateBuilder(args);

// var connectionString = "Host=db-container;Port=5432;Database=monapi;Username=postgres;Password=postgres";
// Console.WriteLine($" Connexion forcée à PostgreSQL avec : {connectionString}");

// try
// {
//     using var conn = new NpgsqlConnection(connectionString);
//     conn.Open();
//     Console.WriteLine(" Connexion PostgreSQL réussie !");

//     //  Créer automatiquement la table `users` si elle n'existe pas
//     using var cmd = new NpgsqlCommand(@"
//         CREATE TABLE IF NOT EXISTS users (
//             id SERIAL PRIMARY KEY,
//             name VARCHAR(100) NOT NULL,
//             email VARCHAR(100) NOT NULL UNIQUE
//         )", conn);
    
//     cmd.ExecuteNonQuery(); //  Exécute le script de création de table
//     Console.WriteLine(" Table `users` vérifiée/créée avec succès !");
// }
// catch (Exception ex)
// {
//     Console.WriteLine(" Erreur de connexion à PostgreSQL : " + ex.Message);
// }

// builder.Services.AddControllers();

// var app = builder.Build();

// app.UseHttpsRedirection();
// app.UseRouting();
// app.UseAuthorization();

// // Test simple pour vérifier que l'API fonctionne
// app.MapGet("/test", () => "API is running fine!");

// // Endpoint pour récupérer les utilisateurs depuis PostgreSQL
// app.MapGet("/api/users", async () =>
// {
//     var users = new List<User>();

//     await using var conn = new NpgsqlConnection(connectionString);
//     await conn.OpenAsync();

//     await using var cmd = new NpgsqlCommand("SELECT id, name, email FROM users", conn);
//     await using var reader = await cmd.ExecuteReaderAsync();

//     while (await reader.ReadAsync())
//     {
//         users.Add(new User
//         {
//             Id = reader.GetInt32(0),
//             Name = reader.GetString(1),
//             Email = reader.GetString(2)
//         });
//     }

//     return Results.Ok(users);
// });

// // Endpoint pour ajouter un utilisateur
// app.MapPost("/api/users", async ([FromBody] User user) =>
// {
//     await using var conn = new NpgsqlConnection(connectionString);
//     await conn.OpenAsync();

//     await using var cmd = new NpgsqlCommand("INSERT INTO users (name, email) VALUES (@name, @email) RETURNING id", conn);
//     cmd.Parameters.AddWithValue("@name", user.Name);
//     cmd.Parameters.AddWithValue("@email", user.Email);

//     user.Id = (int)await cmd.ExecuteScalarAsync();
//     return Results.Created($"/api/users/{user.Id}", user);
// });

// app.Run();
















































// using Microsoft.AspNetCore.Mvc;
// using Npgsql;
// using System.Data;
// using MonApi.Models; // Import du modèle `User`

// var builder = WebApplication.CreateBuilder(args);

// // Connexion PostgreSQL forcée
// var connectionString = "Host=db-container;Port=5432;Database=monapi;Username=postgres;Password=postgres";
// Console.WriteLine($" Connexion forcée à PostgreSQL avec : {connectionString}");

// try
// {
//     using var conn = new Npgsql.NpgsqlConnection(connectionString);
//     conn.Open();
//     Console.WriteLine(" Connexion PostgreSQL réussie !");
// }
// catch (Exception ex)
// {
//     Console.WriteLine(" Erreur de connexion à PostgreSQL : " + ex.Message);
// }

// // Ajout des services pour les contrôleurs
// builder.Services.AddControllers();

// var app = builder.Build();

// app.UseHttpsRedirection();
// app.UseRouting();
// app.UseAuthorization();

// //  Test simple pour voir si l'API fonctionne
// app.MapGet("/test", () => "API is running fine!");

// //  Endpoint pour récupérer les utilisateurs depuis PostgreSQL
// app.MapGet("/api/users", async () =>
// {
//     var users = new List<User>();

//     await using var conn = new NpgsqlConnection(connectionString);
//     await conn.OpenAsync();

//     await using var cmd = new NpgsqlCommand("SELECT id, name, email FROM users", conn);
//     await using var reader = await cmd.ExecuteReaderAsync();

//     while (await reader.ReadAsync())
//     {
//         users.Add(new User
//         {
//             Id = reader.GetInt32(0),
//             Name = reader.GetString(1),
//             Email = reader.GetString(2)
//         });
//     }

//     return Results.Ok(users);
// });

// //  Endpoint pour ajouter un utilisateur
// app.MapPost("/api/users", async ([FromBody] User user) =>
// {
//     await using var conn = new NpgsqlConnection(connectionString);
//     await conn.OpenAsync();

//     await using var cmd = new NpgsqlCommand("INSERT INTO users (name, email) VALUES (@name, @email) RETURNING id", conn);
//     cmd.Parameters.AddWithValue("@name", user.Name);
//     cmd.Parameters.AddWithValue("@email", user.Email);

//     user.Id = (int)await cmd.ExecuteScalarAsync();
//     return Results.Created($"/api/users/{user.Id}", user);
// });

// app.Run();
