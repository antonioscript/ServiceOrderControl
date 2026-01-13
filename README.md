# ServiceOrderControl

## Notas sobre o desafio

Ao longo do desenvolvimento eu busquei:

- Atender todos os requisitos funcionais principais (clientes, OS, status, valor, anexos opcionais);
- Revisar e corrigir problemas estruturais de código (arquitetura, acoplamentos, validações);
- Entregar o projeto sem erros ou warnings de compilação e com análise estática limpa via Sonar;
- Documentar as decisões de design e os pontos que ficaram como melhorias futuras.

### Como executar o projeto (localmente)

**1. Clonar o Repositório**

 ```bash
   git clone https://github.com/antonioscript/ServiceOrderControl
````

**2. Uma instância SQL Server rodando em localhost (Pode ser SQL Server local, Docker ou SQL Express)**
A connection string padrão está em `src/Apis/OsService.ApiService/appsettings.json`



----

### Sumário

- [Sobre o desenvolvimento do Projeto](##sobre-o-desenvolvimento-do-Projeto)
- [Arquitetura da solução](#arquitetura-da-solução)
- [Arquitetura de Código](#arquitetura-de-código)
  - [Camada Domain](#camada-domain)
  - [Camada Application](#camada-application)
  - [Camada Infrastructure](#camada-infrastructure)
  - [Camada ApiService](#camada-apiservice)
- [Módulos e composição](#módulos-e-composição-imodule)
- [UseCases](#usecases)
- [Padrão Mediator](#padrão-mediator)
- [AutoMapper](#automapper)
- [Padrão Result](#padrão-result)
- [Padronização de respostas HTTP (Result → IActionResult)](#padronização-de-respostas-http-result--iactionresult)
- [GlobalExceptionHandler](#globalexceptionhandler)
- [Padrão Unit of Work](#padrão-unit-of-work)
- [Padrão Repository](#padrão-repository)
- [ORM Entity Framework Core](#orm-entity-framework-core)

- [Descritivo do Desafio](#Descritivo-do-Desafio)



## Sobre o desenvolvimento do projeto

Durante o desenvolvimento eu mantive um **Project** no próprio GitHub, usando a aba **Projects** do repositório para organizar tudo que precisava ser feito:  
issues, ideias, ajustes de arquitetura, validações, logs, Docker, etc.

> Board completo aqui:  
> **https://github.com/users/antonioscript/projects/14**

</br>
<img width="1291" height="912" alt="image" src="https://github.com/user-attachments/assets/87d2bc18-da99-4fad-aa70-253546656022" />
</br>

No board é possível acompanhar:

- **Backlog** – itens levantados desde a leitura inicial do desafio (padrões de arquitetura, validações, cache, logs, container, etc.).
- **Ready / In progress** – o que foi priorizado e desenvolvido em cada etapa.
- **Done** – tudo que entrou efetivamente na solução  

Também deixei registrados alguns itens que não eram obrigatórios para a entrega, mas que eu implementaria caso tivesse mais tempo, como:

- Validação mais completa de **CPF/CNPJ** (ex.: usando regex + cálculo de dígitos verificadores).
- Testes unitários para Handlers, Repositórios e Validators.
- Implementar cache em alguns endpoints de leitura.

Para garantir a qualidade do código na entrega, usei a extensão do Sonar integrada ao Visual Studio.  
Ao final do desenvolvimento o relatório indicava nenhum erro ou warning, Atendendo assim ao requisito do desafio de entregar o projetosem erros nem warnings de compilação.

</br>
<img width="964" height="547" alt="image" src="https://github.com/user-attachments/assets/fb87deea-315b-45cb-98d0-fca3e92152d2" />
</br>




## Arquitetura da solução



### Arquitetura de Código

Hoje a solução é composta, no backend, por:

A solução backend foi organizada seguindo um modelo **inspirado em DDD e Clean Architecture**, com separação clara entre:


- `OsService.Domain`
- `OsService.Application`
- `OsService.Infrastructure`
- `OsService.ApiService`
- `OsService.ServiceDefaults`

Além disso, os fluxos seguem um estilo **CQRS por caso de uso** (Commands/Queries + Handlers), o que ajuda a manter cada cenário de negócio pequeno, testável e fácil de evoluir.


> Os projetos de visualização (Blazor/Aspire AppHost) que vinham no template foram **removidos** para focar somente no backend do desafio.
#### Camada Domain

> “The Domain layer is completely persistence-agnostic, it only contains business concepts.”

A camada **Domain** contém:

- Entidades de domínio (`CustomerEntity`, `ServiceOrderEntity`, etc.)
- Enums (`ServiceOrderStatus`)
- Tipos genéricos de suporte, como `BaseEntity` e o **Result Pattern** (`Result`, `Result<T>`, `Error`).

Ela **não conhece** nada sobre EF Core, repositórios, conexões de banco ou infraestrutura. A ideia é manter o domínio “limpo” e estável



#### Camada Application

> “The Application layer defines repository and unit-of-work interfaces as outbound ports, and the Infrastructure layer implements these ports using EF Core.”

Responsável por **casos de uso** e **regras de aplicação**, aqui vivem:

- Interfaces de repositório (ou futuros serviços)
  (`ICustomerRepository`, `IServiceOrderRepository`, `IUnitOfWork`, etc.)
- Casos de uso organizados por **Features/UseCases**  
  Ex.: `Customers.CreateCustomer`, `ServiceOrders.OpenServiceOrder`, etc.
- Cada UseCase segue CQRS:
  - Um `Command` ou `Query`
  - Um `Handler`
  - E, quando faz sentido, um `Response` específico

> “Na camada de Application eu organizei por Features/UseCases. Cada caso de uso segue CQRS: um Command ou Query, com seu Handler, e quando faz sentido, um Response específico. A entidade de domínio (CustomerEntity) não é exposta diretamente, eu mapeio via AutoMapper para um modelo de saída (GetCustomerByIdResponse) que fica dentro da própria Feature. Infraestrutura implementa os repositórios, e a API só conhece os UseCases via MediatR.”

**Observação:**  
A camada de **Application não referencia mais a camada de Infrastructure**.  
As interfaces vivem em Application e são implementadas em Infrastructure, mantendo o sentido de dependência correto.

#### Camada Infrastructure

A camada **Infrastructure** implementa as portas definidas em Application:

- `OsServiceDbContext` com EF Core
- `EfRepository<TEntity>` (repositório genérico com métodos **virtuais**)
- Implementações concretas:
  - `CustomerRepository : EfRepository<CustomerEntity>, ICustomerRepository`
  - `ServiceOrderRepository : EfRepository<ServiceOrderEntity>, IServiceOrderRepository`
- Implementação de `IUnitOfWork`

> Sobre o `EfRepository<TEntity>`: os métodos são **virtuais** justamente para permitir que repositórios específicos sobrescrevam comportamento sem quebrar o contrato base.

#### Camada ApiService

É o projeto ASP.NET Core minimal hosting:

- `Program.cs` como Composition Root
- Controllers
- Registradores de serviços comuns (Swagger, ProblemDetails, ExceptionHandler)
- Conversão de `Result<T>` em `IActionResult`

A API **conhece apenas**:

- As interfaces da camada Application (via DI)
- Os `Command`/`Query` e manda tudo via MediatR

### Módulos e composição (`IModule`)

Para manter o `Program.cs` limpo e escalável, foi introduzido um padrão simples de módulos:


- `ApplicationModule` registra MediatR e AutoMapper
- `InfrastructureModule` registra DbContext, repositórios e Unit of Work
- `ApiServiceModule` (conceito) registra controllers, ProblemDetails, ExceptionHandler, OpenAPI

O `Program.cs` apenas chama:

```csharp
builder.Services.AddModules(
    builder.Configuration,
    typeof(ApplicationModule).Assembly,
    typeof(InfrastructureModule).Assembly,
    typeof(ApiServiceModule).Assembly);

