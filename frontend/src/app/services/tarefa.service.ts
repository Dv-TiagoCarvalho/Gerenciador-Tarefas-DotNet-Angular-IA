import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Tarefa } from '../models/tarefa';

// Service: concentra as chamadas HTTP para a API.
// @Injectable({ providedIn: 'root' }) = uma instância única para o app todo
// (parecido com um @Service singleton do Spring).
@Injectable({ providedIn: 'root' })
export class TarefaService {
  private readonly url = 'http://localhost:5000/api/tarefas';

  // Injeção de dependência pelo construtor, igual ao C# e ao Spring.
  constructor(private http: HttpClient) {}

  // Os métodos devolvem Observable: a requisição só acontece
  // quando alguém chama .subscribe().
  listar(): Observable<Tarefa[]> {
    return this.http.get<Tarefa[]>(this.url);
  }

  criar(titulo: string, descricao: string): Observable<Tarefa> {
    return this.http.post<Tarefa>(this.url, { titulo, descricao });
  }

  atualizar(tarefa: Tarefa): Observable<void> {
    return this.http.put<void>(`${this.url}/${tarefa.id}`, tarefa);
  }

  alternarConclusao(id: number): Observable<void> {
    return this.http.patch<void>(`${this.url}/${id}/concluir`, {});
  }

  excluir(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
