using Microsoft.EntityFrameworkCore;
using TarefasApi.Data;
using TarefasApi.Models;
using TarefasApi.Services;
using Xunit;

namespace TarefasApi.Tests;

// Testes das regras do serviço, usando banco em memória.
// [Fact] equivale ao @Test do JUnit; Assert.Equal ao assertEquals.
public class TarefaServiceTests
{
    // Cada teste recebe um banco novo e vazio (nome único).
    private static TarefaService CriarService()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new TarefaService(new AppDbContext(options));
    }

    [Fact]
    public async Task Criar_DeveSalvarComoNaoConcluidaERemoverEspacos()
    {
        var service = CriarService();

        var criada = await service.CriarAsync(new Tarefa { Titulo = "  Estudar C#  ", Concluida = true });

        Assert.True(criada.Id > 0);
        Assert.Equal("Estudar C#", criada.Titulo);
        Assert.False(criada.Concluida); // o servidor ignora o "Concluida = true" do cliente
    }

    [Fact]
    public async Task AlternarConclusao_DeveInverterOStatus()
    {
        var service = CriarService();
        var tarefa = await service.CriarAsync(new Tarefa { Titulo = "Estudar Angular" });

        await service.AlternarConclusaoAsync(tarefa.Id);

        var atualizada = await service.BuscarPorIdAsync(tarefa.Id);
        Assert.True(atualizada!.Concluida);
    }

    [Fact]
    public async Task Listar_DeveMostrarPendentesAntesDasConcluidas()
    {
        var service = CriarService();
        var a = await service.CriarAsync(new Tarefa { Titulo = "A" });
        await service.CriarAsync(new Tarefa { Titulo = "B" });
        await service.AlternarConclusaoAsync(a.Id); // A fica concluída

        var lista = await service.ListarAsync();

        Assert.Equal("B", lista[0].Titulo);
        Assert.Equal("A", lista[1].Titulo);
    }

    [Fact]
    public async Task Excluir_IdInexistente_DeveRetornarFalse()
    {
        var service = CriarService();

        var resultado = await service.ExcluirAsync(999);

        Assert.False(resultado);
    }
}
