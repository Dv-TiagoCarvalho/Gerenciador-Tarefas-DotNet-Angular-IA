using Microsoft.EntityFrameworkCore;
using TarefasApi.Data;
using TarefasApi.Models;

namespace TarefasApi.Services;

// Interface do serviço: o controller depende da interface, não da implementação.
// Isso facilita testes e troca de implementação (mesma ideia do Spring).
public interface ITarefaService
{
    Task<List<Tarefa>> ListarAsync();
    Task<Tarefa?> BuscarPorIdAsync(int id);
    Task<Tarefa> CriarAsync(Tarefa tarefa);
    Task<bool> AtualizarAsync(int id, Tarefa dados);
    Task<bool> AlternarConclusaoAsync(int id);
    Task<bool> ExcluirAsync(int id);
}

// Implementação com as regras de negócio (como uma classe @Service).
public class TarefaService : ITarefaService
{
    private readonly AppDbContext _db;

    // Injeção de dependência pelo construtor (como @Autowired no construtor).
    public TarefaService(AppDbContext db)
    {
        _db = db;
    }

    // async/await: a thread fica livre enquanto espera o banco responder.
    public async Task<List<Tarefa>> ListarAsync()
    {
        // LINQ: consulta escrita em C# que o EF Core traduz para SQL.
        // SQL gerado: SELECT * FROM Tarefas ORDER BY Concluida, CriadaEm DESC
        return await _db.Tarefas
            .OrderBy(t => t.Concluida)
            .ThenByDescending(t => t.CriadaEm)
            .ToListAsync();
    }

    public async Task<Tarefa?> BuscarPorIdAsync(int id)
    {
        return await _db.Tarefas.FindAsync(id);
    }

    public async Task<Tarefa> CriarAsync(Tarefa tarefa)
    {
        // Regra de negócio: o servidor decide esses campos, não o cliente.
        tarefa.Id = 0;
        tarefa.Titulo = tarefa.Titulo.Trim();
        tarefa.Concluida = false;
        tarefa.CriadaEm = DateTime.Now;

        _db.Tarefas.Add(tarefa);
        await _db.SaveChangesAsync(); // executa o INSERT
        return tarefa;
    }

    public async Task<bool> AtualizarAsync(int id, Tarefa dados)
    {
        var tarefa = await _db.Tarefas.FindAsync(id);
        if (tarefa is null) return false;

        tarefa.Titulo = dados.Titulo.Trim();
        tarefa.Descricao = dados.Descricao;
        tarefa.Concluida = dados.Concluida;

        await _db.SaveChangesAsync(); // o EF detecta o que mudou e faz o UPDATE
        return true;
    }

    public async Task<bool> AlternarConclusaoAsync(int id)
    {
        var tarefa = await _db.Tarefas.FindAsync(id);
        if (tarefa is null) return false;

        tarefa.Concluida = !tarefa.Concluida;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExcluirAsync(int id)
    {
        var tarefa = await _db.Tarefas.FindAsync(id);
        if (tarefa is null) return false;

        _db.Tarefas.Remove(tarefa);
        await _db.SaveChangesAsync(); // executa o DELETE
        return true;
    }
}
