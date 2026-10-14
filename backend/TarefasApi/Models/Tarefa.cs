using System.ComponentModel.DataAnnotations;

namespace TarefasApi.Models;

// Entidade: vira uma tabela no banco (como uma classe com @Entity no JPA).
public class Tarefa
{
    // "Id" é reconhecido automaticamente como chave primária (como @Id + @GeneratedValue).
    public int Id { get; set; }

    // Properties com get; set; substituem os getters e setters do Java.
    [Required(ErrorMessage = "O título é obrigatório.")]
    [MaxLength(100, ErrorMessage = "O título pode ter no máximo 100 caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Descricao { get; set; } // "?" = pode ser nulo

    public bool Concluida { get; set; }

    public DateTime CriadaEm { get; set; } = DateTime.Now;
}
