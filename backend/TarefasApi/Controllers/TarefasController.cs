using Microsoft.AspNetCore.Mvc;
using TarefasApi.Models;
using TarefasApi.Services;

namespace TarefasApi.Controllers;

// [ApiController] + [Route] = @RestController + @RequestMapping("/api/tarefas") do Spring.
// [ApiController] também valida o modelo automaticamente: se o título vier vazio,
// a API já responde 400 Bad Request sem precisar de "if" no código.
[ApiController]
[Route("api/[controller]")]
public class TarefasController : ControllerBase
{
    private readonly ITarefaService _service;

    public TarefasController(ITarefaService service)
    {
        _service = service;
    }

    // GET /api/tarefas  (@GetMapping)
    [HttpGet]
    public async Task<ActionResult<List<Tarefa>>> Listar()
    {
        return Ok(await _service.ListarAsync());
    }

    // GET /api/tarefas/5  (@GetMapping("/{id}") + @PathVariable)
    [HttpGet("{id}")]
    public async Task<ActionResult<Tarefa>> BuscarPorId(int id)
    {
        var tarefa = await _service.BuscarPorIdAsync(id);
        if (tarefa is null) return NotFound(); // 404
        return Ok(tarefa);                      // 200
    }

    // POST /api/tarefas  (@PostMapping + @RequestBody)
    [HttpPost]
    public async Task<ActionResult<Tarefa>> Criar(Tarefa tarefa)
    {
        var criada = await _service.CriarAsync(tarefa);
        // 201 Created, com o endereço da nova tarefa no cabeçalho Location
        return CreatedAtAction(nameof(BuscarPorId), new { id = criada.Id }, criada);
    }

    // PUT /api/tarefas/5  (@PutMapping)
    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, Tarefa dados)
    {
        var ok = await _service.AtualizarAsync(id, dados);
        return ok ? NoContent() : NotFound(); // 204 ou 404
    }

    // PATCH /api/tarefas/5/concluir  (@PatchMapping)
    [HttpPatch("{id}/concluir")]
    public async Task<IActionResult> AlternarConclusao(int id)
    {
        var ok = await _service.AlternarConclusaoAsync(id);
        return ok ? NoContent() : NotFound();
    }

    // DELETE /api/tarefas/5  (@DeleteMapping)
    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(int id)
    {
        var ok = await _service.ExcluirAsync(id);
        return ok ? NoContent() : NotFound();
    }
}
