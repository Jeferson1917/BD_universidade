using Npgsql;
using Universidade.Web.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Universidade");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException(
        "Connection string 'Universidade' não configurada. Rode na pasta do projeto:\n" +
        "dotnet user-secrets set \"ConnectionStrings:Universidade\" \"Host=...;Port=5432;Database=universidade_db;Username=postgres;Password=...;SSL Mode=Require\"");

builder.Services.AddSingleton(NpgsqlDataSource.Create(connectionString));
builder.Services.AddSingleton<Db>();
builder.Services.AddSingleton<UsuarioRepository>();

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
