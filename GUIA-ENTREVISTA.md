# Guia de estudo para a entrevista

Objetivo: conseguir explicar este projeto com segurança e ligar o que você já sabe de Java/Spring ao .NET e ao Angular.

---

## 1. Pitch do projeto (30 segundos)

> "Como minha base é Java com Spring Boot, fiz este projeto para aprender a stack de vocês. É um gerenciador de tarefas com API REST em ASP.NET Core e tela em Angular. O back-end segue a mesma separação que eu usava no Spring: controller recebe a requisição, service tem as regras de negócio e o Entity Framework faz o acesso ao banco, como o JPA. Também escrevi testes com xUnit, que é o equivalente ao JUnit."

---

## 2. Tabela Java/Spring → .NET (a mais importante)

| Java / Spring Boot                  | C# / ASP.NET Core                          | Onde está no projeto        |
|-------------------------------------|--------------------------------------------|-----------------------------|
| `pom.xml` (Maven)                   | `.csproj` (NuGet)                          | `TarefasApi.csproj`         |
| Classe com `@SpringBootApplication` | `Program.cs`                               | `Program.cs`                |
| `application.properties`            | `appsettings.json`                         | `appsettings.json`          |
| `@RestController` + `@RequestMapping` | `[ApiController]` + `[Route]`            | `TarefasController.cs`      |
| `@GetMapping`, `@PostMapping`...    | `[HttpGet]`, `[HttpPost]`...               | `TarefasController.cs`      |
| `@PathVariable` / `@RequestBody`    | parâmetro `int id` / objeto no parâmetro   | `TarefasController.cs`      |
| `ResponseEntity.ok()` / `.notFound()` | `Ok()` / `NotFound()` / `NoContent()`    | `TarefasController.cs`      |
| `@Service`                          | classe registrada com `AddScoped`          | `TarefaService.cs`          |
| `@Autowired` (no construtor)        | injeção pelo construtor (nativa)           | todos os construtores       |
| `@Entity` + `@Id`                   | classe simples + propriedade `Id`          | `Tarefa.cs`                 |
| `@NotBlank`, `@Size`                | `[Required]`, `[MaxLength]`                | `Tarefa.cs`                 |
| `EntityManager` / `JpaRepository`   | `DbContext` + `DbSet<T>`                   | `AppDbContext.cs`           |
| JPQL / métodos do repository        | LINQ (`OrderBy`, `Where`...)               | `TarefaService.cs`          |
| `repository.save()`                 | `Add()` + `SaveChangesAsync()`             | `TarefaService.cs`          |
| getters e setters                   | properties `{ get; set; }`                 | `Tarefa.cs`                 |
| `CompletableFuture`                 | `Task` + `async/await`                     | service e controller        |
| JUnit `@Test` / `assertEquals`      | xUnit `[Fact]` / `Assert.Equal`            | `TarefaServiceTests.cs`     |
| H2 em memória nos testes            | EF Core InMemory                           | `TarefaServiceTests.cs`     |

---

## 3. O caminho de uma requisição (saiba contar isso de cabeça)

Exemplo: o usuário clica em **Adicionar**.

1. **HTML:** o formulário tem `(ngSubmit)="salvar()"`, então o Angular chama `salvar()` no componente.
2. **Componente:** `salvar()` valida o título e chama `tarefaService.criar(titulo, descricao)`.
3. **Service Angular:** faz `http.post('http://localhost:5000/api/tarefas', {...})`. A requisição só sai quando o componente chama `.subscribe()`.
4. **ASP.NET Core:** o middleware de CORS libera a origem `localhost:4200`, e a rota leva até `TarefasController.Criar`.
5. **Validação automática:** como o controller tem `[ApiController]`, se o título vier vazio a API responde **400** antes de executar o método.
6. **Controller:** chama `_service.CriarAsync(tarefa)`.
7. **Service C#:** aplica as regras (remove espaços, força `Concluida = false`, define a data), faz `Add` e `SaveChangesAsync`, e o EF Core gera o `INSERT`.
8. **Resposta:** o controller devolve **201 Created** com a tarefa em JSON.
9. **De volta ao Angular:** o `next` do `subscribe` roda `depoisDeSalvar()`, que limpa o formulário e recarrega a lista.

---

## 4. Conceitos de Angular que aparecem no projeto

