using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api_de_cinema.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Api_de_cinema.controllers
{
    public static class Endpoints
    {
        public static void add_endpoints(this WebApplication app)
        {

            var api = app.MapGroup("/api");

            api.MapGet("/ver_colunas/{name}", async (string name, AppDbContext banco) =>
            {
                var coluns = new List<object>();
                
                var connection = banco.Database.GetDbConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = $"""
                    SELECT * FROM {name};
                """;

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    coluns.Add($"id = {reader.GetInt32(0)},codigo = {reader.GetString(1)},status = {reader.GetInt32(2)}");
                }

                await connection.CloseAsync();

                return Results.Ok(coluns);
            });

            api.MapGet("/ocupar/{lugar}/{name}", async (string lugar, string name, AppDbContext banco) =>
            {
                var sql = $"""
                    INSERT OR IGNORE INTO {name} (Codigo, Status)
                    VALUES ('{lugar}', 0);
                    """;

                var rows = await banco.Database.ExecuteSqlRawAsync(
                    sql
                );

                if (rows == 0)
                    return Results.BadRequest("já está ocupado");

                return Results.Ok($"Ocupado o lugar {lugar} no filme {name}");
            });

            api.MapGet("/todos_as_tabelas", async (AppDbContext banco) =>
            {
                var tabelas = new List<string>();

                var connection = banco.Database.GetDbConnection();
                await connection.OpenAsync();
                using var command = connection.CreateCommand();
                command.CommandText = """
                    SELECT name
                    FROM sqlite_master  
                    WHERE type = 'table'
                    AND name NOT LIKE 'sqlite_sequence'
                """;

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    tabelas.Add(reader.GetString(0));
                }

                await connection.CloseAsync();
                return Results.Ok(tabelas);
            });
        }
        public static void admin(this WebApplication app)
        {

            var admin = app.MapGroup("/api/admin");

            admin.MapPost("/create_table", async (string name, AppDbContext banco) =>
            {
                var SQL = $"""
                    CREATE TABLE IF NOT EXISTS {name} (
                        Id INTEGER PRIMARY KEY,
                        Codigo TEXT NOT NULL UNIQUE,
                        Status INTEGER NOT NULL
                    );
                """;
                await banco.Database.ExecuteSqlRawAsync(SQL);
                return Results.Ok($"tabela criada de nome {name}");
            });
        }
    }
}