using Microsoft.EntityFrameworkCore;
using TarefasApi.Models;

namespace TarefasApi.Data;

// DbContext: a "porta" para o banco de dados.
// Faz o papel do EntityManager do JPA e, junto com o DbSet, dos Repositories do Spring Data.
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Cada DbSet representa uma tabela.
    public DbSet<Tarefa> Tarefas => Set<Tarefa>();
}
