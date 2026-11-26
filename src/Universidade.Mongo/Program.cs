using System.Globalization;
using MongoDB.Driver;
using Universidade.Mongo.Data;

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Universidade");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException(
        "Connection string 'Universidade' não configurada. Rode na pasta do projeto:\n" +
        "dotnet user-secrets set \"ConnectionStrings:Universidade\" \"mongodb://usuario:senha@host:27017/?authSource=admin\"");

var nomeBanco = builder.Configuration["MongoDatabase"] ?? "universidade";

builder.Services.AddSingleton(new MongoClient(connectionString));
builder.Services.AddSingleton(sp => sp.GetRequiredService<MongoClient>().GetDatabase(nomeBanco));
builder.Services.AddSingleton<MongoDb>();
builder.Services.AddSingleton<OpcoesRepository>();
builder.Services.AddSingleton<UsuarioRepository>();
builder.Services.AddSingleton<EstudanteRepository>();
builder.Services.AddSingleton<VinculoRepository>();
builder.Services.AddSingleton<CursoRepository>();

builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