### UseCases

Dentro de `OsService.Application` eu organizei a camada de aplicação por **Features / UseCases**, seguindo um estilo de **CQRS por cenário**.  
Em vez de ter um único `CustomerService` grande, cada caso de uso fica isolado em sua própria pasta.

Exemplo simplificado da estrutura:

```text
OsService.Application
 └─ V1
    └─ UseCases
       └─ Customers
          ├─ CreateCustomer
          │  ├─ CreateCustomer.Command.cs
          │  ├─ CreateCustomer.Handler.cs
          │  └─ CreateCustomer.Validator.cs
          ├─ GetCustomerById
          └─ GetCustomerByContact
```

Cada pasta de UseCase representa um cenário de negócio completo.
No caso de CreateCustomer, por exemplo:

Além disso, os DTOs de resposta (por exemplo, GetCustomerByIdResponse) e os profiles do AutoMapper (CustomerProfile) ficam próximos da Feature. Isso evita perfis genéricos espalhados e deixa claro quem produz e quem consome cada modelo.

Escolhi esse modelo porque a estrutura de pastas reflete a linguagem de domínio, tudo que pertence a determinada feature está junto e também na facilidade de manutenção


### Padrão Mediator

Para orquestrar os UseCases a partir da API, utilizei o **MediatR**, uma implementação do padrão **Mediator**.

