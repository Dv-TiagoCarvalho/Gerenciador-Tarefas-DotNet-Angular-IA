using Microsoft.EntityFrameworkCore;
using TarefasApi.Data;
using TarefasApi.Services;

// Program.cs é o ponto de entrada (como a classe com @SpringBootApplication).
var builder = WebApplication.CreateBuilder(args);

// ---------- 1. Registro de serviços (injeção de dependência) ----------

builder.Services.AddControllers();

// Banco SQLite num arquivo local (a connection string fica no appsettings.json).
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Padrao")));

// AddScoped = uma instância por requisição HTTP.
// (AddSingleton = uma para a aplicação toda; AddTransient = nova a cada injeção.)
builder.Services.AddScoped<ITarefaService, TarefaService>();

// CORS: libera o Angular (porta 4200) para chamar a API (porta 5000).
builder.Services.AddCors(options =>
    options.AddPolicy("Angular", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ---------- 2. Cria o banco na primeira execução ----------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// ---------- 3. Pipeline de requisições (middlewares) ----------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // http://localhost:5000/swagger
}

app.UseCors("Angular");
app.MapControllers();

app.Run();

// Torna a classe Program visível para os testes de integração.
public partial class Program { }
