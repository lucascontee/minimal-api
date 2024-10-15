using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Enuns;
using minimal_api.Dominio.ModelViews;
using minimal_api.Dominio.Servicos;
using minimal_api.Infraestrutura.Db;
using minimal_api.Infraestrutura.Infraestrutura;

#region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<iAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<iVeiculoServico, VeiculoServico>();

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
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
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
}).WithTags("Administradores");

app.MapGet("/administradores", ([FromQuery] int? pagina, iAdministradorServico administradorServico) => {
    var adms = new List<AdministradorModelView>();
    var administradores = administradorServico.Todos(pagina);
    foreach(var adm in administradores){
        adms.Add(new AdministradorModelView{
            Id = adm.Id,
            Email = adm.Email,
            Perfil = adm.Perfil
        });
    }
    return Results.Ok(administradorServico.Todos(pagina));

}).WithTags("Administradores");

app.MapGet("/Administradores/{id}", ([FromRoute] int id, iAdministradorServico administradorServico) => {
    var administrador = administradorServico.BuscaPorId(id);

    if(administrador == null){
        return Results.NotFound();
    }

    return Results.Ok(administrador);
}).WithTags("Administradores");

app.MapPost("/administradores", ([FromBody] AdministradorDTO administradorDTO, iAdministradorServico administradorServico) => {
    var validacao = new ErrosDeValidacao{
        Mensagem = new List<string>()
    }; 

    if(string.IsNullOrEmpty(administradorDTO.Email))
    {
        validacao.Mensagem.Add("Email não pode ser vazio");
    }

    if(string.IsNullOrEmpty(administradorDTO.Senha))
    {
        validacao.Mensagem.Add("Senha não pode ser vazia");
    }

    if(administradorDTO.Perfil == null)
    {
        validacao.Mensagem.Add("Perfil não pode ser vazio");
    }

    if(validacao.Mensagem.Count > 0){
        return Results.BadRequest(validacao);
    }

    var veiculo = new Administrador{
        Email = administradorDTO.Email,
        Senha = administradorDTO.Senha,
        Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString()
    };
    administradorServico.Incluir(veiculo);

    return Results.Created($"/administrador/{veiculo.Id}", veiculo);
}).WithTags("Administradores");

#endregion

#region Veiculos

ErrosDeValidacao validaDTO(VeiculoDTO veiculoDTO)
{
    var validacao = new ErrosDeValidacao{
        Mensagem = new List<string>()
    };

    if(string.IsNullOrEmpty(veiculoDTO.Nome) ){
        validacao.Mensagem.Add("O nome não pode ser vazio");
    }

    if(string.IsNullOrEmpty(veiculoDTO.Marca) ){
        validacao.Mensagem.Add("A marca não pode ser vazia");
    }

    if(veiculoDTO.Ano < 1900){
        validacao.Mensagem.Add("Veículo muito antigo");
    }
 
    return validacao;

}

app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, iVeiculoServico veiculoServico) => {

    
    var validacao = validaDTO(veiculoDTO);
    if(validacao.Mensagem.Count > 0){
        return Results.BadRequest(validacao);
    }

    var veiculo = new Veiculo{
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano
    };
    veiculoServico.Incluir(veiculo);

    return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
}).WithTags("Veiculos");

app.MapGet("/veiculos", ([FromQuery] int pagina, iVeiculoServico veiculoServico) => {
    var veiculos = veiculoServico.Todos(pagina);

    return Results.Ok(veiculos);
}).WithTags("Veiculos");

app.MapGet("/veiculos/{id}", ([FromRoute] int id, iVeiculoServico veiculoServico) => {
    var veiculo = veiculoServico.BuscaPorId(id);

    if(veiculo == null){
        return Results.NotFound();
    }

    return Results.Ok(veiculo);
}).WithTags("Veiculos");

app.MapPut("/veiculos/{id}", ([FromRoute] int id, VeiculoDTO veiculoDTO, iVeiculoServico veiculoServico) => {
    
    var validacao = validaDTO(veiculoDTO);
    if(validacao.Mensagem.Count > 0){
        return Results.BadRequest(validacao);
    }

    var veiculo = veiculoServico.BuscaPorId(id);

    if(veiculo == null){
        return Results.NotFound();
    }
    
    veiculo.Nome = veiculoDTO.Nome;
    veiculo.Marca = veiculoDTO.Marca;
    veiculo.Ano = veiculoDTO.Ano;

    veiculoServico.Atualizar(veiculo);

    return Results.Ok(veiculo);
}).WithTags("Veiculos");

app.MapDelete("/veiculos/{id}", ([FromRoute] int id, iVeiculoServico veiculoServico) => {
    var veiculo = veiculoServico.BuscaPorId(id);

    if(veiculo == null){
        return Results.NotFound();
    }

    veiculoServico.ApagarPorId(veiculo);

    return Results.NoContent();
}).WithTags("Veiculos");


#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
#endregion