Em vez de o Controller conhecer diretamente repositórios, serviços de domínio ou infraestrutura, ele conhece apenas um **IMediator** e envia um `Command` ou `Query`:

O MediatR é responsável por localizar o Handler correto para aquele Command/Query, executar o fluxo de negócio e devolver um Result<T> para o Controller.

Com isso, a camada da API, por exemplo, não precisa saber como o caso de uso é implementado, apenas qual comando enviar.
Isso reduz o risco de referências cruzadas indevidas (ex.: Controllers chamando repositórios diretamente). E não apenas por isso, evita também o caso de dependências circulaes, quando um serviço chama outro serviço, etc. 

E também O padrão Mediator conversa muito bem com a ideia de Commands/Queries separados, reforçando a separação entre operações de escrita e leitura.



## AutoMapper

Para evitar mapeamentos manuais espalhados pelo código, utilizei o **AutoMapper** para transformar:

- Entidades de domínio → DTOs de resposta da API  
- Commands → Entidades de domínio (na criação de cliente, OS, etc.)

A ideia é simples:

- O domínio continua falando a “linguagem da regra de negócio” (`CustomerEntity`, `ServiceOrderEntity`, etc.);
- A API pode expor modelos de saída específicos para cada caso de uso (`GetCustomerByIdResponse`, `GetServiceOrderByIdResponse`, etc.), sem acoplar diretamente à entidade.

Os **profiles** (`CustomerProfile`, `ServiceOrderProfile`, etc.) ficam próximos das Features / UseCases, o que ajuda a:

- Evitar perfis genéricos gigantes;
- Deixar explícito qual caso de uso usa qual mapeamento;
- Diminuir código repetitivo de “atribuição de propriedade” (mapping manual).

Isso torna o código mais enxuto e reduz a chance de erros ao adicionar novos campos ou evoluir o modelo de saída.


## Padrão Result

Para modelar sucesso e erro de forma explícita, a solução utiliza um **Result Pattern**, com os tipos:

- `Result` e `Result<T>`
- `Error` (contendo `Code`, `Message` e `StatusCode`)

Em vez de lançar exceções para qualquer falha de regra de negócio, os casos de uso retornam:

- `Result.Success(...)` quando tudo ocorre bem;
- `Result.Failure(...)` com um `Error` específico quando algo dá errado  
  (ex.: `CustomerErrors.NameRequired`, `ServiceOrderErrors.InvalidStatusTransition`, etc.).

Na camada de API, uma extensão (`ResultExtensions.ToActionResult`) converte esse `Result<T>` em uma resposta HTTP padronizada, incluindo:

- `isSuccess`
- `data` (quando sucesso)
- `error.code` e `error.message` (quando falha)

Com isso, quem consome a API recebe um contrato consistente, e o código de aplicação:

- Fica mais legível (fluxo de sucesso/erro explícito);
- Evita o uso de exceções como controle de fluxo;
- Centraliza a tradução de erros de domínio/aplicação para HTTP.

## Padronização de respostas HTTP (Result → IActionResult)

