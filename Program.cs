using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.ModelViews;
using minimal_api.Dominio.Servicos;
using minimal_api.Infraestrutura.Db;
using minimal_api.Infraestrutura.Infraestrutura;

#region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<iAdministradorServico, AdministradorServico>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DbContexto>(
    options => {
        options.UseMySql(
            builder.Configuration.GetConnectionString("mysql"), 
            ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql")));
    }
);

var app = builder.Build();
#endregion

# region Home
app.MapGet("/", () => Results.Json(new Home()));
#endregion

#region Administradores
app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, iAdministradorServico administradorServico) => {
    if(administradorServico.Login(loginDTO) != null)
    {
        return Results.Ok("Login realizado com sucesso");
    }else
    {
        return Results.Unauthorized();
    }
});
#endregion

#region Veiculos

app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, iAdministradorServico administradorServico) => {
    if(administradorServico.Login(loginDTO) != null)
    {
        return Results.Ok("Login realizado com sucesso");
    }else
    {
        return Results.Unauthorized();
    }
});

#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
#endregion