| Conceito | O que é | Exemplo no projeto |
|---|---|---|
| Componente | Classe + HTML + CSS de um pedaço da tela | `AppComponent` |
| Interpolação `{{ }}` | Mostra um valor na tela | `{{ tarefa.titulo }}` |
| Property binding `[ ]` | Classe → HTML | `[checked]="tarefa.concluida"` |
| Event binding `( )` | HTML → classe | `(click)="excluir(tarefa)"` |
| Two-way binding `[( )]` | Os dois sentidos ao mesmo tempo | `[(ngModel)]="titulo"` |
| `*ngIf` / `*ngFor` | Mostrar condicionalmente / repetir | lista de tarefas |
| Pipe | Formata um valor no HTML | `| date: 'dd/MM/yyyy HH:mm'` |
| Service + `@Injectable` | Lógica reaproveitável, injetada nos componentes | `TarefaService` |
| `HttpClient` | Faz requisições HTTP | dentro do service |
| Observable + `subscribe` | Resultado assíncrono; a requisição só roda quando alguém se inscreve | `listar().subscribe(...)` |
| `ngOnInit` | Roda quando o componente é criado | carrega a lista |

**Comparação útil com React** (você já usou): em React o estado fica em `useState` e você chama `fetch`/`axios`; no Angular o estado são variáveis da classe, o HTML fica num template separado e as chamadas HTTP ficam em services injetados.

---

## 5. Perguntas prováveis e como responder

**"Por que separar controller e service?"**
O controller só cuida de HTTP (rotas, status code). As regras de negócio ficam no service, o que deixa o código organizado e permite testar as regras sem subir a API, que é o que os meus testes fazem.

**"Qual a diferença entre AddScoped, AddTransient e AddSingleton?"**
Scoped: uma instância por requisição HTTP. Transient: uma nova cada vez que é injetado. Singleton: uma só para a aplicação toda. Usei Scoped no service porque o `DbContext` também é Scoped e não deve ser compartilhado entre requisições.

**"Por que usar async/await?"**
Enquanto o banco responde, a thread fica livre para atender outras requisições. Isso aumenta a capacidade do servidor.

**"O que é o Entity Framework?"**
Um ORM, como o Hibernate. Mapeia classes C# para tabelas e traduz as consultas LINQ para SQL.

**"O que é CORS e por que você configurou?"**
O navegador bloqueia chamadas de um site para outra origem. O Angular roda na porta 4200 e a API na 5000, então liberei a origem do Angular na API.

**"Por que PUT para editar e PATCH para concluir?"**
PUT substitui os dados da tarefa. PATCH faz uma alteração parcial, só o status de conclusão.

**"Por que 201, 204 e 404?"**
201 indica que algo foi criado. 204 é sucesso sem corpo na resposta (atualizar e excluir). 404 é quando o id não existe.

**"Por que validar no front-end e no back-end?"**
No front é para dar resposta rápida ao usuário. No back é obrigatório, porque qualquer pessoa pode chamar a API direto, sem passar pela tela.

**"O que você melhoraria?"**
- Usar DTOs em vez de receber a entidade direto no controller, para controlar o que o cliente pode enviar.
- Trocar `EnsureCreated` por migrations, para versionar o banco.
- Usar SQL Server, que é o mais comum com .NET em empresas.
- Autenticação com JWT (já fiz isso em Python no projeto de agendamento).
- Paginação e filtro na listagem.

> Saber o que melhorar mostra maturidade. Fale disso com tranquilidade, não como falha.

**"Você usou IA?"** (se perguntarem)
Seja honesto: diga que usou como apoio, mas que estudou cada parte e sabe explicar o fluxo. Depois mostre que sabe, por exemplo explicando o caminho da requisição (seção 3).

---

## 6. Pontos de C# que vale saber de cabeça

- `string?`: o `?` indica que pode ser nulo (nullable reference types).
- `var`: o tipo é inferido pelo compilador, como o `var` do Java 10+.
- `is null`: forma recomendada de comparar com nulo.
- Lambda: `t => t.Concluida` é como `t -> t.isConcluida()` no Java.
- `=>` em propriedade: `public DbSet<Tarefa> Tarefas => Set<Tarefa>();` é uma propriedade só de leitura.
- `nameof(BuscarPorId)`: pega o nome do método como texto, sem risco de erro de digitação.
- Namespaces fazem o papel dos packages do Java.

---

## 7. Plano de estudo sugerido

1. **Instale e rode** o projeto (README). Crie, edite, conclua e exclua tarefas. Teste também pelo Swagger.
2. **Leia o back-end na ordem:** `Tarefa.cs` → `AppDbContext.cs` → `TarefaService.cs` → `TarefasController.cs` → `Program.cs`. Os comentários explicam cada parte.
3. **Leia o front-end:** `tarefa.ts` → `tarefa.service.ts` → `app.component.ts` → `app.component.html`.
4. **Treine a seção 3 em voz alta** até conseguir contar sem olhar.
5. **Mude algo sozinho**, por exemplo adicionar um campo `Prioridade` (Baixa, Média, Alta). Fazer uma mudança sem ajuda é o que dá segurança de verdade.
6. **Suba no GitHub** e coloque o link no currículo.