Para evitar `if`/`else` repetidos em todos os controllers, a API expõe um método de extensão que converte o `Result<T>` da camada de aplicação em um `IActionResult` padronizado. Existindo assim também uma espécie de dicionário de tipos de erros onde são atrelados automaticamente para o status correto

```csharp
public sealed record Error(string Code, string Message, HttpStatusCode StatusCode)
{
    public static Error Validation(string code, string message) =>
        new(code, message, HttpStatusCode.BadRequest);

    public static Error Conflict(string code, string message) =>
        new(code, message, HttpStatusCode.Conflict);

    public static Error NotFound(string code, string message) =>
        new(code, message, HttpStatusCode.NotFound);

    public static Error Unexpected(string code, string message) =>
        new(code, message, HttpStatusCode.InternalServerError);

    public static readonly Error None =
        new("None", string.Empty, HttpStatusCode.OK);
}
```



A lógica de tradução de resultado de domínio → resposta HTTP fica concentrada em um único lugar. Isso deixa os controllers mais enxutos, evita duplicação de lógica de status code e garante um contrato de resposta consistente em toda a API. Onde as respostas ficam nesse formato: 

```json

{
  "isSuccess": true,
  "data": { ... }
}

// Erro
{
  "isSuccess": false,
  "error": {
    "code": "ServiceOrder.CustomerRequired",
    "message": "Id do cliente é obrigatório."
  }
}

```

## GlobalExceptionHandler

Para tratar exceções não previstas de forma centralizada, a API registra um `GlobalExceptionHandler`. 

Exceções inesperadas (null reference, falha de IO, etc.) caem no GlobalExceptionHandler, que rgistra o erro em log e retorna uma resposta no formato ProblemDetails, com status apropriado( 500). Em vez de cada controller tentar tratar qualque exceção, existe um lugar específico responsável por isso. Isso reduz duplicação e risco de tratamento inconsistente.

## Padrão Unit of Work

Para coordenar o commit das alterações no banco de forma consistente, a camada de infraestrutura expõe uma implementação de **Unit of Work** baseada no Context. 

```csharp
await unitOfWork.CommitAsync(cancellationToken);

```


A ideia é simple, os Handlers da camada de Application trabalham com repositórios e ao final do caso de uso, chamam CommitAsync em IUnitOfWork.

Isso permite que várias operações de escrita (commands), sejam consolidadas em um único SaveChangesAsync, mantendo a transação coerente. A camada Application não conhece diretamente o DbContext nem a forma como o commit é feito, ela depende apenas da abstração IUnitOfWork, o que facilita testes e troca de implementação no futuro, se necessário.




## Padrão Repository


