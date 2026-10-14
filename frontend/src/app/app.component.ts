import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Tarefa } from './models/tarefa';
import { TarefaService } from './services/tarefa.service';

// Componente: junta a lógica (esta classe), o HTML (templateUrl) e o CSS (styleUrl).
// "standalone: true" = não precisa de NgModule; os imports ficam aqui mesmo.
@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule], // FormsModule habilita o [(ngModel)]
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent implements OnInit {
  tarefas: Tarefa[] = [];

  // Campos do formulário (ligados ao HTML com [(ngModel)])
  titulo = '';
  descricao = '';

  // Quando diferente de null, o formulário está editando esta tarefa.
  emEdicao: Tarefa | null = null;

  erro = '';
  carregando = false;

  constructor(private tarefaService: TarefaService) {}

  // ngOnInit roda uma vez quando o componente aparece na tela.
  ngOnInit(): void {
    this.carregar();
  }

  carregar(): void {
    this.carregando = true;
    this.tarefaService.listar().subscribe({
      next: (lista) => {
        this.tarefas = lista;
        this.carregando = false;
      },
      error: () => this.mostrarErro('Não foi possível carregar as tarefas. A API está rodando?'),
    });
  }

  salvar(): void {
    if (!this.titulo.trim()) {
      this.erro = 'Informe um título.';
      return;
    }

    if (this.emEdicao) {
      const atualizada: Tarefa = { ...this.emEdicao, titulo: this.titulo, descricao: this.descricao };
      this.tarefaService.atualizar(atualizada).subscribe({
        next: () => this.depoisDeSalvar(),
        error: () => this.mostrarErro('Erro ao atualizar a tarefa.'),
      });
    } else {
      this.tarefaService.criar(this.titulo, this.descricao).subscribe({
        next: () => this.depoisDeSalvar(),
        error: () => this.mostrarErro('Erro ao criar a tarefa.'),
      });
    }
  }

  editar(tarefa: Tarefa): void {
    this.emEdicao = tarefa;
    this.titulo = tarefa.titulo;
    this.descricao = tarefa.descricao ?? '';
  }

  cancelarEdicao(): void {
    this.emEdicao = null;
    this.limparFormulario();
  }

  alternarConclusao(tarefa: Tarefa): void {
    this.tarefaService.alternarConclusao(tarefa.id).subscribe({
      next: () => this.carregar(),
      error: () => this.mostrarErro('Erro ao atualizar a tarefa.'),
    });
  }

  excluir(tarefa: Tarefa): void {
    if (!confirm(`Excluir "${tarefa.titulo}"?`)) return;
    this.tarefaService.excluir(tarefa.id).subscribe({
      next: () => this.carregar(),
      error: () => this.mostrarErro('Erro ao excluir a tarefa.'),
    });
  }

  get pendentes(): number {
    return this.tarefas.filter((t) => !t.concluida).length;
  }

  private depoisDeSalvar(): void {
    this.emEdicao = null;
    this.limparFormulario();
    this.carregar();
  }

  private limparFormulario(): void {
    this.titulo = '';
    this.descricao = '';
    this.erro = '';
  }

  private mostrarErro(mensagem: string): void {
    this.erro = mensagem;
    this.carregando = false;
  }
}
