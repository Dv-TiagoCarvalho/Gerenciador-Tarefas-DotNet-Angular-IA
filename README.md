# Gerenciador de Tarefas — ASP.NET Core + Angular

Aplicação full stack de cadastro de tarefas (CRUD), feita para aprender a stack .NET + Angular a partir da minha base em Java/Spring Boot.

- **Back-end:** C#, ASP.NET Core 8 (Web API), Entity Framework Core, SQLite
- **Front-end:** Angular 18 (componentes standalone), TypeScript, HttpClient
- **Testes:** xUnit com banco em memória

## Funcionalidades

- Criar, listar, editar e excluir tarefas
- Marcar e desmarcar uma tarefa como concluída
- Tarefas pendentes aparecem primeiro na lista
- Validação do título no back-end (obrigatório, até 100 caracteres) e no front-end

## Estrutura

```
GerenciadorTarefas/
├── backend/
│   ├── TarefasApi/
│   │   ├── Controllers/TarefasController.cs   → endpoints REST
│   │   ├── Services/TarefaService.cs          → regras de negócio
│   │   ├── Data/AppDbContext.cs               → acesso ao banco (EF Core)
│   │   ├── Models/Tarefa.cs                   → entidade
│   │   └── Program.cs                         → configuração e injeção de dependência
│   └── TarefasApi.Tests/                      → testes com xUnit
└── frontend/
    └── src/app/
        ├── models/tarefa.ts                   → interface da tarefa
        ├── services/tarefa.service.ts         → chamadas HTTP para a API
        └── app.component.(ts|html|css)        → tela
```

## Endpoints

| Método | Rota                          | Descrição                    | Resposta        |
|--------|-------------------------------|------------------------------|-----------------|
| GET    | `/api/tarefas`                | Lista todas as tarefas       | 200             |
| GET    | `/api/tarefas/{id}`           | Busca uma tarefa             | 200 ou 404      |
| POST   | `/api/tarefas`                | Cria uma tarefa              | 201 ou 400      |
| PUT    | `/api/tarefas/{id}`           | Atualiza uma tarefa          | 204, 400 ou 404 |
| PATCH  | `/api/tarefas/{id}/concluir`  | Alterna o status de conclusão| 204 ou 404      |
| DELETE | `/api/tarefas/{id}`           | Exclui uma tarefa            | 204 ou 404      |

## Como rodar

**Pré-requisitos:** [.NET SDK 8](https://dotnet.microsoft.com/download) e [Node.js 20+](https://nodejs.org/).

### 1. Back-end

```bash
cd backend/TarefasApi
dotnet run
```

A API sobe em `http://localhost:5000`. O Swagger (para testar pelo navegador) fica em `http://localhost:5000/swagger`. O banco `tarefas.db` é criado automaticamente na primeira execução.

### 2. Front-end (em outro terminal)

```bash
cd frontend
npm install
npm start
```

Abra `http://localhost:4200`.

### 3. Testes

```bash
cd backend
dotnet test
```