Para encapsular o acesso ao banco de dados e evitar que a camada de domínio/aplicação precise falar diretamente com EF Core, foi implementado um repositório genérico ``IRepository<TEntity>```

``` csharp
public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct);

    Task AddAsync(TEntity entity, CancellationToken ct);

    Task UpdateAsync(TEntity entity, CancellationToken ct);

    Task RemoveAsync(TEntity entity, CancellationToken ct);

    Task<bool> ExistsAsync(Guid id, CancellationToken ct);

    Task<IReadOnlyList<TEntity>> ListAsync(CancellationToken ct);
    Task<IReadOnlyList<TEntity>> ListAsync(Expression<Func<TEntity, bool>> predicate,CancellationToken ct);
}
```

Ela abstrai operações básicas (GetByIdAsync, AddAsync, UpdateAsync, RemoveAsync, ListAsync) para qualquer entidade (TEntity).

Os métodos são virtuais, permitindo que repositórios específicos (como CustomerRepository, ServiceOrderRepository) sobrescrevam comportamento quando precisarem de queries mais ricas.

E é utilizado o **AsNoTracking** nas consultas de leitura, para melhorar performance em cenários onde não é necessário rastreamento de mudanças.



## ORM Entity Framework Core

Inicialmente o código utilizava **Dapper** para acesso a dados.  
Para este desafio, optei por utilizar o **Entity Framework Core** como ORM principal, trabalhando diretamente com entidades de domínio (CustomerEntity, ServiceOrderEntity, etc.) em vez de montar e executar SQL manual.

Embora Dapper seja excelente para cenários de alta performance e consultas mais específicas, para este contexto o EF Core traz algumas vantagens importantes:

- **Trabalho direto com entidades**  
  O fluxo do código fica orientado a objetos: criar uma OS significa instanciar um `ServiceOrderEntity` e persistir via repositório, em vez de montar um `INSERT` manual.

- **Menos SQL “espalhado” no código**  
  As consultas passam a ser escritas em **LINQ**, fortemente tipadas, em cima do `DbSet<TEntity>`.  
  Isso reduz:
  - duplicação de comandos SQL,
  - risco de erros de sintaxe,
  - necessidade de manter arquivos `.sql` dentro da solução.

- **Proteção adicional contra SQL Injection**  
  Tanto EF Core quanto Dapper permitem uso seguro com parâmetros, mas com EF Core:
  - o uso de parâmetros é o padrão;
  - é menos provável que apareçam `string.Concat`/`$"SELECT ... WHERE {valor}"` soltos no código.

- **Configuração centralizada do modelo**  
  Em vez de “configurar o banco” via SQL manual, o modelo é configurado em ****classes de configuração**** (ou no próprio `DbContext`), definindo:
  - chaves primárias,
  - tipos de coluna,
  - tamanhos máximos,
  - relacionamentos, etc.

Isso deixa claro que o banco é um reflexo do modelo de domínio, e não o contrário.

A criação do banco também ficou mais simples com o uso do EF Core.  
Foi criada uma classe `DatabaseGenerantor` que delega essa responsabilidade para o próprio `DbContext`:

```csharp
namespace OsService.Infrastructure.Databases;

public class DatabaseGenerantor
{
    private readonly OsServiceDbContext _dbContext;

