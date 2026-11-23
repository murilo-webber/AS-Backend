## Estrutura de pastas

```
Apiaguas/
├── Apiaguas.csproj                   # projeto .NET 10 (ASP.NET Core, EF Core, SQLite)
├── Program.cs                        # inicialização, registro do contexto, middleware e rotas
├── Models/
│   └── AguaEntity.cs                 # entidade: tabela Aguas (Id, Titulo)
├── Dtos/
│   └── AguaInputDto.cs               # tipo de entrada (record) usado no POST e no PUT
├── Data/
│   └── ApiDbContext.cs               # contexto do EF Core (DbSet de AguaEntity)
├── wwwroot/
│   └── index.html                    # interface servida pela própria API
├── bruno/                            # Collection do Bruno
│   ├── bruno.json
│   ├── environments/local.bru        # ambiente local (baseUrl = http://localhost:5162)
│   └── 01-… a 09-….bru               # requisições do roteiro de testes
├── Properties/launchSettings.json    # porta de execução (5162)
├── appsettings*.json                 # configurações de log
└── .gitignore                        # ignora bin/, obj/ e bancos locais
```

### Por que os tipos foram separados
| Pasta | Tipo | Responsabilidade |
|-------|------|------------------|
| `Models/` | `AguaEntity` | Entidade: espelha a tabela `Aguas` e inclui o `Id`, gerado pelo banco. |
| `Dtos/` | `AguaInputDto` | Entrada: só o `Titulo`. Sem `Id`, o cliente não escolhe o identificador. |
| `Data/` | `ApiDbContext` | Contexto do EF Core: liga a entidade ao SQLite. |

### Como o `Program.cs` encontra os tipos
Cada tipo tem um namespace igual ao da sua pasta (`Apiaguas.Models`, `Apiaguas.Dtos`, `Apiaguas.Data`) e o `Program.cs` os importa com `using`:

```csharp
using Apiaguas.Data;     // ApiDbContext
using Apiaguas.Dtos;     // AguaInputDto
using Apiaguas.Models;   // AguaEntity
using Microsoft.EntityFrameworkCore;
```

O `Program.cs` mantém apenas: criação do `builder` e registro do contexto (`AddDbContext` + `UseSqlite`), criação do banco (`EnsureCreated`), middleware de arquivos estáticos (`UseDefaultFiles`, `UseStaticFiles`) e as rotas da API. Nenhum tipo é definido dentro dele.

## Rotas da API

| Método | Rota              | Corpo           | Status de sucesso | Outros status | Descrição            |
|--------|-------------------|-----------------|-------------------|---------------|----------------------|
| GET    | `/api/aguas`      | —               | 200               | —             | Lista todas as águas |
| GET    | `/api/aguas/{id}` | —               | 200               | 404           | Busca por id         |
| POST   | `/api/aguas`      | `AguaInputDto`  | 201               | 400           | Cria uma água        |
| PUT    | `/api/aguas/{id}` | `AguaInputDto`  | 200               | 404, 400      | Atualiza uma água    |
| DELETE | `/api/aguas/{id}` | —               | 204               | 404           | Remove uma água      |

O `400` ocorre quando o corpo não é um JSON válido ou um campo tem tipo incompatível (por exemplo, `titulo` com número). A rota `/` redireciona para a interface (`/index.html`).

### Exemplos de JSON

Corpo enviado no POST e no PUT:

```json
{
  "titulo": "Água mineral sem gás"
}
```

Resposta do POST (`201 Created`, com o `Id` gerado pelo banco) e do PUT (`200 OK`):

```json
{
  "id": 1,
  "titulo": "Água mineral sem gás"
}
```

Se o corpo trouxer um `id`, ele é ignorado: o identificador é sempre gerado pelo banco.

## Interface
Interface em `wwwroot/index.html`, servida pela própria API em http://localhost:5162. Permite listar, cadastrar, editar e excluir águas usando os endpoints reais.

## Collection do Bruno
A Collection está na pasta **`bruno/`** deste repositório.

1. No Bruno: **Open Collection** e selecione a pasta `bruno/`.
2. Escolha o ambiente **local** (canto superior direito). Ele define `baseUrl = http://localhost:5162`.
3. Com a API rodando (`dotnet run`), execute as requisições em ordem:

| # | Requisição | Resultado esperado |
|---|------------|--------------------|
| 01 | GET `/api/aguas` | 200 e a listagem |
| 02 | POST `/api/aguas` | 201 e a água criada (o `id` é guardado na variável `aguaId`) |
| 03 | GET `/api/aguas/{{aguaId}}` | 200 e os mesmos dados criados |
| 04 | PUT `/api/aguas/{{aguaId}}` | 200 e os dados atualizados |
| 05 | GET `/api/aguas/{{aguaId}}` | 200 e os dados já atualizados (confirma o PUT) |
| 06 | DELETE `/api/aguas/{{aguaId}}` | 204, sem corpo |
| 07 | GET `/api/aguas/{{aguaId}}` | 404 após a exclusão |
| 08 | POST `/api/aguas` | 201 (segundo registro, **não** excluído; `id` guardado em `aguaIdPersistencia`) |
| 09 | GET `/api/aguas/{{aguaIdPersistencia}}` | 200 — execute **depois de parar e reiniciar** a API |

O identificador usado nas requisições vem sempre da resposta do POST (variáveis `aguaId` e `aguaIdPersistencia`); não é preciso que seja 1. Entre as requisições 08 e 09, pare a API (`Ctrl+C`), execute `dotnet run` de novo e **não feche o Bruno**, para manter a variável.

Todas as requisições têm *assert* do status HTTP esperado e, quando há corpo na resposta, *tests* que conferem os dados. Para executar tudo de uma vez, use o **Runner** do Bruno (clique com o botão direito na Collection → **Run**) ou, pela linha de comando:


## Vídeo de demonstração — AS
https://drive.google.com/file/d/1j8NclFpV6b0HFK8CESQ2g2tNxFer35Rb/view?usp=sharing

## Observações
- Não versionar `bin/`, `obj/` nem o arquivo `aguas.db` (já listados no `.gitignore`).
- Dados persistem após reiniciar a aplicação.
