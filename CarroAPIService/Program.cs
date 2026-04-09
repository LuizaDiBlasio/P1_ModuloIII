
using System.Text;
using CarroAPIService.Services;
using CarroAPIService.Services.Interfaces;
using CarrosLib.DTOs;
using CarrosLib.Helpers;
using CarrosLib.Helpers.Interfaces;
using CarrosLib.Repositories;
using CarrosLib.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

//Logger

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", 
        rollingInterval: RollingInterval.Day)
    .CreateLogger();


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.-------------------------------------------------------------

builder.Host.UseSerilog();

//CORS

builder.Services.AddCors(options => 
{
    options.AddPolicy("cors",
        policy =>
        {
            policy 
                .AllowAnyOrigin() 
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

//JWT Bearer

var secret_key = builder.Configuration["App:JWT:SECRET_KEY"]; 
var key = Encoding.UTF8.GetBytes(secret_key); 
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) 
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters 
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = "CarrosDB", 
        ValidAudience = "CarrosDB",

        IssuerSigningKey = new SymmetricSecurityKey(key) 
    };
});

//Swagger


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JSON Web Token baseado no esquema Bearer. Exemplo: \"Bearer {token}\""
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

//DI

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ICarroRepository,CarroRepository>();
builder.Services.AddScoped<ICarroService, CarroService>();
builder.Services.AddScoped<IMarcaRepository, MarcaRepository>();
builder.Services.AddScoped<IMarcaService, MarcaService>();
builder.Services.AddScoped<IModeloRepository, ModeloRepository>();
builder.Services.AddScoped<IModeloService, ModeloService>();
builder.Services.AddScoped<IConnectionHelper, ConnectionHelper>();
builder.Services.AddScoped<ILoginHelper, LoginHelper>();

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseCors("cors");

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.

app.MapGet("/", () => "CarrosDB API");


app.MapPost("/login", (LoginDTO login, AuthService auth, ILogger<Program> logger) =>
{
    if (auth.Login(login) == System.Net.HttpStatusCode.OK) 
    {
        var token = auth.GenerateToken(login.Username, secret_key);
        logger.LogInformation($"Endpoint /login token: {token}");

        return Results.Ok(new { token });
    }
    logger.LogWarning($"Tentativa de login INVÁLIDA para o usuário: {login.Username}"); 
    return Results.Unauthorized();
});

app.MapGet("/carros", (ICarroService service) =>
{
    return service.GetAll();
})
.RequireAuthorization();

app.MapPost("/filtro", (ICarroService service, FiltroDTO filtro) =>
{
    return service.FilterCars(filtro);
})
.RequireAuthorization();

app.MapGet("/anos", (ICarroService service) =>
{
    return service.GetAllAnos();
})
.RequireAuthorization();

app.MapGet("/marcas", (IMarcaService service) =>
{
    return service.GetAll();
})
.RequireAuthorization();

app.MapGet("/modelos", (IModeloService service) =>
{
    return service.GetAll();
})
.RequireAuthorization();

app.MapGet("/carros/{id}", (int id, ICarroService service) =>
{
    var p = service.GetById(id);

    return p == null ? Results.NotFound() : Results.Ok(p);
})
    .RequireAuthorization();

app.MapPost("/carros", (CarroCreateDTO dto, ICarroService service) =>
{
    int id = service.Create(dto);

    return Results.Created($"/carros/{id}", new { id });
})
    .RequireAuthorization();

app.MapPut("/carrosUpdate/{id}", (int id, CarroCreateDTO dto, ICarroService service) =>
{
    service.Update(id, dto);

    return Results.Ok();
})
    .RequireAuthorization();

app.MapDelete("/carros/{id}", (int id, ICarroService service) =>
{
    service.Delete(id);

    return Results.Ok();
})
    .RequireAuthorization();

app.MapDelete("/carrosDeleteAll", (ICarroService service) =>
{
    service.DeleteAll();

    return Results.Ok();
})
    .RequireAuthorization();

app.MapPost("/carrosResetDB", (ICarroService service) =>
{
    service.ResetDB();

    return Results.Ok();
})
    .RequireAuthorization();

app.Run();