    public DatabaseGenerantor(OsServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task EnsureCreatedAsync(CancellationToken ct)
    {
        return _dbContext.Database.EnsureCreatedAsync(ct);
    }
}
















---------------------------

# Descritivo do Desafio 

# Instruções

- Caso o tempo não seja suficiente, priorize a **qualidade, o padrão e a estrutura do código**, definindo claramente quais funcionalidades não serão implementadas.
- Caso alguma funcionalidade não seja implementada, isso **deve ser documentado neste README**, explicando o motivo.
- O código fornecido contém **problemas que devem ser identificados e corrigidos**.
- Fique a vontade para criar, renomear e remover pastas,bibliotecas e até a solução não utilizadas.
- O sistema deve **compilar corretamente e executar todas as ações previstas**.
- O código final **não deve apresentar erros nem warnings** durante a compilação.
- Deve ser enviado via e-mail para consultoria com o link do projeto no Github. A consultaoria terá até terça-feira dia 13 as as 13 horas para encaminhar o e-mail.

---

## 1. Introdução

Sistema para um prestador de serviços (ou pequena equipe) registrar clientes, abrir ordens de serviço, acompanhar status, registrar valores e anexar fotos de antes/depois do serviço.

---

## 2. Funcionalidades Detalhadas

### 2.1 Cadastro de Cliente

#### Objetivo
Permitir registrar e consultar dados do cliente para vinculação em Ordens de Serviço (OS).

#### Campos (mínimo)
- Nome (obrigatório, 2–150 caracteres)
- Id (gerado pelo sistema)
- Telefone (opcional, até 30 caracteres)
- E-mail (opcional, até 120 caracteres, formato válido)
- Documento (CPF/CNPJ) (opcional, até 30 caracteres, sem validação pesada)
- Data de criação (gerado pelo sistema)

#### Regras de Negócio
1. Nome é obrigatório e não pode conter apenas whitespace.
2. Telefone e e-mail podem ser nulos; se informados, devem ser trimados.
3. Opcionalmente, bloquear ou alertar duplicidade por:
   - Documento (CPF/CNPJ), quando informado
   - Telefone, quando informado

#### Operações
- Criar cliente
- Consultar cliente por Id
- Buscar cliente por telefone ou documento

#### Casos de Teste
- Criar cliente com nome válido retorna 201 Created + id
- Criar cliente sem nome retorna 400 Validation Error
- Criar cliente com e-mail inválido retorna 400 Validation Error
- Criar cliente com telefone e buscar retorna dados consistentes
- Criar cliente com documento duplicado (se regra ativa) retorna 409 Conflict ou 400

---

### 2.2 Abertura de Ordem de Serviço

#### Objetivo
Criar uma OS vinculada a um cliente, com descrição e dados iniciais.

#### Campos (mínimo)
- ClienteId (obrigatório)
- Descrição do serviço (obrigatório, 1–500 caracteres)
- Número da OS (gerado automaticamente, sequencial/identity)
- Status (inicial = Aberta)
- Data de abertura (gerado pelo sistema)
- Valor do serviço (decimal(18,2)) (opcional no momento da abertura)
- Moeda (BRL)
- Data de atualização valor (opcional)

#### Regras de Negócio
1. Só é possível abrir OS para cliente existente.
2. Descrição é obrigatória.
3. Status inicial deve ser sempre Aberta.
4. Número da OS deve ser único e sequencial.
5. Regra de negócio item 2.4 

#### Operações
- Abrir OS
- Consultar OS por Id
- Listar OS por cliente, status ou período

#### Casos de Teste
- Abrir OS para cliente existente retorna 201 Created
- Abrir OS para cliente inexistente retorna 404 Not Found
- Abrir OS com descrição vazia retorna 400 Bad Request
- Consultar OS recém-criada retorna status Aberta

---

### 2.3 Status da Ordem de Serviço

#### Objetivo
Permitir acompanhar o ciclo do serviço.

#### Estados
- Aberta
- Em Execução
- Finalizada

#### Regras de Transição
- Aberta -> Em Execução (permitido)
- Em Execução -> Finalizada (permitido)
- Aberta -> Finalizada (bloqueado)
- Finalizada -> qualquer outro (bloqueado)

#### Operações
- Alterar status
- Registrar datas opcionais:
  - StartedAt ao entrar em Em Execução
  - FinishedAt ao entrar em Finalizada

#### Casos de Teste
- Alterar Aberta para Em Execução retorna 200 OK
- Alterar Em Execução para Finalizada retorna 200 OK
- Alterar Finalizada para outro status retorna 409 Conflict

---

### 2.4 Valor do Serviço

#### Objetivo
Permitir definir ou ajustar o valor do serviço.

#### Campos
- Valor (decimal(18,2))
- Moeda (BRL)
- Data de atualização (opcional)

#### Regras de Negócio
1. Valor pode ser nulo enquanto Aberta ou Em Execução.
2. Valor pode ser obrigatório para finalizar a OS.
3. Valor não pode ser negativo.
4. Após Finalizada, não permitir alteração.

#### Operações
- Definir ou alterar valor
- Validar valor ao finalizar OS

---

### 2.5 Fotos Antes / Depois (Opcional)

#### Objetivo
Permitir anexar evidências do serviço.

#### Campos do Anexo
- Id
- ServiceOrderId
- Type (Before | After)
- FileName
- ContentType (image/jpeg, image/png)
- SizeBytes
- StoragePath
- UploadedAt

#### Regras de Negócio
1. Aceitar apenas JPG e PNG.
2. Tamanho máximo sugerido: 5MB.
3. Permitir múltiplos anexos.
4. Upload local em /data/uploads (container ou volume).

---

## 3. API Sugerida

### Clientes
- POST /v1/customers
- GET /v1/customers/{id}

### Ordens de Serviço
- POST /v1/service-orders
- GET /v1/service-orders/{id}
- PATCH /v1/service-orders/{id}/status
- PUT /v1/service-orders/{id}/price
- POST /v1/service-orders/{id}/attachments/before
- POST /v1/service-orders/{id}/attachments/after
- GET /v1/service-orders/{id}/attachments

---

## 4. Requisitos Não Funcionais (Opcional)

### Performance
- Upload deve ser feito via streaming, evitando carregar todo o arquivo em memória.

### Segurança
- Validar content-type e extensão real do arquivo.
- Sanitizar nome do arquivo.

### Observabilidade
- Registrar logs para criação de cliente, abertura de OS e mudança de status.


















