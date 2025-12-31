using Api_de_cinema.controllers;
using Api_de_cinema.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=principal.db"));
var app = builder.Build();

//adicionar endpoints na api 
app.add_endpoints();
app.admin();

app.Run();
