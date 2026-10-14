// Interface TypeScript com o mesmo formato do JSON que a API devolve.
// (O ASP.NET converte "Titulo" para "titulo" no JSON automaticamente.)
export interface Tarefa {
  id: number;
  titulo: string;
  descricao?: string | null;
  concluida: boolean;
  criadaEm: string;
}
